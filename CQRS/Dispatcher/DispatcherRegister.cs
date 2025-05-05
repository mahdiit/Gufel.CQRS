using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Gufel.CQRS.Base.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.CQRS.Dispatcher
{
    public static class DispatcherRegister
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
            RegisterTypeImplement(services, assembly, typeof(IPipelineHandler<,>));
            RegisterTypeImplement(services, assembly, typeof(IRequestHandler<,>));
            services.AddSingleton<IDispatcher, Dispatcher >();
        }
    }
}
