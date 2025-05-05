using System.Reflection;
using Gufel.Dispatcher.Base.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement
{
    public static class RegisterDispatcherHelper
    {
        private static void RegisterTypeImplement(IServiceCollection services, Assembly assembly, params Type[] type)
        {
            assembly.GetTypes()
                .Where(t => t.GetInterfaces().Length >= 1 &&
                            t.GetInterfaces().Any(p => p.IsGenericType) &&
                            type.Contains(t.GetInterfaces().First(p => p.IsGenericType).GetGenericTypeDefinition())
                )
                .ToList()
                .ForEach(handler =>
                {
                    var requestInterface = handler.GetInterfaces()
                        .First(p => p.IsGenericType && type.Contains(p.GetGenericTypeDefinition()));
                    services.AddScoped(requestInterface, handler);
                });
        }

        public static void RegisterDispatcher(this IServiceCollection services, Assembly assembly)
        {
            RegisterTypeImplement(services, assembly,
                typeof(IPipelineHandler<,>),
                typeof(IRequestHandler<,>),
                typeof(IPipelineHandler<>),
                typeof(IRequestHandler<>));

            services.AddSingleton<IDispatcher, Dispatcher>();
        }
    }
}
