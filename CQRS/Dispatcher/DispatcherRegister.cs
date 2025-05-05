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
        private static void RegisterTypeImplement(IServiceCollection services, Assembly assembly, Type type)
        {
            var modules = assembly
                .GetTypes()
                .Where(t => type.IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
                .ToList();

            foreach (var module in modules)
            {
                var requestInterface = module.GetInterfaces()
                    .First(p => p.IsGenericType && p.GetGenericTypeDefinition() == type);

                services.AddScoped(requestInterface, module);
            }
        }

        public static void RegisterDispatcher(this IServiceCollection services, Assembly assembly)
        {
            RegisterTypeImplement(services, assembly, typeof(IPipelineHandler<,>));
            RegisterTypeImplement(services, assembly, typeof(IRequestHandler<,>));
            services.AddSingleton<IDispatcher, Dispatcher >();
        }
    }
}
