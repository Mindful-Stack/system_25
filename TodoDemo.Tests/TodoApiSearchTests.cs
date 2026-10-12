using System.Net;
using TodoDemo.Features.Todos;
using TodoDemo.Models;

namespace TodoDemo.Tests;

// Tester för sökningen: GET api/todos?q=<sökord>
//
// Alla tester i den här klassen delar SAMMA databas i minnet (en fixture per klass).
// Därför märker varje test sina todos med en unik "token" (ett Guid) och söker på den.
// Varje test lägger också in en "lockbete"-todo som INTE ska matcha. Utan den skulle
// ett API som struntar i q och returnerar allt ändå klara testet.
public class TodoApiSearchTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiSearchTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static string NewToken() => Guid.NewGuid().ToString("N");

    private async Task<TodoItem> Seed(string title, string? description = null)
    {
        var response = await _client.PostAsJsonAsync("api/todos", new CreateTodo.Request(title, description, false));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TodoItem>())!;
    }

    private async Task<List<TodoItem>> GetTodos(string? q)
    {
        var url = q is null ? "api/todos" : $"api/todos?q={Uri.EscapeDataString(q)}";
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<List<TodoItem>>())!;
    }

    [Fact]
    public async Task SearchMatchesTitle()
    {
        // Arrange
        var token = NewToken();
        var match = await Seed($"Destroy the Ring {token}");
        var decoy = await Seed($"Second breakfast {NewToken()}");

        // Act
        var result = await GetTodos(token);

        // Assert
        Assert.Contains(result, t => t.Id == match.Id);
        Assert.DoesNotContain(result, t => t.Id == decoy.Id);
    }

    [Fact]
    public async Task SearchMatchesDescription()
    {
        // Arrange: sökordet finns BARA i beskrivningen
        var token = NewToken();
        var match = await Seed("Pack for the journey", $"Bring the {token}");
        var decoy = await Seed("Pack lunch", $"Bring bread {NewToken()}");

        // Act
        var result = await GetTodos(token);

        // Assert
        Assert.Contains(result, t => t.Id == match.Id);
        Assert.DoesNotContain(result, t => t.Id == decoy.Id);
    }

    [Fact]
    public async Task SearchIgnoresCase()
    {
        // Arrange: sparat med VERSALER, sökt med gemener
        var token = NewToken();
        var match = await Seed($"RING {token.ToUpperInvariant()}");
        var decoy = await Seed($"RING {NewToken().ToUpperInvariant()}");

        // Act
        var result = await GetTodos($"ring {token}");

        // Assert
        Assert.Contains(result, t => t.Id == match.Id);
        Assert.DoesNotContain(result, t => t.Id == decoy.Id);
    }

    [Fact]
    public async Task SearchWithNoMatchesReturnsEmptyList()
    {
        // Arrange
        await Seed($"Something {NewToken()}");

        // Act
        var result = await GetTodos(NewToken());

        // Assert: 200 OK med en tom lista, inte 404
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    public async Task NoSearchTermReturnsAllTodosNewestFirst(string? q)
    {
        // Arrange
        var older = await Seed($"Older {NewToken()}");
        var newer = await Seed($"Newer {NewToken()}");

        // Act
        var result = await GetTodos(q);

        // Assert: båda finns med, och den nyaste kommer först
        var olderIndex = result.FindIndex(t => t.Id == older.Id);
        var newerIndex = result.FindIndex(t => t.Id == newer.Id);
        Assert.True(olderIndex >= 0, "older todo missing");
        Assert.True(newerIndex >= 0, "newer todo missing");
        Assert.True(newerIndex < olderIndex, "newest todo should come first");
    }
}
