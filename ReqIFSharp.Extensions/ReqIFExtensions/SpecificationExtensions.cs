// -------------------------------------------------------------------------------------------------
// <copyright file="SpecificationExtensions.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.ReqIFExtensions
{
    using System;
    using System.Collections.Generic;

    using ReqIFSharp;

    /// <summary>
    /// The <see cref="SpecificationExtensions"/> class provides a number of extension methods for the <see cref="Specification"/> class
    /// </summary>
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Queries all the contained (child) <see cref="SpecHierarchy"/> objects in a recursive manner from the <see cref="Specification"/>
        /// </summary>
        /// <param name="specification">
        /// The subject <see cref="Specification"/> from which all the child <see cref="SpecHierarchy"/> are queried.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecHierarchy}"/>
        /// </returns>
        public static IEnumerable<SpecHierarchy> QueryAllContainedSpecHierarchies(this Specification specification)
        {
            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }

            var result = new List<SpecHierarchy>();

            foreach (var specHierarchy in specification.Children)
            {
                result.Add(specHierarchy);

                var specHierarchies = specHierarchy.QueryAllContainedSpecHierarchies();
                result.AddRange(specHierarchies);
            }

            return result;
        }

        /// <summary>
        /// Queries all the referenced <see cref="SpecObject"/> objects in a recursive manner from the <see cref="Specification"/>
        /// </summary>
        /// <param name="specification">
        /// The subject <see cref="Specification"/> from which all the referenced <see cref="SpecObject"/> are queried.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecObject}"/>
        /// </returns>
        public static IEnumerable<SpecObject> QueryAllContainedSpecObjects(this Specification specification)
        {
            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }

            var result = new List<SpecObject>();

            var specHierarchies = specification.QueryAllContainedSpecHierarchies();

            foreach (var specHierarchy in specHierarchies)
            {
                result.Add(specHierarchy.Object);
            }

            return result;
        }

        /// <summary>
        /// Queries all the referenced <see cref="SpecObject"/> objects in a recursive manner from the <see cref="Specification"/>
        /// </summary>
        /// <param name="specification">
        /// The subject <see cref="Specification"/> from which all the referenced <see cref="SpecObject"/> are queried.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecObject}"/>
        /// </returns>
        public static IEnumerable<AttributeDefinition> QueryAttributeDefinitions(this Specification specification)
        {
            if (specification == null)
            {
                throw new ArgumentNullException(nameof(specification));
            }

            var result = new HashSet<AttributeDefinition>();

            var specObjects = specification.QueryAllContainedSpecObjects();

            foreach (var specObject in specObjects)
            {
                foreach (var attributeValue in specObject.Values)
                {
                    result.Add(attributeValue.AttributeDefinition);
                }
            }

            return result;
        }
    }
}
