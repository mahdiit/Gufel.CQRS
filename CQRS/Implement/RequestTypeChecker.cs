using Gufel.Dispatcher.Base.Dispatcher;

namespace Gufel.Dispatcher.Implement;

public static class RequestTypeChecker
{
    public static bool IsGenericIRequest(Type type)
    {
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
    }

    public static bool IsNonGenericIRequest(Type type)
    {
        return typeof(IRequest).IsAssignableFrom(type) &&
               !IsGenericIRequest(type);
    }
}