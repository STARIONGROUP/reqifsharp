// -------------------------------------------------------------------------------------------------
// <copyright file="SpecObjectTypeExtensions.cs" company="Starion Group S.A.">
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
    /// The <see cref="SpecObjectTypeExtensions"/> class provides a number of extension methods for the <see cref="SpecObjectType"/> class
    /// </summary>
    public static class SpecObjectTypeExtensions
    {
        /// <summary>
        /// Queries the <see cref="SpecObject"/>s that are referencing the <see cref="SpecObjectType"/>
        /// </summary>
        /// <param name="specObjectType">
        /// The subject <see cref="SpecObjectType"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecObject}"/> that are referencing the <see cref="SpecObjectType"/>
        /// </returns>
        public static IEnumerable<SpecObject> QueryReferencingSpecObjects(this SpecObjectType specObjectType)
        {
            if (specObjectType == null)
            {
                throw new ArgumentNullException(nameof(specObjectType));
            }

            if (specObjectType.ReqIFContent == null)
            {
                throw new InvalidOperationException("The owning ReqIFContent of the SpecObjectType is not set.");
            }

            var specObjects = specObjectType.ReqIFContent.SpecObjects;

            var result =  specObjects.Where(x => x.Type == specObjectType);
            
            return result;
        }
    }
}
