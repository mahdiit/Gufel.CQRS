using Gufel.Dispatcher.Base.MessagePublisher;

namespace Gufel.Dispatcher.Implement.MessagePublisher;

public class MessagePublisherDefaultNameResolver : IMessagePublisherNameResolver
{
    public string ResolveName<TData>(TData data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        var dataType = data.GetType();

        // Look for ISubscribeHandler<T>
        var subscribeHandlerInterface = dataType
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(ISubscribeHandler<>));

        if (subscribeHandlerInterface == null) return $"{dataType.Namespace}:{dataType.Name}";

        // Use the generic argument T from ISubscribeHandler<T>
        var handledType = subscribeHandlerInterface.GetGenericArguments()[0];
        return $"{handledType.Namespace}:{handledType.Name}";
    }
}