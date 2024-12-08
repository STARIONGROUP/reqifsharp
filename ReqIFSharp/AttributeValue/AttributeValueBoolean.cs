﻿// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueBoolean.cs" company="Starion Group S.A.">
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

using Microsoft.Extensions.Logging.Abstractions;

namespace ReqIFSharp
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The purpose of the <see cref="AttributeValueBoolean"/> class is to define a <see cref="bool"/> attribute value.
    /// </summary>
    public class AttributeValueBoolean : AttributeValueSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueBoolean"/> class.
        /// </summary>
        public AttributeValueBoolean()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionBoolean"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        public AttributeValueBoolean(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        /// <summary>
        /// Instantiated a new instance of the <see cref="AttributeValueBoolean"/> class
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinitionBoolean"/> for which this is the default value</param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinitionBoolean"/>
        /// </remarks>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal AttributeValueBoolean(AttributeDefinitionBoolean attributeDefinition, ILoggerFactory loggerFactory)
            : base(attributeDefinition, loggerFactory)
        {
            this.OwningDefinition = attributeDefinition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueBoolean"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal AttributeValueBoolean(SpecElementWithAttributes specElAt, ILoggerFactory loggerFactory)
            : base(specElAt, loggerFactory)
        {
        }

        /// <summary>
        /// Gets or sets the attribute value.
        /// </summary>
        public bool TheValue { get; set; }

        /// <summary>
        /// Gets or sets the value of this <see cref="AttributeValue"/>
        /// </summary>
        /// <remarks>
        /// This is a convenience property to get/set TheValue or Values in concrete implementation
        /// </remarks>
        public override object ObjectValue
        {
            get => this.TheValue;
            set
            {
                if (!(value is bool castValue))
                {
                    throw new InvalidOperationException($"Cannot use {value} as value for this AttributeValueBoolean.");
                }

                this.TheValue = castValue;
            }
        }

        /// <summary>
        /// Gets or sets a reference to the value definition
        /// </summary>
        public AttributeDefinitionBoolean Definition { get; set; }

        /// <summary>
        /// Gets or sets the owning attribute definition.
        /// </summary>
        public AttributeDefinitionBoolean OwningDefinition { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition "/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="AttributeDefinitionBoolean"/>
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
            if (attributeDefinition.GetType() != typeof(AttributeDefinitionBoolean))
            {
                throw new ArgumentException("attributeDefinition must of type AttributeDefinitionBoolean");
            }

            this.Definition = (AttributeDefinitionBoolean)attributeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeValueBoolean"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            var value = reader["THE-VALUE"];

            if (value != null)
            {
                this.TheValue = XmlConvert.ToBoolean(value);
            }

            while (reader.Read())
            {
                if (reader.ReadToDescendant("ATTRIBUTE-DEFINITION-BOOLEAN-REF"))
                {
                    var reference = reader.ReadElementContentAsString();

                    this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionBoolean>().SingleOrDefault(x => x.Identifier == reference);
                    if (this.Definition == null)
                    {
                        throw new InvalidOperationException($"The attribute-definition Boolean {reference} could not be found for the value.");
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="AttributeValueBoolean"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            var value = reader["THE-VALUE"];

            if (value != null)
            {
                this.TheValue = XmlConvert.ToBoolean(value);
            }

            while (await reader.ReadAsync())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (reader.ReadToDescendant("ATTRIBUTE-DEFINITION-BOOLEAN-REF"))
                {
                    var reference = await reader.ReadElementContentAsStringAsync();

                    this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionBoolean>().SingleOrDefault(x => x.Identifier == reference);
                    if (this.Definition == null)
                    {
                        throw new InvalidOperationException($"The attribute-definition Boolean {reference} could not be found for the value.");
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeValueBoolean"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Definition"/> may not be null
        /// </exception>
        internal override void WriteXml(XmlWriter writer)
        {
            if (this.Definition == null)
            {
                throw new SerializationException("The Definition property of an AttributeValueBoolean may not be null");
            }

            writer.WriteAttributeString("THE-VALUE", this.TheValue ? "true" : "false");
            writer.WriteStartElement("DEFINITION");
            writer.WriteElementString("ATTRIBUTE-DEFINITION-BOOLEAN-REF", this.Definition.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeValueBoolean"/> object into its XML representation.
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
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            if (this.Definition == null)
            {
                throw new SerializationException("The Definition property of an AttributeValueBoolean may not be null");
            }

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            await writer.WriteAttributeStringAsync(null, "THE-VALUE", null, this.TheValue ? "true" : "false" );
            await writer.WriteStartElementAsync(null, "DEFINITION", null);
            await writer.WriteElementStringAsync(null, "ATTRIBUTE-DEFINITION-BOOLEAN-REF", null, this.Definition.Identifier);
            await writer.WriteEndElementAsync();
        }
    }
}
