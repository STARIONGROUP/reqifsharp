// -------------------------------------------------------------------------------------------------
// <copyright file="AggregationKind.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

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
