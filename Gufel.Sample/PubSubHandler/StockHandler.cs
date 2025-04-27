using Gufel.CQRS.Base.PubSub;
using Gufel.Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gufel.Sample.PubSubHandler
{
    public class StockHandler(IPubSubHandler<NotificationModel> pubSubHandler) : ISubscribeHandler<OrderModel>
    {
        public async Task HandleAsync(OrderModel data)
        {
            await Task.Delay(3000);
            Console.WriteLine($"{DateTime.UtcNow.ToString("G")}\tProduct id {data.ProductId} by {data.UserId} stock decreased count {data.OrderCount}");
            pubSubHandler.Publish("sms", new NotificationModel() { MobileNo = "09203102059", Text = "Order complete." });
            Console.WriteLine($"{DateTime.UtcNow.ToString("G")}\tSms sent");
        }
    }
}
