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

using Augurk.CommandLine.Entities;
using Augurk.CommandLine.Options;
using Augurk.CommandLine.Plumbing;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Augurk.CommandLine.Commands
{
    /// <summary>
    /// Implements the prune command.
    /// </summary>
    internal class PruneCommand : BaseCommand<PruneOptions>
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        /// <param name="options">A <see cref="PruneOptions"/> instance containing the relevant options for the command.</param>
        public PruneCommand(PruneOptions options)
            : base(options)
        {
        }


        /// <summary>
        /// Called when the command is to be executed.
        /// </summary>
        protected override int ExecuteCore()
        {
            Console.WriteLine($"Pruning features for product {Options.ProductName} in Augurk at {Options.AugurkUrl}");
            using (var client = AugurkHttpClientFactory.CreateHttpClient(Options))
            {
                try
                {
                    // Get features of the provided product (and group name if provided)
                    var groups = string.IsNullOrWhiteSpace(Options.GroupName) ? this.GetGroupsForProduct(client) : this.GetFeaturesForGroup(client);
                    foreach (var group in groups)
                    {
                        Console.WriteLine($"Processing features in group {group.Name}");
                        foreach (var feature in group.Features)
                        {
                            var versions = this.GetVersionsForFeature(client, group.Name, feature).ToList();

                            List<string> versionsToDelete;
                            if (Options.PrereleaseOnly)
                            {
                                versionsToDelete = versions.Where(version => version.Contains("-")).ToList();
                            }
                            else
                            {
                                versionsToDelete = versions.Where(version => Regex.Match(version, Options.VersionRegex).Success).ToList();
                            }

                            Console.WriteLine($"\tFound {versions.Count} version(s) for feature {feature.Title} of which {versionsToDelete.Count} will be deleted");

                            foreach (var versionToDelete in versionsToDelete)
                            {
                                DeleteVersionOfFeature(client, group.Name, feature, versionToDelete);
                            }
                        }
                    }

                    Console.WriteLine("Finished pruning features.");
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occured while pruning features in Augurk at {Options.AugurkUrl}");
                    Console.Error.WriteLine(ex.ToString());
                    return -1;
                }
            }
        }

        private IEnumerable<FeatureGroup> GetGroupsForProduct(HttpClient client)
        {
            var groupsUri = $"{Options.AugurkUrl}/api/v2/products/{Options.ProductName}/groups";
            var response = client.GetAsync(groupsUri).Result;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<IEnumerable<FeatureGroup>>(response.Content.ReadAsStringAsync().Result);
        }

        private IEnumerable<FeatureGroup> GetFeaturesForGroup(HttpClient client)
        {
            var groupUri = $"{Options.AugurkUrl}/api/v2/products/{Options.ProductName}/groups/{Options.GroupName}/features";
            var response = client.GetAsync(groupUri).Result;
            response.EnsureSuccessStatusCode();

            return new FeatureGroup[] { new FeatureGroup
            {
                Name = Options.GroupName,
                Features = JsonConvert.DeserializeObject<IEnumerable<FeatureDescription>>(response.Content.ReadAsStringAsync().Result)
            } };
        }

        private IEnumerable<string> GetVersionsForFeature(HttpClient client, string groupName, FeatureDescription feature)
        {
            var versionsUri = $"{Options.AugurkUrl}/api/v2/products/{Options.ProductName}/groups/{groupName}/features/{feature.Title}/versions";
            var response = client.GetAsync(versionsUri).Result;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content.ReadAsStringAsync().Result);
        }

        private void DeleteVersionOfFeature(HttpClient client, string groupName, FeatureDescription feature, string version)
        {
            var versionUri = $"{Options.AugurkUrl}/api/v2/products/{Options.ProductName}/groups/{groupName}/features/{feature.Title}/versions/{version}/";
            var response = client.DeleteAsync(versionUri).Result;
            Console.WriteLine($"\t\tDeleted version {version} of feature {feature.Title}");
            response.EnsureSuccessStatusCode();
        }
    }
}
