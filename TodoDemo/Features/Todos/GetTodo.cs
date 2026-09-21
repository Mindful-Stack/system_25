using Microsoft.AspNetCore.Http.HttpResults;
using TodoDemo.Data;
using TodoDemo.Models;

namespace TodoDemo.Features.Todos;

public static class GetTodo
{
    public static void Map(IEndpointRouteBuilder app) => app.MapGet("/{id:int}", Handle);

    private static async Task<Results<Ok<TodoItem>, NotFound>> Handle(int id, TodoDbContext db, CancellationToken ct)
    {
        var todo = await db.Todos.FindAsync([id], ct);
        if (todo is null) return TypedResults.NotFound();

        return TypedResults.Ok(todo);
    }
}
