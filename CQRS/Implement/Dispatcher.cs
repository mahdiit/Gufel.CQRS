using Gufel.Dispatcher.Base.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using ZLinq;

namespace Gufel.Dispatcher.Implement
{
    public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
    {
        public async Task Dispatch<TRequest>(TRequest request, CancellationToken cancellation)
            where TRequest : IRequest
        {
            using var pipeLines = serviceProvider
                .GetServices<IPipelineHandler<TRequest>>()
                .AsValueEnumerable().Enumerator;

            while (pipeLines.TryGetNext(out var pipeline))
            {
                await pipeline.Handle(request, cancellation);
            }

            await serviceProvider
                .GetRequiredService<IRequestHandler<TRequest>>()
                .Handle(request, cancellation);
        }

        public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken cancellation)
            where TRequest : IRequest<TResponse>
            where TResponse : IResponse
        {
            using var pipeLines = serviceProvider
                .GetServices<IPipelineHandler<TRequest, TResponse>>()
                .AsValueEnumerable()
                .Enumerator;

            while (pipeLines.TryGetNext(out var pipeline))
            {
                await pipeline.Handle(request, cancellation);
            }

            return await serviceProvider
                .GetRequiredService<IRequestHandler<TRequest, TResponse>>()
                .Handle(request, cancellation);
        }
    }
}
