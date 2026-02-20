//  -------------------------------------------------------------------------------------------------
//  <copyright file="ReqIFContentExtensions.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2026 Starion Group S.A.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  -------------------------------------------------------------------------------------------------

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
