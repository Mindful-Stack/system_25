# TodoDemo Search Filter Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use subagent-driven-development (recommended) or executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Let users search todos by title and description through `GET /api/todos?q=<search>`, from a search box on the home page that keeps the term in the page URL.

**Architecture:** The `GetTodos` endpoint takes an optional `q` and filters in the database with `ToLower().Contains(...)`. `Home.razor` stores the term in its URL (`?q=`). The search box writes to the URL after a 300 ms debounce, and a change to the URL reloads the list.

**Tech Stack:** .NET 10, ASP.NET Core minimal APIs, EF Core 10 (SQLite; SQL Server is optional), Blazor interactive server, xUnit with `WebApplicationFactory`.

**Spec:** [docs/agent/specs/2026-10-12-todo-search-filter-design.md](../specs/2026-10-12-todo-search-filter-design.md)

---

## Before you start

- Work on branch `feature/todo-search-filter`.
- The working tree has unrelated uncommitted changes (`CreateTodo.cs`, `TodoApiCrudTests.cs`, `todos.db`, `DiceGameDemo.Tests`, BlueGreen files). **Do not stage or commit them.** Every commit in this plan names its files with `git add <path>`; never use `git add -A` or `git add .`.
- Baseline: `dotnet test TodoDemo.Tests` passes 5 of 5 tests on this branch.

## Verified framework facts this plan relies on

