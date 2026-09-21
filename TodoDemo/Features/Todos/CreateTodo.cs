using Microsoft.AspNetCore.Http.HttpResults;
using TodoDemo.Data;
using TodoDemo.Models;

namespace TodoDemo.Features.Todos;

public static class CreateTodo
{
    public record Request(string Title, string? Description, bool IsDone);

    public static void Map(IEndpointRouteBuilder app) => app.MapPost("/", Handle);

    private static async Task<Results<Created<TodoItem>, ValidationProblem>> Handle(
        Request request, TodoDbContext db, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["Title"] = ["Title is required."]
            });

        var todo = new TodoItem
        {
            Title = request.Title,
            Description = request.Description,
            IsDone = request.IsDone,
            CreatedAt = DateTime.UtcNow
        };

        db.Todos.Add(todo);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created($"/api/todos/{todo.Id}", todo);
    }
}
