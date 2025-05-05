using Gufel.Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gufel.Dispatcher.Base.MessagePublisher;

namespace Gufel.Sample.PubSubHandler
{
    public class StockHandler(IMessagePublisher pubSubHandler) : ISubscribeHandler<OrderModel>
    {
        public string Topic => "reg-order";

        public async Task HandleAsync(OrderModel data)
        {
            Console.WriteLine($"{DateTime.UtcNow.ToString("G")}\tProduct id {data.ProductId} by {data.UserId} stock decreased count {data.OrderCount}");
            await Task.Delay(3000);
            pubSubHandler.Publish("sms", new NotificationModel() { MobileNo = "09203102059", Text = "Order complete." });
            Console.WriteLine($"{DateTime.UtcNow.ToString("G")}\tSms sent");
        }
    }
}
