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
    /// Command line options that are shared between all verbs.
    /// </summary>
    internal class SharedOptions
    {
        /// <summary>
        /// Gets or sets the URL of the instance of Augurk to which the features files should be published.
        /// </summary>
        [Option("url", HelpText = "URL to the Augurk Instance to which the features files should be published.", Required = true)]
        public string AugurkUrl { get; set; }

        /// <summary>
        /// Flag to indicate that the tool must run under integrated security to access the Augurk API's.
        /// </summary>
        [Option("useIntegratedSecurity", HelpText = "Use integrated security to access the Augurk API's. Do not specify username and password when using integrated security", MutuallyExclusiveSet = "username,password,useBasicAuthentication", Required = false)]
        public bool UseIntegratedSecurity { get; set; }

        /// <summary>
        /// Flag to indicate that the tool must use basic HTTP authentication to access the Augurk API's.
        /// </summary>
        [Option("useBasicAuthentication", HelpText = "Use basic HTTP authentication to access the Augurk API's. You must also specify a username and a password.", MutuallyExclusiveSet = "useIntegratedSecurity", Required = false)]
        public bool UseBasicAuthentication { get; set; }

        /// <summary>
        /// Username for basic HTTP authentication against the Augurk API's.
        /// </summary>
        [Option("username", HelpText = "Username for basic authentication against the Augurk API's.", Required = false)]
        public string BasicAuthenticationUsername { get; set; }

        /// <summary>
        /// Password for basic HTTP authentication against the Augurk API's.
        /// </summary>
        [Option("password", HelpText = "Password for basic authentication against the Augurk API's.", Required = false)]
        public string BasicAuthenticationPassword { get; set; }

        /// <summary>
        /// Compatibility level used while publishing features.
        /// </summary>
        [Option("compat-level", HelpText = "Sets the compatibility level. Currently available levels are: 2", Required = false)]
        public int? CompatibilityLevel { get; set; }
    }
}
