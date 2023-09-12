// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Hosting;
using ScientificBit.MongoDbMQ;
using ScientificBit.MongoDbMQ.Extensions;
using ScientificBit.MongoDbMQConsumersTest;

var host = new HostBuilder()
    .ConfigureHostConfiguration(configHost => {
    })
    .ConfigureServices((_, services) => {
        services.ConfigureMongoDbMessageQueue(opts =>
        {
            // TODO: Replace with your real connection string
            opts.ConnectionString = "mongodb://localhost:27017";
            // TODO: Replace with your real messaged database name
            opts.QueueDbName = "test-messages";

            // Register consumers
            opts.RegisterConsumer<QueueTestMessage, QueueTestMessageConsumer>();
        });
    })
    .UseConsoleLifetime()
    .Build();

host.Run();