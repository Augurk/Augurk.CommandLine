using System.Collections.Generic;

namespace Augurk.CommandLine.Entities
{
    /// <summary>
    /// Represents a feature scenario
    /// </summary>
    public class Scenario
    {
        /// <summary>
        /// Gets or sets the title of this scenario.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of this scenario.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tags of this scenario.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the steps of this scenario.
        /// </summary>
        public IEnumerable<Step> Steps { get; set; }

        /// <summary>
        /// Gets or sets the example sets for this scenario.
        /// </summary>
        public IEnumerable<ExampleSet> ExampleSets { get; set; }
    }
}
