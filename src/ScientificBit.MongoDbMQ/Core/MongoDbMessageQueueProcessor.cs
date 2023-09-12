/* Copyright 2021-present Scientific Bit (www.scientificbit.com)
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScientificBit.MongoDbMQ.Abstraction;
using ScientificBit.MongoDbMQ.Configuration;
using ScientificBit.MongoDbMQ.Database;
using ScientificBit.MongoDbMQ.Models;

namespace ScientificBit.MongoDbMQ.Core;

/// <summary>
/// Processes all the messages in the queue
/// </summary>
internal class MongoDbMessageQueueProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MongoDbMessageQueueOptions _options;
    private readonly MongoDbMessageQueueRepository _queue;
    private readonly ILogger<MongoDbMessageQueueProcessor> _logger;

    // TODO: Make this configurable
    private const int MaxRetries = 3;

    public MongoDbMessageQueueProcessor(ILogger<MongoDbMessageQueueProcessor> logger,
        IServiceProvider serviceProvider,
        MongoDbMessageQueueOptions options, MongoDbMessageQueueRepository queue)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _queue = queue;
        _logger = logger;
    }

    public async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

        foreach (var (messageType, consumerType) in _options.Consumers)
        {
            if (cancellationToken.IsCancellationRequested) break;
            var consumer = (IMongoDbMessageConsumer)_serviceProvider.GetRequiredService(consumerType);
            tasks.Add(ProcessQueueAsync(messageType, consumer, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessQueueAsync(Type messageType, IMongoDbMessageConsumer consumer, CancellationToken stoppingToken)
    {
        try
        {
            var random = new Random();
            MongoDbMessageQueueItem? queueItem;
            do
            {
                queueItem = await _queue.DequeueAsync(messageType);
                if (queueItem != null)
                {
                    try
                    {
                        var message = JsonConvert.DeserializeObject(queueItem.MessageJson, messageType);
                        await consumer.ConsumeAsync(message);
                        await _queue.CompleteAsync(queueItem.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error processing message {MessageId}", queueItem.Id);
                        if (queueItem.RetryCount < MaxRetries)
                            await _queue.RequeueAsync(queueItem.Id);
                        else
                            await _queue.FailAsync(queueItem.Id);
                    }
                }
                else
                {
                    _logger.LogDebug("No message of type {MessageType} found in queue", messageType.Name);
                }

                // Add delay for breathing
                // TODO: Make this configurable
                await Task.Delay(random.Next(200, 1000), stoppingToken);

            } while (queueItem != null && !stoppingToken.IsCancellationRequested);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ProcessQueue failed for {MessageType}", messageType.Name);
        }
    }
}