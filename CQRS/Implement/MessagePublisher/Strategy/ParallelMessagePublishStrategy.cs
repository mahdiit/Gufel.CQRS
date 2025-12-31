using Gufel.Dispatcher.Base.MessagePublisher;

namespace Gufel.Dispatcher.Implement.MessagePublisher.Strategy;

public class ParallelMessagePublishStrategy : IMessagePublishStrategy
{
    public async Task SendMessage<T>(IEnumerable<ISubscribeHandler<T>> subscribers, T value, CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(subscribers, cancellationToken, async (subscriber, ctx) =>
        {
            await subscriber.HandleAsync(value, ctx);
        });
    }
}