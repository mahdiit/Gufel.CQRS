namespace Gufel.Dispatcher.Base.Dispatcher
{
    public interface IDispatcher
    {
        Task<TResponse> Dispatch<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        Task Dispatch<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IRequest;
    }
}
