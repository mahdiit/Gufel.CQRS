using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Gufel.Dispatcher.Base.MessagePublisher;

public sealed class FireAndForgetPublishStrategy : IMessagePublishStrategy, IDisposable
{
    private readonly Channel<Func<Task>> _channel;
    private readonly CancellationTokenSource _cts;
    private readonly Task _consumerTask;
    private readonly ILogger _logger;

    public FireAndForgetPublishStrategy(ILogger<FireAndForgetPublishStrategy> logger, int? capacity = null)
    {
        _logger = logger;
        _channel = capacity.HasValue
            ? Channel.CreateBounded<Func<Task>>(
                new BoundedChannelOptions(capacity.Value)
                {
                    SingleReader = true,
                    SingleWriter = false,
                    FullMode = BoundedChannelFullMode.Wait
                })
            : Channel.CreateUnbounded<Func<Task>>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = false
                });

        _cts = new CancellationTokenSource();
        _consumerTask = Task.Run(() => ConsumeAsync(_cts.Token));
    }

    public Task SendMessage<T>(
        IEnumerable<ISubscribeHandler<T>> subscribers,
        T value,
        CancellationToken cancellationToken)
    {
        var materialized = subscribers.ToList();

        if (materialized.Count == 0)
            return Task.CompletedTask;

        _channel.Writer.TryWrite(() =>
            Parallel.ForEachAsync(
                materialized,
                new ParallelOptions { CancellationToken = cancellationToken },
                async (subscriber, ct) =>
                {
                    try
                    {
                        await subscriber.HandleAsync(value, ct).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "FireAndForgetPublishStrategy: subscriber threw an exception");
                    }
                }
            ));

        return Task.CompletedTask;
    }

    private async Task ConsumeAsync(CancellationToken ct)
    {
        try
        {
            await foreach (var action in _channel.Reader.ReadAllAsync(ct).ConfigureAwait(false))
            {
                try
                {
                    await action().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "FireAndForgetPublishStrategy: subscriber threw an exception");
                }
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            _logger.LogDebug("FireAndForgetPublishStrategy: operation canceled");
        }
    }

    public void Dispose()
    {
        _channel.Writer.TryComplete();
        _cts.Cancel();
        _cts.Dispose();
    }
}
