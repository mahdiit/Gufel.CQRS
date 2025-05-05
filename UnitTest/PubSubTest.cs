using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gufel.Dispatcher.Base.MessagePublisher;
using Gufel.Dispatcher.Implement;

namespace Gufel.UnitTest
{
    public class PubSubTest
    {
        public class TestHandler : ISubscribeHandler<int>
        {
            public int Item {  get; set; }

            public string Topic => "sender";

            public Task HandleAsync(int data)
            {
                Item =  data;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void Subscribe_handler_test()
        {
            var services = new ServiceCollection();
            services.AddMessagePublisher();
            services.AddSingleton<ISubscribeHandler<int>, TestHandler>();

            var app = services.BuildServiceProvider();
            var publisher = app.GetRequiredService<IMessagePublisher>();
            publisher.Publish("sender", 100);

            var testHandler1 = app.GetRequiredService<TestHandler>();
            testHandler1.Item.ShouldBe(100);
        }
    }
}
