using Gufel.CQRS.Base.PubSub;
using System.Collections.Concurrent;

namespace Gufel.CQRS.PubSub
{
    public class MemoryPubSubHandler<T> : IPubSubHandler<T>
    {
        private readonly ConcurrentDictionary<string, List<SubscribeHandler<T>>> _inMemoryDic = new();

        public void Publish(string topic, T value)
        {
            if (!_inMemoryDic.TryGetValue(topic, out List<SubscribeHandler<T>>? items)) return;
            Parallel.ForEach((items), (item) =>
            {
                item.Invoke(value);
            });
        }

        public void Subscribe(string topic, SubscribeHandler<T> handler)
        {
            var items = _inMemoryDic.GetOrAdd(topic, []);
            items.Add(handler);
        }

        public void Unsubscribe(string topic, SubscribeHandler<T> hadnler)
        {
            if (!_inMemoryDic.TryGetValue(topic, out List<SubscribeHandler<T>>? value)) return;
            value.Remove(hadnler);
        }
    }
}
