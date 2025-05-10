using Gufel.Dispatcher.Base.Dispatcher;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement
{
    public class Dispatcher : IDispatcher, IDisposable
    {
        private readonly IServiceScope? _serviceScope;
        private readonly IServiceProvider _serviceProvider;

        public Dispatcher(
            IServiceScopeFactory serviceScope, 
            IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                _serviceProvider = httpContextAccessor.HttpContext.RequestServices;
            }
            else
            {
                _serviceScope = serviceScope.CreateScope();
                _serviceProvider = _serviceScope.ServiceProvider;
            }
        }

        public async Task Dispatch<TRequest>(TRequest request, CancellationToken cancellation)
            where TRequest: IRequest
        {
            var pipeLines = _serviceProvider.GetServices<IPipelineHandler<TRequest>>();
            foreach (var pipeline in pipeLines)
            {
                await pipeline.Handle(request, cancellation);
            }

            var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();
            await handler.Handle(request, cancellation);
        }

        public async Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken cancellation)
            where TRequest : IRequest<TResponse>
            where TResponse : IResponse
        {
            var pipeLines = _serviceProvider.GetServices<IPipelineHandler<TRequest, TResponse>>();
            foreach (var pipeline in pipeLines)
            {
                await pipeline.Handle(request, cancellation);
            }

            var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
            return await handler.Handle(request, cancellation);
        }

        protected virtual void Dispose(bool disposing)
        {
            _serviceScope?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
