// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFHeader.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The <see cref="ReqIFHeader"/> class holds metadata relevant to a ReqIF Exchange Document content.
    /// </summary>
    /// <remarks>
    /// Meta-information held in the <see cref="ReqIFHeader"/> element is applicable to the Exchange Document as a whole.
    /// </remarks>
    public class ReqIFHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIFHeader"/> class
        /// </summary>
        public ReqIFHeader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIFHeader"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal ReqIFHeader(ILoggerFactory loggerFactory)
        {
        }

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
        internal void ReadXml(XmlReader reader)
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
        /// Asynchronously generates a <see cref="ReqIFContent"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (await reader.MoveToContentAsync() == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "REQ-IF-HEADER":
                            this.Identifier = reader.GetAttribute("IDENTIFIER");
                            break;
                        case "COMMENT":
                            this.Comment = await reader.ReadElementContentAsStringAsync();
                            break;
                        case "CREATION-TIME":
                            this.CreationTime = XmlConvert.ToDateTime(await reader.ReadElementContentAsStringAsync(), XmlDateTimeSerializationMode.RoundtripKind);
                            break;
                        case "REPOSITORY-ID":
                            this.RepositoryId = await reader.ReadElementContentAsStringAsync();
                            break;
                        case "REQ-IF-TOOL-ID":
                            this.ReqIFToolId = await reader.ReadElementContentAsStringAsync();
                            break;
                        case "REQ-IF-VERSION":
                            this.ReqIFVersion = await reader.ReadElementContentAsStringAsync();
                            break;
                        case "SOURCE-TOOL-ID":
                            this.SourceToolId = await reader.ReadElementContentAsStringAsync();
                            break;
                        case "TITLE":
                            this.Title = await reader.ReadElementContentAsStringAsync();
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
        internal void WriteXml(XmlWriter writer)
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
        /// Asynchronously writes a <see cref="ReqIFContent"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            await writer.WriteAttributeStringAsync(null,"IDENTIFIER", null, this.Identifier);

            if (!string.IsNullOrEmpty(this.Comment))
            {
                await writer.WriteElementStringAsync(null, "COMMENT",null,  this.Comment);
            }

            await writer.WriteElementStringAsync(null, "CREATION-TIME", null ,XmlConvert.ToString(this.CreationTime, XmlDateTimeSerializationMode.RoundtripKind));

            if (!string.IsNullOrEmpty(this.RepositoryId))
            {
                await writer.WriteElementStringAsync(null, "REPOSITORY-ID", null, this.RepositoryId);
            }

            await writer.WriteElementStringAsync(null, "REQ-IF-TOOL-ID", null, this.ReqIFToolId);
            await writer.WriteElementStringAsync(null, "REQ-IF-VERSION", null, this.ReqIFVersion);
            await writer.WriteElementStringAsync(null, "SOURCE-TOOL-ID", null, this.SourceToolId);
            await writer.WriteElementStringAsync(null, "TITLE", null, this.Title);
        }
    }
}
