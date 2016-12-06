using System.Collections.Generic;

namespace Augurk.CommandLine.Entities
{
    /// <summary>
    /// Represents a feature
    /// </summary>
    public class Feature
    {
        /// <summary>
        /// Gets or sets the title of the feature.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of this feature.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tags of this feature.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the scenarios of this feature.
        /// </summary>
        public IEnumerable<Scenario> Scenarios { get; set; }

        /// <summary>
        /// Gets or sets the background of this feature.
        /// </summary>
        public Background Background { get; set; }
    }
}
