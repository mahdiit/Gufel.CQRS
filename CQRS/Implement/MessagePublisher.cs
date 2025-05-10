using System.Collections;
using System.Collections.Concurrent;
using Gufel.Dispatcher.Base.MessagePublisher;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gufel.Dispatcher.Implement
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly IServiceScope? _serviceScope;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessagePublishStrategy _strategy;

        public MessagePublisher(
            IServiceScopeFactory serviceScope,
            IHttpContextAccessor httpContextAccessor,
            IMessagePublishStrategy strategy)
        {
            _strategy = strategy;
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

        public void Publish<T>(string topic, T value)
        {
            var subscribers = _serviceProvider.GetServices<ISubscribeHandler<T>>()
                .Where(x => x.Topic == topic);
            _strategy.SendMessage(subscribers, value);
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
