# MyProject

An ASP.NET Core 8 Web API with separate DataAccess, Repository, Business, and API projects.

## Architecture

`API (controller) -> Business (service) -> Repository -> DataAccess (EF Core DbContext) -> SQL Server`

- **DataAccess** defines the `Item` entity, `ItemState` enum, EF Core context, and SQL Server mapping.
- **Repository** owns EF Core queries and persistence behind `IItemRepository`.
- **Business** defines the shared `ItemRequest` and `ItemResponse` DTOs, implements `IItemService`, applies the requested operation, and maps entities to responses.
- **API** validates HTTP input, passes the shared request DTO to the business service, and exposes the endpoints.

`ItemState` is defined in DataAccess because it is used by both the persisted entity and API request model. Keeping it outside the API prevents the database project from depending on the presentation layer.

Each project keeps its namespace imports in a `GlobalUsings.cs` file instead of repeating `using` directives in class files.

## Configure and run

The default connection string in `API/appsettings.json` uses SQL Server LocalDB. Change `ConnectionStrings:DefaultConnection` for another SQL Server instance. Ensure the target SQL Server is running and accessible.

The initial migration is included in `DataAccess/Migrations`. From the solution directory, install the EF CLI tool if needed and apply the migration:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.22
dotnet ef database update --project DataAccess\MyProject.DataAccess.csproj --startup-project API\MyProject.API.csproj
dotnet run --project API\MyProject.API.csproj
```

If `dotnet-ef` is already installed, update it instead of installing it again:

```powershell
dotnet tool update --global dotnet-ef --version 8.0.22
```

For later model changes, create a new migration before applying it:

```powershell
dotnet ef migrations add <MigrationName> --project DataAccess\MyProject.DataAccess.csproj --startup-project API\MyProject.API.csproj --output-dir Migrations
dotnet ef database update --project DataAccess\MyProject.DataAccess.csproj --startup-project API\MyProject.API.csproj
```

Swagger is enabled in the Development environment.

## Endpoints

### Add an item

`POST /api/items`

```json
{
  "id": 0,
  "name": "Keyboard",
  "description": "Mechanical keyboard",
  "price": 79.99,
  "itemState": 1
}
```

`ItemState.Add` (1) always inserts a new item. The generated item is returned with `201 Created`.

### Add or update an item

Insert by setting `id` to `0`:

```json
{
  "id": 0,
  "name": "Mouse",
  "description": "Wireless mouse",
  "price": 39.5,
  "itemState": 2
}
```

Update by supplying an existing ID:

```json
{
  "id": 1,
  "name": "Keyboard Pro",
  "description": "Updated model",
  "price": 99.99,
  "itemState": 2
}
```

`ItemState.AddOrEdit` (2) inserts when `id` is `0`, updates when `id` is greater than `0`, and returns `404 Not Found` when the requested item does not exist.

### Delete an item

`POST /api/items`

```json
{
  "id": 1,
  "name": "",
  "description": null,
  "price": 0,
  "itemState": 3
}
```

`ItemState.Delete` (3) deletes by ID and returns `204 No Content`, or `404 Not Found` if the item is missing.

### Get all items

`GET /api/items`

Returns an array of item response objects.

### Get an item by ID

`GET /api/items/1`

Returns an item response object, or `404 Not Found`.

All response objects contain `id`, `name`, `description`, `price`, and `itemState`. Invalid request bodies return `400 Bad Request`; unhandled exceptions are emitted as Problem Details responses.
