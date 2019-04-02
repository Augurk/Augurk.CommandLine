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
using Augurk.CommandLine.Plumbing;
using System;

namespace Augurk.CommandLine.Commands
{
    /// <summary>
    /// Implements the delete command.
    /// </summary>
    internal class DeleteCommand : BaseCommand<DeleteOptions>
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        /// <param name="options">A <see cref="DeleteOptions"/> instance containing the relevant options for the command.</param>
        public DeleteCommand(DeleteOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Called when the command is to be executed.
        /// </summary>
        protected override int ExecuteCore()
        {
            // Determine the base Uri we're going to perform the delete on
            var baseUri = new Uri($"{Options.AugurkUrl}/api/v2/products/{Options.ProductName}/");
            var deleteUri = baseUri;

            // If a group name is specified
            if (!String.IsNullOrWhiteSpace(Options.GroupName))
            {
                // Append it to the base uri
                deleteUri = new Uri(deleteUri, $"groups/{Options.GroupName}/");
            }

            // If a feature name is specified
            if (!String.IsNullOrWhiteSpace(Options.FeatureName))
            {
                // Make sure that the group name is also specified
                if (String.IsNullOrWhiteSpace(Options.GroupName))
                {
                    Console.WriteLine("When deleting a specific feature a group name that the feature belongs to must also be specified.");
                    return -1;
                }

                // Append the feature name to the base uri
                deleteUri = new Uri(deleteUri, $"features/{Options.FeatureName}/");
            }

            // If a version is specified
            if (!String.IsNullOrWhiteSpace(Options.Version))
            {
                // Append the version to the base uri
                deleteUri = new Uri(deleteUri, $"versions/{Options.Version}/");
            }

            // Perform the delete operation
            using (var client = AugurkHttpClientFactory.CreateHttpClient(Options))
            {
                // Call the URL
                var response = client.DeleteAsync(deleteUri).Result;
                if (response.IsSuccessStatusCode)
                {
                    if (!String.IsNullOrWhiteSpace(Options.FeatureName))
                    {
                        Console.WriteLine($"Succesfully deleted feature {Options.FeatureName} from Augurk at {Options.AugurkUrl}");
                    }
                    else
                    {
                        Console.WriteLine($"Succesfully deleted features from Augurk at {Options.AugurkUrl}");
                    }

                    return 0;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(Options.FeatureName))
                    {
                        Console.WriteLine($"Deleting feature {Options.FeatureName} from Augurk at {Options.AugurkUrl} failed with statuscode {response.StatusCode}");
                    }
                    else
                    {
                        Console.WriteLine($"Deleting features from Augurk at {Options.AugurkUrl} failed with statuscode {response.StatusCode}");
                    }

                    return -1;
                }
            }
        }
    }
}
