// -------------------------------------------------------------------------------------------------
// <copyright file="SpecElementWithAttributes.cs" company="Starion Group S.A.">
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
    /// An abstract super class for elements that can own attributes.
    /// </summary>
    /// <remarks>
    /// Any element that can own attributes, like a requirement, a specification, or a relation between requirements needs to be
    /// an instance of a concrete subclass of this abstract class.
    /// While this class aggregates the values of the attributes, the association to the attributes’ types that define the acceptable
    /// values for the attributes is realized by concrete sub classes of this class.
    /// </remarks>
    public abstract class SpecElementWithAttributes : Identifiable
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<SpecElementWithAttributes> logger;

        /// <summary>
        /// Backing field for the <see cref="Values"/> property.
        /// </summary>
        private readonly List<AttributeValue> values = new List<AttributeValue>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecElementWithAttributes"/> class.
        /// </summary>
        protected SpecElementWithAttributes()
        {
            this.logger = NullLogger<SpecElementWithAttributes>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecElementWithAttributes"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected SpecElementWithAttributes(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<SpecElementWithAttributes>.Instance : this.loggerFactory.CreateLogger<SpecElementWithAttributes>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecElementWithAttributes"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected SpecElementWithAttributes(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.ReqIFContent = reqIfContent;

            this.logger = this.loggerFactory == null ? NullLogger<SpecElementWithAttributes>.Instance : this.loggerFactory.CreateLogger<SpecElementWithAttributes>();
        }

        /// <summary>
        /// Gets the values of the attributes owned by the element.
        /// </summary>
        public List<AttributeValue> Values => this.values;

        /// <summary>
        /// Gets or sets the owning <see cref="ReqIFContent"/>
        /// </summary>
        public ReqIFContent ReqIFContent { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SpecType"/>
        /// </summary>
        public SpecType SpecType 
        {
            get => this.GetSpecType();

            set => this.SetSpecType(value);
        }

        /// <summary>
        /// Gets the <see cref="SpecType"/> from the subclass
        /// </summary>
        /// <returns>
        /// an instance of <see cref="SpecType"/>
        /// </returns>
        protected abstract SpecType GetSpecType();

        /// <summary>
        /// Sets the <see cref="SpecType"/> to the subclass
        /// </summary>
        /// <param name="specType">
        /// The <see cref="SpecType"/> to set.
        /// </param>
        protected abstract void SetSpecType(SpecType specType);

        /// <summary>
        /// Generates a <see cref="SpecElementWithAttributes"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            using (var specElementWithAttributesReader = reader.ReadSubtree())
            {
                while (specElementWithAttributesReader.Read())
                {
                    var xmlLineInfo = reader as IXmlLineInfo;

                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element)
                    {
                        switch (specElementWithAttributesReader.LocalName)
                        {
                            case "ALTERNATIVE-ID":
                                this.ReadAlternativeId(specElementWithAttributesReader);
                                break;
                            case "TYPE":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    this.ReadSpecType(subtree);
                                }
                                break;
                            case "CHILDREN":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    this.ReadHierarchy(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-BOOLEAN":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueBoolean = new AttributeValueBoolean(this, this.loggerFactory);
                                    attributeValueBoolean.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-DATE":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueDate = new AttributeValueDate(this, this.loggerFactory);
                                    attributeValueDate.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-ENUMERATION":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueEnumeration = new AttributeValueEnumeration(this, this.loggerFactory);
                                    attributeValueEnumeration.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-INTEGER":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueInteger = new AttributeValueInteger(this, this.loggerFactory);
                                    attributeValueInteger.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-REAL":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueReal = new AttributeValueReal(this, this.loggerFactory);
                                    attributeValueReal.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-STRING":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueString = new AttributeValueString(this, this.loggerFactory);
                                    attributeValueString.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-XHTML":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueXhtml = new AttributeValueXHTML(this, this.loggerFactory);
                                    attributeValueXhtml.ReadXml(subtree);
                                }
                                break;
                            default:
                                this.logger.LogWarning("The {LocalName} element at line:position {LineNumber}:{LinePosition} is not supported", specElementWithAttributesReader.LocalName, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);
                                break;
                        }
                    }

                    this.ReadObjectSpecificElements(specElementWithAttributesReader);
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="SpecElementWithAttributes"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            base.ReadXml(reader);
            
            using (var specElementWithAttributesReader = reader.ReadSubtree())
            {
                while (await specElementWithAttributesReader.ReadAsync())
                {
                    token.ThrowIfCancellationRequested();

                    if (await specElementWithAttributesReader.MoveToContentAsync() == XmlNodeType.Element)
                    {
                        var xmlLineInfo = reader as IXmlLineInfo;

                        switch (specElementWithAttributesReader.LocalName)
                        {
                            case "ALTERNATIVE-ID":
                                await this.ReadAlternativeIdAsync(reader, token);
                                break;
                            case "TYPE":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    await this.ReadSpecTypeAsync(subtree, token);
                                }
                                break;
                            case "CHILDREN":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    await this.ReadHierarchyAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-BOOLEAN":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueBoolean = new AttributeValueBoolean(this, this.loggerFactory);
                                    await attributeValueBoolean.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-DATE":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueDate = new AttributeValueDate(this, this.loggerFactory);
                                    await attributeValueDate.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-ENUMERATION":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueEnumeration = new AttributeValueEnumeration(this, this.loggerFactory);
                                    await attributeValueEnumeration.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-INTEGER":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueInteger = new AttributeValueInteger(this, this.loggerFactory);
                                    await attributeValueInteger.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-REAL":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueReal = new AttributeValueReal(this, this.loggerFactory);
                                    await attributeValueReal.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-STRING":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueString = new AttributeValueString(this, this.loggerFactory);
                                    await attributeValueString.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-XHTML":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueXhtml = new AttributeValueXHTML(this, this.loggerFactory);
                                    await attributeValueXhtml.ReadXmlAsync(subtree, token);
                                }
                                break;
                            default:
                                this.logger.LogWarning("The {LocalName} element at line:position {LineNumber}:{LinePosition} is not supported", specElementWithAttributesReader.LocalName, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);
                                break;
                        }
                    }

                    await this.ReadObjectSpecificElementsAsync(specElementWithAttributesReader, token);
                }
            }
        }

        /// <summary>
        /// Reads the concrete <see cref="SpecElementWithAttributes"/> elements
        /// </summary>
        /// <param name="reader">The current <see cref="XmlReader"/></param>
        protected virtual void ReadObjectSpecificElements(XmlReader reader)
        {
        }

        /// <summary>
        /// Asynchronously reads the concrete <see cref="SpecElementWithAttributes"/> elements
        /// </summary>
        /// <param name="reader">
        /// The current <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        protected virtual Task ReadObjectSpecificElementsAsync(XmlReader reader, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="SpecElementWithAttributes"/> sub class
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected abstract void ReadSpecType(XmlReader reader);

        /// <summary>
        /// Asynchronously reads the <see cref="SpecType"/> which is specific to the <see cref="SpecElementWithAttributes"/> sub class
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        protected abstract Task ReadSpecTypeAsync(XmlReader reader, CancellationToken token);

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="SpecElementWithAttributes"/> sub class
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected abstract void ReadHierarchy(XmlReader reader);

        /// <summary>
        /// Asynchronously reads the <see cref="SpecType"/> which is specific to the <see cref="SpecElementWithAttributes"/> sub class
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        protected abstract Task ReadHierarchyAsync(XmlReader reader, CancellationToken token);

        /// <summary>
        /// Converts a <see cref="SpecElementWithAttributes"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            this.WriteValues(writer);
        }

        /// <summary>
        /// Asynchronously converts a <see cref="SpecElementWithAttributes"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            await base.WriteXmlAsync(writer, token);

            await this.WriteValuesAsync(writer, token);
        }

        /// <summary>
        /// Writes the <see cref="AttributeValue"/> objects from the <see cref="Values"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteValues(XmlWriter writer)
        {
            if (this.values.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("VALUES");

            foreach (var attributeValue in this.values)
            {
                if (attributeValue is AttributeValueBoolean)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-BOOLEAN");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueDate)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-DATE");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueEnumeration)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-ENUMERATION");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueInteger)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-INTEGER");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueReal)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-REAL");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueString)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-STRING");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueXHTML)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-XHTML");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="AttributeValue"/> objects from the <see cref="Values"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteValuesAsync(XmlWriter writer, CancellationToken token)
        {
            if (this.values.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null,"VALUES", null);

            foreach (var attributeValue in this.values)
            {
                if (attributeValue is AttributeValueBoolean)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-BOOLEAN", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueDate)
                {
                    await writer.WriteStartElementAsync(null,"ATTRIBUTE-VALUE-DATE", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueEnumeration)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-ENUMERATION", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueInteger)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-INTEGER", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueReal)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-REAL", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueString)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-STRING", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueXHTML)
                {
                    await writer.WriteStartElementAsync(null,"ATTRIBUTE-VALUE-XHTML", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }
            }

            await writer.WriteEndElementAsync();
        }
    }
}
