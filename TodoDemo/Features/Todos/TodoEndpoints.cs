namespace TodoDemo.Features.Todos;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var todos = app.MapGroup("/api/todos");

        GetTodos.Map(todos);
        GetTodo.Map(todos);
        CreateTodo.Map(todos);
        UpdateTodo.Map(todos);
        DeleteTodo.Map(todos);
    }
}
