using Gufel.CQRS.Base.Dispatcher;
using Gufel.CQRS.Base.PubSub;
using Gufel.CQRS.Dispatcher;
using Gufel.CQRS.PubSub;
using Gufel.Sample.Models;
using Gufel.Sample.PubSubHandler;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Sample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Sart");
            var services = new ServiceCollection();
            services.AddMessagePublisher();
            services.AddSingleton<ISubscribeHandler<OrderModel>, StockHandler>();
            services.AddSingleton<ISubscribeHandler<NotificationModel>, SmsHandler>();

            services.RegisterDispatcher(typeof(Program).Assembly);

            await using var app = services.BuildServiceProvider();
            //var publisher = app.GetRequiredService<IMessagePublisher>();
            //var order = new OrderModel() { Id = 1, OrderCount = 22, ProductId = 13, UserId = 1300 };
            //publisher.Publish("reg-order", order);

            var dispatcher = app.GetRequiredService<ICommandDispatcher>();
            var command = new SampleRequest() { Id = 1200 };

            var result  = await dispatcher.Dispatch<SampleRequest, SampleResponse>(command, CancellationToken.None);
            Console.WriteLine("Dispatch result: " + result.Result);

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
