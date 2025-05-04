using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gufel.CQRS.Base.PubSub
{
    public class WhenAllMessagePublishStrategy : IMessagePublishStrategy
    {
        public Task SendMessage<T>(IEnumerable<ISubscribeHandler<T>> subscribers, T value)
        {
            var tasks = subscribers.Select(subscriber => subscriber.HandleAsync(value)).ToList();
            return Task.WhenAll(tasks);
        }
    }
}
