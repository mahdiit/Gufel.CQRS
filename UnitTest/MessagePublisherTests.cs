using Gufel.Dispatcher.Base.MessagePublisher;
using Gufel.Dispatcher.Implement;
using Gufel.Dispatcher.Implement.MessagePublisher.Strategy;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Gufel.UnitTest;

public class TestMessage
{
    public string Content { get; set; } = string.Empty;
}

public class TestSubscriber(string topic) : ISubscribeHandler<TestMessage>
{
    public string Topic { get; } = topic;
    public bool WasHandled { get; private set; }
    public TestMessage? ReceivedMessage { get; private set; }

    public Task HandleAsync(TestMessage data, CancellationToken cancellationToken)
    {
        WasHandled = true;
        ReceivedMessage = data;
        return Task.CompletedTask;
    }
}

public class MessagePublisherTests
{
    private readonly IMessagePublisher _publisher;
    private readonly TestSubscriber _subscriber1;
    private readonly TestSubscriber _subscriber2;
    private readonly TestSubscriber _subscriber3;

    public MessagePublisherTests()
    {
        var services = new ServiceCollection();

        _subscriber1 = new TestSubscriber("topic1");
        _subscriber2 = new TestSubscriber("topic1");
        _subscriber3 = new TestSubscriber("topic2");

        services.AddSingleton<ISubscribeHandler<TestMessage>>(_subscriber1);
        services.AddSingleton<ISubscribeHandler<TestMessage>>(_subscriber2);
        services.AddSingleton<ISubscribeHandler<TestMessage>>(_subscriber3);

        // Use WhenAllMessagePublishStrategy for predictable test behavior
        services.AddMessagePublisher(new WhenAllMessagePublishStrategy());

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        _publisher = serviceProvider.GetRequiredService<IMessagePublisher>();
    }

    [Fact]
    public async Task Publish_WithMatchingTopic_ShouldNotifyAllSubscribers()
    {
        // Arrange
        var message = new TestMessage { Content = "test message" };

        // Act
        await _publisher.Publish("topic1", message, CancellationToken.None);

        // Wait a bit for async operations to complete
        await Task.Delay(100);

        // Assert
        _subscriber1.WasHandled.ShouldBeTrue();
        _subscriber2.WasHandled.ShouldBeTrue();
        _subscriber3.WasHandled.ShouldBeFalse();

        _subscriber1.ReceivedMessage.ShouldBe(message);
        _subscriber2.ReceivedMessage.ShouldBe(message);
        _subscriber3.ReceivedMessage.ShouldBeNull();
    }

    [Fact]
    public async Task Publish_WithNonMatchingTopic_ShouldNotNotifyAnySubscribers()
    {
        // Arrange
        var message = new TestMessage { Content = "test message" };

        // Act
        await _publisher.Publish("nonexistent-topic", message, CancellationToken.None);

        // Wait a bit for async operations to complete
        await Task.Delay(100);

        // Assert
        _subscriber1.WasHandled.ShouldBeFalse();
        _subscriber2.WasHandled.ShouldBeFalse();
        _subscriber3.WasHandled.ShouldBeFalse();
    }

    [Fact]
    public async Task Publish_WithParallelStrategy_ShouldHandleConcurrentSubscribers()
    {
        // Arrange
        var services = new ServiceCollection();
        var subscriber1 = new TestSubscriber("topic1");
        var subscriber2 = new TestSubscriber("topic1");

        services.AddSingleton<ISubscribeHandler<TestMessage>>(subscriber1);
        services.AddSingleton<ISubscribeHandler<TestMessage>>(subscriber2);
        services.AddMessagePublisher(new ParallelMessagePublishStrategy());

        var serviceProvider = services.BuildServiceProvider();
        var publisher = serviceProvider.GetRequiredService<IMessagePublisher>();
        var message = new TestMessage { Content = "test message" };

        // Act
        await publisher.Publish("topic1", message, CancellationToken.None);

        // Wait a bit for async operations to complete
        await Task.Delay(100);

        // Assert
        subscriber1.WasHandled.ShouldBeTrue();
        subscriber2.WasHandled.ShouldBeTrue();
        subscriber1.ReceivedMessage.ShouldBe(message);
        subscriber2.ReceivedMessage.ShouldBe(message);
    }
}