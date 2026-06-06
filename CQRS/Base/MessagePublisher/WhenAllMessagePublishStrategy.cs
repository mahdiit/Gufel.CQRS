namespace Gufel.Dispatcher.Base.MessagePublisher
{
    public sealed class WhenAllMessagePublishStrategy : IMessagePublishStrategy
    {
        public async Task SendMessage<T>(IEnumerable<ISubscribeHandler<T>> subscribers, T value, CancellationToken cancellationToken)
        {
            var tasks = subscribers is ICollection<ISubscribeHandler<T>> collection
                ? new List<Task>(collection.Count)
                : new List<Task>();
            foreach (var subscriber in subscribers)
            {
                tasks.Add(subscriber.HandleAsync(value, cancellationToken));
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
