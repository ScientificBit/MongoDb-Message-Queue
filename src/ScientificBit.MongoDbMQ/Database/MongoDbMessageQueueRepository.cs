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

using MongoDB.Driver;
using ScientificBit.MongoDbMQ.Enums;
using ScientificBit.MongoDbMQ.Models;

namespace ScientificBit.MongoDbMQ.Database;

internal sealed class MongoDbMessageQueueRepository
{
    private readonly IMongoCollection<MongoDbMessageQueueItem> _collection;

    public MongoDbMessageQueueRepository(MongoDbMessageQueueDbContext dbContext)
    {
        _collection = dbContext.GetCollection<MongoDbMessageQueueItem>("_queue_messages");
    }

    public async Task<string> AddAsync(Type messageType, string messageJson)
    {
        var item = new MongoDbMessageQueueItem
        {
            MessageJson = messageJson,
            MessageType = GetMessageType(messageType),
            Status = (int)MongoDbMessageStatus.Queued
        };

        await _collection.InsertOneAsync(item);
        return item.Id;
    }

    public async Task<MongoDbMessageQueueItem?> DequeueAsync(Type messageType)
    {
        var filterBuilder = Builders<MongoDbMessageQueueItem>.Filter;
        var filter = filterBuilder.And(
            filterBuilder.Eq(x => x.MessageType, GetMessageType(messageType)),
            filterBuilder.Eq(x => x.Status, (int) MongoDbMessageStatus.Queued)
        );
        var update = Builders<MongoDbMessageQueueItem>.Update.Set(x => x.Status, (int)MongoDbMessageStatus.InProgress);
        var item = await _collection.FindOneAndUpdateAsync(filter, update);
        return item;
    }

    public async Task<bool> RequeueAsync(string queueItemId)
    {
        var updateBuilder = Builders<MongoDbMessageQueueItem>.Update;
        var update = updateBuilder.Inc(x => x.RetryCount, 1)
            .Set(x => x.Status, (int)MongoDbMessageStatus.Queued);
        var result = await _collection.UpdateOneAsync(d => d.Id == queueItemId, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> FailAsync(string queueItemId)
    {
        var update = Builders<MongoDbMessageQueueItem>.Update.Set(x => x.Status, (int)MongoDbMessageStatus.Failed);
        var result = await _collection.UpdateOneAsync(d => d.Id == queueItemId, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> CompleteAsync(string queueItemId)
    {
        var update = Builders<MongoDbMessageQueueItem>.Update.Set(x => x.Status, (int)MongoDbMessageStatus.Completed);
        var result = await _collection.UpdateOneAsync(d => d.Id == queueItemId, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    private string GetMessageType(Type messageType)
    {
        // TODO: Use a better way to get the type name
        // Let's not use FullName as it will need shared Message models
        // return messageType.FullName ?? messageType.Name;
        return messageType.Name;
    }
}