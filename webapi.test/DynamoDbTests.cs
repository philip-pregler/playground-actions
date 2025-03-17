using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Xunit;

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
        }

        [Fact]
        public async Task CanConnectToDynamoDB()
        {
            // Führe eine einfache DynamoDB-Operation aus, um die Verbindung zu testen
            var listTablesRequest = new ListTablesRequest();
            var listTablesResponse = await _dynamoDbClient.ListTablesAsync(listTablesRequest);

            // Überprüfe, ob die Verbindung erfolgreich war
            Assert.NotNull(listTablesResponse);
            Assert.NotEmpty(listTablesResponse.TableNames);
        }
    }
}