namespace Gufel.CQRS.Base.Dispatcher
{
    public interface IDispatcher
    {
        Task Dispatch<TRequest>(TRequest request, CancellationToken cancellation)
            where TRequest: IRequest;

        Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken cancellation)
            where TRequest: IRequest<TResponse>
            where TResponse : IResponse;
    }

    public interface IRequest { }

    public interface IRequest<TResponse>
        : IRequest where TResponse : IResponse;

    public interface IResponse { }
}
