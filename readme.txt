Time taken: 2h30m

Bugs found and fixed
--------------------
- InterviewTestPagination.Tests.csproj did not include Models\TodoServiceTest.cs, so the tests never compiled.
- The test project referenced MSTest v1 (a DLL that ships with Visual Studio), which is not discoverable outside VS.
  Migrated to MSTest v2 (MSTest.TestFramework / MSTest.TestAdapter 2.2.10 via NuGet). No test code changes needed.
- TodoController worked by accident: 'api/Todo/Todos' matched the default route 'api/{controller}/{id}' with id = "Todos".
  Adding a second GET action would break it. Added attribute routing ([RoutePrefix] / [Route]).
- TodoController bypassed the service layer (called _todoService.Repository.All() directly).
- Requests made straight from the browser failed: content negotiation picked the XML formatter and Todo is not
  XML-serializable (no parameterless constructor). Removed the XmlFormatter since the api is JSON only.
- main.js used an ES6 arrow function while the stack is ES5.

Decisions
---------
- Contract: GET api/Todo/Todos?page&pageSize&sortBy&sortDesc
  -> PagedResult<T> { items, page, pageSize, totalItems, totalPages }
- pageSize = 0 means "all". Out of range pages are clamped by the server, which returns the page it actually used.
- Sorting is done server-side over the whole list, before paging. Sortable properties are whitelisted
  (id, task, createdDate; case insensitive). Unknown/empty -> default sort (createdDate desc).
- Layers: controller (http) -> service (paging rules) -> repository (count / sorted skip-take).
  The repository doesn't know what a "page" is; PageRequest/PagedResult<T> are generic and reusable by other models.
- The pagination directive is model agnostic: isolated scope + onChange({ page, pageSize }) callback.
  The list directive owns the state and fetches the data; the pagination only displays it and notifies changes.
- Responses of outdated requests are discarded (fast clicking can't show the wrong page).
- 12 unit tests for TodoService, including one with a fake repository to test the service in isolation.

Notes / with more time
----------------------
- Sorting by 'task' is alphabetical ("do 1", "do 10", "do 11", ...), as expected for a string property.
- Dependency injection for the service/repository (the controller still news up TodoService).
- I usually work with xUnit; kept MSTest since it came with the project.
