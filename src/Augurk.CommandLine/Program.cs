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
using Augurk.CommandLine.Commands;
using Augurk.CommandLine.Options;
using CommandLine;

namespace Augurk.CommandLine
{
    /// <summary>
    /// Entry-point for the command line tool.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Called when the command line tool starts.
        /// </summary>
        /// <param name="args">Arguments for the application.</param>
        static void Main(string[] args)
        {
            // Parse the command line arguments
            int exitCode = 0;
            exitCode = Parser.Default.ParseArguments<PublishOptions, DeleteOptions, PruneOptions>(args)
                .MapResult(
                    (PublishOptions options) => new PublishCommand(options).Execute(),
                    (DeleteOptions options) => new DeleteCommand(options).Execute(),
                    (PruneOptions options) => new PruneCommand(options).Execute(),
                    errs => -1
                );

            Environment.Exit(exitCode);
        }
    }
}
