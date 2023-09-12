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

namespace ScientificBit.MongoDbMQ.Abstraction;

/// <summary>
/// Base class for message consumers
/// </summary>
/// <typeparam name="TMessage"></typeparam>
public abstract class MongoDbMessageConsumer<TMessage> : IMongoDbMessageConsumer where TMessage : new()
{
    /// <summary>
    /// Override this method to consume the message
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected abstract Task ConsumeAsync(TMessage message);

    public async Task ConsumeAsync(object? messageObj)
    {
        if (messageObj is TMessage message)
        {
            await ConsumeAsync(message);
        }
    }
}