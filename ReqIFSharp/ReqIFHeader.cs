// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFHeader.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2021 RHEA System S.A.
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
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="ReqIFHeader"/> class holds metadata relevant to a ReqIF Exchange Document content.
    /// </summary>
    /// <remarks>
    /// Meta-information held in the <see cref="ReqIFHeader"/> element is applicable to the Exchange Document as a whole.
    /// </remarks>
    public class ReqIFHeader : IXmlSerializable
    {
        /// <summary>
        /// Gets or sets an optional comment associated with the Exchange Document as a whole
        /// </summary>
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
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the optional unique identifier of the repository containing the requirements that have been exported.
        /// </summary>
        /// <remarks>
        /// Examples for repositoryID: databaseId, URL.
        /// </remarks>
        public string RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the exporting "ReqIF" tool.
        /// </summary>
        public string ReqIFToolId { get; set; }

        /// <summary>
        /// Gets or sets the ReqIF interchange format and protocol version
        /// </summary>
        public string ReqIFVersion { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the exporting requirements management tool
        /// </summary>
        public string SourceToolId { get; set; }

        /// <summary>
        /// Gets or sets the title of the Exchange Document.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for whole exchange XML document.
        /// </summary>
        /// <remarks>
        /// The value of the identifier is of the XML Schema data type “xsd::ID”
        /// </remarks>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the document root element.
        /// </summary>
        public ReqIF DocumentRoot { get; set; }

        /// <summary>
        /// Generates a <see cref="ReqIFContent"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "REQ-IF-HEADER":
                            this.Identifier = reader.GetAttribute("IDENTIFIER");
                            break;
                        case "COMMENT":
                            this.Comment = reader.ReadElementContentAsString();
                            break;
                        case "CREATION-TIME":
                            this.CreationTime = XmlConvert.ToDateTime(reader.ReadElementContentAsString(), XmlDateTimeSerializationMode.RoundtripKind);
                            break;
                        case "REPOSITORY-ID":
                            this.RepositoryId = reader.ReadElementContentAsString();
                            break;
                        case "REQ-IF-TOOL-ID":
                            this.ReqIFToolId = reader.ReadElementContentAsString();
                            break;
                        case "REQ-IF-VERSION":
                            this.ReqIFVersion = reader.ReadElementContentAsString();
                            break;
                        case "SOURCE-TOOL-ID":
                            this.SourceToolId = reader.ReadElementContentAsString();
                            break;
                        case "TITLE":
                            this.Title = reader.ReadElementContentAsString();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="ReqIFContent"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("IDENTIFIER", this.Identifier);

            if (!string.IsNullOrEmpty(this.Comment))
            {
                writer.WriteElementString("COMMENT", this.Comment);
            }

            writer.WriteElementString("CREATION-TIME", XmlConvert.ToString(this.CreationTime, XmlDateTimeSerializationMode.RoundtripKind));

            if (!string.IsNullOrEmpty(this.RepositoryId))
            {
                writer.WriteElementString("REPOSITORY-ID", this.RepositoryId);
            }

            writer.WriteElementString("REQ-IF-TOOL-ID", this.ReqIFToolId);
            writer.WriteElementString("REQ-IF-VERSION", this.ReqIFVersion);
            writer.WriteElementString("SOURCE-TOOL-ID", this.SourceToolId);
            writer.WriteElementString("TITLE", this.Title);
        }

        /// <summary>
        /// This method is reserved and should not be used.
        /// </summary>
        /// <returns>returns null</returns>
        /// <remarks>
        /// When implementing the IXmlSerializable interface, you should return null
        /// </remarks>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
    }
}
