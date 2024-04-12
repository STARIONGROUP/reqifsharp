//  -------------------------------------------------------------------------------------------------
//  <copyright file="ExternalObjectExtensions.cs" company="RHEA System S.A.">
// 
//    Copyright 2017-2024 RHEA System S.A.
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
    using System.Text;

    using ReqIFSharp;

    /// <summary>
    /// The <see cref="ExternalObjectExtensions"/> class provides a number of extension methods for the <see cref="ExternalObject"/> class
    /// </summary>
    public static class ExternalObjectExtensions
    {
        /// <summary>
        /// Creates the url of a <see cref="ExternalObject"/>
        /// </summary>
        /// <param name="externalObject">
        /// The <see cref="ExternalObject"/> of which the url is to be created
        /// </param>
        /// <returns>
        /// a string that represents the web-app url
        /// </returns>
        public static string CreateUrl(this ExternalObject externalObject)
        {
            var url = new StringBuilder();
            url.Append($"/reqif/{externalObject.Owner.SpecElAt.ReqIFContent.DocumentRoot.TheHeader.Identifier}");
            url.Append($"/externalobject/{externalObject.Uri.Base64Encode()} ");

            return url.ToString();
        }
    }
}