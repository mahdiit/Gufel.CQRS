using Gufel.Dispatcher.Base.MessagePublisher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement
{
    public sealed class MessagePublisher(IServiceProvider serviceProvider,
        IMessagePublishStrategy strategy) : IMessagePublisher
    {
        public void Publish<T>(string topic, T value)
        {
            var subscribers = serviceProvider.GetServices<ISubscribeHandler<T>>()
                .Where(x => x.Topic == topic);
            strategy.SendMessage(subscribers, value);
        }
    }
}
