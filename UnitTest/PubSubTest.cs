using Gufel.CQRS.Base.PubSub;
using Gufel.CQRS.PubSub;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class PubSubTest
    {
        public class TestHandler : ISubscribeHandler<int>
        {
            public int Item {  get; set; }
            public Task HandleAsync(int data)
            {
                Item =  data;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void Subscribe_handler_test()
        {
            var pubSubHandler = new MemoryPubSubHandler<int>();

            var testHandler1 = new TestHandler();
            var testHandler2 = new TestHandler();

            pubSubHandler.Subscribe("sender", testHandler1);
            pubSubHandler.Subscribe("sender", testHandler2);
            pubSubHandler.Publish("sender", 100);

            testHandler1.Item.ShouldBe(100);            
            testHandler1.Item.ShouldBe(testHandler2.Item);
        }

        [Fact]
        public void Unsubscribe_handler_test()
        {
            var pubSubHandler = new MemoryPubSubHandler<int>();
            var testHandler1 = new TestHandler();

            pubSubHandler.Subscribe("sender", testHandler1);
            var trueResult  = pubSubHandler.Unsubscribe("sender", testHandler1);
            trueResult.ShouldBeTrue();

            var falseResult = pubSubHandler.Unsubscribe("sender", testHandler1);
            falseResult.ShouldBeFalse();
        }
    }
}
