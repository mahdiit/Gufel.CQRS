﻿using Gufel.Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gufel.Dispatcher.Base.MessagePublisher;

namespace Gufel.Sample.PubSubHandler
{
    public class SmsHandler : ISubscribeHandler<NotificationModel>
    {
        public string Topic => "sms";

        public async Task HandleAsync(NotificationModel data)
        {
            await Task.Delay(1000);
            Console.WriteLine($"{DateTime.UtcNow:G}\t{data.MobileNo}\t{data.Text}");
        }
    }
}
