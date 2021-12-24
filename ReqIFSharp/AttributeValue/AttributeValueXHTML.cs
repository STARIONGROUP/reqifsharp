// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueXHTML.cs" company="RHEA System S.A.">
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
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The purpose of the <see cref="AttributeValueXHTML"/> class is to define an attribute value with XHTML contents.
    /// </summary>
    [Serializable]
    public class AttributeValueXHTML : AttributeValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueXHTML"/> class.
        /// </summary>
        public AttributeValueXHTML()
        {
            this.ExternalObjects = new List<ExternalObject>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueXHTML"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        internal AttributeValueXHTML(SpecElementWithAttributes specElAt)
            : base(specElAt)
        {
            this.ExternalObjects = new List<ExternalObject>();
        }

        /// <summary>
        /// Instantiated a new instance of the <see cref="AttributeValueXHTML"/> class
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinitionXHTML"/> for which this is the default value</param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinitionXHTML"/>
        /// </remarks>
        internal AttributeValueXHTML(AttributeDefinitionXHTML attributeDefinition)
            : base(attributeDefinition)
        {
            this.ExternalObjects = new List<ExternalObject>();

            this.OwningDefinition = attributeDefinition;
        }

        /// <summary>
        /// Gets or sets the XHTML Content
        /// </summary>
        public string TheValue { get; set; }

        /// <summary>
        /// Gets or sets the Linkage to the original attribute value that has been saved if isSimplified is true.
        /// </summary>
        public string TheOriginalValue { get; set; }

        /// <summary>
        /// Gets or sets the value of this <see cref="AttributeValue"/>
        /// </summary>
        /// <remarks>
        /// This is a convenience property to get/set TheValue or Values in concrete implementation
        /// </remarks>
        public override object ObjectValue
        {
            get => this.TheValue;
            set => this.TheValue = value.ToString();
        }

        /// <summary>
        /// Gets the <see cref="List{ExternalObject}"/> that may be present as xhtml:object in the XHTML content
        /// </summary>
        public List<ExternalObject> ExternalObjects { get; private set; }

        /// <summary>
        /// Gets or sets the Reference to the attribute definition that relates the value to its data type.
        /// </summary>
        public AttributeDefinitionXHTML Definition { get; set; }

        /// <summary>
        /// Gets or sets the owning attribute definition.
        /// </summary>
        public AttributeDefinitionXHTML OwningDefinition { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition "/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="AttributeDefinitionXHTML"/>
        /// </returns>
        protected override AttributeDefinition GetAttributeDefinition()
        {
            return this.Definition;
        }

        /// <summary>
        /// Sets the <see cref="AttributeDefinition"/>
        /// </summary>
        /// <param name="attributeDefinition">
        /// The <see cref="AttributeDefinition"/> to set
        /// </param>
        protected override void SetAttributeDefinition(AttributeDefinition attributeDefinition)
        {
            if (attributeDefinition.GetType() != typeof(AttributeDefinitionXHTML))
            {
                throw new ArgumentException("attributeDefinition must of type AttributeDefinitionXHTML");
            }

            this.Definition = (AttributeDefinitionXHTML)attributeDefinition;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute value is a simplified representation of the original value.
        /// </summary>
        public bool IsSimplified { get; set; }

        /// <summary>
        /// extract text from the value of the removing all the formatting xhtml nodes 
        /// </summary>
        /// <returns>
        /// a string stripped of xhtml formatting elements
        /// </returns>
        public string ExtractUnformattedTextFromValue()
        {
            if (string.IsNullOrEmpty(this.TheValue))
            {
                return string.Empty;
            }

            var result = new List<string>();
            
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(this.TheValue)))
            {
                using (var xmlTextReader = new XmlTextReader(memoryStream))
                {
                    xmlTextReader.Namespaces = false;

                    xmlTextReader.MoveToContent();

                    using (var subtree = xmlTextReader.ReadSubtree())
                    {
                        while (subtree.Read())
                        {
                            if (subtree.MoveToContent() == XmlNodeType.Text)
                            {
                                result.Add(subtree.Value);
                            }
                        }
                    }
                }
            }

            return result.Any() ? string.Join(" ", result) : string.Empty;
        }

        /// <summary>
        /// Generates a <see cref="AttributeValueXHTML"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            var isSimplified = reader["IS-SIMPLIFIED"];
            if (!string.IsNullOrEmpty(isSimplified))
            {
                this.IsSimplified = XmlConvert.ToBoolean(isSimplified);
            }

            using (var subtree = reader.ReadSubtree())
            {
                while (subtree.Read())
                {
                    if (subtree.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ATTRIBUTE-DEFINITION-XHTML-REF")
                    {
                        var reference = reader.ReadElementContentAsString();
                        this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionXHTML>().SingleOrDefault(x => x.Identifier == reference);
                        if (this.Definition == null)
                        {
                            throw new InvalidOperationException($"The attribute-definition XHTML {reference} could not be found for the value.");
                        }
                    }

                    if (subtree.MoveToContent() == XmlNodeType.Element && reader.LocalName == "THE-VALUE")
                    {
                        this.TheValue = subtree.ReadInnerXml().Trim();

                        if (this.TheValue.Contains("xhtml:object data"))
                        {
                            var externalObjects = this.CreateExternalObjects(this.TheValue);
                            this.ExternalObjects.AddRange(externalObjects);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="AttributeValueXHTML"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        public override async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            var isSimplified = reader["IS-SIMPLIFIED"];
            if (!string.IsNullOrEmpty(isSimplified))
            {
                this.IsSimplified = XmlConvert.ToBoolean(isSimplified);
            }

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            using (var subtree = reader.ReadSubtree())
            {
                while (await subtree.ReadAsync())
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    if (await subtree.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName == "ATTRIBUTE-DEFINITION-XHTML-REF")
                    {
                        var reference = await reader.ReadElementContentAsStringAsync();
                        this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionXHTML>().SingleOrDefault(x => x.Identifier == reference);
                        if (this.Definition == null)
                        {
                            throw new InvalidOperationException($"The attribute-definition XHTML {reference} could not be found for the value.");
                        }
                    }

                    if (await subtree.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName == "THE-VALUE")
                    {
                        this.TheValue = (await subtree.ReadInnerXmlAsync()).Trim();

                        if (this.TheValue.Contains("xhtml:object data"))
                        {
                            var externalObjects = this.CreateExternalObjects(this.TheValue);
                            this.ExternalObjects.AddRange(externalObjects);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates <see cref="ExternalObject"/>s from provided XHTML
        /// </summary>
        /// <param name="xhtml">
        /// The XHTML that may contain <see cref="ExternalObject"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{ExternalObject}"/> which may be empty
        /// </returns>
        private IEnumerable<ExternalObject> CreateExternalObjects(string xhtml)
        {
            var result = new List<ExternalObject>();

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xhtml);

            var xmlNodeList = xmlDocument.GetElementsByTagName("xhtml:object").OfType<XmlElement>() ;
            
            foreach (var xmlElement in xmlNodeList)
            {
                var mimeTypeAttribute = xmlElement.GetAttribute("type");
                var uriAttribute = xmlElement.GetAttribute("data");
                if (!string.IsNullOrEmpty(uriAttribute))
                {
                    uriAttribute = Uri.UnescapeDataString(uriAttribute);
                }

                var heightAttribute = xmlElement.GetAttribute("height");
                var widthAttribute = xmlElement.GetAttribute("width");
                
                var externalObject = new ExternalObject(this)
                {
                    MimeType = mimeTypeAttribute, 
                    Uri = uriAttribute
                };

                if (!string.IsNullOrEmpty(heightAttribute) && int.TryParse(heightAttribute, out int height))
                {
                    externalObject.Height = height;
                }

                if (!string.IsNullOrEmpty(widthAttribute) && int.TryParse(widthAttribute, out int width))
                {
                    externalObject.Width = width;
                }

                result.Add(externalObject);
            }

            return result;
        }

        /// <summary>
        /// Converts a <see cref="AttributeValueXHTML"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Definition"/> may not be null
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            if (this.Definition == null)
            {
                throw new SerializationException("The Definition property of an AttributeValueXHTML may not be null");
            }

            if (this.IsSimplified)
            {
                writer.WriteAttributeString("IS-SIMPLIFIED", "true");
            }

            writer.WriteStartElement("DEFINITION");
            writer.WriteElementString("ATTRIBUTE-DEFINITION-XHTML-REF", this.Definition.Identifier);
            writer.WriteEndElement();

            writer.WriteStartElement("THE-VALUE");
            writer.WriteRaw(this.TheValue);
            writer.WriteEndElement();

            if (!string.IsNullOrEmpty(this.TheOriginalValue))
            {
                writer.WriteStartElement("THE-ORIGINAL-VALUE");
                writer.WriteRaw(this.TheOriginalValue);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeValueXHTML"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Definition"/> may not be null
        /// </exception>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        public override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            if (this.Definition == null)
            {
                throw new SerializationException("The Definition property of an AttributeValueXHTML may not be null");
            }

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (this.IsSimplified)
            {
                await writer.WriteAttributeStringAsync(null, "IS-SIMPLIFIED", null, "true");
            }

            await writer.WriteStartElementAsync(null, "DEFINITION", null);
            await writer.WriteElementStringAsync(null, "ATTRIBUTE-DEFINITION-XHTML-REF", null, this.Definition.Identifier);
            await writer.WriteEndElementAsync();

            await writer.WriteStartElementAsync(null, "THE-VALUE", null);
            await writer.WriteRawAsync(this.TheValue);
            await writer.WriteEndElementAsync();

            if (!string.IsNullOrEmpty(this.TheOriginalValue))
            {
                await writer.WriteStartElementAsync(null, "THE-ORIGINAL-VALUE", null);
                await writer.WriteRawAsync(this.TheOriginalValue);
                await writer.WriteEndElementAsync();
            }
        }
    }
}
