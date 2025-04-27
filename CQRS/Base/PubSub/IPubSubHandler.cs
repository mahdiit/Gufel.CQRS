namespace Gufel.CQRS.Base.PubSub
{
    public delegate Task SubscribeHandler<in TInputData>(TInputData data);

    public interface IPubSubHandler<TData>
    {
        void Subscribe(string topic, SubscribeHandler<TData> handler);
        void Unsubscribe(string topic, SubscribeHandler<TData> hadnler);
        void Publish(string topic, TData value);
    }
}
