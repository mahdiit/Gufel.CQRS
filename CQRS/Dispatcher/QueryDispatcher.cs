using Gufel.CQRS.Base.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.CQRS.Dispatcher
{
    public class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
    {
        public Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation)
        {
            var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();
            return handler.Handle(query, cancellation);
        }
    }
}
