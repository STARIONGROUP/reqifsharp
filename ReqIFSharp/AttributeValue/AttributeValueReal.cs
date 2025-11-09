// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueReal.cs" company="Starion Group S.A.">
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
    /// The purpose of the <see cref="AttributeValueReal"/> class is to define a real attribute value.
    /// </summary>
    public class AttributeValueReal : AttributeValueSimple
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<AttributeValueReal> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueReal"/> class.
        /// </summary>
        public AttributeValueReal()
        {
            this.logger = NullLogger<AttributeValueReal>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueReal"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public AttributeValueReal(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AttributeValueReal>.Instance : this.loggerFactory.CreateLogger<AttributeValueReal>();
        }

        /// <summary>
        /// Instantiated a new instance of the <see cref="AttributeValueReal"/> class
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinitionReal"/> for which this is the default value</param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinitionReal"/>
        /// </remarks>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal AttributeValueReal(AttributeDefinitionReal attributeDefinition, ILoggerFactory loggerFactory)
            : base(attributeDefinition, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AttributeValueReal>.Instance : this.loggerFactory.CreateLogger<AttributeValueReal>();

            this.OwningDefinition = attributeDefinition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueReal"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal AttributeValueReal(SpecElementWithAttributes specElAt, ILoggerFactory loggerFactory)
            : base(specElAt, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AttributeValueReal>.Instance : this.loggerFactory.CreateLogger<AttributeValueReal>();
        }

        /// <summary>
        /// Gets or sets the attribute value
        /// </summary>
        public double TheValue { get; set; }

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
                if (!(value is double castValue))
                {
                    throw new InvalidOperationException($"Cannot use {value} as value for this AttributeValueDouble.");
                }

                this.TheValue = castValue;
            }
        }

        /// <summary>
        /// Gets or sets the reference to the value definition
        /// </summary>
        public AttributeDefinitionReal Definition { get; set; }

        /// <summary>
        /// Gets or sets the owning attribute definition.
        /// </summary>
        public AttributeDefinitionReal OwningDefinition { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition "/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="AttributeDefinitionReal"/>
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
            if (attributeDefinition == null)
            {
                throw new ArgumentNullException(nameof(attributeDefinition));
            }

            if (attributeDefinition.GetType() != typeof(AttributeDefinitionReal))
            {
                throw new ArgumentException($"{nameof(attributeDefinition)} must of type AttributeDefinitionReal");
            }

            this.Definition = (AttributeDefinitionReal)attributeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeValueReal"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            this.ReadXmlAttributes(reader);

            while (reader.Read())
            {
                if (reader.ReadToDescendant("ATTRIBUTE-DEFINITION-REAL-REF"))
                {
                    var reference = reader.ReadElementContentAsString();

                    this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionReal>().SingleOrDefault(x => x.Identifier == reference);
                    if (this.Definition == null)
                    {
                        throw new InvalidOperationException($"The attribute-definition Real {reference} could not be found for the value.");
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="AttributeValueReal"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            this.ReadXmlAttributes(reader);

            while (await reader.ReadAsync())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (reader.ReadToDescendant("ATTRIBUTE-DEFINITION-REAL-REF"))
                {
                    var reference = await reader.ReadElementContentAsStringAsync();

                    this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionReal>().SingleOrDefault(x => x.Identifier == reference);
                    if (this.Definition == null)
                    {
                        throw new InvalidOperationException($"The attribute-definition Real {reference} could not be found for the value.");
                    }
                }
            }
        }

        /// <summary>
        /// Reads the properties that are defined as XML Attributes (THE-VALUE)
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void ReadXmlAttributes(XmlReader reader)
        {
            var xmlLineInfo = reader as IXmlLineInfo;

            this.logger.LogTrace("reading THE-VALUE at line:position {LineNumber}:{LinePosition}", xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

            var theValue = reader.GetAttribute("THE-VALUE");
            if (!string.IsNullOrWhiteSpace(theValue))
            {
                try
                {
                    this.TheValue = XmlConvert.ToDouble(theValue);
                }
                catch (OverflowException)
                {
                    this.logger.LogWarning("The AttributeValueReal.THE-VALUE: {Value} at line:position {LineNumber}:{LinePosition} could not be processed. TheValue is set to 0",
                        theValue, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

                    this.TheValue = default;
                }
                catch (Exception e)
                {
                    throw new SerializationException($"The AttributeValueReal.THE-VALUE {theValue} at line:position {xmlLineInfo?.LineNumber}:{xmlLineInfo?.LinePosition} could not be converted to a REAL", e);
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeValueReal"/> object into its XML representation.
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
                throw new SerializationException("The Definition property of an AttributeValueReal may not be null");
            }
            
            writer.WriteAttributeString("THE-VALUE", this.TheValue.ToString(CultureInfo.InvariantCulture));
            writer.WriteStartElement("DEFINITION");
            writer.WriteElementString("ATTRIBUTE-DEFINITION-REAL-REF", this.Definition.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeValueReal"/> object into its XML representation.
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
                throw new SerializationException("The Definition property of an AttributeValueReal may not be null");
            }

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            await writer.WriteAttributeStringAsync(null,"THE-VALUE", null, this.TheValue.ToString(CultureInfo.InvariantCulture));
            await writer.WriteStartElementAsync(null, "DEFINITION", null);
            await writer.WriteElementStringAsync(null, "ATTRIBUTE-DEFINITION-REAL-REF", null, this.Definition.Identifier);
            await writer.WriteEndElementAsync();
        }
    }
}
