# E-Commerce Microservices Migration Plan

## Overview
Convert your monolithic .NET application to independent microservices with SQL Server databases.

---

## Phase 1: Service Decomposition

### Current Monolith Structure
- **Users**: Authentication, profiles
- **Products**: Product catalog with categories
- **Cart**: Shopping cart operations
- **Orders**: Order management
- **Payments**: Payment processing
- **Addresses**: Delivery addresses

### Target Microservices Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     API Gateway (Ocelot)                   │
│              Route, Auth, Rate Limit, Logging              │
└─────────────────────────────────────────────────────────────┘
           │              │              │              │
    ┌──────▼────┐  ┌──────▼────┐  ┌──────▼────┐  ┌──────▼────┐
    │   Auth    │  │ Product   │  │   Cart    │  │  Order    │
    │  Service  │  │  Service  │  │  Service  │  │  Service  │
    └──────┬────┘  └──────┬────┘  └──────┬────┘  └──────┬────┘
      SQL DB1        SQL DB2         SQL DB3        SQL DB4
           │              │              │              │
           └──────────────┴──────────────┴──────────────┘
                        │
                  Message Queue (RabbitMQ/Azure Service Bus)
                  - OrderCreated
                  - PaymentProcessed
                  - InventoryUpdated
```

### Service Definition

| Service | Responsibility | Database | Dependencies |
|---------|----------------|----------|--------------|
| **Auth Service** | User login, JWT tokens, user profiles | AuthDB | - |
| **Product Service** | Product CRUD, categories, inventory | ProductDB | - |
| **Cart Service** | Add/remove items, view cart | CartDB | Product Service (HTTP) |
| **Order Service** | Create orders, order history | OrderDB | Cart, Product, User (HTTP) + Payment (async) |
| **Payment Service** | Process payments (async) | PaymentDB | Order Service (async event) |
| **Address Service** | User addresses | UserDB (shared with Auth) | Auth Service (HTTP) |

---

## Phase 2: Database Strategy (SQL Server)

### Option A: Database Per Service (Recommended)
**Pros**: Independent scaling, no shared state  
**Cons**: Data consistency challenges

```sql
-- Separate databases
CREATE DATABASE [Ecommerce_Auth]
CREATE DATABASE [Ecommerce_Product]
CREATE DATABASE [Ecommerce_Cart]
CREATE DATABASE [Ecommerce_Order]
CREATE DATABASE [Ecommerce_Payment]
```

**Implementation**: Each service gets its own DbContext pointing to separate DB.

### Option B: Shared Database with Separate Schemas (Transitional)
**Pros**: Easier initial migration  
**Cons**: Tight coupling

```sql
-- Single database, separate schemas
CREATE SCHEMA Auth
CREATE SCHEMA Product
CREATE SCHEMA Cart
CREATE SCHEMA [Order]
CREATE SCHEMA Payment
```

### Data Migration Strategy
1. Create new databases/schemas
2. Copy relevant tables for each service
3. Implement dual-write pattern (write to both old + new)
4. Gradually route reads to new databases
5. Remove dual-write when migration complete

---

## Phase 3: Service Extraction Order (Sequential)

### Step 1: Auth Service (Foundation)
- Most stable, no internal dependencies
- All other services depend on it for validation
- Extract JWT validation logic

### Step 2: Product Service
- No internal dependencies, lowest risk
- Used by Cart and Order services
- Expose via REST API

### Step 3: Address Service
- Depends on Auth Service only
- Lightweight, low risk

### Step 4: Cart Service
- Depends on Product and Auth
- Medium complexity

### Step 5: Order Service
- Depends on all previous services
- Most complex, event-driven

### Step 6: Payment Service
- Depends on Order Service (async)
- Can be extracted last

---

## Phase 4: Communication Patterns

### Synchronous (REST API)
```
Cart Service → Product Service: GET /api/products/{id}
Order Service → Auth Service: GET /api/auth/users/{userId}
```

### Asynchronous (Message Queue)
```
Order Service → (publishes) → OrderCreated Event
Payment Service → (consumes) → OrderCreated Event
Inventory Service → (publishes) → InventoryDecremented Event
Cart Service → (consumes) → InventoryDecremented Event
```

### API Gateway Routing
```
POST   /api/auth/login              → Auth Service
GET    /api/products                → Product Service
POST   /api/cart                    → Cart Service
GET    /api/orders/{id}             → Order Service
```

---

## Phase 5: Implementation Steps

### For Each Service, Follow This Template:

```
ServiceName/
├── ServiceName.API/
│   ├── Controllers/
│   ├── Program.cs
│   └── appsettings.json
├── ServiceName.BLL/
│   ├── Services/
│   └── Interfaces/
├── ServiceName.DAL/
│   ├── Repository/
│   ├── DbContext/
│   └── Models/
├── ServiceName.Models/
│   ├── DTOs/
│   └── Entities/
└── Dockerfile
```

### Key Changes Per Service

1. **Separate DbContext** (only relevant tables)
2. **Separate appsettings.json** (point to new database)
3. **Remove direct DAO dependencies** (use HTTP clients instead)
4. **Implement IHttpClientFactory** (for resilience)
5. **Add Polly for retries/circuit breaker**
6. **Message queue client** (RabbitMQ/Service Bus)

---

## Phase 6: Cross-Service Communication

### HTTP Client Setup (All Services)
```csharp
builder.Services.AddHttpClient<IProductClient>(client =>
{
    client.BaseAddress = new Uri("https://product-service:5001");
});
```

### Message Queue Setup
```csharp
// RabbitMQ
builder.Services.AddScoped<IMessageBus, RabbitMqBus>();

