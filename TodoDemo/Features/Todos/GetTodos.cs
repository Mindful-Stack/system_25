using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoDemo.Data;
using TodoDemo.Models;

namespace TodoDemo.Features.Todos;

public static class GetTodos
{
    public static void Map(IEndpointRouteBuilder app) => app.MapGet("/", Handle);

    private static async Task<Ok<List<TodoItem>>> Handle([FromQuery] string? q, TodoDbContext db, CancellationToken ct)
    {
        var query = db.Todos.AsNoTracking();

        var term = q?.Trim().ToLowerInvariant();
        if (!string.IsNullOrEmpty(term))
        {
            query = query.Where(t =>
                t.Title.ToLower().Contains(term) ||
                (t.Description != null && t.Description.ToLower().Contains(term)));
        }

        var todos = await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

        return TypedResults.Ok(todos);
    }
}
