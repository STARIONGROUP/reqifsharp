// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionDate.cs" company="Starion Group S.A.">
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
// -------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="AttributeDefinitionBoolean"/> class is to define a <see cref="DateTime"/> attribute.
    /// </summary>
    /// <remarks>
    /// An <see cref="AttributeDefinitionDate"/> element relates an <see cref="AttributeValueDate"/> element to a <see cref="DatatypeDefinitionDate"/>
    /// element via its <see cref="Type"/> attribute.
    /// An <see cref="AttributeDefinitionDate"/> element MAY contain a default value that represents the value that is used as an attribute
    /// value if no attribute value is supplied by the user of the requirements authoring tool
    /// </remarks>    
    public class AttributeDefinitionDate : AttributeDefinitionSimple
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<AttributeDefinitionDate> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionDate"/> class.
        /// </summary>
        public AttributeDefinitionDate()
        {
            this.logger = NullLogger<AttributeDefinitionDate>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionDate"/> class.
        /// </summary>
        /// <param name="specType">
        /// The owning <see cref="SpecType"/>.
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal AttributeDefinitionDate(SpecType specType, ILoggerFactory loggerFactory)
            : base(specType, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AttributeDefinitionDate>.Instance : this.loggerFactory.CreateLogger<AttributeDefinitionDate>();
        }

        /// <summary>
        /// Gets or sets the owned default value that is used if no attribute value is supplied 
        /// by the user of the requirements authoring tool.
        /// </summary>
        public AttributeValueDate DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        public DatatypeDefinitionDate Type { get; set; }

        /// <summary>
        /// Gets the <see cref="DatatypeDefinition"/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="DatatypeDefinitionDate"/>
        /// </returns>
        protected override DatatypeDefinition GetDatatypeDefinition()
        {
            return this.Type;
        }

        /// <summary>
        /// Sets the <see cref="DatatypeDefinitionDate"/>
        /// </summary>
        /// <param name="datatypeDefinition">
        /// The instance of <see cref="DatatypeDefinitionDate"/> that is to be set.
        /// </param>
        protected override void SetDatatypeDefinition(DatatypeDefinition datatypeDefinition)
        {
            if (datatypeDefinition.GetType() != typeof(DatatypeDefinitionDate))
            {
                throw new ArgumentException("datatypeDefinition must of type DatatypeDefinitionDate");
            }

            this.Type = (DatatypeDefinitionDate)datatypeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeDefinitionDate"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "ALTERNATIVE-ID":
                            this.ReadAlternativeId(reader);
                            break;
                        case "ATTRIBUTE-VALUE-DATE":
                            this.DefaultValue = new AttributeValueDate(this, this.loggerFactory);
                            using (var valuesubtree = reader.ReadSubtree())
                            {
                                valuesubtree.MoveToContent();
                                this.DefaultValue.ReadXml(valuesubtree);
                            }
                            break;
                        case "DATATYPE-DEFINITION-DATE-REF":
                            var reference = reader.ReadElementContentAsString();
                            var datatypeDefinition = (DatatypeDefinitionDate)this.SpecType.ReqIFContent.DataTypes.SingleOrDefault(x => x.Identifier == reference);
                            this.Type = datatypeDefinition;

                            if (datatypeDefinition == null)
                            {
                                this.logger.LogTrace("The DatatypeDefinitionDate:{Reference} could not be found and has been set to null on AttributeDefinitionDate:{Identifier}", reference, Identifier);
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="AttributeDefinitionDate"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
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
                            await this.ReadAlternativeIdAsync(reader, token);
                            break;
                        case "ATTRIBUTE-VALUE-DATE":
                            this.DefaultValue = new AttributeValueDate(this, this.loggerFactory);
                            using (var valueSubtree = reader.ReadSubtree())
                            {
                                await valueSubtree.MoveToContentAsync();
                                await this.DefaultValue.ReadXmlAsync(valueSubtree, token);
                            }
                            break;
                        case "DATATYPE-DEFINITION-DATE-REF":
                            var reference = await reader.ReadElementContentAsStringAsync();
                            var datatypeDefinition = (DatatypeDefinitionDate)this.SpecType.ReqIFContent.DataTypes.SingleOrDefault(x => x.Identifier == reference);
                            this.Type = datatypeDefinition;

                            if (datatypeDefinition == null)
                            {
                                this.logger.LogTrace("The DatatypeDefinitionDate:{Reference} could not be found and has been set to null on AttributeDefinitionDate:{Identifier}", reference, Identifier);
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeDefinitionDate"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Type"/> may not be null
        /// </exception>
        internal override void WriteXml(XmlWriter writer)
        {
            if (this.Type == null)
            {
                throw new SerializationException($"The Type property of AttributeDefinitionDate {this.Identifier}:{this.LongName} may not be null");
            }

            base.WriteXml(writer);

            if (this.DefaultValue != null)
            {
                writer.WriteStartElement("DEFAULT-VALUE");
                    writer.WriteStartElement("ATTRIBUTE-VALUE-DATE");
                    writer.WriteAttributeString("THE-VALUE", this.DefaultValue.TheValue.ToString("o"));
                        writer.WriteStartElement("DEFINITION");
                            writer.WriteElementString("ATTRIBUTE-DEFINITION-DATE-REF", this.DefaultValue.Definition.Identifier);
                        writer.WriteEndElement();
                    writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteStartElement("TYPE");
            writer.WriteElementString("DATATYPE-DEFINITION-DATE-REF", this.Type.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeDefinitionDate"/> object into its XML representation.
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
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            await base.WriteXmlAsync(writer, token);

            if (this.DefaultValue != null)
            {
                await writer.WriteStartElementAsync(null,"DEFAULT-VALUE", null);
                await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-DATE", null);
                await writer.WriteAttributeStringAsync(null, "THE-VALUE", null, this.DefaultValue.TheValue.ToString("o"));
                await writer.WriteStartElementAsync(null, "DEFINITION", null);
                await writer.WriteElementStringAsync(null, "ATTRIBUTE-DEFINITION-DATE-REF", null, this.DefaultValue.Definition.Identifier);
                await writer.WriteEndElementAsync();
                await writer.WriteEndElementAsync();
                await writer.WriteEndElementAsync();
            }

            await writer.WriteStartElementAsync(null, "TYPE", null);
            await writer.WriteElementStringAsync(null, "DATATYPE-DEFINITION-DATE-REF", null, this.Type.Identifier);
            await writer.WriteEndElementAsync();
        }
    }
}
