namespace Gufel.Dispatcher.Base.MessagePublisher
{
    public interface IMessagePublisher
    {
        void Publish<TData>(string topic, TData value);
    }
}
