//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecElementWithAttributesExtensions.cs" company="Starion Group S.A.">
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
    using System.Threading;
    using System.Threading.Tasks;

    using ReqIFSharp.Extensions.Services;

    /// <summary>
    /// The <see cref="SpecElementWithAttributesExtensions"/> class provides a number of extension methods for the <see cref="SpecElementWithAttributes"/> class
    /// </summary>
    public static class SpecElementWithAttributesExtensions
    {
        /// <summary>
        /// Queries the <see cref="ExternalObject"/> from a <see cref="SpecElementWithAttributes"/> 
        /// </summary>
        /// <param name="specElementWithAttributes">
        /// The <see cref="SpecElementWithAttributes"/> to query the <see cref="ExternalObject"/>s from
        /// </param>
        /// <returns>
        /// an <see cref="IEnumerable{ExternalObject}"/>
        /// </returns>
        public static IEnumerable<ExternalObject> QueryExternalObjects(this SpecElementWithAttributes specElementWithAttributes)
        {
            if (specElementWithAttributes == null)
            {
                throw new ArgumentNullException(nameof(specElementWithAttributes));
            }

            var result = new List<ExternalObject>();

            foreach (var attributeValue in specElementWithAttributes.Values.OfType<AttributeValueXHTML>())
            {
                result.AddRange(attributeValue.ExternalObjects);
            }

            return result;
        }

        /// <summary>
        /// Queries the base64 payloads of the <see cref="SpecElementWithAttributes"/>
        /// </summary>
        /// <param name="specElementWithAttributes">
        /// The <see cref="SpecElementWithAttributes"/> to query the base64 payloads from
        /// </param>
        /// <param name="reqIfLoaderService">
        /// The <see cref="IReqIFLoaderService"/> that is used to query the payload from the associated reqifz file-stream
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// an <see cref="IEnumerable{String}"/> that contains base64 encoded strings
        /// </returns>
        public static Task<IEnumerable<Tuple<ExternalObject, string>>> QueryBase64PayloadsAsync(this SpecElementWithAttributes specElementWithAttributes, IReqIFLoaderService reqIfLoaderService, CancellationToken token = default)
        {
            if (specElementWithAttributes == null)
            {
                throw new ArgumentNullException(nameof(specElementWithAttributes));
            }

            if (reqIfLoaderService == null)
            {
                throw new ArgumentNullException(nameof(reqIfLoaderService));
            }

            return QueryBase64PayloadsInternalAsync(specElementWithAttributes, reqIfLoaderService, token);
        }

        /// <summary>
        /// Queries the base64 payloads of the <see cref="SpecElementWithAttributes"/>
        /// </summary>
        /// <param name="specElementWithAttributes">
        /// The <see cref="SpecElementWithAttributes"/> to query the base64 payloads from
        /// </param>
        /// <param name="reqIfLoaderService">
        /// The <see cref="IReqIFLoaderService"/> that is used to query the payload from the associated reqifz file-stream
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// an <see cref="IEnumerable{String}"/> that contains base64 encoded strings
        /// </returns>
        private static async Task<IEnumerable<Tuple<ExternalObject, string>>> QueryBase64PayloadsInternalAsync(this SpecElementWithAttributes specElementWithAttributes, IReqIFLoaderService reqIfLoaderService, CancellationToken token)
        {
            var result = new List<Tuple<ExternalObject, string>>();

            foreach (var specObjectValue in specElementWithAttributes.Values.OfType<AttributeValueXHTML>())
            {
                foreach (var externalObject in specObjectValue.ExternalObjects)
                {
                    var payload = await reqIfLoaderService.QueryDataAsync(externalObject, token);
                    result.Add(new Tuple<ExternalObject, string>(externalObject, payload));
                }
            }

            return result;
        }
    }
}
