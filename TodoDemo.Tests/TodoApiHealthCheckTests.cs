namespace TodoDemo.Tests;

public class TodoApiHealthCheckTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiHealthCheckTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ApiIsHealthy()
    {
        // Arrange
        var requestUri = "/api/todos";
        
        // Act
        var response = await _client.GetAsync(requestUri);
        var body = await response.Content.ReadAsStringAsync();
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        Assert.True(response.Content.Headers.ContentLength > 0);
    }
}