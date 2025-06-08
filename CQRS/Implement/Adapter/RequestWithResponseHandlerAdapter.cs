using Gufel.Dispatcher.Base.Dispatcher;

namespace Gufel.Dispatcher.Implement.Adapter;

public class RequestWithResponseHandlerAdapter<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> inner)
    : IRequestHandler<IRequest<TResponse>, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(IRequest<TResponse> request, CancellationToken cancellationToken)
    {
        return inner.Handle((TRequest)request, cancellationToken);
    }
}