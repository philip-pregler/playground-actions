using System.Net.Http.Json;using System.Security.AccessControl;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace test
{
    public class WebApplicationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public WebApplicationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestWeatherForecastEndpoint()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/weatherforecast");

            // Assert
            response.EnsureSuccessStatusCode();
            var weatherForecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
            Assert.NotNull(weatherForecasts);
            Assert.Equal(5, weatherForecasts.Length);
            foreach (var forecast in weatherForecasts)
            {
                Assert.NotNull(forecast.Summary);
                Assert.InRange(forecast.TemperatureC, -20, 55);
                Assert.InRange(forecast.TemperatureF, -4, 131);
            }
        }

        [Fact]
        public async Task TestSwaggerConfiguration()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/swagger/index.html");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Swagger UI", await response.Content.ReadAsStringAsync());
        }
    }
}