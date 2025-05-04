namespace Gufel.CQRS.Base.PubSub;

public interface IMessagePublishStrategy
{
    Task SendMessage<T>(IEnumerable<ISubscribeHandler<T>> subscribers, T value);
}