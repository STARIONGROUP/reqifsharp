// -------------------------------------------------------------------------------------------------
// <copyright file="SpecRelationTypeExtensions.cs" company="Starion Group S.A.">
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
    /// The <see cref="SpecRelationTypeExtensions"/> class provides a number of extension methods for the <see cref="SpecRelationType"/> class
    /// </summary>
    public static class SpecRelationTypeExtensions
    {
        /// <summary>
        /// Queries the <see cref="SpecRelation"/>s that are referencing the <see cref="SpecRelationType"/>
        /// </summary>
        /// <param name="specRelationType">
        /// The subject <see cref="SpecRelationType"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecRelation}"/> that are referencing the <see cref="SpecRelationType"/>
        /// </returns>
        public static IEnumerable<SpecRelation> QueryReferencingSpecRelations(this SpecRelationType specRelationType)
        {
            if (specRelationType == null)
            {
                throw new ArgumentNullException(nameof(specRelationType));
            }

            if (specRelationType.ReqIFContent == null)
            {
                throw new InvalidOperationException("The owning ReqIFContent of the SpecRelationType is not set.");
            }

            var specRelations = specRelationType.ReqIFContent.SpecRelations;

            var result = specRelations.Where(x => x.Type == specRelationType);

            return result;
        }
    }
}
