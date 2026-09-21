using Microsoft.AspNetCore.Http.HttpResults;
using TodoDemo.Data;

namespace TodoDemo.Features.Todos;

public static class UpdateTodo
{
    public record Request(int Id, string Title, string? Description, bool IsDone);

    public static void Map(IEndpointRouteBuilder app) => app.MapPut("/{id:int}", Handle);

    private static async Task<Results<NoContent, NotFound, BadRequest, ValidationProblem>> Handle(
        int id, Request request, TodoDbContext db, CancellationToken ct)
    {
        if (id != request.Id) return TypedResults.BadRequest();

        if (string.IsNullOrWhiteSpace(request.Title))
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["Title"] = ["Title is required."]
            });

        var todo = await db.Todos.FindAsync([id], ct);
        if (todo is null) return TypedResults.NotFound();

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.IsDone = request.IsDone;
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
