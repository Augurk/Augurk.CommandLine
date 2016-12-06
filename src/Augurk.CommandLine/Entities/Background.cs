using System.Collections.Generic;

namespace Augurk.CommandLine.Entities
{
    /// <summary>
    /// Represents a feature background
    /// </summary>
    public class Background
    {
        /// <summary>
        /// Gets or sets the title of this background.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the localized keyword for this background.
        /// </summary>
        public string Keyword { get; set; }


        /// <summary>
        /// Gets or sets the steps of this background.
        /// </summary>
        public IEnumerable<Step> Steps { get; set; }
    }
}
