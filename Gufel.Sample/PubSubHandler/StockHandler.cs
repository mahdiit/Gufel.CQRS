using Gufel.Dispatcher.Base.MessagePublisher;
using Gufel.Dispatcher.Implement.MessagePublisher;
using Gufel.Sample.Models;

namespace Gufel.Sample.PubSubHandler
{
    public class StockHandler(IMessagePublisher pubSubHandler, IMessagePublisherNameResolver nameResolver) : SubscribeHandler<OrderModel>(nameResolver)
    {
        public override async Task HandleAsync(OrderModel data, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"{DateTime.UtcNow:G}\tProduct id {data.ProductId} by {data.UserId} stock decreased count {data.OrderCount}");

            await Task.Delay(3000, cancellationToken);

            await pubSubHandler.Publish(new NotificationModel() { MobileNo = "09203102059", Text = "Order complete." }, cancellationToken);

            Console.WriteLine($"{DateTime.UtcNow:G}\tSms sent");
        }
    }
}
