using System;
using System.Linq;
using Gufel.CQRS.Base.Dispatcher;

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