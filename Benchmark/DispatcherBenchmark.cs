using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Gufel.Dispatcher.Base.Dispatcher;
using Gufel.Dispatcher.Implement;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Benchmark;

[MemoryDiagnoser]
[SimpleJob]
public class DispatcherBenchmark
{
    private IDispatcher _dispatcher = default!;
    private IMediator _mediator = default!;
    private TestRequest _request = default!;
    private TestMediatRRequest _mediatrRequest = default!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();

        // ===== YOUR DISPATCHER =====
        for (int i = 0; i < 100; i++)
        {
            services.AddScoped<
                Gufel.Dispatcher.Base.Dispatcher.IRequestHandler<TestRequest, TestResponse>,
                TestHandler>();
        }

        services.AddDispatcher(Assembly.GetExecutingAssembly());

        // ===== MEDIATR =====
        for (int i = 0; i < 100; i++)
        {
            services.AddScoped<
                MediatR.IRequestHandler<TestMediatRRequest, TestMediatRResponse>,
                TestMediatRHandler>();
        }

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        var serviceProvider = services.BuildServiceProvider();

        _dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        _mediator = serviceProvider.GetRequiredService<IMediator>();

        _request = new TestRequest { Id = 1 };
        _mediatrRequest = new TestMediatRRequest { Id = 1 };
    }

    [Benchmark(Baseline = true)]
    public async Task Dispatcher_Dispatch()
    {
        await _dispatcher.Dispatch(_request);
    }

    [Benchmark]
    public async Task MediatR_Send()
    {
        await _mediator.Send(_mediatrRequest);
    }
}

// Dispatcher Test Classes
public record TestRequest : Gufel.Dispatcher.Base.Dispatcher.IRequest<TestResponse>
{
    public int Id { get; set; }
}

public record TestResponse
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

// MediatR Test Classes
public record TestMediatRRequest : MediatR.IRequest<TestMediatRResponse>
{
    public int Id { get; set; }
}

public record TestMediatRResponse
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

        var dispatcher = summary.Reports
            .First(r => r.BenchmarkCase.Descriptor.WorkloadMethod.Name == "Dispatcher_Dispatch");

        var mediator = summary.Reports
            .First(r => r.BenchmarkCase.Descriptor.WorkloadMethod.Name == "MediatR_Send");

        var ratio = mediator.ResultStatistics.Mean / dispatcher.ResultStatistics.Mean;

        Console.WriteLine($"Dispatcher is {ratio:F2}x faster than MediatR");
    }
}