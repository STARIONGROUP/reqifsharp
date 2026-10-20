// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIfPropertyAttribute.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;

    /// <summary>
    /// Attribute used to decorate the model properties with metadata sourced from the ReqIF metamodel,
    /// so that the multiplicity, ownership and characteristics of each property are self-documenting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ReqIfPropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIfPropertyAttribute"/> class.
        /// </summary>
        /// <param name="aggregation">
        /// The <see cref="AggregationKind"/> that specifies whether the property references or owns its value(s).
        /// </param>
        /// <param name="lowerValue">
        /// The lower bound (minimum multiplicity) of the property.
        /// </param>
        /// <param name="upperValue">
        /// The upper bound (maximum multiplicity) of the property. Use <see cref="int.MaxValue"/> for unbounded.
        /// </param>
        /// <param name="isOrdered">
        /// A value indicating whether the values of a multivalued property are ordered.
        /// </param>
        /// <param name="isReadOnly">
        /// A value indicating whether the property is read-only.
        /// </param>
        /// <param name="isDerived">
        /// A value indicating whether the property is derived (its value is computed from other values).
        /// </param>
        /// <param name="isDerivedUnion">
        /// A value indicating whether the property is a derived union.
        /// </param>
        /// <param name="isUnique">
        /// A value indicating whether the values of a multivalued property are unique.
        /// </param>
        /// <param name="defaultValue">
        /// The default value of the property, if any.
        /// </param>
        public ReqIfPropertyAttribute(AggregationKind aggregation = AggregationKind.None, int lowerValue = 1, int upperValue = 1,
            bool isOrdered = false,
            bool isReadOnly = false,
            bool isDerived = false,
            bool isDerivedUnion = false,
            bool isUnique = true,
            string defaultValue = null)
        {
            this.Aggregation = aggregation;
            this.LowerValue = lowerValue;
            this.UpperValue = upperValue;
            this.IsOrdered = isOrdered;
            this.IsReadOnly = isReadOnly;
            this.IsDerived = isDerived;
            this.IsDerivedUnion = isDerivedUnion;
            this.IsUnique = isUnique;
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets or sets the <see cref="AggregationKind"/>.
        /// </summary>
        public AggregationKind Aggregation { get; set; }

        /// <summary>
        /// Gets or sets the lower bound (minimum multiplicity) of the property.
        /// </summary>
        public int LowerValue { get; set; }

        /// <summary>
        /// Gets or sets the upper bound (maximum multiplicity) of the property.
        /// </summary>
        public int UpperValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the values of a multivalued property are ordered.
        /// </summary>
        public bool IsOrdered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property is read-only.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property is derived.
        /// </summary>
        public bool IsDerived { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property is a derived union.
        /// </summary>
        public bool IsDerivedUnion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the values of a multivalued property are unique.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets the default value of the property, if any.
        /// </summary>
        public string DefaultValue { get; set; }
    }
}
