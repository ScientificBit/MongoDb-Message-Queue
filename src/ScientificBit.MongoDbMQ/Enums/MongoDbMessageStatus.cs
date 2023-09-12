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

namespace ScientificBit.MongoDbMQ.Enums;

/// <summary>
/// For queue item status - used internally
/// </summary>
public enum MongoDbMessageStatus
{
    /// <summary>
    /// Message is queued
    /// </summary>
    Queued = 0,

    /// <summary>
    /// Message was dequeued, but not processed/consumed yet
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Message was processed/consumed successfully
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Message for some reason failed to be processed/consumed
    /// </summary>
    Failed = 3
}