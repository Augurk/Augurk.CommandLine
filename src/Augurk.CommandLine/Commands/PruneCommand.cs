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
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Augurk.CommandLine.Commands
{
    /// <summary>
    /// Implements the prune command.
    /// </summary>
    [Export(typeof(ICommand))]
    [ExportMetadata("Verb", PruneOptions.VERB_NAME)]
    internal class PruneCommand : ICommand
    {
        private readonly PruneOptions _options;

        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        /// <param name="options">A <see cref="PruneOptions"/> instance containing the relevant options for the command.</param>
        [ImportingConstructor]
        public PruneCommand(PruneOptions options)
        {
            _options = options;
        }


        /// <summary>
        /// Called when the command is to be executed.
        /// </summary>
        public void Execute()
        {
            Console.WriteLine($"Pruning features in Augurk at {_options.AugurkUrl}");
            using (var client = AugurkHttpClientFactory.CreateHttpClient(_options))
            {
                try
                {
                    // Get features of the provided product (and group name if provided)
                    var groups = string.IsNullOrWhiteSpace(_options.GroupName) ? this.GetFeaturesForProduct(client) : this.GetFeaturesForGroup(client);
                    foreach (var group in groups)
                    {
                        Console.WriteLine($"Processing features in group {group.Name}");
                        foreach (var feature in group.Features)
                        {
                            var versions = this.GetVersionsForFeature(client, group.Name, feature).ToList();

                            List<string> versionsToDelete;
                            if (_options.PrereleaseOnly)
                            {
                                versionsToDelete = versions.Where(version => version.Contains("-")).ToList();
                            }
                            else
                            {
                                versionsToDelete = versions.Where(version => Regex.Match(version, _options.VersionRegex).Success).ToList();
                            }

                            Console.WriteLine($"\tFound {versions.Count} versions for feature {feature.Title} of which {versionsToDelete.Count} will be deleted");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occured while pruning features in Augurk at {_options.AugurkUrl}");
                    Console.Error.WriteLine(ex.ToString());
                }
            }
        }

        private IEnumerable<FeatureGroup> GetFeaturesForProduct(HttpClient client)
        {
            var groupsUri = $"{_options.AugurkUrl}/api/v2/products/{_options.ProductName}/groups";
            var response = client.GetAsync(groupsUri).Result;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<IEnumerable<FeatureGroup>>(response.Content.ReadAsStringAsync().Result);
        }

        private IEnumerable<FeatureGroup> GetFeaturesForGroup(HttpClient client)
        {
            var groupUri = $"{_options.AugurkUrl}/api/v2/products/{_options.ProductName}/groups/{_options.GroupName}/features";
            var response = client.GetAsync(groupUri).Result;
            response.EnsureSuccessStatusCode();

            return new FeatureGroup[] { new FeatureGroup
            {
                Name = _options.GroupName,
                Features = JsonConvert.DeserializeObject<IEnumerable<FeatureDescription>>(response.Content.ReadAsStringAsync().Result)
            } };
        }

        private IEnumerable<string> GetVersionsForFeature(HttpClient client, string groupName, FeatureDescription feature)
        {
            var versionsUri = $"{_options.AugurkUrl}/api/v2/products/{_options.ProductName}/groups/{groupName}/features/{feature.Title}/versions";
            Console.WriteLine(versionsUri);
            var response = client.GetAsync(versionsUri).Result;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content.ReadAsStringAsync().Result);
        }
    }
}
