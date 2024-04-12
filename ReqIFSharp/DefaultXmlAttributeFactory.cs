// -------------------------------------------------------------------------------------------------
// <copyright file="DefaultXmlAttributeFactory.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2024 RHEA System S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System.Linq;

    /// <summary>
    /// The purpose of the <see cref="DefaultXmlAttributeFactory"/> is to create namespace attributes
    /// </summary>
    public static class DefaultXmlAttributeFactory
    {
        /// <summary>
        /// The <see cref="ReqIF"/> schema Uri
        /// </summary>
        public const string ReqIFSchemaUri = @"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd";

        /// <summary>
        /// The XHTML schema uri
        /// </summary>
        public const string XHTMLNameSpaceUri = @"http://www.w3.org/1999/xhtml";

        /// <summary>
        /// Creates the XHTML namespace attribute in case the <see cref="ReqIF"/> document
        /// contains XHTML data
        /// </summary>
        /// <param name="reqIf">
        /// The <see cref="ReqIF"/> document that is to be serialized
        /// </param>
        /// <returns>
        /// An instance of <see cref="XmlAttribute"/>
        /// </returns>
        internal static XmlAttribute CreateXHTMLNameSpaceAttribute(ReqIF reqIf)
        {
            // only add the xhtml namespace if we have any xhtml data
            if (reqIf.CoreContent.DataTypes.OfType<DatatypeDefinitionXHTML>().Any())
            {
                var xHtmlAttribute = new XmlAttribute
                {
                    LocalName = "xhtml",
                    Prefix = "xmlns",
                    Value = XHTMLNameSpaceUri
                };

                return xHtmlAttribute;
            }

            return null;
        }
    }
}
