// -------------------------------------------------------------------------------------------------
// <copyright file="DefaultXmlAttributeFactory.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
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
        /// The reserved URI used by the XML parser for namespace declaration attributes (xmlns / xmlns:prefix)
        /// </summary>
        public const string XmlNamespaceDeclarationUri = @"http://www.w3.org/2000/xmlns/";

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
                    NamespaceUri = XmlNamespaceDeclarationUri,
                    Value = XHTMLNameSpaceUri
                };

                return xHtmlAttribute;
            }

            return null;
        }
    }
}
