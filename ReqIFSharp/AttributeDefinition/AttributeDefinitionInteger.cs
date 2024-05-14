// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionInteger.cs" company="Starion Group S.A.">
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
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="AttributeDefinitionInteger"/> class is to define an Integer attribute.
    /// </summary>
    /// <remarks>
    /// An <see cref="AttributeDefinitionInteger"/> element relates an <see cref="AttributeValueInteger"/> element to a
    /// <see cref="DatatypeDefinitionInteger"/> element via its <see cref="Type"/> attribute.
    /// An <see cref="AttributeDefinitionInteger"/> element MAY contain a default value that represents the value that is used as an attribute
    /// value if no attribute value is supplied by the user of the requirements authoring tool.
    /// ReqIfSharp supports 64 bit singed integers (long) with the following range: -9223372036854775808 to 9223372036854775807
    /// </remarks>
    public class AttributeDefinitionInteger : AttributeDefinitionSimple
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<AttributeDefinitionInteger> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionInteger"/> class.
        /// </summary>
        public AttributeDefinitionInteger()
        {
            this.logger = NullLogger<AttributeDefinitionInteger>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionInteger"/> class.
        /// </summary>
        /// <param name="specType">
        /// The owning <see cref="SpecType"/>.
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal AttributeDefinitionInteger(SpecType specType, ILoggerFactory loggerFactory)
            : base(specType, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AttributeDefinitionInteger>.Instance : this.loggerFactory.CreateLogger<AttributeDefinitionInteger>();
        }

        /// <summary>
        /// Gets or sets the owned default value that is used if no attribute value is supplied 
        /// by the user of the requirements authoring tool.
        /// </summary>
        public AttributeValueInteger DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        public DatatypeDefinitionInteger Type { get; set; }

        /// <summary>
        /// Gets the <see cref="DatatypeDefinition"/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="DatatypeDefinitionInteger"/>
        /// </returns>
        protected override DatatypeDefinition GetDatatypeDefinition()
        {
            return this.Type;
        }

        /// <summary>
        /// Sets the <see cref="DatatypeDefinitionInteger"/>
        /// </summary>
        /// <param name="datatypeDefinition">
        /// The instance of <see cref="DatatypeDefinitionInteger"/> that is to be set.
        /// </param>
        protected override void SetDatatypeDefinition(DatatypeDefinition datatypeDefinition)
        {
            if (datatypeDefinition.GetType() != typeof(DatatypeDefinitionInteger))
            {
                throw new ArgumentException("datatypeDefinition must of type DatatypeDefinitionInteger");
            }

            this.Type = (DatatypeDefinitionInteger)datatypeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeDefinitionInteger"/> object from its XML representation.
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
                        case "DATATYPE-DEFINITION-INTEGER-REF":
                            var reference = reader.ReadElementContentAsString();
                            var datatypeDefinition = (DatatypeDefinitionInteger)this.SpecType.ReqIFContent.DataTypes.SingleOrDefault(x => x.Identifier == reference);
                            this.Type = datatypeDefinition;

                            if (datatypeDefinition == null)
                            {
                                this.logger.LogTrace("The DatatypeDefinitionInteger:{reference} could not be found and has been set to null on AttributeDefinitionInteger:{Identifier}", reference, Identifier);
                            }

                            break;
                        case "ATTRIBUTE-VALUE-INTEGER":
                            this.DefaultValue = new AttributeValueInteger(this, this.loggerFactory);
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
        /// Asynchronously generates a <see cref="AttributeDefinitionInteger"/> object from its XML representation.
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
                        case "DATATYPE-DEFINITION-INTEGER-REF":
                            var reference = await reader.ReadElementContentAsStringAsync();
                            var datatypeDefinition = (DatatypeDefinitionInteger)this.SpecType.ReqIFContent.DataTypes.SingleOrDefault(x => x.Identifier == reference);
                            this.Type = datatypeDefinition;

                            if (datatypeDefinition == null)
                            {
                                this.logger.LogTrace("The DatatypeDefinitionInteger:{reference} could not be found and has been set to null on AttributeDefinitionInteger:{Identifier}", reference, Identifier);
                            }

                            break;
                        case "ATTRIBUTE-VALUE-INTEGER":
                            this.DefaultValue = new AttributeValueInteger(this, this.loggerFactory);
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
        /// Converts a <see cref="AttributeDefinitionInteger"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Type"/> may not be null
        /// </exception>
        internal override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            if (this.DefaultValue != null)
            {
                writer.WriteStartElement("DEFAULT-VALUE");
                    writer.WriteStartElement("ATTRIBUTE-VALUE-INTEGER");
                    writer.WriteAttributeString("THE-VALUE", this.DefaultValue.TheValue.ToString(NumberFormatInfo.InvariantInfo));
                        writer.WriteStartElement("DEFINITION");
                            writer.WriteElementString("ATTRIBUTE-DEFINITION-INTEGER-REF", this.DefaultValue.Definition.Identifier);
                        writer.WriteEndElement();
                    writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteStartElement("TYPE");
            writer.WriteElementString("DATATYPE-DEFINITION-INTEGER-REF", this.Type.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeDefinitionInteger"/> object into its XML representation.
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
                await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-INTEGER", null);
                await writer.WriteAttributeStringAsync(null, "THE-VALUE", null, this.DefaultValue.TheValue.ToString(NumberFormatInfo.InvariantInfo));
                await writer.WriteStartElementAsync(null, "DEFINITION", null);
                await writer.WriteElementStringAsync(null, "ATTRIBUTE-DEFINITION-INTEGER-REF", null, this.DefaultValue.Definition.Identifier);
                await writer.WriteEndElementAsync();
                await writer.WriteEndElementAsync();
                await writer.WriteEndElementAsync();
            }

            await writer.WriteStartElementAsync(null, "TYPE", null);
            await writer.WriteElementStringAsync(null, "DATATYPE-DEFINITION-INTEGER-REF", null, this.Type.Identifier);
            await writer.WriteEndElementAsync();
        }
    }
}
