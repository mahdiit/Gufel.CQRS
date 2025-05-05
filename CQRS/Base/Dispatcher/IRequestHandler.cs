namespace Gufel.Dispatcher.Base.Dispatcher
{
    public interface IRequestHandler<in TRequest> where TRequest : IRequest
    {
        Task Handle(TRequest request, CancellationToken cancellation);
    }

    public interface IRequestHandler<in TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
        where TResponse: IResponse
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellation);
    }
}
