// -------------------------------------------------------------------------------------------------
// <copyright file="RelationGroupTypeExtensions.cs" company="Starion Group S.A.">
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
    using System.Linq;

    using ReqIFSharp;

    /// <summary>
    /// The <see cref="RelationGroupTypeExtensions"/> class provides a number of extension methods for the <see cref="RelationGroupType"/> class
    /// </summary>
    public static class RelationGroupTypeExtensions
    {
        /// <summary>
        /// Queries the <see cref="RelationGroup"/>s that are referencing the <see cref="RelationGroupType"/>
        /// </summary>
        /// <param name="relationGroupType">
        /// The subject <see cref="SpecObjectType"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecObject}"/> that are referencing the <see cref="RelationGroupType"/>
        /// </returns>
        public static IEnumerable<RelationGroup> QueryReferencingRelationGroups(this RelationGroupType relationGroupType)
        {
            if (relationGroupType == null)
            {
                throw new ArgumentNullException(nameof(relationGroupType));
            }

            if (relationGroupType.ReqIFContent == null)
            {
                throw new InvalidOperationException("The owning ReqIFContent of the RelationGroupType is not set.");
            }

            var relationGroups = relationGroupType.ReqIFContent.SpecRelationGroups;

            var result = relationGroups.Where(x => x.Type == relationGroupType);

            return result;
        }
    }
}
