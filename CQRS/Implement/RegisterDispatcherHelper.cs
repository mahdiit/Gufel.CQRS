using Gufel.Dispatcher.Base.Dispatcher;
using Gufel.Dispatcher.Base.MessagePublisher;
using Gufel.Dispatcher.Implement.Adapter;
using Gufel.Dispatcher.Implement.MessagePublisher;
using Gufel.Dispatcher.Implement.MessagePublisher.Strategy;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace Gufel.Dispatcher.Implement
{
    public static class RegisterHelper
    {
        public static void AddDispatcher(this IServiceCollection services, Assembly assembly)
        {
            RegisterTypeImplementations(
                services,
                assembly,
                ServiceLifetime.Scoped,
                typeof(IPipelineHandler<,>),
                typeof(IRequestHandler<,>),
                typeof(IPipelineHandler<>),
                typeof(IRequestHandler<>)
            );

            services.AddScoped<IDispatcher, Dispatcher>();
        }

        public static void AddMessagePublisher(this IServiceCollection services,
            IMessagePublishStrategy? strategy = null,
            IMessagePublisherNameResolver? nameResolver = null)
        {
            services.AddSingleton(nameResolver ?? new MessagePublisherDefaultNameResolver());
            services.AddSingleton(strategy ?? new ParallelMessagePublishStrategy());
            services.AddScoped<IMessagePublisher, MessagePublisher.MessagePublisher>();
        }

        private static void RegisterTypeImplementations(
            IServiceCollection services,
            Assembly assembly,
            ServiceLifetime lifetime,
            params Type[] openGenericTypes)
        {
            var openGenericSet = new HashSet<Type>(openGenericTypes);
            var pipelineMetadata = new List<(Type Request, Type? Response)>();

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface)
                    continue;

                var interfaces = type.GetInterfaces();

                foreach (var iface in interfaces)
                {
                    if (!iface.IsGenericType)
                        continue;

                    var genericDef = iface.GetGenericTypeDefinition();
                    if (!openGenericSet.Contains(genericDef))
                        continue;

                    // Register concrete implementation
                    Register(services, lifetime, iface, type);

                    var args = iface.GetGenericArguments();

                    // One-argument handlers
                    if (args.Length == 1)
                    {
                        if (genericDef == typeof(IPipelineHandler<>))
                        {
                            pipelineMetadata.Add((args[0], null));
                        }

                        continue;
                    }

                    // Two-argument handlers
                    var requestType = args[0];
                    var responseType = args[1];

                    var iRequest = typeof(IRequest<>).MakeGenericType(responseType);

                    if (genericDef == typeof(IRequestHandler<,>))
                    {
                        Register(
                            services,
                            lifetime,
                            typeof(IRequestHandler<,>).MakeGenericType(iRequest, responseType),
                            typeof(RequestWithResponseHandlerAdapter<,>).MakeGenericType(requestType, responseType)
                        );
                    }
                    else if (genericDef == typeof(IPipelineHandler<,>))
                    {
                        Register(
                            services,
                            lifetime,
                            typeof(IPipelineHandler<,>).MakeGenericType(iRequest, responseType),
                            typeof(PipelineWithResponseHandlerAdapter<,>).MakeGenericType(requestType, responseType)
                        );

                        pipelineMetadata.Add((iRequest, responseType));
                    }
                }
            }

            services.AddSingleton<IRequestPipelineMetadata>(
                new RequestPipelineMetadata(pipelineMetadata)
            );
        }

        private static void Register(
            IServiceCollection services,
            ServiceLifetime lifetime,
            Type serviceType,
            Type implementationType)
        {
            services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
        }
    }

}
