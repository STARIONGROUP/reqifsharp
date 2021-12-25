// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionXHTML.cs" company="RHEA System S.A.">
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
// -------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The purpose of the <see cref="AttributeDefinitionBoolean"/> class is to define an XHTML attribute.
    /// </summary>
    /// <remarks>
    /// An <see cref="AttributeDefinitionXHTML"/> element relates an <see cref="AttributeValueXHTML"/> element to a
    /// <see cref="DatatypeDefinitionXHTML"/> element via its <see cref="Type"/> attribute.
    /// An <see cref="AttributeDefinitionXHTML"/> element MAY contain a default value that represents the value that is used as an attribute
    /// value if no attribute value is supplied by the user of the requirements authoring tool.
    /// </remarks>
    public class AttributeDefinitionXHTML : AttributeDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionXHTML"/> class.
        /// </summary>
        public AttributeDefinitionXHTML()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionXHTML"/> class.
        /// </summary>
        /// <param name="specType">
        /// The owning <see cref="SpecType"/>.
        /// </param>
        internal AttributeDefinitionXHTML(SpecType specType) 
            : base(specType)
        {
        }

        /// <summary>
        /// Gets or sets the owned default value that is used if no attribute value is supplied 
        /// by the user of the requirements authoring tool.
        /// </summary>
        public AttributeValueXHTML DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        public DatatypeDefinitionXHTML Type { get; set; }

        /// <summary>
        /// Gets the <see cref="DatatypeDefinition"/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="DatatypeDefinitionXHTML"/>
        /// </returns>
        protected override DatatypeDefinition GetDatatypeDefinition()
        {
            return this.Type;
        }

        /// <summary>
        /// Sets the <see cref="DatatypeDefinitionXHTML"/>
        /// </summary>
        /// <param name="datatypeDefinition">
        /// The instance of <see cref="DatatypeDefinitionXHTML"/> that is to be set.
        /// </param>
        protected override void SetDatatypeDefinition(DatatypeDefinition datatypeDefinition)
        {
            if (datatypeDefinition.GetType() != typeof(DatatypeDefinitionXHTML))
            {
                throw new ArgumentException("datatypeDefinition must of type DatatypeDefinitionXHTML");
            }

            this.Type = (DatatypeDefinitionXHTML)datatypeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeDefinitionXHTML"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "ALTERNATIVE-ID":
                            var alternativeId = new AlternativeId(this);
                            alternativeId.ReadXml(reader);
                            break;
                        case "DATATYPE-DEFINITION-XHTML-REF":
                            var reference = reader.ReadElementContentAsString();
                            var datatypeDefinition = (DatatypeDefinitionXHTML)this.SpecType.ReqIFContent.DataTypes.SingleOrDefault(x => x.Identifier == reference);
                            this.Type = datatypeDefinition;
                            break;
                        case "ATTRIBUTE-VALUE-XHTML":
                            this.DefaultValue = new AttributeValueXHTML(this);
                            using (var valueSubtree = reader.ReadSubtree())
                            {
                                valueSubtree.MoveToContent();
                                this.DefaultValue.ReadXml(valueSubtree);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="AttributeDefinitionXHTML"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        public override async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            base.ReadXml(reader);

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
                        case "ALTERNATIVE-ID":
                            var alternativeId = new AlternativeId(this);
                            alternativeId.ReadXml(reader);
                            break;
                        case "DATATYPE-DEFINITION-XHTML-REF":
                            var reference = await reader.ReadElementContentAsStringAsync();
                            var datatypeDefinition = (DatatypeDefinitionXHTML)this.SpecType.ReqIFContent.DataTypes.SingleOrDefault(x => x.Identifier == reference);
                            this.Type = datatypeDefinition;
                            break;
                        case "ATTRIBUTE-VALUE-XHTML":
                            this.DefaultValue = new AttributeValueXHTML(this);
                            using (var valueSubtree = reader.ReadSubtree())
                            {
                                await valueSubtree.MoveToContentAsync();
                                await this.DefaultValue.ReadXmlAsync(valueSubtree, token);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeDefinitionXHTML"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Type"/> may not be null
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            if (this.DefaultValue != null)
            {
                writer.WriteStartElement("DEFAULT-VALUE");
                    writer.WriteStartElement("ATTRIBUTE-VALUE-XHTML");
                        writer.WriteStartElement("THE-VALUE");
                            writer.WriteRaw(this.DefaultValue.TheValue);
                        writer.WriteEndElement();
                        writer.WriteStartElement("DEFINITION");
                            writer.WriteElementString("ATTRIBUTE-DEFINITION-XHTML-REF", this.DefaultValue.Definition.Identifier);
                        writer.WriteEndElement();
                    writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteStartElement("TYPE");
            writer.WriteElementString("DATATYPE-DEFINITION-XHTML-REF", this.Type.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeDefinitionXHTML"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Type"/> may not be null
        /// </exception>
        public override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            await base.WriteXmlAsync(writer, token);

            if (this.DefaultValue != null)
            {
                await writer.WriteStartElementAsync(null,"DEFAULT-VALUE", null);
                await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-XHTML", null);
                await writer.WriteStartElementAsync(null, "THE-VALUE", null);
                await writer.WriteRawAsync(this.DefaultValue.TheValue);
                await writer.WriteEndElementAsync();
                await writer.WriteStartElementAsync(null, "DEFINITION", null);
                await writer.WriteElementStringAsync(null, "ATTRIBUTE-DEFINITION-XHTML-REF", null, this.DefaultValue.Definition.Identifier);
                await writer.WriteEndElementAsync();
                await writer.WriteEndElementAsync();
                await writer.WriteEndElementAsync();
            }

            await writer.WriteStartElementAsync(null, "TYPE", null);
            await writer.WriteElementStringAsync(null, "DATATYPE-DEFINITION-XHTML-REF", null, this.Type.Identifier);
            await writer.WriteEndElementAsync();
        }
    }
}
