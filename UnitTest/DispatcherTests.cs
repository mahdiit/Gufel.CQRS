using Gufel.Dispatcher.Base.Dispatcher;
using Gufel.Dispatcher.Implement;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Gufel.UnitTest;

public class TestRequest : IRequest { }
public class TestRequestWithResponse : IRequest<TestResponse> { }
public class TestResponse { }

public class TestPipelineHandler : IPipelineHandler<TestRequest>
{
    public bool WasHandled { get; private set; }
    public Task Handle(TestRequest request, CancellationToken cancellation)
    {
        WasHandled = true;
        return Task.CompletedTask;
    }
}

public class TestPipelineHandlerWithResponse : IPipelineHandler<TestRequestWithResponse, TestResponse>
{
    public bool WasHandled { get; private set; }
    public Task Handle(TestRequestWithResponse request, CancellationToken cancellation)
    {
        WasHandled = true;
        return Task.CompletedTask;
    }
}

public class TestRequestHandler : IRequestHandler<TestRequest>
{
    public bool WasHandled { get; private set; }
    public Task Handle(TestRequest request, CancellationToken cancellation)
    {
        WasHandled = true;
        return Task.CompletedTask;
    }
}

public class TestRequestHandlerWithResponse : IRequestHandler<TestRequestWithResponse, TestResponse>
{
    public bool WasHandled { get; private set; }
    public Task<TestResponse> Handle(TestRequestWithResponse request, CancellationToken cancellation)
    {
        WasHandled = true;
        return Task.FromResult(new TestResponse());
    }
}

public class DispatcherTests
{
    private readonly IDispatcher _dispatcher;
    private readonly TestPipelineHandler _pipelineHandler;
    private readonly TestPipelineHandlerWithResponse _pipelineHandlerWithResponse;
    private readonly TestRequestHandler _requestHandler;
    private readonly TestRequestHandlerWithResponse _requestHandlerWithResponse;

    public DispatcherTests()
    {
        var services = new ServiceCollection();
        services.AddDispatcher(typeof(DispatcherTests).Assembly);

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        _dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        _pipelineHandler = (TestPipelineHandler)serviceProvider.GetRequiredService<IPipelineHandler<TestRequest>>();
        _pipelineHandlerWithResponse = (TestPipelineHandlerWithResponse)serviceProvider.GetRequiredService<IPipelineHandler<TestRequestWithResponse, TestResponse>>();
        _requestHandler = (TestRequestHandler)serviceProvider.GetRequiredService<IRequestHandler<TestRequest>>();
        _requestHandlerWithResponse = (TestRequestHandlerWithResponse)serviceProvider.GetRequiredService<IRequestHandler<TestRequestWithResponse, TestResponse>>();
    }

    [Fact]
    public async Task Dispatch_NonGenericRequest_ShouldExecutePipelineAndHandler()
    {
        // Arrange
        var request = new TestRequest();

        // Act
        await _dispatcher.Dispatch(request, CancellationToken.None);

        // Assert
        _pipelineHandler.WasHandled.ShouldBeTrue();
        _requestHandler.WasHandled.ShouldBeTrue();
    }

    [Fact]
    public async Task Dispatch_GenericRequest_ShouldExecutePipelineAndHandler()
    {
        // Arrange
        var request = new TestRequestWithResponse();

        // Act
        var response = await _dispatcher.Dispatch(request, CancellationToken.None);

        // Assert
        _pipelineHandlerWithResponse.WasHandled.ShouldBeTrue();
        _requestHandlerWithResponse.WasHandled.ShouldBeTrue();
        response.ShouldNotBeNull();
    }
}