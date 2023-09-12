using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ScientificBit.MongoDbMQ.Abstraction;
using ScientificBit.MongoDbMQ.Configuration;
using ScientificBit.MongoDbMQ.Core;
using ScientificBit.MongoDbMQ.Database;

namespace ScientificBit.MongoDbMQ.Extensions;

public static class MongoDbMessageQueueServiceExtension
{
    public static IServiceCollection ConfigureMongoDbMessageQueue(this IServiceCollection services, Action<MongoDbMessageQueueOptions> configurator)
    {
        var options = new MongoDbMessageQueueOptions();
        configurator(options);

        // Check for required options
        if (string.IsNullOrEmpty(options.ConnectionString)) throw new MongoConfigurationException("ConnectionString is required");
        if (string.IsNullOrEmpty(options.QueueDbName)) throw new MongoConfigurationException("QueueDbName is required");

        // Setup consumers for DI 
        foreach (var consumerType in options.Consumers.Values)
        {
            services.AddScoped(consumerType);
        }

        services.AddSingleton(options);
        services.AddSingleton<MongoDbMessageQueueDbContext>();
        services.AddScoped<MongoDbMessageQueueRepository>();
        services.AddScoped<IMongoDbMessageBus, MongoDbMessageBus>();

        // Setup hosted service only if there are consumers
        // If no consumers are registered, the intention is to use the message bus only
        if (options.Consumers.Any())
        {
            services.AddScoped<MongoDbMessageQueueProcessor>();
            services.AddHostedService<MongoDbMessageQueueHostedService>();
        }

        return services;
    }
}