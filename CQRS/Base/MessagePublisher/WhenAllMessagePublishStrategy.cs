namespace Gufel.Dispatcher.Base.MessagePublisher
{
    public sealed class WhenAllMessagePublishStrategy : IMessagePublishStrategy
    {
        public async Task SendMessage<T>(IEnumerable<ISubscribeHandler<T>> subscribers, T value, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            foreach (var subscriber in subscribers)
            {
                tasks.Add(subscriber.HandleAsync(value, cancellationToken));
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
