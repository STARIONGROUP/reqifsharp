// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIfPropertyAttribute.cs" company="Starion Group S.A.">
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
