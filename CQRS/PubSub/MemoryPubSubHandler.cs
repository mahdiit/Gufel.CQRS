using Gufel.CQRS.Base.PubSub;
using System.Collections.Concurrent;

namespace Gufel.CQRS.PubSub
{
    public class MemoryPubSubHandler<T> : IPubSubHandler<T>
    {
        private readonly ConcurrentDictionary<string, List<ISubscribeHandler<T>>> _inMemoryDic = new();

        public void Publish(string topic, T value)
        {
            if (!_inMemoryDic.TryGetValue(topic, out List<ISubscribeHandler<T>>? items)) return;
            Parallel.ForEach(items, async (item) =>
            {
                await item.HandleAsync(value);
            });
        }

        public bool Subscribe(string topic, ISubscribeHandler<T> handler)
        {
            var items = _inMemoryDic.GetOrAdd(topic, []);
            items.Add(handler);
            return true;
        }

        public bool Unsubscribe(string topic, ISubscribeHandler<T> hadnler)
        {
            if (!_inMemoryDic.TryGetValue(topic, out List<ISubscribeHandler<T>>? value)
                ||  !value.Contains(hadnler))
                return false;
            value.Remove(hadnler);
            return true;
        }
    }
}
