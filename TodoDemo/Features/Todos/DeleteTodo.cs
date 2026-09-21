using Microsoft.AspNetCore.Http.HttpResults;
using TodoDemo.Data;

namespace TodoDemo.Features.Todos;

public static class DeleteTodo
{
    public static void Map(IEndpointRouteBuilder app) => app.MapDelete("/{id:int}", Handle);

    private static async Task<Results<NoContent, NotFound>> Handle(int id, TodoDbContext db, CancellationToken ct)
    {
        var todo = await db.Todos.FindAsync([id], ct);
        if (todo is null) return TypedResults.NotFound();

        db.Todos.Remove(todo);
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