// Or Azure Service Bus
builder.Services.AddScoped<IMessageBus, AzureServiceBusMessageBus>();
```

---

## Phase 7: Deployment & Infrastructure

### Docker Compose (Local Development)
```yaml
version: '3.8'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: YourPassword123
  
  auth-service:
    build: ./Services/AuthService
    depends_on:
      - sqlserver
    ports:
      - "5001:5000"
  
  product-service:
    build: ./Services/ProductService
    depends_on:
      - sqlserver
    ports:
      - "5002:5000"
  
  api-gateway:
    build: ./ApiGateway
    ports:
      - "5000:5000"
    depends_on:
      - auth-service
      - product-service
```

### Kubernetes Deployment (Production)
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: product-service
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: product-service
        image: ecommerce/product-service:latest
```

---

## Phase 8: API Gateway (Ocelot)

### ocelot.json Configuration
```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/auth/{everything}",
      "DownstreamScheme": "https",
      "DownstreamHostAndPorts": [
        { "Host": "auth-service", "Port": 5001 }
      ],
      "UpstreamPathTemplate": "/api/auth/{everything}",
      "UpstreamHttpMethod": [ "GET", "POST", "PUT", "DELETE" ]
    },
    {
      "DownstreamPathTemplate": "/api/products/{everything}",
      "DownstreamScheme": "https",
      "DownstreamHostAndPorts": [
        { "Host": "product-service", "Port": 5002 }
      ],
      "UpstreamPathTemplate": "/api/products/{everything}"
    }
  ]
}
```

---

## Phase 9: Data Consistency Patterns

### Saga Pattern (Distributed Transactions)
```
Order Created → Reserve Inventory → Process Payment
   (success)          (success)        (success)
   
If Payment Fails:
   Reverse Inventory Reserve → Mark Order Failed
```

### Event Sourcing
All state changes published as events:
```
OrderCreated → PaymentProcessed → OrderCompleted
```

### Retry & Circuit Breaker (Polly)
```csharp
var policy = Policy.Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    .WaitAndRetryAsync(3, retryAttempt => 
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
    .WrapAsync(Policy.BulkheadAsync(10));
```

---

## Phase 10: Monitoring & Logging

### Centralized Logging (ELK Stack / Application Insights)
```csharp
builder.Services.AddApplicationInsightsTelemetry();
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.ApplicationInsights("instrumentation-key");
});
```

### Distributed Tracing (Jaeger/Zipkin)
```csharp
services.AddOpenTelemetry()
    .WithTracing(tracerProvider => tracerProvider
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddJaegerExporter());
```

---

## Execution Timeline

| Phase | Duration | Tasks |
|-------|----------|-------|
| **Phase 1-2** | 1-2 weeks | Planning, DB setup, branching |
| **Phase 3 (Auth)** | 1-2 weeks | Extract, test, deploy |
| **Phase 4 (Product)** | 1-2 weeks | Extract, expose API, test |
| **Phase 5 (Address)** | 1 week | Extract, integrate with Auth |
| **Phase 6 (Cart)** | 2 weeks | Extract, HTTP client to Product |
| **Phase 7 (Order)** | 2-3 weeks | Extract, message queue, saga pattern |
| **Phase 8 (Payment)** | 1-2 weeks | Extract, async processing |
| **Phase 9 (API Gateway)** | 1 week | Route all requests through gateway |
| **Phase 10 (Monitoring)** | Ongoing | Implement observability |

**Total: 2-3 months** (with parallel work)

---

## Risks & Mitigation

| Risk | Mitigation |
|------|-----------|
| **Data inconsistency** | Implement Saga pattern, eventual consistency |
| **Breaking changes** | API versioning, feature flags |
| **Network latency** | Circuit breaker, caching, async messaging |
| **Deployment complexity** | Docker, Kubernetes, CI/CD automation |
| **Testing difficulty** | Contract testing, integration tests with testcontainers |

---

## Quick Start: Extract First Service (Auth Service)

1. Create new project: `AuthService.API`
2. Create new database: `Ecommerce_Auth`
3. Copy User-related tables to new DB
4. Implement AuthService with minimal changes
5. Test locally with Docker Compose
6. Deploy to test environment
7. Update frontend to call Auth Service endpoint
8. Monitor and iterate

---

## Tools & Dependencies

- **Message Queue**: RabbitMQ or Azure Service Bus
- **API Gateway**: Ocelot or Azure API Management
- **Distributed Tracing**: Jaeger or Zipkin
- **Monitoring**: Application Insights or Prometheus + Grafana
- **Containerization**: Docker + Docker Compose
- **Orchestration**: Kubernetes or Azure Container Instances
- **Service Mesh** (optional): Istio or Linkerd

---

## Next Steps

1. Choose which service to extract first
2. Set up new database schema
3. Create new service project structure
4. Copy relevant code from monolith
5. Implement inter-service communication
6. Test end-to-end
7. Deploy and monitor
8. Repeat for next service

