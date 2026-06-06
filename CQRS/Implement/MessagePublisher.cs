using Gufel.Dispatcher.Base.MessagePublisher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement
{
    public sealed class MessagePublisher(IServiceProvider serviceProvider,
        IMessagePublishStrategy strategy) : IMessagePublisher
    {
        public async Task Publish<T>(string topic, T value, CancellationToken cancellationToken = default)
        {
            var allSubscribers = serviceProvider.GetServices<ISubscribeHandler<T>>();
            var filtered = new List<ISubscribeHandler<T>>();
            foreach (var sub in allSubscribers)
            {
                if (sub.Topic == topic)
                {
                    filtered.Add(sub);
                }
            }

            if (filtered.Count == 0)
                return;

            await strategy.SendMessage(filtered, value, cancellationToken).ConfigureAwait(false);
        }
    }
}
