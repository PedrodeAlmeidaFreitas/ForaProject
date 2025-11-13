# ForaProject - GitHub Copilot Instructions

## Project Overview

ForaProject is a .NET 8 application built following Clean Architecture principles, Domain-Driven Design (DDD), and SOLID principles.

## Architecture Layers

### 1. Domain Layer (ForaProject.Domain)

- **Core business logic and entities**
- Contains:
  - **Entities**: Rich domain models with business logic and invariants
  - **Value Objects**: Immutable objects defined by their attributes
  - **Aggregates**: Cluster of domain objects treated as a single unit
  - **Interfaces**: Repository and service contracts
  - **Events**: Domain events for cross-aggregate communication
- **Rules**:
  - No dependencies on other layers
  - Pure business logic only
  - No infrastructure concerns
  - Use private setters and factories for entity creation
  - Validate invariants in constructors

### 2. Application Layer (ForaProject.Application)

- **Application business rules and use cases**
- Contains:
  - **Services**: Application services orchestrating use cases
  - **DTOs**: Data Transfer Objects for external communication
  - **Interfaces**: Service contracts and abstractions
  - **Validators**: FluentValidation validators for DTOs
  - **Mappings**: AutoMapper profiles for object mapping
- **Rules**:
  - Depends only on Domain layer
  - No database or infrastructure code
  - Implement CQRS pattern when applicable
  - Use DTOs for input/output
  - Validate all inputs using FluentValidation

### 3. Infrastructure Layer (ForaProject.Infrastructure)

- **External concerns and implementations**
- Contains:
  - **Data/Contexts**: EF Core DbContext
  - **Data/Configurations**: Entity configurations (Fluent API)
  - **Data/Repositories**: Repository implementations
  - **Services**: External service implementations
  - **CrossCutting**: Logging, caching, etc.
- **Rules**:
  - Implements Domain and Application interfaces
  - Contains all EF Core configurations
  - Handle database migrations
  - Implement Unit of Work pattern
  - Use dependency injection

### 4. API Layer (ForaProject.API)

- **Presentation layer and HTTP endpoints**
- Contains:
  - **Controllers**: REST API endpoints
  - **Middlewares**: Request/response pipeline components
  - **Filters**: Action filters, exception filters
  - **Program.cs**: Application bootstrapping and DI configuration
- **Rules**:
  - Thin controllers (delegate to Application services)
  - Use API versioning
  - Implement global exception handling
  - Return proper HTTP status codes
  - Use async/await for all I/O operations

## Design Principles

### SOLID Principles

- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived classes must be substitutable for base classes
- **Interface Segregation**: Many specific interfaces over one general interface
- **Dependency Inversion**: Depend on abstractions, not concretions

### DRY (Don't Repeat Yourself)

- Extract common logic into reusable methods/classes
- Use inheritance and composition appropriately
- Create base classes for shared behavior

### KISS (Keep It Simple, Stupid)

- Prefer simple solutions over complex ones
- Write readable and maintainable code
- Avoid premature optimization

### DDD Patterns

- Use Ubiquitous Language from domain experts
- Encapsulate business logic in Aggregates
- Define clear Bounded Contexts
- Use Value Objects for concepts without identity
- Raise Domain Events for important business occurrences

## Coding Standards

### Naming Conventions

- **Classes/Interfaces**: PascalCase (e.g., `OrderService`, `IRepository`)
- **Methods**: PascalCase (e.g., `CreateOrder`, `GetById`)
- **Properties**: PascalCase (e.g., `CustomerId`, `OrderDate`)
- **Private fields**: \_camelCase (e.g., `_repository`, `_logger`)
- **Parameters/Variables**: camelCase (e.g., `orderId`, `customerName`)

### Code Organization

- One class per file
- File name matches class name
- Group related classes in appropriate folders
- Keep methods small and focused (< 20 lines ideally)

### Error Handling

- Use custom exception classes for domain errors
- Implement global exception handling middleware
- Log all exceptions with appropriate levels
- Return meaningful error messages in API responses
- Use Result pattern for operation outcomes

### Async/Await

- Always use async/await for I/O operations
- Suffix async methods with "Async"
- Avoid async void (except event handlers)
- Use ConfigureAwait(false) in library code

### Testing

- Write unit tests for all business logic
- Use AAA pattern (Arrange, Act, Assert)
- Mock external dependencies
- Achieve minimum 80% code coverage
- Write integration tests for critical paths

### Dependency Injection

- Register services in Program.cs
- Use constructor injection
- Prefer interfaces over concrete types
- Use appropriate service lifetimes (Scoped, Transient, Singleton)

### Entity Framework Core

- Use Fluent API for entity configuration
- Keep configurations in separate files
- Use migrations for schema changes
- Implement soft delete pattern
- Add audit fields (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)

### API Best Practices

- Use proper HTTP verbs (GET, POST, PUT, DELETE, PATCH)
- Version your APIs (e.g., /api/v1/orders)
- Use DTOs for requests/responses
- Implement pagination for list endpoints
- Add XML comments for Swagger documentation
- Use ActionResult<T> return types

## Example Patterns

### Repository Pattern

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
```

### Service Pattern

```csharp
public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderDto> GetOrderByIdAsync(Guid id);
}
```

### Entity Base Class

```csharp
public abstract class Entity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
}
```

## When Generating Code

1. **Always** follow the layer responsibilities
2. **Always** use dependency injection
3. **Always** implement interfaces before concrete classes
4. **Always** add XML documentation comments
5. **Always** use async/await for I/O operations
6. **Always** validate inputs
7. **Always** handle exceptions appropriately
8. **Always** write testable code
9. **Never** put business logic in controllers
10. **Never** reference Infrastructure from Domain
11. **Never** use concrete types when abstraction exists
12. **Never** expose entities directly through API

## Questions to Ask Before Coding

1. Which layer does this belong to?
2. Does this violate any SOLID principles?
3. Is this code reusable and testable?
4. Are dependencies properly abstracted?
5. Does this follow DDD patterns?
6. Is error handling implemented?
7. Are there appropriate validations?
8. Is this documented clearly?

---

**Remember**: Clean architecture is about separating concerns and maintaining clear boundaries. When in doubt, favor simplicity and follow established patterns.
