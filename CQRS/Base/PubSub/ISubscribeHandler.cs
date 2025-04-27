namespace Gufel.CQRS.Base.PubSub
{
    public interface ISubscribeHandler<in TData>
    {
        Task HandleAsync(TData data);
    }
}
