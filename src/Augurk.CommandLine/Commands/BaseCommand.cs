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
using Augurk.CommandLine.Options;
using System;
using System.Runtime.InteropServices;

namespace Augurk.CommandLine.Commands
{
    /// <summary>
    /// Abstract base class for commands.
    /// </summary>
    /// <typeparam name="T">Type of the options class required by this command, must derive from SharedOptions.</typeparam>
    internal abstract class BaseCommand<T> : ICommand
        where T : SharedOptions
    {
        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="options">An instance of the options class with the parsed options passed on the command line.</param>
        protected BaseCommand(T options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Gets the configured options for this command.
        /// </summary>
        protected T Options { get; }

        /// <summary>
        /// Called when the command is being executed.
        /// </summary>
        /// <returns>Returns the exit code where 0 indicates success, while any non-zero value indicates failure.</returns>
        public int Execute()
        {
            // Validate the shared options
            if (Options.UseIntegratedSecurity && Options.UseBasicAuthentication)
            {
                Console.WriteLine("Cannot use both basic and integrated authentication at the same time.");
                return -1;
            }
            if (Options.UseIntegratedSecurity && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Integrated security is only available on Windows.");
                return -1;
            }

            return ExecuteCore();
        }

        /// <summary>
        /// Must be implemented by derived classes to implement the actual functionality of the command.
        /// </summary>
        /// <returns>Returns the exit code where 0 indicates success, while any non-zero value indicates failure.</returns>
        protected abstract int ExecuteCore();
    }
}
