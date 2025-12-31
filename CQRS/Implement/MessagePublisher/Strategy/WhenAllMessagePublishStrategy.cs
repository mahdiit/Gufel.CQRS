using Gufel.Dispatcher.Base.MessagePublisher;

namespace Gufel.Dispatcher.Implement.MessagePublisher.Strategy
{
    public class WhenAllMessagePublishStrategy : IMessagePublishStrategy
    {
        public async Task SendMessage<T>(IEnumerable<ISubscribeHandler<T>> subscribers, T value, CancellationToken cancellationToken)
        {
            var tasks = subscribers.Select(subscriber => subscriber.HandleAsync(value, cancellationToken)).ToList();
            await Task.WhenAll(tasks);
        }
    }
}
