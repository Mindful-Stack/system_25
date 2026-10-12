# TodoDemo search filter: design

Date: 2026-10-12
Status: approved design, not yet implemented

## Goal

Let users search their todos. The backend does the filtering. The client sends the term as a query parameter: `GET /api/todos?q=<search>`. The home page gets a search box that sends it and keeps the term in the page URL.

## Decisions

| Question | Choice |
|---|---|
| Scope | API and UI |
| Fields matched | Title and description |
| Case | Case-insensitive |
| Database filter | `ToLower().Contains(...)`, so no LIKE escaping is needed |
| When the UI searches | As the user types, debounced at 300 ms. Clearing the box searches right away |
| Page URL | Kept in sync with `?q=`, so a search survives a refresh and can be shared |

No architecture decision record: the parameter is an optional, additive change to the API contract and is easy to reverse.

## 1. API: `GET /api/todos?q=<search>`

Change only `TodoDemo/Features/Todos/GetTodos.cs`. The route and `TodoEndpoints.cs` stay as they are.

- The handler takes an optional `[FromQuery] string? q`.
- Trim `q`. If the result is null, empty or only spaces, do not filter. The endpoint then behaves exactly as it does today.
- Otherwise convert the trimmed term to lowercase once, in C#, and filter:

  ```csharp
  query = query.Where(t =>
      t.Title.ToLower().Contains(term) ||
      (t.Description != null && t.Description.ToLower().Contains(term)));
  ```

- Results stay newest first, under the existing `OrderByDescending(t => t.CreatedAt)`.
- The response type stays `Ok<List<TodoItem>>`. No matches returns `200` with `[]`, never `404`.
- There is no validation and no length limit. EF parameterises the term, so injection is not a risk.

**Known limitation:** SQLite's `lower()` folds ASCII letters only, so on SQLite `ö` does not match `Ö`. SQL Server is not affected. We accept this and do not work around it.

## 2. UI: search box in `Home.razor`

The page URL holds the search term. Typing changes the URL, and a URL change loads the list. Refresh, shared links and back/forward therefore all take the same path.

- **Reading the URL:** add `[SupplyParameterFromQuery(Name = "q")] public string? Q { get; set; }`. Move the initial fetch from `OnInitializedAsync` to `OnParametersSetAsync`, so that a new `Q` reloads the list.
- **Input:** an `<input type="search">` in the page header, next to "New todo". It is bound with `@bind:event="oninput"` and starts with the value of `Q`.
- **Debounce:** each change cancels the previous `CancellationTokenSource` and waits 300 ms. An empty box skips the wait. After the wait the component calls `NavigationManager.NavigateTo(Nav.GetUriWithQueryParameter("q", term), replace: true)`. Passing `null` removes `?q=`, and `replace: true` keeps keystrokes out of the browser history.
- **Fetch:** `Reload()` calls `api/todos`, adding `?q=` plus `Uri.EscapeDataString(Q)` when a term is set. If an earlier request completes after a newer one has started, its response is discarded.
- **Loading state:** "Loading..." appears only on the first load. During a search the current results stay on screen until the new ones arrive, so the table does not flicker and the input keeps focus.
- **Empty states:**
  - No term and no todos: the existing "Nothing left to do" message.
  - A term with no matches: *No todos match "ring".*
- **After changes:** save, delete and toggle already call `Reload()`, and the current filter stays applied.
- **Summary line:** the counts cover the filtered list. The line needs no change.

## 3. Testing

### Integration tests

Add a new file, `TodoDemo.Tests/TodoApiSearchTests.cs`, using `IClassFixture<CustomWebApplicationFactory<Program>>`. All tests in the class share one in-memory database, so each test seeds todos that contain a unique token (part of a `Guid`) and searches for that token.

| Test | Seeds | Request | Expects |
|---|---|---|---|
| Matches title | `"Destroy the Ring {token}"` | `?q={token}` | only that todo |
| Matches description | title without the token, description with it | `?q={token}` | that todo |
| Case-insensitive | `"RING {token}"` | `?q=ring {token}` in lowercase | a match |
| No match | one todo | `?q={another token}` | `200`, `[]` |
| No filter | several todos | no `q`, and `?q=%20%20` | all todos, newest first |

The tests run against SQLite, as the app does. Non-ASCII case is not tested; see the known limitation above. The new file follows the existing test files and uses Swedish teaching comments.

### UI

The repo has no bUnit or Playwright, and this feature does not add them. Check the UI by hand in the running app:

1. Typing filters the list after the pause.
2. `?q=` appears in the URL.
3. Refreshing keeps the search.
4. Clearing the box shows every todo again.
5. A search with no results shows the "No todos match" message.

## Error handling

The API adds no new failure cases. In the UI, a failed fetch behaves as it does today. A request cancelled by the debounce, or superseded by a newer one, is dropped silently and not treated as an error.

## Out of scope

- Full-text search, ranking and stemming
- Filtering by done state
- Paging
- Matching non-ASCII case on SQLite
