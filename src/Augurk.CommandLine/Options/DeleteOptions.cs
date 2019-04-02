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
    /// Represents the available command line options when deleting features.
    /// </summary>
    [Verb(VERB_NAME, HelpText = "Delete features from Augurk.")]
    internal class DeleteOptions : SharedOptions
    {
        /// <summary>
        /// Name of the verb for this set of options
        /// </summary>
        public const string VERB_NAME = "delete";

        /// <summary>
        /// Gets or sets the name of the product under which the feature files should be published.
        /// </summary>
        [Option("productName", HelpText = "Name of the product containing the features to delete.", Required = true)]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the name of the group under which the feature files should be published.
        /// </summary>
        [Option("groupName", HelpText = "Name of the group containing the features to delete.", Required = false)]
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the name of the group under which the feature files should be published.
        /// </summary>
        [Option("featureName", HelpText = "Name of the feature to delete.", Required = false)]
        public string FeatureName { get; set; }

        /// <summary>
        /// Gets or sets the version of the feature files that are being published.
        /// </summary>
        [Option("version", HelpText = "Version of the feature(s) to delete.", Required = false)]
        public string Version { get; set; }
    }
}
