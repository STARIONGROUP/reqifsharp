// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIF.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
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
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The <see cref="ReqIF"/> class constitutes the root element of a ReqIF Exchange Document.
    /// </summary>
    public class ReqIF
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<ReqIF> logger;

        /// <summary>
        /// an <see cref="IEnumerable{XmlAttribute}"/> that is stored when reading an xml reqif file so that when
        /// the <see cref="ReqIF"/> object is serialized again, the original attributes and namespaces are serialized again
        /// </summary>
        private readonly List<XmlAttribute> attributes = new List<XmlAttribute>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIF"/> class
        /// </summary>
        public ReqIF()
        {
            this.logger = NullLogger<ReqIF>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIF"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal ReqIF(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            this.logger = loggerFactory == null ? NullLogger<ReqIF>.Instance : loggerFactory.CreateLogger<ReqIF>();
        }

        /// <summary>
        /// Gets the mandatory Exchange Document header, which contains metadata relevant for this exchange.
        /// </summary>
        public ReqIFHeader TheHeader { get; set; }

        /// <summary>
        /// Gets the mandatory Exchange Document content.
        /// </summary>
        public ReqIFContent CoreContent { get; set; }

        /// <summary>
        /// Gets the optional Exchange Document content based on tool extensions, if such extensions and content are present.
        /// </summary>
        public List<ReqIFToolExtension> ToolExtension { get; set; } = new List<ReqIFToolExtension>();

        /// <summary>
        /// Gets or sets the default language encoding of the Exchange XML Document content
        /// </summary>
        /// <remarks>
        /// The format is defined by the standard for specifying languages in XML documents proposed by the <a href="http://www.w3.org/TR/xml11/#sec-lang-tag"> W3C </a>
        /// </remarks>
        public string Lang { get; set; }

        /// <summary>
        /// Generates a <see cref="ReqIF"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal void ReadXml(XmlReader reader)
        {
            this.Lang = reader.GetAttribute("xml:lang");

            while (reader.MoveToNextAttribute())
            {
                if (reader.Name != "xml:lang")
                {
                    var xmlAttribute = new ReqIFSharp.XmlAttribute
                    {
                        Prefix = reader.Prefix,
                        LocalName = reader.LocalName,
                        Value = reader.Value
                    };

                    attributes.Add(xmlAttribute);
                }
            }

            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element)
                {
                    var xmlLineInfo = reader as IXmlLineInfo;

                    switch (reader.LocalName)
                    {
                        case "THE-HEADER":
                            var headerSubTreeXmlReader = reader.ReadSubtree();
                            this.TheHeader = new ReqIFHeader(this.loggerFactory) { DocumentRoot = this };
                            this.TheHeader.ReadXml(headerSubTreeXmlReader);
                            break;
                        case "CORE-CONTENT":
                            var coreContentTreeXmlReader = reader.ReadSubtree();
                            this.CoreContent = new ReqIFContent(this.loggerFactory) { DocumentRoot = this };
                            this.CoreContent.ReadXml(coreContentTreeXmlReader);
                            break;
                        case "TOOL-EXTENSIONS":
                            var toolExtensionsXmlReader = reader.ReadSubtree();

                            while (toolExtensionsXmlReader.Read())
                            {
                                if (toolExtensionsXmlReader.MoveToContent() == XmlNodeType.Element && toolExtensionsXmlReader.LocalName == "REQ-IF-TOOL-EXTENSION")
                                {
                                    var reqIfToolExtensionSubTreeXmlReader = toolExtensionsXmlReader.ReadSubtree();

                                    var reqIfToolExtension = new ReqIFToolExtension(this.loggerFactory);
                                    reqIfToolExtension.ReadXml(reqIfToolExtensionSubTreeXmlReader);
                                    this.ToolExtension.Add(reqIfToolExtension);
                                }
                            }
                            break;
                        default:
                            this.logger.LogWarning("The {LocalName} element at line:position {LineNumber}:{LinePosition} is not supported", reader.LocalName, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="ReqIF"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            this.Lang = reader.GetAttribute("xml:lang");

            while (reader.MoveToNextAttribute())
            {
                token.ThrowIfCancellationRequested();

                if (reader.Name != "xml:lang")
                {
                    var xmlAttribute = new ReqIFSharp.XmlAttribute
                    {
                        Prefix = reader.Prefix,
                        LocalName = reader.LocalName,
                        Value = reader.Value
                    };

                    attributes.Add(xmlAttribute);
                }
            }

            while (await reader.ReadAsync())
            {
                token.ThrowIfCancellationRequested();

                if (await reader.MoveToContentAsync() == XmlNodeType.Element)
                {
                    var xmlLineInfo = reader as IXmlLineInfo;

                    switch (reader.LocalName)
                    {
                        case "THE-HEADER":
                            var headerSubTreeXmlReader = reader.ReadSubtree();
                            this.TheHeader = new ReqIFHeader(this.loggerFactory) { DocumentRoot = this };
                            await this.TheHeader.ReadXmlAsync(headerSubTreeXmlReader, token);
                            break;
                        case "CORE-CONTENT":
                            var coreContentTreeXmlReader = reader.ReadSubtree();
                            this.CoreContent = new ReqIFContent(this.loggerFactory) { DocumentRoot = this };
                            await this.CoreContent.ReadXmlAsync(coreContentTreeXmlReader, token);
                            break;
                        case "TOOL-EXTENSIONS":
                            var toolExtensionsXmlReader = reader.ReadSubtree();

                            while (await toolExtensionsXmlReader.ReadAsync())
                            {
                                if (await toolExtensionsXmlReader.MoveToContentAsync() == XmlNodeType.Element && toolExtensionsXmlReader.LocalName == "REQ-IF-TOOL-EXTENSION")
                                {
                                    var reqIfToolExtensionSubTreeXmlReader = toolExtensionsXmlReader.ReadSubtree();

                                    var reqIfToolExtension = new ReqIFToolExtension(this.loggerFactory);
                                    await reqIfToolExtension.ReadXmlAsync(reqIfToolExtensionSubTreeXmlReader);
                                    this.ToolExtension.Add(reqIfToolExtension);
                                }
                            }
                            break;
                        default:
                            this.logger.LogWarning("The {LocalName} element at line:position {LineNumber}:{LinePosition} is not supported", reader.LocalName, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="ReqIF"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal void WriteXml(XmlWriter writer)
        {
            this.WriteNameSpaceAttributes(writer);

            if (!string.IsNullOrEmpty(this.Lang))
            {
                writer.WriteAttributeString("xml", "lang", null, this.Lang);
            }

            this.WriteTheHeader(writer);
            this.WriteCoreContent(writer);
            this.WriteToolExtension(writer);
        }

        /// <summary>
        /// Asynchronously converts a <see cref="ReqIF"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await this.WriteNameSpaceAttributesAsync(writer);

            if (!string.IsNullOrEmpty(this.Lang))
            {
                await writer.WriteAttributeStringAsync("xml", "lang", null, this.Lang);
            }

            await this.WriteTheHeaderAsync(writer, token);
            await this.WriteCoreContentAsync(writer, token);
            await this.WriteToolExtensionAsync(writer, token);
        }

        /// <summary>
        /// Writes the namespace attributes to the REQ-IF XML Element
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteNameSpaceAttributes(XmlWriter writer)
        {
            if (attributes.TrueForAll(x => x.Value != DefaultXmlAttributeFactory.XHTMLNameSpaceUri))
            {
                var xmlAttribute = DefaultXmlAttributeFactory.CreateXHTMLNameSpaceAttribute(this);
                if (xmlAttribute != null)
                {
                    this.attributes.Add(xmlAttribute);
                }
            }

            foreach (var xmlAttribute in this.attributes)
            {
                if (xmlAttribute.Prefix != string.Empty)
                {
                    if (xmlAttribute.Prefix == "xmlns")
                    {
                        writer.WriteAttributeString(xmlAttribute.Prefix, xmlAttribute.LocalName, null, xmlAttribute.Value);
                    }
                    else
                    {
                        writer.WriteAttributeString(xmlAttribute.LocalName, xmlAttribute.Prefix, xmlAttribute.Value);
                    }
                }
                else
                {
                    writer.WriteAttributeString(xmlAttribute.LocalName, xmlAttribute.Value);
                }
            }
        }

        /// <summary>
        /// Asynchronously writes the namespace attributes to the REQ-IF XML Element
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private async Task WriteNameSpaceAttributesAsync(XmlWriter writer)
        {
            if (attributes.TrueForAll(x => x.Value != DefaultXmlAttributeFactory.XHTMLNameSpaceUri))
            {
                var xmlAttribute = DefaultXmlAttributeFactory.CreateXHTMLNameSpaceAttribute(this);
                if (xmlAttribute != null)
                {
                    this.attributes.Add(xmlAttribute);
                }
            }

            foreach (var xmlAttribute in this.attributes)
            {
                if (xmlAttribute.Prefix != string.Empty)
                {
                    if (xmlAttribute.Prefix == "xmlns")
                    {
                        await writer.WriteAttributeStringAsync(xmlAttribute.Prefix, xmlAttribute.LocalName, null, xmlAttribute.Value);
                    }
                    else
                    {
                        await writer.WriteAttributeStringAsync(null, xmlAttribute.LocalName, xmlAttribute.Prefix, xmlAttribute.Value);
                    }
                }
                else
                {
                    await writer.WriteAttributeStringAsync(null, xmlAttribute.LocalName, null, xmlAttribute.Value);
                }
            }
        }

        /// <summary>
        /// Writes the <see cref="ReqIFHeader"/> 
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteTheHeader(XmlWriter writer)
        {
            writer.WriteStartElement("THE-HEADER");
            writer.WriteStartElement("REQ-IF-HEADER");
            this.TheHeader.WriteXml(writer);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="ReqIFHeader"/> 
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteTheHeaderAsync(XmlWriter writer, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await writer.WriteStartElementAsync(null, "THE-HEADER", null);
            await writer.WriteStartElementAsync(null,"REQ-IF-HEADER", null);
            await this.TheHeader.WriteXmlAsync(writer, token);
            await writer.WriteEndElementAsync();
            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// Writes the <see cref="ReqIFContent"/> 
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteCoreContent(XmlWriter writer)
        {
            writer.WriteStartElement("CORE-CONTENT");
            writer.WriteStartElement("REQ-IF-CONTENT");
            this.CoreContent.WriteXml(writer);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="ReqIFContent"/> 
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteCoreContentAsync(XmlWriter writer, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await writer.WriteStartElementAsync(null,"CORE-CONTENT", null);
            await writer.WriteStartElementAsync(null,"REQ-IF-CONTENT", null);
            await this.CoreContent.WriteXmlAsync(writer, token);
            await writer.WriteEndElementAsync();
            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// Writes the <see cref="ReqIFToolExtension"/> 
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteToolExtension(XmlWriter writer)
        {
            if (this.ToolExtension.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("TOOL-EXTENSIONS");
            foreach (var reqIfToolExtension in this.ToolExtension)
            {
                writer.WriteStartElement("REQ-IF-TOOL-EXTENSION");
                reqIfToolExtension.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="ReqIFToolExtension"/> 
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteToolExtensionAsync(XmlWriter writer, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (this.ToolExtension.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "TOOL-EXTENSIONS", null);
            foreach (var reqIfToolExtension in this.ToolExtension)
            {
                await writer.WriteStartElementAsync(null, "REQ-IF-TOOL-EXTENSION", null);
                await reqIfToolExtension.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }
            await writer.WriteEndElementAsync();
        }
    }
}
