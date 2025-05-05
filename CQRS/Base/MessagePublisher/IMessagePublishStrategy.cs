namespace Gufel.Dispatcher.Base.MessagePublisher;

public interface IMessagePublishStrategy
{
    Task SendMessage<T>(IEnumerable<ISubscribeHandler<T>> subscribers, T value);
}