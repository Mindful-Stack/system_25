using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TodoDemo.Data;
using TodoDemo.Models;

namespace TodoDemo.Features.Todos;

public static class GetTodos
{
    public static void Map(IEndpointRouteBuilder app) => app.MapGet("/", Handle);

    private static async Task<Ok<List<TodoItem>>> Handle(TodoDbContext db, CancellationToken ct)
    {
        var todos = await db.Todos
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        return TypedResults.Ok(todos);
    }
}
