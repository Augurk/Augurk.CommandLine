/*
 Copyright 2017, Augurk
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/

namespace Augurk.CommandLine.Entities
{
    /// <summary>
    /// Represents the source location that identifies where it was defined.
    /// </summary>
    public class SourceLocation
    {
        /// <summary>
        /// Gets or sets the column index for this source location.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the line number for this source location.
        /// </summary>
        public int Line { get; set; }
    }
}
