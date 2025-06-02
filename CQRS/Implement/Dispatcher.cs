using Gufel.Dispatcher.Base.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement
{
    public sealed class Dispatcher(IServiceProvider serviceProvider, IRequestPipelineMetadata pipelineMetadata) : IDispatcher
    {
        public async Task Dispatch<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest
        {
            if (pipelineMetadata.HasPipeline(typeof(TRequest), null))
            {
                var pipelines = serviceProvider.GetService<IEnumerable<IPipelineHandler<TRequest>>>();
                if (pipelines != null)
                {
                    foreach (var pipeline in pipelines)
                    {
                        await pipeline.Handle(request, cancellationToken);
                    }
                }
            }

            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();
            await handler.Handle(request, cancellationToken);
        }

        public async Task<TResponse> Dispatch<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (pipelineMetadata.HasPipeline(typeof(IRequest<TResponse>), typeof(TResponse)))
            {
                var pipelines = serviceProvider.GetService<IEnumerable<IPipelineHandler<IRequest<TResponse>, TResponse>>>();
                if (pipelines != null)
                {
                    foreach (var pipeline in pipelines)
                    {
                        await pipeline.Handle(request, cancellationToken);
                    }
                }
            }

            var handler = serviceProvider.GetRequiredService<IRequestHandler<IRequest<TResponse>, TResponse>>();
            return await handler.Handle(request, cancellationToken);
        }
    }

}
