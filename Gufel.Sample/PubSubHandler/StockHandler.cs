using Gufel.Sample.Models;
using Gufel.Dispatcher.Base.MessagePublisher;

namespace Gufel.Sample.PubSubHandler
{
    public class StockHandler(IMessagePublisher pubSubHandler) : ISubscribeHandler<OrderModel>
    {
        public string Topic => "reg-order";

        public async Task HandleAsync(OrderModel data, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{DateTime.UtcNow:G}\tProduct id {data.ProductId} by {data.UserId} stock decreased count {data.OrderCount}");
            await Task.Delay(3000, cancellationToken);
            await pubSubHandler.Publish("sms", new NotificationModel() { MobileNo = "09203102059", Text = "Order complete." }, cancellationToken);
            Console.WriteLine($"{DateTime.UtcNow:G}\tSms sent event");
        }
    }
}
