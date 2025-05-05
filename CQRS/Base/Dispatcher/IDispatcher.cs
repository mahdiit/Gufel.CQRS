namespace Gufel.Dispatcher.Base.Dispatcher
{
    public interface IDispatcher
    {
        Task Dispatch<TRequest>(TRequest request, CancellationToken cancellation)
            where TRequest: IRequest;

        Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken cancellation)
            where TRequest: IRequest<TResponse>
            where TResponse : IResponse;
    }
}
