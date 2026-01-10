# eShop Microservices

A microservices-based eCommerce platform built with .NET 8.0, implementing Domain-Driven Design (DDD), CQRS, and Clean Architecture principles.

This project demonstrates a modern approach to building distributed systems using microservices architecture. Each service is independently deployable, has its own database (Database per Service pattern), and communicates with other services through well-defined APIs. The solution showcases enterprise-level patterns including vertical slice architecture, the mediator pattern for decoupling, and cross-cutting concerns handled through shared building blocks.

The platform consists of four core microservices that work together to provide a complete eCommerce experience:
- **Catalog Service** handles product inventory and catalog browsing
- **Basket Service** manages shopping carts with Redis-backed distributed caching
- **Discount Service** provides coupon and discount functionality via high-performance gRPC
- **Ordering Service** processes orders using full DDD tactical patterns including aggregates, value objects, and domain events

The architecture emphasizes separation of concerns, testability, and scalability. Services use different database technologies based on their specific needs (PostgreSQL with Marten for document storage, SQL Server with Entity Framework Core for relational data, SQLite for lightweight storage), demonstrating polyglot persistence in practice.

## Architecture Overview

```
                    ┌─────────────────┐
                    │   API Gateway   │
                    └────────┬────────┘
                             │
       ┌─────────────────────┼─────────────────────┐
       │                     │                     │
       ▼                     ▼                     ▼
┌─────────────┐      ┌─────────────┐      ┌─────────────┐
│  Catalog    │      │   Basket    │◄────►│  Discount   │
│   Service   │      │   Service   │ gRPC │   Service   │
└──────┬──────┘      └──────┬──────┘      └──────┬──────┘
       │                    │                    │
       ▼                    ▼                    ▼
  PostgreSQL           PostgreSQL             SQLite
   (Marten)         (Marten + Redis)

                    ┌─────────────┐
                    │  Ordering   │
                    │   Service   │
                    └──────┬──────┘
                           │
                           ▼
                      SQL Server
```

## Microservices

| Service | Description | Database | Port |
|---------|-------------|----------|------|
| **Catalog.API** | Product catalog management | PostgreSQL (Marten) | 6000 |
| **Basket.API** | Shopping cart with distributed caching | PostgreSQL + Redis | 6001 |
| **Discount.Grpc** | Coupon/discount management via gRPC | SQLite | 6002 |
| **Ordering.API** | Order processing with DDD patterns | SQL Server | - |

## Technologies

### Core
- .NET 8.0 / ASP.NET Core 8.0
- C# with nullable reference types

### Architecture & Patterns
- **MediatR** - CQRS and mediator pattern
- **FluentValidation** - Request validation pipeline
- **Mapster** - Object mapping
- **Carter** - Minimal API endpoints

### Data Access
- **Entity Framework Core** - ORM for Ordering/Discount
- **Marten** - Document database for Catalog/Basket
- **StackExchange.Redis** - Distributed caching

### Communication
- **gRPC** - Inter-service communication (Basket → Discount)
- **Protocol Buffers** - gRPC contract definitions

### Infrastructure
- **Docker** - Containerization
- **Docker Compose** - Local orchestration
- **Health Checks** - Service health monitoring

## Project Structure

```
eshop-microservicesDotNet/
├── BuildingBlocks/
│   └── BuildingBlocks/
│       ├── Behaviors/           # MediatR pipeline behaviors
│       ├── CQRS/                # Command/Query interfaces
│       └── Exceptions/          # Custom exception handling
│
├── Services/
│   ├── Catalog/
│   │   └── Catalog.API/         # Product catalog service
│   │
│   ├── Basket/
│   │   └── Basket.API/          # Shopping cart service
│   │
│   ├── Discount/
│   │   └── Discount.Grpc/       # gRPC discount service
│   │
│   └── Ordering/
│       ├── Ordering.API/        # API layer
│       ├── Ordering.Application/# Use cases & CQRS handlers
│       ├── Ordering.Domain/     # Domain entities & events
│       └── Ordering.Infrastructure/ # Data access & EF Core
│
├── docker-compose.yml
├── docker-compose.override.yml
└── eshop-microservices.sln
```

## Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Running with Docker Compose

