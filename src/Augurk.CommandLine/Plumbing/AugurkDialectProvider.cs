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

using Gherkin;
using Gherkin.Ast;

namespace Augurk.CommandLine.Plumbing
{
    /// <summary>
    /// An Augurk specific <see cref="GherkinDialectProvider"/>.
    /// </summary>
    internal class AugurkDialectProvider : GherkinDialectProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AugurkDialectProvider"/> class.
        /// </summary>
        /// <param name="defaultLanguage">Default language to use if no language is specific in the Gherkin feature file.</param>
        public AugurkDialectProvider(string defaultLanguage)
            : base(defaultLanguage)
        {
        }

        /// <summary>
        /// Gets the dialect for the provided <paramref name="language"/> at the provided <paramref name="location"/>.
        /// </summary>
        /// <param name="language">Language of the feature file.</param>
        /// <param name="location">Location in the file where the language is specified.</param>
        /// <returns>Returns the appropriate <see cref="GherkinDialectProvider"/>.</returns>
        public override GherkinDialect GetDialect(string language, Location location)
        {
            if (language.Contains("-"))
            {
                try
                {
                    return base.GetDialect(language, location);
                }
                catch (NoSuchLanguageException)
                {
                    var languageBase = language.Split('-')[0];
                    var languageBaseDialect = base.GetDialect(languageBase, location);
                    return new GherkinDialect(
                        language,
                        languageBaseDialect.FeatureKeywords,
                        languageBaseDialect.RuleKeywords,
                        languageBaseDialect.BackgroundKeywords,
                        languageBaseDialect.ScenarioKeywords,
                        languageBaseDialect.ScenarioOutlineKeywords,
                        languageBaseDialect.ExamplesKeywords,
                        languageBaseDialect.GivenStepKeywords,
                        languageBaseDialect.WhenStepKeywords,
                        languageBaseDialect.ThenStepKeywords,
                        languageBaseDialect.AndStepKeywords,
                        languageBaseDialect.ButStepKeywords);
                }
            }

            return base.GetDialect(language, location);
        }
    }
}
