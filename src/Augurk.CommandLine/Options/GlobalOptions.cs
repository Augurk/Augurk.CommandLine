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

using CommandLine;
using CommandLine.Text;

namespace Augurk.CommandLine.Options
{
    /// <summary>
    /// Represents the available command line options.
    /// </summary>
    internal class GlobalOptions
    {
        /// <summary>
        /// Options when publishing features.
        /// </summary>
        [VerbOption(PublishOptions.VERB_NAME, HelpText = "Publish features to Augurk.")]
        public PublishOptions PublishVerb { get; set; }

        /// <summary>
        /// Options when deleting features.
        /// </summary>
        [VerbOption(DeleteOptions.VERB_NAME, HelpText = "Delete features from Augurk.")]
        public DeleteOptions DeleteVerb { get; set; }

        /// <summary>
        /// Gets the usage of the command line tool for the specified verb.
        /// </summary>
        /// <param name="verb">Name of the verb for which the usage should be retrieved.</param>
        /// <returns>Returns a string containing the usage of the specified verb.</returns>
        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }
}
