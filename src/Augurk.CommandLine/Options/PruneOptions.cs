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

namespace Augurk.CommandLine.Options
{
    /// <summary>
    /// Represents the available command line options for the prune command.
    /// </summary>
    [Verb(VERB_NAME, HelpText = "Prunes specific versions from products in Augurk.")]
    internal class PruneOptions : SharedOptions
    {
        /// <summary>
        /// Name of the verb for this set of options
        /// </summary>
        public const string VERB_NAME = "prune";

        /// <summary>
        /// Gets or sets the name of the product for which to prune the features.
        /// </summary>
        [Option("productName", HelpText = "Name of the product for which to prune the features.", Required = true)]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the optional name of the group for which to prune the features.
        /// </summary>
        [Option("groupName", HelpText = "Name of the group containing the features to delete.", Required = false)]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether pre-release versions should be pruned.
        /// </summary>
        [Option("prerelease", HelpText = "Indicates that pre-release feature versions for which a matching release version exists should be pruned.", SetName = "version", Required = false)]
        public bool PrereleaseOnly { get; set; }

        /// <summary>
        /// 'Gets or sets a regular expression that should be run on the feature version in order to determine if it should be pruned.
        /// </summary>
        [Option("versionRegex", HelpText = "A regular expression that determines the versions that should be pruned.", SetName = "version", Required = false)]
        public string VersionRegex { get; set; }
    }
}
