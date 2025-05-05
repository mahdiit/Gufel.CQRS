namespace Gufel.Dispatcher.Base.MessagePublisher;

public class ParallelMessagePublishStrategy : IMessagePublishStrategy
{
    public Task SendMessage<T>(IEnumerable<ISubscribeHandler<T>> subscribers, T value)
    {
        Parallel.ForEach(subscribers, async (subscriber) =>
        {
            await subscriber.HandleAsync(value);
        });
        return Task.CompletedTask;
    }
}