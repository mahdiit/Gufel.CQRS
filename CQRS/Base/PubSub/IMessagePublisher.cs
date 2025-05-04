namespace Gufel.CQRS.Base.PubSub
{

    public interface IMessagePublisher
    {
        void Publish<TData>(string topic, TData value);
    }
}
