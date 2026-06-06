namespace Gufel.Dispatcher.Base.MessagePublisher;

public sealed class ParallelMessagePublishStrategy : IMessagePublishStrategy
{
    public async Task SendMessage<T>(IEnumerable<ISubscribeHandler<T>> subscribers, T value, CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(
            subscribers,
            new ParallelOptions { CancellationToken = cancellationToken },
            (subscriber, ct) => new ValueTask(subscriber.HandleAsync(value, ct))
        ).ConfigureAwait(false);
    }
}