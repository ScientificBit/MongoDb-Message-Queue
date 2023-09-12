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
using Microsoft.Extensions.Hosting;

namespace ScientificBit.MongoDbMQ.Core;

/// <summary>
/// Background service to consume messages from the queue
/// </summary>
internal sealed class MongoDbMessageQueueHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private bool _isRunning;

    // TODO: Make this configurable
    private const int SleepTime = 2000;

    public MongoDbMessageQueueHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _isRunning = true;
        var random = new Random();
        while (_isRunning && !stoppingToken.IsCancellationRequested)
        {
            // Wait for random time to avoid all instances processing the queue at the same time
            await Task.Delay(SleepTime + random.Next(1000), stoppingToken);

            // A new scope is created for each iteration to avoid memory leaks
            using var scope = _serviceProvider.CreateScope();
            var queueProcessor = scope.ServiceProvider.GetRequiredService<MongoDbMessageQueueProcessor>();
            await queueProcessor.ProcessQueueAsync(stoppingToken);
        }
    }
}