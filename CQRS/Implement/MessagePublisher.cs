using System.Collections;
using System.Collections.Concurrent;
using Gufel.Dispatcher.Base.MessagePublisher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gufel.Dispatcher.Implement
{
    public class MessagePublisher(IServiceProvider services, IMessagePublishStrategy strategy) : IMessagePublisher
    {
        public void Publish<T>(string topic, T value)
        {
            var subscribers = services.GetServices<ISubscribeHandler<T>>()
                .Where(x => x.Topic == topic);
            strategy.SendMessage(subscribers, value);
        }
    }

    public static class MessagePublisherRegister
    {
        public static void AddMessagePublisher(this IServiceCollection services, IMessagePublishStrategy? strategy = null)
        {
            services.AddSingleton(x => strategy ?? new ParallelMessagePublishStrategy());
            services.AddSingleton<IMessagePublisher, MessagePublisher>();
        }
    }
}
