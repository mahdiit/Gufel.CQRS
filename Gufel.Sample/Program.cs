using Gufel.CQRS.Base.PubSub;
using Gufel.CQRS.PubSub;
using Gufel.Sample.Models;
using Gufel.Sample.PubSubHandler;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sart");
            var services = new ServiceCollection();
            services.AddMessagePublisher();
            services.AddSingleton<ISubscribeHandler<OrderModel>, StockHandler>();
            services.AddSingleton<ISubscribeHandler<NotificationModel>, SmsHandler>();

            using var app = services.BuildServiceProvider();
            var publisher = app.GetRequiredService<IMessagePublisher>();

            var order = new OrderModel() { Id = 1, OrderCount = 22, ProductId = 13, UserId = 1300 };
            publisher.Publish("reg-order", order);

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
