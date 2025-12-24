namespace Gufel.Dispatcher.Base.MessagePublisher;

public class ParallelMessagePublishStrategy : IMessagePublishStrategy
{
    public async Task SendMessage<T>(IEnumerable<ISubscribeHandler<T>> subscribers, T value)
    {
        await Parallel.ForEachAsync(subscribers, async (subscriber, _) =>
        {
            await subscriber.HandleAsync(value);
        });
    }
}