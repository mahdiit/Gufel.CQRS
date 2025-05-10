using System.Reflection;
using Gufel.Dispatcher.Base.Dispatcher;
using Gufel.Dispatcher.Base.MessagePublisher;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement
{
    public static class RegisterHelper
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

        private static void AddHttpContextAccessor(IServiceCollection services)
        {
            if (services.All(sd => sd.ServiceType != typeof(IHttpContextAccessor)))
            {
                services.AddHttpContextAccessor();
            }
        }

        public static void AddDispatcher(this IServiceCollection services, Assembly assembly)
        {
            AddHttpContextAccessor(services);

            RegisterTypeImplement(services, assembly,
                typeof(IPipelineHandler<,>),
                typeof(IRequestHandler<,>),
                typeof(IPipelineHandler<>),
                typeof(IRequestHandler<>));

            services.AddScoped<IDispatcher, Dispatcher>();
        }

        public static void AddMessagePublisher(this IServiceCollection services, IMessagePublishStrategy? strategy = null)
        {
            AddHttpContextAccessor(services);

            services.AddSingleton(x => strategy ?? new ParallelMessagePublishStrategy());

            services.AddScoped<IMessagePublisher, MessagePublisher>();
        }
    }
}
