using Gufel.Sample.Models;
using Gufel.Dispatcher.Base.MessagePublisher;

namespace Gufel.Sample.PubSubHandler
{
    public class SmsHandler : ISubscribeHandler<NotificationModel>
    {
        public string Topic => "sms";

        public async Task HandleAsync(NotificationModel data, CancellationToken cancellationToken)
        {
            await Task.Delay(1000, cancellationToken);
            Console.WriteLine($"{DateTime.UtcNow:G}\t{data.MobileNo}\t{data.Text} event");
        }
    }
}
