namespace Gufel.Dispatcher.Base.MessagePublisher
{
    public interface IMessagePublisher
    {
        Task Publish<TData>(string topic, TData value, CancellationToken cancellationToken);

        Task Publish<TData>(TData value, CancellationToken cancellationToken);
    }
}
