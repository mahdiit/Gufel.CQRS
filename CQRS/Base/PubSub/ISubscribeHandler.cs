namespace Gufel.CQRS.Base.PubSub
{
    public interface ISubscribeHandler<in TData>
    {
        string Topic { get; }
        Task HandleAsync(TData data);
    }
}
