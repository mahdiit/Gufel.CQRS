# Gufel CQRS

A lightweight CQRS (Command Query Responsibility Segregation) and in-memory pub/sub implementation for .NET applications.

## Features

- Command/Query handling with pipeline support
- In-memory message publishing and subscription
- Support for both synchronous and parallel message publishing strategies
- Dependency injection integration
- Easy to use and extend

## Installation

```bash
dotnet add package Gufel.Dispatcher
```
## Benchmark

| Method              | Mean     | Error    | StdDev   | Ratio | Allocated | Alloc Ratio |
|-------------------- |---------:|---------:|---------:|------:|----------:|------------:|
| Dispatcher_Dispatch | 15.71 ms | 0.113 ms | 0.106 ms |  1.00 |     619 B |        1.00 |
| MediatR_Send        | 15.61 ms | 0.174 ms | 0.163 ms |  0.99 |     662 B |        1.07 |

## Usage

### 1. Dispatcher (CQRS)

The Dispatcher handles commands and queries with support for pipeline handlers.

#### Basic Setup

```csharp
// Register services
var services = new ServiceCollection();
services.AddDispatcher(typeof(Program).Assembly);
var serviceProvider = services.BuildServiceProvider();

// Get dispatcher instance
var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
```

#### Sample Command/Query Implementation

```csharp
// Define your request and response
public class CreateOrderRequest : IRequest<CreateOrderResponse>
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderResponse : IResponse
{
    public int OrderId { get; set; }
    public bool Success { get; set; }
}

// Implement request handler
public class CreateOrderHandler : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
{
    public async Task<CreateOrderResponse> Handle(CreateOrderRequest request, CancellationToken cancellation)
    {
        // Handle the request
        return new CreateOrderResponse 
        { 
            OrderId = 1,
            Success = true 
        };
    }
}

// Optional: Implement pipeline handler
public class LoggingPipeline : IPipelineHandler<CreateOrderRequest, CreateOrderResponse>
{
    public async Task Handle(CreateOrderRequest request, CancellationToken cancellation)
    {
        Console.WriteLine($"Processing order for product {request.ProductId}");
        await Task.CompletedTask;
    }
}

// Usage
var request = new CreateOrderRequest { ProductId = 1, Quantity = 2 };
var response = await dispatcher.Dispatch(request, CancellationToken.None);
```

### 2. Message Publisher (Pub/Sub)

The Message Publisher provides in-memory publish/subscribe functionality.

#### Basic Setup

```csharp
// Register services
var services = new ServiceCollection();
services.AddMessagePublisher(); // Uses ParallelMessagePublishStrategy by default
// OR
services.AddMessagePublisher(new WhenAllMessagePublishStrategy());
var serviceProvider = services.BuildServiceProvider();

// Get publisher instance
var publisher = serviceProvider.GetRequiredService<IMessagePublisher>();
```

#### Sample Publisher/Subscriber Implementation

```csharp
// Define your message model
public class OrderCreatedMessage
{
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Implement subscriber
public class OrderNotificationHandler : ISubscribeHandler<OrderCreatedMessage>
{
    public string Topic => "order-created";

    public async Task HandleAsync(OrderCreatedMessage data)
    {
        Console.WriteLine($"Order {data.OrderId} was created at {data.CreatedAt}");
        await Task.CompletedTask;
    }
}

// Register subscriber
services.AddSingleton<ISubscribeHandler<OrderCreatedMessage>, OrderNotificationHandler>();

// Publish message
var message = new OrderCreatedMessage 
{ 
    OrderId = 1, 
    CreatedAt = DateTime.UtcNow 
};
publisher.Publish("order-created", message);
```

## Publishing Strategies

The Message Publisher supports two publishing strategies:

1. **ParallelMessagePublishStrategy** (Default)
   - Processes subscribers in parallel
   - Best for independent subscribers
   - Faster execution

2. **WhenAllMessagePublishStrategy**
   - Processes subscribers sequentially
   - Waits for all subscribers to complete
   - Better for dependent subscribers

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