```bash
# Clone the repository
git clone <repository-url>
cd eshop-microservicesDotNet

# Start all services
docker-compose up -d

# View running containers
docker-compose ps

# View logs
docker-compose logs -f
```

### Service Endpoints

| Service | HTTP | HTTPS |
|---------|------|-------|
| Catalog API | http://localhost:6000 | https://localhost:6060 |
| Basket API | http://localhost:6001 | https://localhost:6061 |
| Discount gRPC | http://localhost:6002 | https://localhost:6062 |

### Health Checks

Each service exposes a `/health` endpoint for monitoring:
- http://localhost:6000/health (Catalog)
- http://localhost:6001/health (Basket)

## Architecture Patterns

### Domain-Driven Design (Ordering Service)

The Ordering service demonstrates full DDD implementation:

- **Aggregate Root**: `Order` manages `OrderItem` entities
- **Value Objects**: `OrderId`, `CustomerId`, `Address`, `Payment`
- **Domain Events**: `OrderCreatedEvent`, `OrderUpdatedEvent`
- **Repository Pattern**: Abstracted via `IApplicationDbContext`

### CQRS Pattern

Commands and queries are separated using MediatR:

```csharp
// Command
public record CreateOrderCommand(OrderDto Order) : ICommand<CreateOrderResult>;

// Query
public record GetProductsQuery() : IQuery<GetProductsResult>;
```

### Cross-Cutting Concerns (BuildingBlocks)

1. **ValidationBehavior** - Automatic FluentValidation before handlers
2. **LoggingBehavior** - Request/response logging with performance warnings
3. **CustomExceptionHandler** - Standardized ProblemDetails responses

### Decorator Pattern (Basket Service)

```csharp
// CachedBasketRepository decorates BasketRepository for Redis caching
services.AddScoped<IBasketRepository, CachedBasketRepository>();
```

## API Examples

### Catalog Service

```bash
# Get all products
GET /products

# Get product by ID
GET /products/{id}

# Create product
POST /products
{
  "name": "Product Name",
  "category": ["Category1"],
  "description": "Description",
  "imageFile": "image.png",
  "price": 99.99
}

# Get products by category
GET /products/category/{category}
```

### Basket Service

```bash
# Get basket by username
GET /basket/{userName}

# Store/update basket
POST /basket
{
  "userName": "user1",
  "items": [
    {
      "quantity": 2,
      "color": "Red",
      "price": 99.99,
      "productId": "guid",
      "productName": "Product"
    }
  ]
}

# Delete basket
DELETE /basket/{userName}
```

### Discount Service (gRPC)

```protobuf
service DiscountProtoService {
  rpc GetDiscount (GetDiscountRequest) returns (CouponModel);
  rpc CreateDiscount (CreateDiscountRequest) returns (CouponModel);
  rpc UpdateDiscount (UpdateDiscountRequest) returns (CouponModel);
  rpc DeleteDiscount (DeleteDiscountRequest) returns (DeleteDiscountResponse);
}
```

## Database Configuration

| Service | Database | Connection |
|---------|----------|------------|
| Catalog | PostgreSQL | `catalogdb:5432` |
| Basket | PostgreSQL + Redis | `basketdb:5432`, `distributedcache:6379` |
| Discount | SQLite | File-based |
| Ordering | SQL Server | `orderdb:1433` |

## Development

### Running Individual Services

```bash
# Catalog API
cd Services/Catalog/Catalog.API
dotnet run

# Basket API
cd Services/Basket/Basket.API
dotnet run

# Discount gRPC
cd Services/Discount/Discount.Grpc
dotnet run

# Ordering API
cd Services/Ordering/Ordering.API
dotnet run
```

### Database Migrations (Ordering Service)

```bash
cd Services/Ordering/Ordering.Infrastructure
dotnet ef migrations add InitialCreate -s ../Ordering.API
dotnet ef database update -s ../Ordering.API
```

## Docker Services

```yaml
Services:
  catalogdb:      PostgreSQL (5432)
  basketdb:       PostgreSQL (5433)
  distributedcache: Redis (6379)
  orderdb:        SQL Server (1433)
  catalog.api:    Catalog Service
  basket.api:     Basket Service
  discount.grpc:  Discount Service
```

## License

This project is licensed under the MIT License.
