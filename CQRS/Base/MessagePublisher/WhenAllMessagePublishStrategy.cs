namespace Gufel.Dispatcher.Base.MessagePublisher
{
    public class WhenAllMessagePublishStrategy : IMessagePublishStrategy
    {
        public Task SendMessage<T>(IEnumerable<ISubscribeHandler<T>> subscribers, T value)
        {
            var tasks = subscribers.Select(subscriber => subscriber.HandleAsync(value)).ToList();
            return Task.WhenAll(tasks);
        }
    }
}
