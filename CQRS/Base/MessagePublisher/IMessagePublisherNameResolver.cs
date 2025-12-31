namespace Gufel.Dispatcher.Base.MessagePublisher;

public interface IMessagePublisherNameResolver
{
    string ResolveName<TData>(TData data);
}