using System.Reflection;
using Gufel.Dispatcher.Base.Dispatcher;
using Gufel.Dispatcher.Base.MessagePublisher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement
{
    public static class RegisterHelper
    {
        public static void RegisterTypeImplementations(IServiceCollection services, Assembly assembly, ServiceLifetime lifetime, params Type[] openGenericTypes)
        {
            var implementationTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && openGenericTypes.Contains(i.GetGenericTypeDefinition())))
                .ToList();

            foreach (var implementationType in implementationTypes)
            {
                var matchingInterface = implementationType.GetInterfaces()
                    .First(i => i.IsGenericType && openGenericTypes.Contains(i.GetGenericTypeDefinition()));

                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(matchingInterface, implementationType);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(matchingInterface, implementationType);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(matchingInterface, implementationType);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(lifetime), $"Unsupported lifetime: {lifetime}");
                }
            }
        }

        public static void AddDispatcher(this IServiceCollection services, Assembly assembly)
        {
            RegisterTypeImplementations(services, assembly, ServiceLifetime.Scoped,
                typeof(IPipelineHandler<,>),
                typeof(IRequestHandler<,>),
                typeof(IPipelineHandler<>),
                typeof(IRequestHandler<>));

            services.AddScoped<IDispatcher, Dispatcher>();
        }

        public static void AddMessagePublisher(this IServiceCollection services, IMessagePublishStrategy? strategy = null)
        {
            services.AddSingleton(_ => strategy ?? new ParallelMessagePublishStrategy());

            services.AddScoped<IMessagePublisher, MessagePublisher>();
        }
    }
}
