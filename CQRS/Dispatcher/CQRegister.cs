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
    public static class CqRegister
    {
        private static void RegisterTypeImplement(IServiceCollection services, Assembly assembly, Type type)
        {
            assembly.GetTypes()
                .Where(t => t.GetInterfaces().Length >= 1 &&
                            t.GetInterfaces().Any(p => p.IsGenericType) &&
                            t.GetInterfaces().First(p => p.IsGenericType).GetGenericTypeDefinition() == type
                )
                .ToList()
                .ForEach(handler =>
                {
                    var requestInterface = handler.GetInterfaces()
                        .First(p => p.IsGenericType && p.GetGenericTypeDefinition() == type);
                    services.AddScoped(requestInterface, handler);
                });
        }

        public static void RegisterDispatcher(this IServiceCollection services, Assembly assembly)
        {
            RegisterTypeImplement(services, assembly, typeof(IQueryHandler<,>));
            RegisterTypeImplement(services, assembly, typeof(ICommandHandler<,>));
            RegisterTypeImplement(services, assembly, typeof(IPipelineHandler<,>));

            services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
            services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
        }
    }
}
