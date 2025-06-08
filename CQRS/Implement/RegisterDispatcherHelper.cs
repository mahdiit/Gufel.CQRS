using System.Reflection;
using Gufel.Dispatcher.Base.Dispatcher;
using Gufel.Dispatcher.Base.MessagePublisher;
using Gufel.Dispatcher.Implement.Adapter;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Dispatcher.Implement
{
    public static class RegisterHelper
    {
        private static void RegisterTypeImplementations(IServiceCollection services, Assembly assembly, ServiceLifetime lifetime, params Type[] openGenericTypes)
        {
            var implementationTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && openGenericTypes.Contains(i.GetGenericTypeDefinition())))
                .ToList();

            var pipeLineTypes = new List<(Type, Type?)>();

            foreach (var implementationType in implementationTypes)
            {
                var matchingInterface = implementationType.GetInterfaces()
                    .First(i => i.IsGenericType && openGenericTypes.Contains(i.GetGenericTypeDefinition()));

                Register(services, lifetime, matchingInterface, implementationType);

                var args = matchingInterface.GetGenericArguments();
                if (args.Length != 2)
                {
                    if (matchingInterface.Name.Contains("IPipelineHandler"))
                        pipeLineTypes.Add(new ValueTuple<Type, Type?>(args[0], null));

                    continue;
                }

                var requestType = args[0];
                var responseType = args[1];

                var iRequest = typeof(IRequest<>).MakeGenericType(responseType);
                Type adapterHandler; Type interfaceHandler;

                if (matchingInterface.Name.Contains("IRequestHandler"))
                {
                    adapterHandler = typeof(RequestWithResponseHandlerAdapter<,>).MakeGenericType(requestType, responseType);
                    interfaceHandler = typeof(IRequestHandler<,>).MakeGenericType(iRequest, responseType);
                }
                else
                {
                    adapterHandler = typeof(PipelineWithResponseHandlerAdapter<,>).MakeGenericType(requestType, responseType);
                    interfaceHandler = typeof(IPipelineHandler<,>).MakeGenericType(iRequest, responseType);
                    pipeLineTypes.Add(new ValueTuple<Type, Type?>(iRequest, responseType));
                }

                Register(services, lifetime, interfaceHandler, adapterHandler);
            }

            services.AddSingleton<IRequestPipelineMetadata>(new RequestPipelineMetadata(pipeLineTypes));
        }

        private static void Register(IServiceCollection services, ServiceLifetime lifetime, Type fromInterface, Type toImplementType)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(fromInterface, toImplementType);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(fromInterface, toImplementType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(fromInterface, toImplementType);
                    break;
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
