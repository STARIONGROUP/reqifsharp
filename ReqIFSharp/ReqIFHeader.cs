// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFHeader.cs" company="RHEA System S.A.">
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
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="ReqIFHeader"/> class holds metadata relevant to a ReqIF Exchange Document content.
    /// </summary>
    /// <remarks>
    /// Metainformation held in the <see cref="ReqIFHeader"/> element is applicable to the Exchange Document as a whole.
    /// </remarks>
    [Serializable]
    [XmlType(TypeName = "REQ-IF-HEADER", Namespace = "http://www.omg.org/spec/ReqIF/20110401/reqif.xsd")]
    public class ReqIFHeader
    {
        /// <summary>
        /// Gets or sets an optional comment associated with the Exchange Document as a whole
        /// </summary>
        [XmlElement("COMMENT")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the time of creation of the exchange XML document.
        /// </summary>
        /// <remarks>
        /// the format of the XML Schema data type “dateTime” which specifies the time format as <code>CCYY-MM-DDThh:mm:ss</code> with optional time zone indicator as a suffix <code>±hh:mm</code>.
        /// </remarks>
        /// <example>
        /// Example: 2005-03-04T10:24:18+01:00 (MET time zone).
        /// </example>
        [XmlElement("CREATION-TIME")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the optional unique identifier of the repository containing the requirements that have been exported.
        /// </summary>
        /// <remarks>
        /// Examples for repositoryID: databaseId, URL.
        /// </remarks>
        [XmlElement("REPOSITORY-ID")]
        public string RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the exporting "ReqIF" tool.
        /// </summary>
        [XmlElement("REQ-IF-TOOL-ID")]
        public string ReqIFToolId { get; set; }

        /// <summary>
        /// Gets or sets the ReqIF interchange format and protocol version
        /// </summary>
        [XmlElement("REQ-IF-VERSION")]
        public string ReqIFVersion { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the exporting requirements management tool
        /// </summary>
        [XmlElement("SOURCE-TOOL-ID")]
        public string SourceToolId { get; set; }

        /// <summary>
        /// Gets or sets the title of the Exchange Document.
        /// </summary>
        [XmlElement("TITLE")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for whole exchange XML document.
        /// </summary>
        /// <remarks>
        /// The value of the identifier is of the XML Schema data type “xsd::ID”
        /// </remarks>
        [XmlAttribute(AttributeName = "IDENTIFIER", DataType = "ID")]
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the document root element.
        /// </summary>
        public ReqIF DocumentRoot { get; set; }
    }
}
