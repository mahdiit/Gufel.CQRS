using Gufel.CQRS.PubSub;
using Gufel.Sample.Models;
using Gufel.Sample.PubSubHandler;

namespace Gufel.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sart");
            var orderMemoryHolder = new MemoryPubSubHandler<OrderModel>();
            var smsHolder = new MemoryPubSubHandler<NotificationModel>();

            orderMemoryHolder.Subscribe("reg-order", new StockHandler(smsHolder));
            smsHolder.Subscribe("sms", new SmsHandler());

            var order = new OrderModel() { Id = 1, OrderCount = 22, ProductId = 13, UserId = 1300 };
            orderMemoryHolder.Publish("reg-order", order);

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
