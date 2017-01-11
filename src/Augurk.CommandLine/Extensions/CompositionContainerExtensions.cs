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

using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace System.ComponentModel.Composition
{
    /// <summary>
    /// Contains extension methods for the MEF <see cref="CompositionContainer"/>.
    /// </summary>
    internal static class CompositionContainerExtensions
    {
        /// <summary>
        /// Adds the <paramref name="exportedValue"/> instance into the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">A <see cref="CompositionContainer"/> to inject the exported value into.</param>
        /// <param name="exportedValue">A value to inject into the <paramref name="container"/>.</param>
        public static void ComposeExportedValue(this CompositionContainer container, object exportedValue)
        {
            // Validate arguments
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            if (exportedValue == null)
                throw new ArgumentNullException(nameof(exportedValue));

            // Create the composition batch
            CompositionBatch batch = new CompositionBatch();
            var metadata = new Dictionary<string, object> {
                { "ExportTypeIdentity", AttributedModelServices.GetTypeIdentity(exportedValue.GetType()) }
            };

            // Add the exported value
            var contractName = AttributedModelServices.GetContractName(exportedValue.GetType());
            batch.AddExport(new Export(contractName, metadata, () => exportedValue));
            container.Compose(batch);
        }
    }
}
