# ProcessingOrders

ProcessingOrders is a microservice for processing orders, built using Domain-Driven Design (DDD) principles.

## Project Structure

The project is organized according to the principles of clean architecture and DDD:

- **ProcessingOrders.CoreDomain** - the core domain, containing business entities, rules, and repository interfaces
- **ProcessingOrders.Application** - contains the application business logic, implemented through commands/queries (CQRS)
- **ProcessingOrders.Api** - the API layer, providing an HTTP interface for working with orders
- **ProcessingOrders.Persistence** - data access implementation, including repositories and Entity Framework
- **ProcessingOrders.Infrastructure** - infrastructure services, including a GRPC client for interacting with the promotions microservice

## Main Features

- Order creation
- Order status management (processing, completion, cancellation)
- Adding and removing items from the order
- Automatic total cost calculation
- Retrieving order information and a list of customer orders
- Integration with the promotions service via gRPC (stub)

## Running the Project

1. Install .NET 7.0 SDK
2. Configure the connection string in `appsettings.json`
3. Apply database migrations: `dotnet ef database update --project ProcessingOrders.Persistence --startup-project ProcessingOrders.Api`
4. Run the API: `dotnet run --project ProcessingOrders.Api`

## API

The API is documented using Swagger. After starting the application, the documentation is available at `/swagger`.

Main endpoints:

- `POST /api/orders` - create a new order
- `PUT /api/orders/{id}/status` - update order status
- `GET /api/orders/{id}` - get order by ID
- `GET /api/orders/customer/{customerId}` - get a list of customer orders

## Domain Model

### Order Aggregate

The main aggregate is `Order`. The aggregate consists of:
- `Order` - the aggregate root
- `OrderItem` - an item in the order
- `OrderEvent` - an order event

### CQRS Support

The application uses the CQRS pattern through MediatR:
- Commands: CreateOrderCommand, UpdateOrderStatusCommand
- Queries: (stubs for future implementation)

### Integration with Other Microservices

The project contains a stub for integrating with the promotions microservice through gRPC. Full implementation requires the implementation of the gRPC server-side service.