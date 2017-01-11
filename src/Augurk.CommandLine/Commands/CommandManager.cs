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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Augurk.CommandLine.Commands
{
    [Export(typeof(CommandManager))]
    internal class CommandManager
    {
        [ImportMany]
        public IEnumerable<Lazy<ICommand, ICommandMetadata>> Commands;

        public void ExecuteCommand(string commandName)
        {
            // Find the command that implements the verb
            var command = Commands.FirstOrDefault(c => c.Metadata.Verb == commandName);
            if (command == null)
            {
                // Unknown verb
                return;
            }

            // Execute the command
            command.Value.Execute();
        }
    }
}
