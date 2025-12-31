using Gufel.Dispatcher.Base.MessagePublisher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement.MessagePublisher
{
    public sealed class MessagePublisher(IServiceProvider serviceProvider,
        IMessagePublishStrategy strategy, IMessagePublisherNameResolver nameResolver) : IMessagePublisher
    {
        public async Task Publish<T>(string topic, T value, CancellationToken cancellationToken)
        {
            var subscribers = serviceProvider.GetRequiredService<IEnumerable<ISubscribeHandler<T>>>()
                .Where(x => x.Topic == topic);

            await strategy.SendMessage(subscribers, value, cancellationToken);
        }

        public async Task Publish<T>(T value, CancellationToken cancellationToken)
        {
            await Publish(nameResolver.ResolveName(value), value, cancellationToken);
        }
    }
}
