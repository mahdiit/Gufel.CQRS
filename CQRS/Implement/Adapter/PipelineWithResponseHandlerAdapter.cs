using Gufel.Dispatcher.Base.Dispatcher;

namespace Gufel.Dispatcher.Implement.Adapter;

public class PipelineWithResponseHandlerAdapter<TRequest, TResponse>(IPipelineHandler<TRequest, TResponse> inner)
    : IPipelineHandler<IRequest<TResponse>, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task Handle(IRequest<TResponse> command, CancellationToken cancellationToken)
    {
        return inner.Handle((TRequest)command, cancellationToken);
    }
}