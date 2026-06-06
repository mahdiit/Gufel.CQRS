using Gufel.Dispatcher.Base.MessagePublisher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement
{
    public sealed class MessagePublisher(IServiceProvider serviceProvider,
        IMessagePublishStrategy strategy) : IMessagePublisher
    {
        public Task Publish<T>(string topic, T value, CancellationToken cancellationToken = default)
        {
            var allSubscribers = serviceProvider.GetServices<ISubscribeHandler<T>>();

            List<ISubscribeHandler<T>>? filtered = null;
            foreach (var sub in allSubscribers)
            {
                if (sub.Topic == topic)
                {
                    filtered ??= [];
                    filtered.Add(sub);
                }
            }

            if (filtered is null)
                return Task.CompletedTask;

            return strategy.SendMessage(filtered, value, cancellationToken);
        }
    }
}
