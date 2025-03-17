using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace webapi.test
{
    public class DynamoDbTests
    {
        private AmazonDynamoDBClient _dynamoDbClient;

        public DynamoDbTests()
        {
            _dynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = Environment.GetEnvironmentVariable("DYNAMODB_ENDPOINT")
            });

            // Erstelle die DynamoDB-Tabelle, falls sie noch nicht existiert
            CreateTableIfNotExists();
        }

        private void CreateTableIfNotExists()
        {
            var tableDescription = GetTableDescription("TestTable");
            if (tableDescription == null)
            {
                CreateTable();
            }
        }

        private void CreateTable()
        {
            var createTableRequest = new CreateTableRequest
            {
                TableName = "TestTable",
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition { AttributeName = "Id", AttributeType = "S" }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement { AttributeName = "Id", KeyType = "HASH" }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 1,
                    WriteCapacityUnits = 1
                }
            };

            _dynamoDbClient.CreateTableAsync(createTableRequest).GetAwaiter().GetResult();
            Console.WriteLine("DynamoDB table 'TestTable' created.");
        }

        private TableDescription GetTableDescription(string tableName)
        {
            try
            {
                var describeTableRequest = new DescribeTableRequest { TableName = tableName };
                var response = _dynamoDbClient.DescribeTableAsync(describeTableRequest).GetAwaiter().GetResult();
                return response.Table;
            }
            catch (ResourceNotFoundException)
            {
                return null;
            }
        }

        [Fact]
        public async Task CanCreateAndDeleteItem()
        {
            Console.WriteLine("Creating test item...");
            var testItem = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = "123" } },
                { "Name", new AttributeValue { S = "Test Item" } }
            };

            var putItemRequest = new PutItemRequest
            {
                TableName = "TestTable",
                Item = testItem
            };

            await _dynamoDbClient.PutItemAsync(putItemRequest);
            Console.WriteLine($"Created test item: {string.Join(", ", testItem.Select(x => $"{x.Key}={x.Value.S}"))}");

            Console.WriteLine("Retrieving test item...");
            var getItemRequest = new GetItemRequest
            {
                TableName = "TestTable",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = "123" } }
                }
            };

            var response = await _dynamoDbClient.GetItemAsync(getItemRequest);
            Assert.True(response.Item.ContainsKey("Name"));
            Console.WriteLine($"Retrieved test item: {string.Join(", ", response.Item.Select(x => $"{x.Key}={x.Value.S}"))}");

            Console.WriteLine("Deleting test item...");
            await _dynamoDbClient.DeleteItemAsync("TestTable", new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = "123" } }
            });
            Console.WriteLine("Test item deleted successfully.");
        }
    }
}