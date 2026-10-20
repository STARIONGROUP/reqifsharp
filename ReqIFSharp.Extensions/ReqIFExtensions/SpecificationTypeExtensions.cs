// -------------------------------------------------------------------------------------------------
// <copyright file="SpecificationTypeExtensions.cs" company="Starion Group S.A.">
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
    /// The <see cref="SpecificationTypeExtensions"/> class provides a number of extension methods for the <see cref="SpecificationType"/> class
    /// </summary>
    public static class SpecificationTypeExtensions
    {
        /// <summary>
        /// Queries the <see cref="Specification"/>s that are referencing the <see cref="SpecificationType"/>
        /// </summary>
        /// <param name="specificationType">
        /// The subject <see cref="SpecificationType"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{Specification}"/> that are referencing the <see cref="SpecificationType"/>
        /// </returns>
        public static IEnumerable<Specification> QueryReferencingSpecifications(this SpecificationType specificationType)
        {
            if (specificationType == null)
            {
                throw new ArgumentNullException(nameof(specificationType));
            }

            if (specificationType.ReqIFContent == null)
            {
                throw new InvalidOperationException("The owning ReqIFContent of the SpecificationType is not set.");
            }

            var specifications = specificationType.ReqIFContent.Specifications;

            var result = specifications.Where(x => x.Type == specificationType);

            return result;
        }
    }
}
