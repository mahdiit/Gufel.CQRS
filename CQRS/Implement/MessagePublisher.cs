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

            var count = 0;
            foreach (var sub in allSubscribers)
            {
                if (sub.Topic == topic)
                    count++;
            }

            if (count == 0)
                return Task.CompletedTask;

            if (count == 1)
            {
                foreach (var sub in allSubscribers)
                {
                    if (sub.Topic == topic)
                        return strategy.SendMessage([sub], value, cancellationToken);
                }
            }

            var filtered = new List<ISubscribeHandler<T>>(count);
            foreach (var sub in allSubscribers)
            {
                if (sub.Topic == topic)
                    filtered.Add(sub);
            }

            return strategy.SendMessage(filtered, value, cancellationToken);
        }
    }
}
