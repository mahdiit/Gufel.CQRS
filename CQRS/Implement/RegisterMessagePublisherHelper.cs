using Gufel.Dispatcher.Base.MessagePublisher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement;

public static class RegisterMessagePublisherHelper
{
    public static void AddMessagePublisher(this IServiceCollection services, IMessagePublishStrategy? strategy = null)
    {
        services.AddSingleton(x => strategy ?? new ParallelMessagePublishStrategy());
        services.AddSingleton<IMessagePublisher, MessagePublisher>();
    }
}