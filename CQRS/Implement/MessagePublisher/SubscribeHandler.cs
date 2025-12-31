using Gufel.Dispatcher.Base.MessagePublisher;

namespace Gufel.Dispatcher.Implement.MessagePublisher;

public abstract class SubscribeHandler<TData>(IMessagePublisherNameResolver nameResolver)
    : ISubscribeHandler<TData>
{
    public string Topic => nameResolver.ResolveName(this);

    public abstract Task HandleAsync(TData data, CancellationToken cancellationToken = default);
}