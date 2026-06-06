using Gufel.Dispatcher.Base.Dispatcher;

namespace Gufel.Dispatcher.Implement;

public static class RequestTypeChecker
{
    public static bool IsGenericIRequest(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IRequest<>))
            return true;

        foreach (var i in type.GetInterfaces())
        {
            if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                return true;
        }
        return false;
    }

    public static bool IsNonGenericIRequest(Type type)
    {
        if (!typeof(IRequest).IsAssignableFrom(type))
            return false;

        return !IsGenericIRequest(type);
    }
}