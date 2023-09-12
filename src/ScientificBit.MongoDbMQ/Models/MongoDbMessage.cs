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

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ScientificBit.MongoDbMQ.Models;

/// <summary>
/// MongoDB document model for message queue item
/// </summary>
internal sealed class MongoDbMessageQueueItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    public string MessageType { get; set; } = string.Empty;

    public int Status { get; set; }

    public int RetryCount { get; set; } = 1;

    public string MessageJson { get; set; } = string.Empty;
}