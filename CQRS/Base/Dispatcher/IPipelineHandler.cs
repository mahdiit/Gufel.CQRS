namespace Gufel.Dispatcher.Base.Dispatcher
{
    public interface IPipelineHandler<in TRequest> where TRequest : IRequest
    {
        Task Handle(TRequest command, CancellationToken cancellation);
    }

    public interface IPipelineHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IResponse
    {
        Task Handle(TRequest request, CancellationToken cancellation);
    }
}
