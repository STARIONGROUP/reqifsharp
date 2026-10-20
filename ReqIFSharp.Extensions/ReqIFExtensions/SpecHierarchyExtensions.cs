// -------------------------------------------------------------------------------------------------
// <copyright file="SpecHierarchyExtensions.cs" company="Starion Group S.A.">
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
    /// The <see cref="SpecHierarchyExtensions"/> class provides a number of extension methods for the <see cref="SpecHierarchy"/> class
    /// </summary>
    public static class SpecHierarchyExtensions
    {
        /// <summary>
        /// Queries all the contained (child) <see cref="SpecHierarchy"/> objects in a recursive manner from the <see cref="SpecHierarchy"/>
        /// </summary>
        /// <param name="specHierarchy">
        /// The subject <see cref="SpecHierarchy"/> from which all the child <see cref="SpecHierarchy"/> are queried.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecHierarchy}"/>
        /// </returns>
        public static IEnumerable<SpecHierarchy> QueryAllContainedSpecHierarchies(this SpecHierarchy specHierarchy)
        {
            if (specHierarchy == null)
            {
                throw new ArgumentNullException(nameof(specHierarchy));
            }

            var result = new List<SpecHierarchy>();

            foreach (var child in specHierarchy.Children)
            {
                result.Add(child);

                var subChildren = child.QueryAllContainedSpecHierarchies();

                result.AddRange(subChildren);
            }

            return result;
        }
    }
}
