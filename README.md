# CQRS Clean Architecture .NET API

A comprehensive .NET 7 Web API demonstrating **CQRS (Command Query Responsibility Segregation)** pattern with **Clean Architecture**, **Repository Pattern**, **Unit of Work**, and **MediatR**.

![.NET](https://img.shields.io/badge/.NET-7.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-11-239120?style=flat&logo=c-sharp)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?style=flat&logo=microsoft-sql-server)
![EF Core](https://img.shields.io/badge/EF%20Core-7.0-purple?style=flat)
![License](https://img.shields.io/badge/License-MIT-green.svg)

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Patterns Implemented](#patterns-implemented)
- [Database Schema](#database-schema)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [CQRS Flow](#cqrs-flow)
- [Domain Logic Examples](#domain-logic-examples)
- [EF Core Migrations](#ef-core-migrations)

---

## Overview

This project serves as a learning resource and reference implementation for building scalable .NET APIs using modern architectural patterns. It features an e-commerce domain with **Products**, **Categories**, and **Orders** to demonstrate real-world scenarios.

### Key Features

- **Clean Architecture** with clear separation of concerns
- **CQRS Pattern** separating read and write operations
- **MediatR** for in-process messaging and pipeline behaviors
- **Repository Pattern** abstracting data access
- **Unit of Work** for transaction management
- **FluentValidation** for request validation
- **Rich Domain Model** with business logic in entities
- **EF Core Migrations** for database versioning
- **Global Exception Handling** for consistent error responses

---

## Architecture

```
+---------------------------------------------------------+
|                      CQRS.Api                           |
|                  (Presentation Layer)                   |
|            Controllers, Middleware, DI Setup            |
+----------------------------+----------------------------+
                             |
                             v
+---------------------------------------------------------+
|                   CQRS.Application                      |
|                  (Application Layer)                    |
|        Commands, Queries, Handlers, Validators          |
+----------------------------+----------------------------+
                             |
                             v
+---------------------------------------------------------+
|                     CQRS.Domain                         |
|                    (Domain Layer)                       |
|       Entities, Business Rules, Domain Exceptions       |
+---------------------------------------------------------+
                             ^
                             |
+---------------------------------------------------------+
|                  CQRS.Infrastructure                    |
|                 (Infrastructure Layer)                  |
|       DbContext, Repositories, Unit of Work, Migrations |
+---------------------------------------------------------+
```

### Dependency Flow

- **API** -> Application -> Domain
- **Infrastructure** -> Application -> Domain
- **Domain has no dependencies** (innermost layer)

---

## Technologies Used

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 7.0 | Framework |
| ASP.NET Core Web API | 7.0 | HTTP API |
| Entity Framework Core | 7.0.20 | ORM |
| SQL Server LocalDB | - | Database |
| MediatR | 12.1.1 | CQRS & Mediator Pattern |
| FluentValidation | 11.9.0 | Request Validation |
| AutoMapper | 12.0.1 | Object Mapping |
| Swagger/OpenAPI | - | API Documentation |

---

## Project Structure

```
CQRS/
|-- src/
|   |-- CQRS.Domain/                      # Enterprise Business Rules
|   |   +-- Entities/
|   |       |-- Category.cs
|   |       |-- Product.cs
|   |       |-- Order.cs                  # Rich domain model
|   |       |-- OrderItem.cs
|   |       +-- DomainException.cs
|   |
|   |-- CQRS.Application/                 # Application Business Rules
|   |   |-- Common/
|   |   |   |-- Behaviours/
|   |   |   |   +-- ValidationBehaviour.cs
|   |   |   |-- Interfaces/
|   |   |   |   |-- IRepository.cs        # Generic repository
|   |   |   |   |-- ICategoryRepository.cs
|   |   |   |   |-- IProductRepository.cs
|   |   |   |   |-- IOrderRepository.cs
|   |   |   |   |-- IUnitOfWork.cs
|   |   |   |   +-- IApplicationDbContext.cs
|   |   |   +-- Mappings/
|   |   |       +-- MappingProfile.cs
|   |   |-- Categories/
|   |   |   |-- Commands/
|   |   |   |   +-- CreateCategory/
|   |   |   +-- Queries/
|   |   |       |-- GetCategories/
|   |   |       +-- GetCategoryById/
|   |   |-- Products/
|   |   |   |-- Commands/
|   |   |   |   |-- CreateProduct/
|   |   |   |   |-- UpdateProduct/
|   |   |   |   +-- DeleteProduct/
|   |   |   +-- Queries/
|   |   |       |-- GetProducts/
|   |   |       +-- GetProductById/
|   |   |-- Orders/
|   |   |   |-- Commands/
|   |   |   |   |-- CreateOrder/
|   |   |   |   |-- AddItemToOrder/
|   |   |   |   |-- SubmitOrder/
|   |   |   |   +-- CancelOrder/
|   |   |   +-- Queries/
|   |   |       |-- GetOrders/
|   |   |       +-- GetOrderById/
|   |   +-- DependencyInjection.cs
|   |
|   |-- CQRS.Infrastructure/              # External Concerns
|   |   |-- Data/
|   |   |   |-- ApplicationDbContext.cs
|   |   |   |-- ApplicationDbContextSeed.cs
|   |   |   +-- Migrations/               # EF Core Migrations
|   |   |       |-- 20260116_InitialCreate.cs
|   |   |       +-- ApplicationDbContextModelSnapshot.cs
|   |   |-- Repositories/
|   |   |   |-- Repository.cs             # Generic repository
|   |   |   |-- CategoryRepository.cs
|   |   |   |-- ProductRepository.cs
|   |   |   |-- OrderRepository.cs
|   |   |   +-- UnitOfWork.cs
|   |   +-- DependencyInjection.cs
|   |
|   +-- CQRS.Api/                         # Presentation Layer
|       |-- Controllers/
|       |   |-- CategoriesController.cs
|       |   |-- ProductsController.cs
|       |   +-- OrdersController.cs
|       |-- Middleware/
|       |   +-- ExceptionHandlingMiddleware.cs
|       |-- Program.cs
|       +-- appsettings.json
|
|-- .config/
|   +-- dotnet-tools.json                 # Local EF Core tools
|
+-- CQRS.sln
```

---

## Patterns Implemented

### 1. CQRS (Command Query Responsibility Segregation)

Separates read and write operations into distinct models:

```
+-------------+         +---------------------+
|   Command   |-------->|   CommandHandler    |----> Write to DB
|   (Write)   |         |  (Business Logic)   |
+-------------+         +---------------------+

+-------------+         +---------------------+
|    Query    |-------->|    QueryHandler     |----> Read from DB
|   (Read)    |         |  (Data Retrieval)   |
+-------------+         +---------------------+
```

**Commands** (modify state):
- `CreateCategoryCommand`
- `CreateProductCommand`, `UpdateProductCommand`, `DeleteProductCommand`
- `CreateOrderCommand`, `AddItemToOrderCommand`, `SubmitOrderCommand`, `CancelOrderCommand`

**Queries** (return data):
- `GetCategoriesQuery`, `GetCategoryByIdQuery`
- `GetProductsQuery`, `GetProductByIdQuery`
- `GetOrdersQuery`, `GetOrderByIdQuery`

### 2. MediatR Pipeline

```
Request --> ValidationBehaviour --> Handler --> Response
                    |
                    +--> Throws ValidationException if invalid
```

### 3. Repository Pattern

Abstracts data access behind interfaces:

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
}
```

### 4. Unit of Work

Manages transactions and coordinates repository operations:

```csharp
public interface IUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

### 5. Rich Domain Model

Business logic encapsulated in domain entities:

```csharp
public class Order
{
    public void Submit()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("Only draft orders can be submitted");

        if (!OrderItems.Any())
            throw new DomainException("Cannot submit an empty order");

        if (TotalAmount < 10)
            throw new DomainException("Minimum order amount is $10");

        Status = OrderStatus.Submitted;
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            throw new DomainException("Cannot cancel shipped or delivered orders");

        Status = OrderStatus.Cancelled;
        Notes = $"Cancelled: {reason}";
    }
}
```

---

## Database Schema

```
+----------------+       +----------------+       +----------------+
|   Categories   |       |    Products    |       |     Orders     |
+----------------+       +----------------+       +----------------+
| Id (PK)        |<---+  | Id (PK)        |  +--->| Id (PK)        |
| Name           |    |  | Name           |  |    | OrderNumber    |
| Description    |    |  | Description    |  |    | OrderDate      |
+----------------+    |  | Price          |  |    | Status         |
                      |  | CategoryId(FK) |--+    | SubTotal       |
                      +--|                |       | DiscountAmount |
                         +----------------+       | TaxAmount      |
                               ^                  | TotalAmount    |
                               |                  | Notes          |
                               |                  +----------------+
                               |                         ^
                               |                         |
                         +----------------+              |
                         |   OrderItems   |--------------+
                         +----------------+
                         | Id (PK)        |
                         | OrderId (FK)   |
                         | ProductId (FK) |
                         | ProductName    |
                         | UnitPrice      |
                         | Quantity       |
                         | LineTotal      |
                         +----------------+
```

---

## Getting Started

### Prerequisites

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with Visual Studio)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/CQRS-CleanArchitecture.git
   cd CQRS-CleanArchitecture
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Restore local tools (EF Core)**
   ```bash
   dotnet tool restore
   ```

4. **Build the solution**
   ```bash
   dotnet build
   ```

5. **Run the API**
   ```bash
   dotnet run --project src/CQRS.Api
   ```

6. **Open Swagger UI**
   ```
   http://localhost:5000/swagger
   ```

### Database

The database is automatically created and migrations are applied on first run. No seed data is included - add data through the API or SQL Server Management Studio.

**Connection String** (appsettings.json):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CqrsDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

---

## API Endpoints

### Categories

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories` | Get all categories |
| GET | `/api/categories/{id}` | Get category with products |
| POST | `/api/categories` | Create a new category |

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products?categoryId={id}` | Filter by category |
| GET | `/api/products/{id}` | Get product details |
| POST | `/api/products` | Create a new product |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |

### Orders

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | Get all orders |
| GET | `/api/orders?status={status}` | Filter by status |
| GET | `/api/orders/{id}` | Get order with items |
| POST | `/api/orders` | Create order with items |
| POST | `/api/orders/{id}/items` | Add item to order |
| POST | `/api/orders/{id}/submit` | Submit order |
| POST | `/api/orders/{id}/cancel` | Cancel order |

---

## CQRS Flow

### Command Flow (Write Operation)

```
HTTP POST /api/orders
        |
        v
+-------------------+
| OrdersController  |
|                   |
| _mediator.Send()  |
+---------+---------+
          |
          v
+---------------------------+
|     MediatR Pipeline      |
|                           |
| +------------------------+|
| | ValidationBehaviour    ||
| | (FluentValidation)     ||
| +-----------+------------+|
|             v             |
| +------------------------+|
| | CreateOrderHandler     ||
| +------------------------+|
+-------------+-------------+
              |
              v
+---------------------------+
|       Domain Logic        |
|                           |
| Order.Create()            |
| Order.AddItem()           |
| Order.CalculateTotals()   |
+-------------+-------------+
              |
              v
+---------------------------+
|       Unit of Work        |
|                           |
| BeginTransaction()        |
| Orders.Add(order)         |
| SaveChangesAsync()        |
| CommitTransaction()       |
+---------------------------+
```

### Query Flow (Read Operation)

```
HTTP GET /api/orders/1
        |
        v
+-------------------+
| OrdersController  |
|                   |
| _mediator.Send()  |
+---------+---------+
          |
          v
+---------------------------+
| GetOrderByIdQueryHandler  |
|                           |
| _unitOfWork.Orders        |
|   .GetOrderWithItemsAsync |
+-------------+-------------+
              |
              v
+---------------------------+
|      AutoMapper           |
|                           |
| Order -> OrderDetailDto   |
+---------------------------+
```

---

## Domain Logic Examples

### Order Business Rules

```csharp
public class Order
{
    // Rule: Calculate discount based on order total
    private decimal CalculateDiscount()
    {
        // 10% discount for orders over $500
        if (SubTotal >= 500)
            return SubTotal * 0.10m;

        // 5% discount for 5+ items
        if (OrderItems.Sum(x => x.Quantity) >= 5)
            return SubTotal * 0.05m;

        return 0;
    }

    // Rule: 10% tax applied after discount
    private decimal CalculateTax()
    {
        return (SubTotal - DiscountAmount) * 0.10m;
    }
}
```

### Example: Create Order Request

```bash
POST /api/orders
Content-Type: application/json

{
  "items": [
    { "productId": 1, "quantity": 2 },
    { "productId": 2, "quantity": 1 }
  ]
}
```

### Example: Response (with discount applied)

```json
{
  "orderId": 1,
  "orderNumber": "ORD-20260116-A1B2C3D4",
  "subTotal": 600.00,
  "discountAmount": 60.00,
  "discountReason": "10% discount for orders over $500",
  "taxAmount": 54.00,
  "totalAmount": 594.00
}
```

---

## EF Core Migrations

This project uses EF Core Migrations for database versioning. A local tool manifest is configured for consistent versioning.

### Common Commands

```bash
# Add a new migration
dotnet dotnet-ef migrations add <MigrationName> \
  --project src/CQRS.Infrastructure \
  --startup-project src/CQRS.Api \
  --output-dir Data/Migrations

# Update database to latest migration
dotnet dotnet-ef database update \
  --project src/CQRS.Infrastructure \
  --startup-project src/CQRS.Api

# Remove last migration (if not applied)
dotnet dotnet-ef migrations remove \
  --project src/CQRS.Infrastructure \
  --startup-project src/CQRS.Api

# Generate SQL script
dotnet dotnet-ef migrations script \
  --project src/CQRS.Infrastructure \
  --startup-project src/CQRS.Api \
  --output migrations.sql

# Drop database
dotnet dotnet-ef database drop \
  --project src/CQRS.Infrastructure \
  --startup-project src/CQRS.Api \
  --force
```

### Auto-Migration on Startup

Migrations are automatically applied when the application starts:

```csharp
// Program.cs
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}
```

---

## Error Handling

The API uses a global exception handler that converts exceptions to consistent responses:

**Domain Exception (400 Bad Request):**
```json
{
  "type": "DomainError",
  "message": "Only draft orders can be submitted"
}
```

**Validation Exception (400 Bad Request):**
```json
{
  "type": "ValidationError",
  "message": "Validation failed",
  "errors": [
    {
      "property": "Items",
      "message": "Order must contain at least one item"
    }
  ]
}
```

**Server Error (500 Internal Server Error):**
```json
{
  "type": "ServerError",
  "message": "An unexpected error occurred"
}
```

---

## Why These Patterns?

### Why CQRS?

| Benefit | Explanation |
|---------|-------------|
| **Separation of Concerns** | Read and write models can evolve independently |
| **Scalability** | Can scale read and write sides separately |
| **Optimized Queries** | Queries can be optimized without affecting commands |
| **Simplified Models** | Each side only contains what it needs |

### Why Repository + Unit of Work?

| Benefit | Explanation |
|---------|-------------|
| **Abstraction** | Easy to switch data sources or add caching |
| **Testability** | Mock repositories in unit tests |
| **Transaction Management** | Explicit control over transactions |
| **Complex Queries** | Encapsulate complex query logic |

### Why Rich Domain Model?

| Benefit | Explanation |
|---------|-------------|
| **Consistency** | Rules enforced regardless of caller |
| **Discoverability** | Logic is where you expect it |
| **Testability** | Test business rules in isolation |
| **DRY** | No duplicate validation across handlers |

---

## License

This project is licensed under the MIT License.

---

## References

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS by Martin Fowler](https://martinfowler.com/bliki/CQRS.html)
- [MediatR by Jimmy Bogard](https://github.com/jbogard/MediatR)
- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
