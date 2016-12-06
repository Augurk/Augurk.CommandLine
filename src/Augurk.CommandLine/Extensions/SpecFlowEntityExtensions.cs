/*
 Copyright 2014, Mark Taling
 
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
using System.Linq;

namespace Augurk.CommandLine.Entities
{
    /// <summary>
    /// Contains extension methods to transform entities from the <see cref="Gherkin.Ast"/> namespace 
    /// into entities from the <see cref="Augurk.Entities"/> namespace.
    /// </summary>
    public static class GherkinEntityExtensions
    {
        /// <summary>
        /// Converts the provided <see cref="Gherkin.Ast.Feature"/> instance into a <see cref="Augurk.Entities.Feature"/> instance.
        /// </summary>
        /// <param name="feature">The <see cref="Gherkin.Ast.Feature"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Feature"/> instance.</returns>
        public static Feature ConvertToFeature(this Gherkin.Ast.Feature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }

            var background = feature.Children.OfType<Gherkin.Ast.Background>().FirstOrDefault();
            var scenarios = feature.Children.Where(definition => !(definition is Gherkin.Ast.Background));

            return new Feature()
            {
                Title = feature.Name,
                Description = feature.Description,
                Tags = feature.Tags.ConvertToStrings(),
                Scenarios = scenarios.Select(scenario => scenario.ConvertToScenario()).ToArray(),
                Background = background?.ConvertToBackground()
            };
        }

        /// <summary>
        /// Converts the provided <see cref="Gherkin.Ast.Tag"/> instances into an enumerable collection of strings.
        /// </summary>
        /// <param name="tags">The <see cref="Gherkin.Ast.Tag"/> instances that should be converted.</param>
        /// <returns>An enumerable collection of strings.</returns>
        public static IEnumerable<string> ConvertToStrings(this IEnumerable<Gherkin.Ast.Tag> tags)
        {
            if (tags == null)
            {
                return new string[0];
            }

            return tags.Select(t => t.Name.Substring(1)).ToArray();
        }

        /// <summary>
        /// Converts the provided <see cref="Gherkin.Ast.ScenarioDefinition"/> instance into a <see cref="Augurk.Entities.Scenario"/> instance.
        /// </summary>
        /// <param name="scenarioDefinition">The <see cref="Gherkin.Ast.ScenarioDefinition"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Scenario"/> instance.</returns>
        public static Scenario ConvertToScenario(this Gherkin.Ast.ScenarioDefinition scenarioDefinition)
        {
            if (scenarioDefinition == null)
            {
                throw new ArgumentNullException("scenario");
            }

            Gherkin.Ast.ScenarioOutline outline = scenarioDefinition as Gherkin.Ast.ScenarioOutline;
            if (outline != null)
            {
                return outline.ConvertToScenario();
            }

            Gherkin.Ast.Scenario scenario = scenarioDefinition as Gherkin.Ast.Scenario;

            return new Scenario()
            {
                Title = scenario.Name,
                Description = scenario.Description,
                Tags = scenario.Tags.ConvertToStrings(),
                Steps = scenario.Steps.ConvertToSteps()
            };
        }

        /// <summary>
        /// Converts the provided <see cref=" Gherkin.Ast.ScenarioOutline"/> instance into a <see cref="Augurk.Entities.Scenario"/> instance.
        /// </summary>
        /// <param name="scenarioOutline">The <see cref=" Gherkin.Ast.ScenarioOutline"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Scenario"/> instance.</returns>
        public static Scenario ConvertToScenario(this Gherkin.Ast.ScenarioOutline scenarioOutline)
        {
            if (scenarioOutline == null)
            {
                throw new ArgumentNullException("scenarioOutline");
            }

            return new Scenario()
                {
                    Title = scenarioOutline.Name,
                    Description = scenarioOutline.Description,
                    Tags = scenarioOutline.Tags.ConvertToStrings(),
                    Steps = scenarioOutline.Steps.ConvertToSteps(),
                    ExampleSets = scenarioOutline.Examples.ConvertToExampleSets()
                };
        }

        /// <summary>
        /// Converts the provided <see cref=" Gherkin.Ast.Background"/> instance into a <see cref="Augurk.Entities.Background"/> instance.
        /// </summary>
        /// <param name="background">The <see cref=" Gherkin.Ast.Background"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Background"/> instance.</returns>
        public static Background ConvertToBackground(this Gherkin.Ast.Background background)
        {
            if (background == null)
            {
                return null;
            }

            return new Background()
            {
                Title = background.Name,
                Keyword = background.Keyword,
                Steps = background.Steps.ConvertToSteps()
            };
        }

        /// <summary>
        /// Converts the provided <see cref="Gherkin.Ast.Examples"/> instances into an enumerable collection of <see cref="Augurk.Entities.ExampleSet"/> instances.
        /// </summary>
        /// <param name="examples">The <see cref="Gherkin.Ast.Examples"/> instances that should be converted.</param>
        /// <returns>An enumerable collection of <see cref="Augurk.Entities.ExampleSet"/> instances.</returns>
        public static IEnumerable<ExampleSet> ConvertToExampleSets(this IEnumerable<Gherkin.Ast.Examples> examples)
        {
            if (examples == null)
            {
                throw new ArgumentNullException("examples");
            }

            return examples.Select(exampleSet => exampleSet.ConvertToExampleSet()).ToArray();
        }

        /// <summary>
        /// Converts the provided <see cref="Gherkin.Ast.Examples"/> instance into a<see cref="Augurk.Entities.ExampleSet"/> instance.
        /// </summary>
        /// <param name="examples">The <see cref="Gherkin.Ast.Examples"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.ExampleSet"/> instance.</returns>
        public static ExampleSet ConvertToExampleSet(this Gherkin.Ast.Examples examples)
        {
            if (examples == null)
            {
                throw new ArgumentNullException("exampleSet");
            }

            return new ExampleSet()
                {
                    Title = examples.Name,
                    Description = examples.Description,
                    Keyword = examples.Keyword,
                    Tags = examples.Tags.ConvertToStrings(),
                    Columns = examples.TableHeader.Cells.Select(cell => cell.Value).ToArray(),
                    Rows = examples.TableBody.Select(row => row.Cells.Select(cell => cell.Value).ToArray()).ToArray()
                };
        } 

        /// <summary>
        /// Converts the provided <see cref="Gherkin.Ast.Step"/> instances into an enumerable collection of <see cref="Augurk.Entities.Step"/> instances.
        /// </summary>
        /// <param name="steps">The <see cref="Gherkin.Ast.Step"/> instances that should be converted.</param>
        /// <returns>An enumerable collection of <see cref="Augurk.Entities.Step"/> instances.</returns>
        public static IEnumerable<Step> ConvertToSteps(this IEnumerable<Gherkin.Ast.Step> steps)
        {
            if (steps == null)
            {
                return new Step[0];
            }

            return steps.Select(step => step.ConvertToStep()).ToArray();
        }

        /// <summary>
        /// Converts the provided <see cref="Gherkin.Ast.Step"/> instance into a <see cref="Augurk.Entities.Step"/> instance.
        /// </summary>
        /// <param name="step">The <see cref="Gherkin.Ast.Step"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Step"/> instance.</returns>
        public static Step ConvertToStep(this Gherkin.Ast.Step step)
        {
            if (step == null)
            {
                throw new ArgumentNullException("step");
            }

            return new Step()
                {
                    BlockKeyword = BlockKeyword.None, // TODO Figure out what this is and where we're using it 
                    StepKeyword = step.Keyword.ConvertToStepKeyword(),
                    Keyword = step.Keyword,
                    Content = step.Text,
                    TableArgument = step.Argument.ConvertToTable()
                };
        }

        /// <summary>
        /// Converts the provided <see cref="Gherkin.StepKeyword"/> into a <see cref="Augurk.Entities.StepKeyword"/>.
        /// </summary>
        /// <param name="stepKeyword">The <see cref="Gherkin.StepKeyword"/> that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.StepKeyword"/>.</returns>
        public static StepKeyword ConvertToStepKeyword(this string stepKeyword)
        {
            switch (stepKeyword)
            {
                case "Given":
                    return StepKeyword.Given;
                case "Then":
                    return StepKeyword.Then;
                case "When":
                    return StepKeyword.When;
                case "And":
                    return StepKeyword.And;
                default:
                    return StepKeyword.None;
            }
        }

        /// <summary>
        /// Converts the provided <see cref="Gherkin.Ast.StepArgument"/> instance into a <see cref="Augurk.Entities.Table"/> instance.
        /// </summary>
        /// <param name="table">The <see cref="Gherkin.Ast.StepArgument"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Table"/> instanc, or <c>null</c> if the provided <paramref name="argument"/> is not a <see cref="Gherkin.Ast.DataTable"/>.</returns>
        public static Table ConvertToTable(this Gherkin.Ast.StepArgument argument)
        {
            Gherkin.Ast.DataTable table = argument as Gherkin.Ast.DataTable;
            if (table == null)
            {
                return null;
            }

            return new Table
            {
                Columns = table.Rows.FirstOrDefault()?.Cells.Select(cell => cell.Value).ToArray(),
                Rows = table.Rows.Skip(1).Select(row => row.Cells.Select(cell => cell.Value).ToArray()).ToArray()
            };
        }
    }
}
