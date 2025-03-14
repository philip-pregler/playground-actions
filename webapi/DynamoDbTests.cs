using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace webapi
{
    [TestClass]
    public class DynamoDbTests
    {
        private AmazonDynamoDBClient _dynamoDbClient;

        [TestInitialize]
        public void Setup()
        {
            _dynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = Environment.GetEnvironmentVariable("DYNAMODB_ENDPOINT")
            });
        }

        [TestMethod]
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
            Assert.IsTrue(response.Item.ContainsKey("Name"));
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