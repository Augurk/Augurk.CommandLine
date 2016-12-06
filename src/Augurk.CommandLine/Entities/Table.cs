using System.Collections.Generic;

namespace Augurk.CommandLine.Entities
{
    /// <summary>
    /// Represents a table
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Gets or sets the columns for this table.
        /// </summary>
        public IEnumerable<string> Columns { get; set; }

        /// <summary>
        /// Gets or sets the rows for this table.
        /// </summary>
        public IEnumerable<IEnumerable<string>> Rows { get; set; }
    }
}
