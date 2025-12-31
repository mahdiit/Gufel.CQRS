namespace Gufel.Dispatcher.Base.MessagePublisher
{
    public interface ISubscribeHandler<in TData>
    {
        string Topic { get; }

        Task HandleAsync(TData data, CancellationToken cancellationToken);
    }
}
