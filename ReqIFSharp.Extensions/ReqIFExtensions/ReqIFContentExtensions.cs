// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFContentExtensions.cs" company="Starion Group S.A.">
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
    /// The <see cref="ReqIFContentExtensions"/> class provides a number of extension methods for the <see cref="ReqIFContent"/> class
    /// </summary>
    public static class ReqIFContentExtensions
    {
        /// <summary>
        /// Queries the <see cref="ExternalObject"/> from a <see cref="ReqIFContent"/> object
        /// </summary>
        /// <param name="reqIfContent">
        /// The <see cref="ReqIFContent"/> to query the <see cref="ExternalObject"/>s from
        /// </param>
        /// <returns>
        /// an <see cref="IEnumerable{ExternalObject}"/>
        /// </returns>
        public static IEnumerable<ExternalObject> QueryExternalObjects(this ReqIFContent reqIfContent)
        {
            if (reqIfContent == null)
            {
                throw new ArgumentNullException(nameof(reqIfContent));
            }

            var specElementWithAttributesList = new List<SpecElementWithAttributes>();

            specElementWithAttributesList.AddRange(reqIfContent.SpecRelationGroups);
            specElementWithAttributesList.AddRange(reqIfContent.Specifications);
            specElementWithAttributesList.AddRange(reqIfContent.SpecObjects);
            specElementWithAttributesList.AddRange(reqIfContent.SpecRelations);

            var result = new List<ExternalObject>();

            foreach (var specElementWithAttributes in specElementWithAttributesList)
            {
                var externalObjects = specElementWithAttributes.QueryExternalObjects().ToList();

                result.AddRange(externalObjects);
            }

            return result;
        }
    }
}
