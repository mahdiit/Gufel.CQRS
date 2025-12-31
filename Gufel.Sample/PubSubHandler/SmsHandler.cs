using Gufel.Dispatcher.Base.MessagePublisher;
using Gufel.Dispatcher.Implement.MessagePublisher;
using Gufel.Sample.Models;

namespace Gufel.Sample.PubSubHandler
{
    public class SmsHandler(IMessagePublisherNameResolver nameResolver)
        : SubscribeHandler<NotificationModel>(nameResolver)
    {
        public override async Task HandleAsync(NotificationModel data, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);

            Console.WriteLine($"{DateTime.UtcNow:G}\t{data.MobileNo}\t{data.Text}");
        }
    }
}
