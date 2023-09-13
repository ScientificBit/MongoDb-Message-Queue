# MongoDbMQ

A Simple .NET Core based Message Queue which uses MongoDb.

# Usage

## Define a message class

```
public class QueueTestMessage
{
    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

## Define Consumers

```
public class QueueTestMessageConsumer : MongoDbMessageConsumer<QueueTestMessage>
{
    protected override Task ConsumeAsync(QueueTestMessage message)
    {
        Console.WriteLine($"Received message: {message.Message}");
        return Task.CompletedTask;
    }
}
```

## Configure MongoDbMQ for publishing and consuming messages

```
    // Assuming IServiceCollection is already created
    services.ConfigureMongoDbMessageQueue(opts =>
    {
        // Replace with your real connection string
        opts.ConnectionString = "mongodb://localhost:27017";
        // Replace with your real messaged database name
        opts.QueueDbName = "test-messages";

        // Register consumers - This is required, only if the application consumes messages
        opts.RegisterConsumer<QueueTestMessage, QueueTestMessageConsumer>();
    });
```

## Publishing a Message

// Inject in service

```
using ScientificBit.MongoDbMQ.Abstraction;

public class MessagePublisher
{
    private readonly IMongoDbMessageBus _messageBus;

    public class MessagePublisher(IMongoDbMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task<string> PublishMessage(string msg)
    {
        var messageId = await _messageBus.PublishAsync(msg);
        return messageId;
    }
}

```

# Issues

TBD

# Contributing

TBD

# Documentation

TBD

# About Scientific Bit
Please visit https://www.scientificbit.com