- **EF Core SQLite translation** ([EF Core docs, SQLite function mappings](https://learn.microsoft.com/ef/core/providers/sqlite/functions)): `stringValue.Contains(value)` becomes `instr(@stringValue, @value) > 0`, and `stringValue.ToLower()` becomes `lower(@stringValue)`. SQLite's `lower()` folds ASCII letters only, which is the known limitation in the spec.
- **`FocusOnNavigate` does not steal focus on query-only navigation.** `Routes.razor` contains `<FocusOnNavigate Selector="h1"/>`. Its source (`dotnet/aspnetcore`, `release/10.0`, `src/Components/Web/src/Routing/FocusOnNavigate.cs`) refocuses only when the page type changes: `if (RouteData!.PageType != _lastNavigatedPageType)`. Moving from `/` to `/?q=x` stays on the same page type, so the search input keeps focus.
- **Removing a query parameter:** `Navigation.GetUriWithQueryParameter("name", (string)null)` removes it ([Blazor navigation docs](https://learn.microsoft.com/aspnet/core/blazor/fundamentals/navigation), "Remove a query parameter when the parameter value is `null`").

## File map

| File | Change | Responsibility |
|---|---|---|
| `TodoDemo.Tests/TodoApiSearchTests.cs` | Create | Integration tests for `?q=` |
| `TodoDemo/Features/Todos/GetTodos.cs` | Modify | Accept `q`, filter in the database |
| `TodoDemo/Components/Pages/Home.razor` | Modify | Search box, URL sync, debounce, empty state for no matches |
| `TodoDemo/wwwroot/app.css` | Modify | Layout of the search box beside "New todo" |

---

### Task 1: API search filter (test-first)

**Files:**
- Create: `TodoDemo.Tests/TodoApiSearchTests.cs`
- Modify: `TodoDemo/Features/Todos/GetTodos.cs` (the whole `Handle` method)

**Shared-database rule:** all tests in one class share a single in-memory SQLite database. `CustomWebApplicationFactory` registers one `SqliteConnection` as a singleton, and xUnit creates one fixture per class. Each test must therefore:
1. Put a fresh `Guid` token in the todos it seeds.
2. Seed a **decoy** todo that must not match. Without a decoy, an endpoint that ignores `q` and returns everything would still pass "returns the matching todo".
3. Never assert on the total count of todos. Other tests' rows are in the database too.

- [ ] **Step 1: Write the failing tests**

Create `TodoDemo.Tests/TodoApiSearchTests.cs`:

```csharp
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
```

What each test catches:
- `SearchMatchesTitle`: `q` is ignored, so the decoy is returned.
- `SearchMatchesDescription`: only the title is searched, so the match is missing. It also catches `q` being ignored, through the decoy.
- `SearchIgnoresCase`: the filter is case-sensitive. The stored text is uppercase and the query is lowercase, so a case-sensitive `instr` finds nothing. It also catches the term being lowercased on only one side.
- `SearchWithNoMatchesReturnsEmptyList`: `q` is ignored (the list would not be empty), or "no matches" returns 404 (`GetTodos` asserts `200`).
- `NoSearchTermReturnsAllTodosNewestFirst`: this is a **regression guard**, and it passes before Task 1's implementation. The `"  "` case catches an implementation that skips the trim. `Contains("  ")` would drop both todos.

- [ ] **Step 2: Run the tests and confirm they fail for the right reason**

Run: `dotnet test TodoDemo.Tests --filter "FullyQualifiedName~TodoApiSearchTests"`

Expected: 4 failed, 2 passed.
- `SearchMatchesTitle`, `SearchMatchesDescription` and `SearchIgnoresCase` fail on `Assert.DoesNotContain` because the decoy is returned.
- `SearchWithNoMatchesReturnsEmptyList` fails on `Assert.Empty`.
- Both `NoSearchTermReturnsAllTodosNewestFirst` cases pass.

The requirement is that the failures are **assertion** failures. A compile error or an exception means the test itself is broken.

- [ ] **Step 3: Implement the filter**

Replace the contents of `TodoDemo/Features/Todos/GetTodos.cs`:

```csharp
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
```

Properties this code must have:
- `term` is lowercased **in C#**, outside the expression tree. `ToLowerInvariant` avoids culture surprises such as the Turkish dotless i. Inside the lambda, use `ToLower()`, because that is what EF translates to `lower()`.
- A null, empty or whitespace `q` adds no `Where` at all.
- The `Description != null` guard stays, so a todo with no description never matches through that field.

- [ ] **Step 4: Run the tests and confirm they pass**

Run: `dotnet test TodoDemo.Tests`

Expected: `Passed! - Failed: 0, Passed: 12` (the 5 existing tests and the 6 new ones).

- [ ] **Step 5: Commit**

```bash
git add TodoDemo.Tests/TodoApiSearchTests.cs TodoDemo/Features/Todos/GetTodos.cs
git commit -m "Add q search filter to GET /api/todos"
```

---

### Task 2: Search box in the home page

**Files:**
- Modify: `TodoDemo/Components/Pages/Home.razor`
- Modify: `TodoDemo/wwwroot/app.css` (the `.page-header > .btn` rule, around line 147)

The repo has no bUnit or Playwright tests, and this plan does not add them. Task 3 verifies this task by hand.

- [ ] **Step 1: Add the header layout CSS**

In `TodoDemo/wwwroot/app.css`, replace:

```css
.page-header > .btn {
    order: 3;
}
```

with:

```css
.page-header > .page-actions {
    order: 3;
    display: flex;
    gap: 0.75rem;
    width: 100%;
    max-width: 28rem;
}

.page-actions .search-input {
    flex: 1;
}
```

The "New todo" button moves into `.page-actions`, so the old rule would no longer match anything. The search input reuses the existing `.form-control` styling.

- [ ] **Step 2: Update the top of `Home.razor` (directives and header)**

Replace the lines from `@page "/"` down to the closing `</header>` with:

```razor
@page "/"
@using TodoDemo.Shared
@implements IDisposable

@inject System.Net.Http.HttpClient Http
@inject NavigationManager Nav

<PageTitle>The Quest</PageTitle>

<header class="page-header">
    <div>
        <h1>The Quest</h1>
        <p class="page-subtitle">Things to do before the Fourth Age.</p>
    </div>
    <div class="page-actions">
        <input type="search" class="form-control search-input"
               placeholder="Search todos" aria-label="Search todos"
               @bind="searchText" @bind:event="oninput" @bind:after="OnSearchInput" />
        <button class="btn btn-primary" @onclick="ShowCreate">New todo</button>
    </div>
</header>
```

- [ ] **Step 3: Split the empty state**

Replace the existing `else if (todos == null || !todos.Any()) { ... }` block with:

```razor
else if (todos == null || !todos.Any())
{
    @if (currentQ is not null)
    {
        <div class="empty-state">
            <p>No todos match "@currentQ".</p>
        </div>
    }
    else
    {
        <div class="empty-state">
            <p>Nothing left to do. Go home to the Shire.</p>
            <button class="btn btn-primary" @onclick="ShowCreate">Add a todo</button>
        </div>
    }
}
```

This prevents an empty search from claiming that there is nothing left to do.

- [ ] **Step 4: Replace the loading and state code in `@code`**

In the `@code` block, replace everything from `List<TodoDto>? todos;` through the end of the `Reload()` method with the code below. Leave `ShowCreate`, `ShowEdit`, `Delete`, `ToggleStatus`, `OnSaved`, `OnCancelled` and the `TodoDto` record unchanged. They already call `Reload()`, which keeps using `currentQ`.

```csharp
    [SupplyParameterFromQuery(Name = "q")]
    public string? Q { get; set; }

    List<TodoDto>? todos;
    bool isLoading = true;

    // Search state
    string searchText = "";      // what is in the box right now (may have trailing spaces while typing)
    string? currentQ;            // the normalised term the list was loaded for; null = no filter
    CancellationTokenSource? debounceCts;
    CancellationTokenSource? loadCts;

    // Editor state
    bool showEditor = false;
    int editingId = 0;

    // The URL is the source of truth: typing changes ?q=, and a ?q= change lands here.
    protected override async Task OnParametersSetAsync()
    {
        var q = Normalize(Q);
        if (!isLoading && q == currentQ) return;

        currentQ = q;
        // Refresh, a shared link or back/forward: show the URL's term in the box.
        if (Normalize(searchText) != q) searchText = q ?? "";

        await Reload();
    }

    async Task OnSearchInput()
    {
        debounceCts?.Cancel();
        debounceCts = new CancellationTokenSource();
        var ct = debounceCts.Token;
        var term = Normalize(searchText);

        // A cleared box searches right away; anything else waits until typing pauses.
        if (term is not null)
        {
            try { await Task.Delay(300, ct); }
            catch (TaskCanceledException) { return; } // the user kept typing
        }

        // null removes ?q= from the URL; replace keeps keystrokes out of browser history.
        Nav.NavigateTo(Nav.GetUriWithQueryParameter("q", term), replace: true);
    }

    async Task Reload()
    {
        loadCts?.Cancel();
        loadCts = new CancellationTokenSource();
        var ct = loadCts.Token;

        var url = currentQ is null ? "api/todos" : $"api/todos?q={Uri.EscapeDataString(currentQ)}";
        try
        {
            if (isLoading) await Task.Delay(100, ct); // Small delay to show loading state
            var result = await Http.GetFromJsonAsync<List<TodoDto>>(url, ct);
            if (ct.IsCancellationRequested) return;
            todos = result;
        }
        catch (OperationCanceledException)
        {
            return; // a newer Reload started; its result wins
        }

        isLoading = false;
    }

    static string? Normalize(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    public void Dispose()
    {
        debounceCts?.Cancel();
        loadCts?.Cancel();
    }
```

Also delete the old `OnInitializedAsync` method. `OnParametersSetAsync` now does the first load.

Properties this code must have:
- **No stale overwrite.** A response from an older `Reload` must never replace a newer one. Each call cancels the previous token, and a cancelled call returns before assigning `todos`.
- **"Loading..." only on first load.** `isLoading` starts `true` and only ever becomes `false`. Later searches keep the old rows visible, so the `<input>` (which sits outside the `isLoading` branch) is never re-rendered away and keeps focus.
- **The trailing space is preserved while typing.** `OnParametersSetAsync` overwrites `searchText` only when its *normalised* value differs from the URL's. Typing `"ring "` navigates to `?q=ring` and leaves the box at `"ring "`.
- **No redundant reload.** Once loaded, an `OnParametersSetAsync` call with the same normalised term (for example after typing a trailing space) returns without fetching.
- **Failures behave as before.** Only `OperationCanceledException` is caught. Any other HTTP failure propagates exactly as it does today.
- **Re-rendering.** No explicit `StateHasChanged()` is needed. Blazor re-renders after lifecycle methods and event-handler `Task`s complete, and `Reload()` is always awaited from one of those.

- [ ] **Step 5: Build**

Run: `dotnet build TodoDemo`

Expected: `Build succeeded` with 0 errors. Fix any new warnings in `Home.razor`.

- [ ] **Step 6: Run the test suite (regression)**

Run: `dotnet test TodoDemo.Tests`

Expected: `Passed! - Failed: 0, Passed: 12`.

- [ ] **Step 7: Commit**

```bash
git add TodoDemo/Components/Pages/Home.razor TodoDemo/wwwroot/app.css
git commit -m "Add debounced search box synced to ?q= on the home page"
```

---

**Review amendments (applied after code review):** the code above had four defects, fixed in a follow-up commit:
- **Typed characters overwritten.** `NavigateTo` is a round trip on Blazor Server, so copying `Q` into `searchText` on every parameter set overwrote characters typed mid-round-trip. The box is now seeded once in `OnInitialized`, and that line is gone from `OnParametersSetAsync`.
- **Stale navigation.** `OnSearchInput` now rechecks the token after the delay, so a debounce cancelled after its delay finished does not navigate.
- **Wrong row on revert.** `ToggleStatus` now finds the row again by id when reverting, because a search reload can replace `todos` while the PUT is in flight.
- **Button wrapping.** The "New todo" button no longer shrinks or wraps.

---

### Task 3: Manual UI verification

**Files:** none, unless a check fails.

- [ ] **Step 1: Start the app**

Run: `dotnet run --project TodoDemo --launch-profile https`. Open the `https://localhost:...` URL it prints. The app uses `TodoDemo/todos.db`. Add two or three todos with distinct words if the list is empty, and do not commit `todos.db`.

- [ ] **Step 2: Walk the spec's checklist**

Each item names the defect it would expose:

1. Type a word from one todo's **description**, then pause. The list narrows after about 300 ms. *Catches: the debounce never navigates, or `q` is not sent.*
2. While typing quickly, the list does not change on every key, and the input keeps focus throughout. *Catches: no debounce, or the input re-rendered away by the loading branch.*
3. The address bar shows `?q=<word>`. *Catches: the URL is not updated.*
4. Refresh the page. The box and the list still show the search. *Catches: `Q` not read on load, or `searchText` not seeded from the URL.*
5. Clear the box (the native ✕ or backspace). Every todo comes back immediately, and `?q=` disappears from the URL. *Catches: the empty term still being debounced, or the `null` value not removing the parameter.*
6. Search for `zzzz`. The page shows *No todos match "zzzz".* and not "Nothing left to do". *Catches: the empty-state split missing.*
7. With a search active, toggle a todo's checkbox, then edit and save one. The list stays filtered. *Catches: `Reload()` dropping `currentQ`.*

- [ ] **Step 3: Fix and commit if needed**

If any check fails, fix it in `Home.razor` or `app.css` and rerun Task 2 Steps 5 and 6. Then commit:

```bash
git add TodoDemo/Components/Pages/Home.razor TodoDemo/wwwroot/app.css
git commit -m "Fix search box: <what was wrong>"
```

If every check passes, there is nothing to commit.
