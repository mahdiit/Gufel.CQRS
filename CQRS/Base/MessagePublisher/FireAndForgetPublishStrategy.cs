namespace Gufel.Dispatcher.Base.MessagePublisher;

public sealed class FireAndForgetPublishStrategy(IMessagePublishStrategy inner) : IMessagePublishStrategy
{
    public Task SendMessage<T>(
        IEnumerable<ISubscribeHandler<T>> subscribers,
        T value,
        CancellationToken cancellationToken)
    {
        var materialized = subscribers.ToList();

        if (materialized.Count == 0)
            return Task.CompletedTask;

        _ = Task.Run(() => inner.SendMessage(materialized, value, cancellationToken));
        return Task.CompletedTask;
    }
}
