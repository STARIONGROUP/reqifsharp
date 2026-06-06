// -------------------------------------------------------------------------------------------------
// <copyright file="AggregationKind.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    /// <summary>
    /// Specifies the kind of aggregation that applies to a property, mirroring the UML
    /// <c>AggregationKind</c> enumeration. It documents whether a property merely references
    /// another model element or owns it (containment).
    /// </summary>
    public enum AggregationKind
    {
        /// <summary>
        /// The property is a plain reference (or a scalar value); the referenced element is not owned.
        /// </summary>
        None,

        /// <summary>
        /// The property is a shared aggregation; the referenced element may be shared by multiple owners.
        /// </summary>
        Shared,

        /// <summary>
        /// The property is a composite aggregation; the owning element contains and owns the value(s).
        /// </summary>
        Composite
    }
}
