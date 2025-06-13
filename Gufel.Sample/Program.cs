﻿using Gufel.Dispatcher.Base.Dispatcher;
using Gufel.Dispatcher.Base.MessagePublisher;
using Gufel.Dispatcher.Implement;
using Gufel.Sample.Models;
using Gufel.Sample.PubSubHandler;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.Sample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start");
            var services = new ServiceCollection();
            services.AddMessagePublisher();
            services.AddSingleton<ISubscribeHandler<OrderModel>, StockHandler>();
            services.AddSingleton<ISubscribeHandler<NotificationModel>, SmsHandler>();

            services.AddDispatcher(typeof(Program).Assembly);

            await using var app = services.BuildServiceProvider();
            var publisher = app.GetRequiredService<IMessagePublisher>();
            var order = new OrderModel() { Id = 1, OrderCount = 22, ProductId = 13, UserId = 1300 };
            publisher.Publish("reg-order", order);

            var dispatcher = app.GetRequiredService<IDispatcher>();
            var command = new SampleRequest() { Id = 1200 };

            var bool1 = RequestTypeChecker.IsGenericIRequest(typeof(SampleRequestNoResponse));
            var bool2 = RequestTypeChecker.IsNonGenericIRequest(typeof(SampleRequestNoResponse));
            Console.WriteLine(bool1);
            Console.WriteLine(bool2);

            var result = await dispatcher.Dispatch(command, CancellationToken.None);
            Console.WriteLine("Dispatch result: " + result.Result);

            var cmd2 = new SampleRequestNoResponse() { Id = 1300 };
            await dispatcher.Dispatch(cmd2, CancellationToken.None);
            Console.WriteLine("Dispatch result: no result ");

            Console.WriteLine("End");
            Console.ReadKey();
        }
    }
}
