using System.Net;
using TodoDemo.Models;

namespace TodoDemo.Tests;

public class TodoApiCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiCrudTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CanCreateTodo()
    {
        // Arrange
        var newItem = new TodoItem
        {
            Title = "Test",
            Description = "Test of integration test",
            IsDone = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/todos", newItem);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal((HttpStatusCode)StatusCodes.Status201Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.Equal("Test", created?.Title);
        Assert.Equal("Test of integration test", created?.Description);
        Assert.False(created is { IsDone: true });
        Assert.True(created is { Id: > 0 });
    }
    
    [Fact]
    public async Task CanNotCreateTodoWithoutTitle()
    {
        // Arrange
        var newItem = new TodoItem
        {
            Title = "",
            Description = "Test of integration test",
            IsDone = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("api/todos", newItem);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CanReadTodo()
    {
        // Arrange
        var newItem = new TodoItem
        {
            Title = "Test",
            Description = "Test of integration test",
            IsDone = false
        };
        var createResponse = await _client.PostAsJsonAsync("api/todos", newItem);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();
        
        // Act
        var getResponse = await _client.GetAsync($"api/todos/{created?.Id}");
        var item = await getResponse.Content.ReadFromJsonAsync<TodoItem>();
        
        // Assert
        getResponse.EnsureSuccessStatusCode();
        Assert.Equal("Test", item?.Title);
        Assert.Equal("Test of integration test", item?.Description);
        Assert.False(created is { IsDone: true });
        Assert.True(created is { Id: > 0 });
    }
    
    [Fact]
    public async Task CanDeleteTodo()
    {
        // Arrange
        var newItem = new TodoItem
        {
            Title = "Test",
            Description = "Test of integration test",
            IsDone = false
        };
        
        var createResponse = await _client.PostAsJsonAsync("api/todos", newItem);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();
        
        // Act
        var deleteResponse = await _client.DeleteAsync($"api/todos/{created?.Id}");
        
        // Assert
        deleteResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        
        var  getResponse = await _client.GetAsync($"api/todos/{created?.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}