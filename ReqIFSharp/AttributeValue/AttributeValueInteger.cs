// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueInteger.cs" company="Starion Group S.A.">
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
    /// The purpose of the <see cref="AttributeValueInteger"/> class is to define an Integer attribute value.
    /// </summary>
    /// <remarks>
    /// ReqIfSharp supports 64 bit signed integers (long) with the following range: -9223372036854775808 to 9223372036854775807
    /// </remarks>
    public class AttributeValueInteger : AttributeValueSimple
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<AttributeValueInteger> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueInteger"/> class.
        /// </summary>
        public AttributeValueInteger()
        {
            this.logger = NullLogger<AttributeValueInteger>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueInteger"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public AttributeValueInteger(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AttributeValueInteger>.Instance : this.loggerFactory.CreateLogger<AttributeValueInteger>();
        }

        /// <summary>
        /// Instantiated a new instance of the <see cref="AttributeValueInteger"/> class
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinitionInteger"/> for which this is the default value</param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinitionInteger"/>
        /// </remarks>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal AttributeValueInteger(AttributeDefinitionInteger attributeDefinition, ILoggerFactory loggerFactory)
            : base(attributeDefinition, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AttributeValueInteger>.Instance : this.loggerFactory.CreateLogger<AttributeValueInteger>();

            this.OwningDefinition = attributeDefinition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueInteger"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal AttributeValueInteger(SpecElementWithAttributes specElAt, ILoggerFactory loggerFactory)
            : base(specElAt, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AttributeValueInteger>.Instance : this.loggerFactory.CreateLogger<AttributeValueInteger>();
        }

        /// <summary>
        /// Gets or sets the attribute value
        /// </summary>
        public long TheValue { get; set; }

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
                if (!(value is long castValue))
                {
                    throw new InvalidOperationException($"Cannot use {value} as value for this AttributeValueInteger.");
                }

                this.TheValue = castValue;
            }
        }

        /// <summary>
        /// Gets or sets the Reference to the value definition.
        /// </summary>
        public AttributeDefinitionInteger Definition { get; set; }

        /// <summary>
        /// Gets or sets the owning attribute definition.
        /// </summary>
        public AttributeDefinitionInteger OwningDefinition { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition "/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="AttributeDefinitionInteger"/>
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

            if (attributeDefinition.GetType() != typeof(AttributeDefinitionInteger))
            {
                throw new ArgumentException($"{nameof(attributeDefinition)} must of type AttributeDefinitionInteger");
            }

            this.Definition = (AttributeDefinitionInteger)attributeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeValueInteger"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            this.ReadXmlAttributes(reader);

            while (reader.Read())
            {
                if (reader.ReadToDescendant("ATTRIBUTE-DEFINITION-INTEGER-REF"))
                {
                    var reference = reader.ReadElementContentAsString();

                    this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionInteger>().SingleOrDefault(x => x.Identifier == reference);
                    if (this.Definition == null)
                    {
                        throw new InvalidOperationException($"The attribute-definition XHTML {reference} could not be found for the value.");
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="AttributeValueInteger"/> object from its XML representation.
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

                if (reader.ReadToDescendant("ATTRIBUTE-DEFINITION-INTEGER-REF"))
                {
                    var reference = await reader.ReadElementContentAsStringAsync();

                    this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionInteger>().SingleOrDefault(x => x.Identifier == reference);
                    if (this.Definition == null)
                    {
                        throw new InvalidOperationException($"The attribute-definition XHTML {reference} could not be found for the value.");
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
                    this.TheValue = XmlConvert.ToInt64(theValue);
                }
                catch (OverflowException)
                {
                    this.logger.LogWarning("The AttributeValueInteger.THE-VALUE: {Value} at line:position {LineNumber}:{LinePosition} could not be processed. TheValue is set to 0",
                        theValue, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

                    this.TheValue = default;
                }
                catch (Exception e)
                {
                    throw new SerializationException($"The AttributeValueInteger.THE-VALUE {theValue} at line:position {xmlLineInfo?.LineNumber}:{xmlLineInfo?.LinePosition} could not be converted to an INTEGER", e);
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeValueInteger"/> object into its XML representation.
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
                throw new SerializationException("The Definition property of an AttributeValueInteger may not be null");
            }

            writer.WriteAttributeString("THE-VALUE",  this.TheValue.ToString(NumberFormatInfo.InvariantInfo));
            writer.WriteStartElement("DEFINITION");
            writer.WriteElementString("ATTRIBUTE-DEFINITION-INTEGER-REF", this.Definition.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeValueInteger"/> object into its XML representation.
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
                throw new SerializationException("The Definition property of an AttributeValueInteger may not be null");
            }

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            await writer.WriteAttributeStringAsync(null,"THE-VALUE",null, this.TheValue.ToString(NumberFormatInfo.InvariantInfo));
            await writer.WriteStartElementAsync(null, "DEFINITION",null);
            await writer.WriteElementStringAsync(null, "ATTRIBUTE-DEFINITION-INTEGER-REF", null,this.Definition.Identifier);
            await writer.WriteEndElementAsync();
        }
    }
}
