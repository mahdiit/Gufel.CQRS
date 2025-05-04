using Gufel.CQRS.Base.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.CQRS.Dispatcher
{
    public class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
    {
        public async Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation)
        {
            var pipelines = serviceProvider.GetServices<IPipelineHandler<TQuery, TQueryResult>>();
            foreach (var pipeline in pipelines)
            {
                await pipeline.Handle(query, cancellation);
            }

            var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();
            return await handler.Handle(query, cancellation);
        }
    }
}
