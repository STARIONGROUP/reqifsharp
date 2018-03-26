// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIF.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="ReqIF"/> class constitutes the root element of a ReqIF Exchange Document.
    /// </summary>
    [Serializable]
    [XmlType(TypeName = "REQ-IF", Namespace = "http://www.omg.org/spec/ReqIF/20110401/reqif.xsd")]
    [XmlRoot("REQ-IF", Namespace = "http://www.omg.org/spec/ReqIF/20110401/reqif.xsd", IsNullable = false)]
    public class ReqIF
    {
        /// <summary>
        /// Backing field for the <see cref="TheHeader"/> property.
        /// </summary>
        private readonly List<ReqIFHeader> theHeader = new List<ReqIFHeader>();

        /// <summary>
        /// Backing field for the <see cref="CoreContent"/> property.
        /// </summary>
        private readonly List<ReqIFContent> coreContent = new List<ReqIFContent>();

        /// <summary>
        /// Backing field for the <see cref="ToolExtensions"/> property.
        /// </summary>
        private readonly List<ReqIFToolExtension> toolExtensions = new List<ReqIFToolExtension>();

        /// <summary>
        /// Gets the mandatory Exchange Document header, which contains metadata relevant for this exchange.
        /// </summary>
        [XmlArray("THE-HEADER")]
        [XmlArrayItem(IsNullable = false)]
        public List<ReqIFHeader> TheHeader
        {
            get
            {
                return this.theHeader;
            }
        }

        /// <summary>
        /// Gets the mandatory Exchange Document content.
        /// </summary>
        [XmlArray("CORE-CONTENT")]
        [XmlArrayItem("REQ-IF-CONTENT", typeof(ReqIFContent))]
        public List<ReqIFContent> CoreContent 
        {
            get
            {
                return this.coreContent;
            }
        }

        /// <summary>
        /// Gets the optional Exchange Document content based on tool extensions, if such extensions and content are present.
        /// </summary>
        [XmlArray("TOOL-EXTENSIONS")]
        [XmlArrayItem(IsNullable = false)]
        public List<ReqIFToolExtension> ToolExtensions 
        {
            get
            {
                return this.toolExtensions;
            }
        }

        /// <summary>
        /// Gets or sets the default language encoding of the Exchange XML Document content
        /// </summary>
        /// <remarks>
        /// The format is defined by the standard for specifying languages in XML documents proposed by the W3C <see cref="http://www.w3.org/TR/xml11/#sec-lang-tag"/>
        /// </remarks>
        [XmlAttribute(AttributeName = "lang", Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang { get; set; }

        /// <summary>
        /// Merge multiple <see cref="ReqIF"/> instances into one <see cref="ReqIF"/>
        /// </summary>
        /// <param name="reqifs">The <see cref="ReqIF"/> to merge</param>
        /// <returns>The <see cref="ReqIF"/></returns>
        internal static ReqIF MergeReqIf(IReadOnlyList<ReqIF> reqifs)
        {
            var reqif = new ReqIF();
            reqif.TheHeader.AddRange(reqifs.SelectMany(x => x.TheHeader));
            reqif.CoreContent.AddRange(reqifs.SelectMany(x => x.CoreContent));
            reqif.ToolExtensions.AddRange(reqifs.SelectMany(x => x.ToolExtensions));
            return reqif;
        }
    }
}
