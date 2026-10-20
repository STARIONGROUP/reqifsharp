// -------------------------------------------------------------------------------------------------
// <copyright file="ExternalObjectExtensions.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.ReqIFExtensions
{
    using System;
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
            if (externalObject == null)
            {
                throw new ArgumentNullException(nameof(externalObject));
            }

            var url = new StringBuilder();
            url.Append($"/reqif/{externalObject.Owner.SpecElAt.ReqIFContent.DocumentRoot.TheHeader.Identifier}");
            url.Append($"/externalobject/{externalObject.Uri.Base64Encode()}");

            return url.ToString();
        }
    }
}
