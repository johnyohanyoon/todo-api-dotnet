# Todo API - .NET 9 Backend

A RESTful API built with .NET 9, Entity Framework Core, and Azure SQL Database.

## Features
- Full CRUD operations for todo items
- Server-side pagination (handles 100,000+ records efficiently)
- Input validation with data annotations
- Swagger/OpenAPI documentation
- CORS configuration for Angular frontend
- Azure SQL Database with retry logic
- CI/CD via GitHub Actions

## Tech Stack
- .NET 9
- Entity Framework Core 9
- Azure SQL Database
- NSwag (Swagger generation)
- Azure App Service

## Architecture
```
Client → API Layer → Service Layer → EF Core → Azure SQL Database
```

## API Endpoints
### Get All Todos (Paginated)
```http
GET /api/TodoItems?pageNumber=1&pageSize=20
```

### Get Todo by ID
```http
GET /api/TodoItems/{id}
```

### Create Todo
```http
POST /api/TodoItems
Content-Type: application/json

{
  "name": "New todo item",
  "isComplete": false
}
```

### Update Todo
```http
PUT /api/TodoItems/{id}
Content-Type: application/json

{
  "id": 1,
  "name": "Updated todo",
  "isComplete": true
}
```

### Delete Todo
```http
DELETE /api/TodoItems/{id}
```


## Local Development
### Prerequisites
- .NET 9 SDK
- Azure SQL Database (or SQL Server)

### Setup

1. Clone the repository
```bash
git clone https://github.com/johnyohanyoon/todo-api-dotnet.git
cd todo-api-dotnet
```

2. Configure User Secrets
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "SECRET_CONNECTION_STRING"
```

3. Run migrations
```bash
dotnet ef database update
```

4. Run the application
```bash
dotnet run
```

5. Access Swagger UI
```
https://localhost:5126/swagger
```
