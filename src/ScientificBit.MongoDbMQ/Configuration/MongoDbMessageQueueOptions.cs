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

using ScientificBit.MongoDbMQ.Abstraction;

namespace ScientificBit.MongoDbMQ.Configuration;

/// <summary>
/// Configuration options for MongoDbMessageQueue
/// </summary>
public sealed class MongoDbMessageQueueOptions
{
    /// <summary>
    /// Registered message consumers - used by library
    /// </summary>
    public IDictionary<Type, Type> Consumers { get; } = new Dictionary<Type, Type>();

    /// <summary>
    /// MongoDb connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Mongo database name
    /// </summary>
    public string QueueDbName { get; set; } = string.Empty;

    /// <summary>
    /// Registers single message consumer for a message type
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TMessageConsumer"></typeparam>
    public void RegisterConsumer<TMessage, TMessageConsumer>()
        where TMessage : new()
        where TMessageConsumer : MongoDbMessageConsumer<TMessage>
    {
        Consumers.Add(typeof(TMessage), typeof(TMessageConsumer));
    }
}