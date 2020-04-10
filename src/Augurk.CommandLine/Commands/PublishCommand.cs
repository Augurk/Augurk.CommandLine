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
using System.IO;
using System.Linq;
using System.Net.Http;
using Augurk.CommandLine.Options;
using Augurk.CommandLine.Entities;
using Augurk.CommandLine.Extensions;
using Augurk.CommandLine.Plumbing;
using Gherkin;

namespace Augurk.CommandLine.Commands
{
    /// <summary>
    /// Implements the publish command.
    /// </summary>
    internal class PublishCommand : BaseCommand<PublishOptions>
    {
        public PublishCommand(PublishOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        protected override int ExecuteCore()
        {
            // Publish the feature files
            Console.WriteLine("Starting publishing of feature files...");
            int exitCode = PublishFeatureFiles();
            if (exitCode != 0)
            {
                return exitCode;
            }

            // Check for the existence of a product description
            if (!string.IsNullOrWhiteSpace(Options.ProductName) && !string.IsNullOrWhiteSpace(Options.ProductDescription))
            {
                Console.WriteLine("Start publishing product description...");
                PublishProductDescription();
                Console.WriteLine("Done publishing product description.");
            }

            Console.WriteLine("Done publishing feature files.");
            return 0;
        }

        private int PublishFeatureFiles()
        {
            // Create the HttpClient that will communicate with the API
            bool usev2api = !string.IsNullOrWhiteSpace(Options.ProductName);
            using (var client = AugurkHttpClientFactory.CreateHttpClient(Options))
            {
                // Get the base uri for all further operations
                string groupUri = GetGroupUri(usev2api);

                // Clear any existing features in this group, if required
                if (!usev2api && Options.ClearGroup)
                {
                    Console.WriteLine($"Clearing existing features in group {Options.GroupName ?? "Default"} for branch {Options.BranchName}.");
                    try
                    {
                        client.DeleteAsync(groupUri).Wait();
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine($"An exception occured while clearing existing features in group {Options.GroupName ?? "Default"} for branch {Options.BranchName}.");
                        Console.Error.WriteLine(e.ToString());
                        return -1;
                    }
                }

                // Parse and publish each of the provided feature files
                var expandedList = Expand(Options.FeatureFiles);
                foreach (var featureFile in expandedList)
                {
                    try
                    {
                        // Parse the feature and convert it to the correct format
                        Feature feature = ParseFeatureFile(featureFile);

                        // Get the uri to which the feature should be published
                        string targetUri = GetTargetUri(usev2api, groupUri, feature);

                        // Publish the feature
                        var response = client.PostAsJsonAsync<Feature>(targetUri, feature).Result;

                        // Process the result
                        if (response.IsSuccessStatusCode)
                        {
                            WriteSuccesfulPublishMessage(usev2api, feature);
                        }
                        else
                        {
                            WriteUnsuccesfulPublishMessage(usev2api, targetUri, feature, response);
                            return -1;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        Console.Out.WriteLine($"WARNING: Unable to parse feature file '{featureFile}' since it doesn't contain any Gherkin content.");
                    }
                    catch (CompositeParserException)
                    {
                        Console.Out.WriteLine($"WARNING: Unable to parse feature file '{featureFile}'. Are you missing a language comment or --language option?");
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine($"An exception occured while uploading feature file '{featureFile}");
                        Console.Error.WriteLine(e.ToString());
                        return -1;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets the Uri to the group where the feature should be uploaded to.
        /// </summary>
        /// <param name="usev2api">Indicates whether version 2 of the API is being used.</param>
        /// <returns>Returns a string containing the Uri to the group where features should be uploaded to.</returns>
        private string GetGroupUri(bool usev2api)
        {
            string encodedGroupName = Uri.EscapeDataString(Options.GroupName);
            if (usev2api)
            {
                return $"{Options.AugurkUrl.TrimEnd('/')}/api/v2/products/{Options.ProductName}/groups/{encodedGroupName}/features";
            }
            else
            {
                return $"{Options.AugurkUrl.TrimEnd('/')}/api/features/{Options.BranchName}/{encodedGroupName ?? "Default"}";
            }
        }

        /// <summary>
        /// Gets the Uri for the provided <paramref name="feature"/> within the provided <paramref name="groupUri"/>.
        /// </summary>
        /// <param name="usev2api">Indicates whether version 2 of the API is being used.</param>
        /// <param name="groupUri">Uri to the group where the feature is going to be uploaded.</param>
        /// <param name="feature">A <see cref="Feature"/> instance that is going to be uploaded.</param>
        /// <returns>Returns a string containing the Uri to the feature file.</returns>
        private string GetTargetUri(bool usev2api, string groupUri, Feature feature)
        {
            var encodedFeatureTitle = Uri.EscapeDataString(feature.Title);
            if (usev2api)
            {
                return $"{groupUri}/{encodedFeatureTitle}/versions/{Options.Version}/";
            }
            else
            {
                return $"{groupUri}/{encodedFeatureTitle}";
            }
        }

        /// <summary>
        /// Writes a message to the console indicating a succesful publish.
        /// </summary>
        /// <param name="usev2api">Indicates whether version 2 of the API is being used.</param>
        /// <param name="feature">A <see cref="Feature"/> that was succesfully published.</param>
        private void WriteSuccesfulPublishMessage(bool usev2api, Feature feature)
        {
            if (usev2api)
            {
                Console.WriteLine($"Succesfully published feature '{feature.Title}' version '{Options.Version}' for product '{Options.ProductName}' to group '{Options.GroupName}'.");
            }
            else
            {
                Console.WriteLine($"Succesfully published feature '{feature.Title}' to group '{Options.GroupName ?? "Default"}' for branch {Options.BranchName}.");
            }
        }

        /// <summary>
        /// Writes a message to the console indicating an unsuccesful publish.
        /// </summary>
        /// <param name="usev2api">Indicates whether version 2 of the API is being used.</param>
        /// <param name="targetUri">Uri to which the upload was attempted.</param>
        /// <param name="feature">A <see cref="Feature"/> that was succesfully published.</param>
        /// <param name="responseMessage">Response message that was received.</param>
        private void WriteUnsuccesfulPublishMessage(bool usev2api, string targetUri, Feature feature, HttpResponseMessage responseMessage)
        {
            if (usev2api)
            {
                Console.Error.WriteLine($"Publishing feature '{feature.Title}' version '{Options.Version}' to uri '{targetUri}' resulted in statuscode '{responseMessage.StatusCode}'");
            }
            else
            {
                Console.Error.WriteLine($"Publishing feature '{feature.Title}' to uri '{targetUri}' resulted in statuscode '{responseMessage.StatusCode}'");
            }
        }

        /// <summary>
        /// Expands a list of feature file specifications by resolving wildcards or getting all
        /// .feature files when it's a directory.
        /// </summary>
        /// <remarks>
        /// Wildcard characters only work for file specifications, not for directories.
        /// Specifications with * or ? in the directory specification will be ignored.
        /// </remarks>
        /// <param name="featureFiles">List of feature files specified by the user.</param>
        /// <returns>Expanded set of file names.</returns>
        private static IEnumerable<string> Expand(IEnumerable<string> featureFiles)
        {
            var expandedList = new List<string>();

            foreach (var fileSpec in featureFiles)
            {
                if (Directory.Exists(fileSpec))
                {
                    // spec is a directory, automatically expand to *.feature
                    var files = Directory.GetFiles(fileSpec, "*.feature");
                    expandedList.AddRange(files);
                    continue;
                }

                if (fileSpec.Contains('?') || fileSpec.Contains('*'))
                {
                    // resolve wildcard in file spec
                    var directory = Path.GetDirectoryName(fileSpec);
                    var spec = Path.GetFileName(fileSpec);

                    if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
                    {
                        if (!string.IsNullOrEmpty(spec))
                        {
                            var files = Directory.GetFiles(directory, spec);
                            expandedList.AddRange(files);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Skipping invalid directory specification '{directory}'.");
                    }
                    continue;
                }

                string filePath = fileSpec;
                if (!Path.IsPathRooted(fileSpec))
                {
                    filePath = Path.Combine(Environment.CurrentDirectory, fileSpec);
                }

                if (File.Exists(filePath))
                {
                    expandedList.Add(filePath);
                }
                else
                {
                    Console.WriteLine($"Skipping file '{fileSpec}' because it does not exist.");
                }
            }

            return expandedList;
        }

        private Feature ParseFeatureFile(string featureFile)
        {
            using (var reader = new StreamReader(featureFile))
            {
                var parser = new Parser();
                var dialectProvider = new AugurkDialectProvider(Options.Language);
                var tokenScanner = new TokenScanner(reader);
                var tokenMatcher = new TokenMatcher(dialectProvider);
                var document = parser.Parse(tokenScanner, tokenMatcher);
                if (document.Feature == null)
                {
                    throw new InvalidOperationException("Feature file failed to parse.");
                }

                var feature = document.Feature.ConvertToFeature(dialectProvider.GetDialect(document.Feature.Language, document.Feature.Location));
                feature.SourceFilename = featureFile;

                // change directory to feature-files directory
                string savedDirectory = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(Path.GetDirectoryName(featureFile));

                feature.Description = ProcessDescription(feature.Description);
                foreach (var scenario in feature.Scenarios)
                {
                    scenario.Description = ProcessDescription(scenario.Description);
                }

                // reset current directory
                Directory.SetCurrentDirectory(savedDirectory);

                return feature;
            }
        }

        private string ProcessDescription(string originalDescription)
        {
            if (String.IsNullOrEmpty(originalDescription))
            {
                return originalDescription;
            }

            string result = originalDescription;
            if (Options.CompatibilityLevel <= 2)
            {
                result = result.TrimLineStart();
            }

            if (Options.Embed)
            {
                result = result.EmbedImages();
            }

            return result;
        }

        /// <summary>
        /// Publishes the product description.
        /// </summary>
        private void PublishProductDescription()
        {
            // Make sure that the product description file exists
            if (!File.Exists(Options.ProductDescription))
            {
                Console.Error.WriteLine($"Product description file {Options.ProductDescription} does not exist!");
                return;
            }

            // Upload the contents of the file to Augurk
            using (var client = AugurkHttpClientFactory.CreateHttpClient(Options))
            {
                // Determine the Uri for the product and read the contents of the file
                string productUri = $"{Options.AugurkUrl.TrimEnd('/')}/api/v2/products/{Options.ProductName}/description";
                string productDescription = File.ReadAllText(Options.ProductDescription);

                // Process the description through the images embedder
                productDescription = ProcessDescription(productDescription);

                try
                {
                    // Perform a Put request to the API
                    var response = client.PutAsync(productUri, new StringContent(productDescription, System.Text.Encoding.UTF8, "text/plain")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Succesfully published description for product {Options.ProductName} from {Options.ProductDescription}");
                    }
                    else
                    {
                        Console.WriteLine($"Publishing description for product {Options.ProductName} to {productUri} resulted in status code {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occured while publishing the product description file {Options.ProductDescription}");
                    Console.Error.WriteLine(ex.ToString());
                }
            }
        }
    }
}
