using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Gufel.Dispatcher.Base.Dispatcher;
using Gufel.Dispatcher.Implement;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmark;

[MemoryDiagnoser]
public class DispatcherBenchmark
{
    private readonly IDispatcher _dispatcher;
    private readonly IMediator _mediator;
    private readonly TestRequest _request;
    private readonly TestMediatRRequest _mediatRRequest;

    public DispatcherBenchmark()
    {
        var services = new ServiceCollection();
        
        // Register 100 handlers for Dispatcher
        for (int i = 0; i < 100; i++)
        {
            services.AddScoped<Gufel.Dispatcher.Base.Dispatcher.IRequestHandler<TestRequest, TestResponse>, TestHandler>();
        }

        services.AddDispatcher(typeof(DispatcherBenchmark).Assembly);
        
        // Register 100 handlers for MediatR
        for (int i = 0; i < 100; i++)
        {
            services.AddScoped<MediatR.IRequestHandler<TestMediatRRequest, TestMediatRResponse>, TestMediatRHandler>();
        }
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DispatcherBenchmark).Assembly));

        var serviceProvider = services.BuildServiceProvider();
        _dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        _mediator = serviceProvider.GetRequiredService<IMediator>();
        
        _request = new TestRequest { Id = 1 };
        _mediatRRequest = new TestMediatRRequest { Id = 1 };
    }

    [Benchmark]
    public async Task Dispatcher_100Handlers()
    {
        await _dispatcher.Dispatch<TestRequest, TestResponse>(_request, CancellationToken.None);
    }

    [Benchmark]
    public async Task MediatR_100Handlers()
    {
        await _mediator.Send(_mediatRRequest, CancellationToken.None);
    }
}

// Dispatcher Test Classes
public class TestRequest : Gufel.Dispatcher.Base.Dispatcher.IRequest<TestResponse>
{
    public int Id { get; set; }
}

public class TestResponse : IResponse
{
    public int Result { get; set; }
}

public class TestHandler : Gufel.Dispatcher.Base.Dispatcher.IRequestHandler<TestRequest, TestResponse>
{
    public async Task<TestResponse> Handle(TestRequest request, CancellationToken cancellation)
    {
        await Task.Delay(1, cancellation); // Simulate some work
        return new TestResponse { Result = request.Id };
    }
}

public class TestPipeline : IPipelineHandler<TestRequest, TestResponse>
{
    public async Task Handle(TestRequest request, CancellationToken cancellation)
    {
        await Task.Delay(1, cancellation); // Simulate some work
    }
}

// MediatR Test Classes
public class TestMediatRRequest : MediatR.IRequest<TestMediatRResponse>
{
    public int Id { get; set; }
}

public class TestMediatRResponse
{
    public int Result { get; set; }
}

public class TestMediatRHandler : MediatR.IRequestHandler<TestMediatRRequest, TestMediatRResponse>
{
    public async Task<TestMediatRResponse> Handle(TestMediatRRequest request, CancellationToken cancellation)
    {
        await Task.Delay(1, cancellation); // Simulate some work
        return new TestMediatRResponse { Result = request.Id };
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<DispatcherBenchmark>();
    }
} 