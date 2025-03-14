using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using NUnit.Framework;

namespace DefaultNamespace;
public class DynamoDbTests
{
    private AmazonDynamoDBClient _dynamoDbClient;

    [SetUp]
    public void Setup()
    {
        _dynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
        {
            ServiceURL = Environment.GetEnvironVariable("DYNAMODB_ENDPOINT")
        });
    }

    [Test]
    public async Task CanCreateAndDeleteItem()
    {
        var putItemRequest = new PutItemRequest
        {
            TableName = "TestTable",
            Item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = "123" } },
                { "Name", new AttributeValue { S = "Test Item" } }
            }
        };

        await _dynamoDbClient.PutItemAsync(putItemRequest);

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

        await _dynamoDbClient.DeleteItemAsync("TestTable", new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = "123" } }
        });
    }
}