namespace Gufel.Dispatcher.Base.Dispatcher;

public interface IRequest<TResponse>
    : IRequest where TResponse : IResponse;

public interface IRequest
{
}