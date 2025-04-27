namespace Gufel.CQRS.Base.PubSub
{

    public interface IPubSubHandler<TData>
    {
        bool Subscribe(string topic, ISubscribeHandler<TData> handler);
        bool Unsubscribe(string topic, ISubscribeHandler<TData> hadnler);
        void Publish(string topic, TData value);
    }
}
