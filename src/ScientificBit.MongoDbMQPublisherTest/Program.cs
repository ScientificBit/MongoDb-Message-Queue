// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using ScientificBit.MongoDbMQ;
using ScientificBit.MongoDbMQ.Abstraction;
using ScientificBit.MongoDbMQ.Extensions;

var serviceProvider = new ServiceCollection()
    .ConfigureMongoDbMessageQueue(opts =>
    {
        // TODO: Replace with your real connection string
        opts.ConnectionString = "mongodb://localhost:27017";
        // TODO: Replace with your real messaged database name
        opts.QueueDbName = "test-messages";
    })
    .BuildServiceProvider();

var messageBus = serviceProvider.GetRequiredService<IMongoDbMessageBus>();

var queueId = await messageBus.PublishAsync(new QueueTestMessage {Message = "Hello, World!"});
Console.WriteLine($"Published message with queue id: {queueId}");

queueId = await messageBus.PublishAsync(new QueueTestMessage {Message = "Hello, again!"});
Console.WriteLine($"Published message with queue id: {queueId}");

queueId = await messageBus.PublishAsync(new QueueTestMessage {Message = "Hello, Scientific Bit!"});
Console.WriteLine($"Published message with queue id: {queueId}");

Console.WriteLine("Done!");