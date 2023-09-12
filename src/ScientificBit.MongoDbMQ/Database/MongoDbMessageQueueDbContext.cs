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
using ScientificBit.MongoDbMQ.Configuration;

namespace ScientificBit.MongoDbMQ.Database;

/// <summary>
/// Virtual database context for MongoDB
/// </summary>
internal sealed class MongoDbMessageQueueDbContext
{
    private readonly IMongoDatabase _db;

    public MongoDbMessageQueueDbContext(MongoDbMessageQueueOptions options)
    {
        var client = new MongoClient(options.ConnectionString);
        _db = client.GetDatabase(options.QueueDbName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _db.GetCollection<T>(collectionName);
    }
}