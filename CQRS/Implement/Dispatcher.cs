using Gufel.Dispatcher.Base.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement
{
    public class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
    {
        public async Task Dispatch<TRequest>(TRequest request, CancellationToken cancellation)
            where TRequest: IRequest
        {
            var pipeLines = serviceProvider.GetServices<IPipelineHandler<TRequest>>();
            foreach (var pipeline in pipeLines)
            {
                await pipeline.Handle(request, cancellation);
            }

            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();
            await handler.Handle(request, cancellation);
        }

        public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken cancellation)
            where TRequest : IRequest<TResponse>
            where TResponse : IResponse
        {
            var pipeLines = serviceProvider.GetServices<IPipelineHandler<TRequest, TResponse>>();
            foreach (var pipeline in pipeLines)
            {
                await pipeline.Handle(request, cancellation);
            }

            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            return await handler.Handle(request, cancellation);
        }
    }
}
