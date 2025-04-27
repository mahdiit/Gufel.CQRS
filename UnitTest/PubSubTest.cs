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
        [Fact]
        public async Task Subscribe_handler_test()
        {
            var pubSubHandler = new MemoryPubSubHandler<int>();
            int? senderResult = null;

            pubSubHandler.Subscribe("sender", async (item) => { senderResult = item; });
            pubSubHandler.Publish("sender", 100);

            senderResult.ShouldBe(100);
        }
    }
}
