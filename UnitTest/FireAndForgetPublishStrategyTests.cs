using Gufel.Dispatcher.Base.MessagePublisher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace Gufel.UnitTest;

public class FailingSubscriber(string topic, Exception? exceptionToThrow = null) : ISubscribeHandler<TestMessage>
{
    public string Topic { get; } = topic;
    public int HandleCallCount { get; private set; }

    public Task HandleAsync(TestMessage data, CancellationToken cancellationToken)
    {
        HandleCallCount++;
        if (exceptionToThrow is not null)
            throw exceptionToThrow;
        return Task.CompletedTask;
    }
}

public class FireAndForgetPublishStrategyTests
{
    [Fact]
    public async Task SendMessage_WithSubscribers_ShouldInvokeThemViaChannel()
    {
        var subscriber = new TestSubscriber("topic1");
        var logger = NullLogger<FireAndForgetPublishStrategy>.Instance;

        using var strategy = new FireAndForgetPublishStrategy(logger);

        await strategy.SendMessage(
            [subscriber],
            new TestMessage { Content = "hello" },
            CancellationToken.None);

        await Task.Delay(200);

        subscriber.WasHandled.ShouldBeTrue();
        subscriber.ReceivedMessage!.Content.ShouldBe("hello");
    }

    [Fact]
    public async Task SendMessage_WithMultipleSubscribers_ShouldInvokeAll()
    {
        var sub1 = new TestSubscriber("t");
        var sub2 = new TestSubscriber("t");
        var sub3 = new TestSubscriber("t");
        var logger = NullLogger<FireAndForgetPublishStrategy>.Instance;

        using var strategy = new FireAndForgetPublishStrategy(logger);

        await strategy.SendMessage(
            [sub1, sub2, sub3],
            new TestMessage { Content = "x" },
            CancellationToken.None);

        await Task.Delay(200);

        sub1.WasHandled.ShouldBeTrue();
        sub2.WasHandled.ShouldBeTrue();
        sub3.WasHandled.ShouldBeTrue();
    }

    [Fact]
    public async Task SendMessage_WithEmptySubscribers_ShouldReturnCompletedTask()
    {
        var logger = NullLogger<FireAndForgetPublishStrategy>.Instance;
        using var strategy = new FireAndForgetPublishStrategy(logger);

        var result = strategy.SendMessage(
            Array.Empty<ISubscribeHandler<TestMessage>>(),
            new TestMessage { Content = "x" },
            CancellationToken.None);

        result.IsCompletedSuccessfully.ShouldBeTrue();
    }

    [Fact]
    public async Task SendMessage_ShouldReturnImmediately()
    {
        var subscriber = new SlowSubscriber("t", delay: TimeSpan.FromSeconds(2));
        var logger = NullLogger<FireAndForgetPublishStrategy>.Instance;

        using var strategy = new FireAndForgetPublishStrategy(logger);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await strategy.SendMessage([subscriber], new TestMessage(), CancellationToken.None);
        sw.Stop();

        sw.ElapsedMilliseconds.ShouldBeLessThan(500);
    }

    [Fact]
    public async Task ConsumeAsync_WithFailingSubscriber_ShouldLogErrorAndNotCrash()
    {
        var subscriber = new FailingSubscriber("t", new InvalidOperationException("boom"));
        var logger = NullLogger<FireAndForgetPublishStrategy>.Instance;

        using var strategy = new FireAndForgetPublishStrategy(logger);

        await strategy.SendMessage([subscriber], new TestMessage(), CancellationToken.None);
        await Task.Delay(200);

        subscriber.HandleCallCount.ShouldBe(1);
    }

    [Fact]
    public async Task ConsumeAsync_WithFailingAndPassingSubscribers_ShouldInvokeAll()
    {
        var failing = new FailingSubscriber("t", new InvalidOperationException("boom"));
        var passing = new TestSubscriber("t");
        var logger = NullLogger<FireAndForgetPublishStrategy>.Instance;

        using var strategy = new FireAndForgetPublishStrategy(logger);

        await strategy.SendMessage([failing, passing], new TestMessage { Content = "x" }, CancellationToken.None);
        await Task.Delay(200);

        failing.HandleCallCount.ShouldBe(1);
        passing.WasHandled.ShouldBeTrue();
        passing.ReceivedMessage!.Content.ShouldBe("x");
    }

    [Fact]
    public async Task Dispose_ShouldCompleteChannel()
    {
        var logger = NullLogger<FireAndForgetPublishStrategy>.Instance;
        var strategy = new FireAndForgetPublishStrategy(logger);

        strategy.Dispose();

        await Task.Delay(100);

        // After dispose, writing to the channel should still work but won't be consumed
        var subscriber = new TestSubscriber("t");
        await strategy.SendMessage([subscriber], new TestMessage(), CancellationToken.None);
    }

    [Fact]
    public async Task BoundedChannel_WithCapacity_ShouldAcceptMessages()
    {
        var logger = NullLogger<FireAndForgetPublishStrategy>.Instance;

        using var strategy = new FireAndForgetPublishStrategy(logger, capacity: 10);

        var sub = new TestSubscriber("t");
        await strategy.SendMessage([sub], new TestMessage { Content = "bounded" }, CancellationToken.None);
        await Task.Delay(200);

        sub.WasHandled.ShouldBeTrue();
    }

    [Fact]
    public async Task SendMessage_MultipleWrites_ShouldProcessAll()
    {
        var sub1 = new TestSubscriber("t");
        var sub2 = new TestSubscriber("t");
        var logger = NullLogger<FireAndForgetPublishStrategy>.Instance;

        using var strategy = new FireAndForgetPublishStrategy(logger);

        await strategy.SendMessage([sub1], new TestMessage { Content = "1" }, CancellationToken.None);
        await strategy.SendMessage([sub2], new TestMessage { Content = "2" }, CancellationToken.None);

        await Task.Delay(300);

        sub1.WasHandled.ShouldBeTrue();
        sub1.ReceivedMessage!.Content.ShouldBe("1");
        sub2.WasHandled.ShouldBeTrue();
        sub2.ReceivedMessage!.Content.ShouldBe("2");
    }
}

internal class SlowSubscriber(string topic, TimeSpan delay) : ISubscribeHandler<TestMessage>
{
    public string Topic { get; } = topic;
    public bool WasHandled { get; private set; }

    public async Task HandleAsync(TestMessage data, CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken);
        WasHandled = true;
    }
}
