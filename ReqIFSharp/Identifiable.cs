// -------------------------------------------------------------------------------------------------
// <copyright file="Identifiable.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The <see cref="Identifiable"/> Abstract base class provides an identification concept for <see cref="ReqIF"/> elements.
    /// </summary>
    [ReqIfClass(name: "IDENTIFIABLE", isAbstract: true)]
    public abstract class Identifiable
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        protected readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<Identifiable> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifiable"/> class.
        /// </summary>
        /// <remarks>
        /// The <see cref="LastChange"/> property is set to the current time.
        /// </remarks>
        protected Identifiable()
        {
            this.logger = NullLogger<Identifiable>.Instance;

            this.LastChange = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Identifiable"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        /// <remarks>
        /// The <see cref="LastChange"/> property is set to the current time.
        /// </remarks>
        protected Identifiable(ILoggerFactory loggerFactory)
            : this()
        {
            this.loggerFactory = loggerFactory;

            this.logger = this.loggerFactory == null ? NullLogger<Identifiable>.Instance : this.loggerFactory.CreateLogger<Identifiable>();
        }

        /// <summary>
        /// Gets or sets the optional additional description for the information element.
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.None, lowerValue: 0, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the lifetime immutable identifier for an instance of a <see cref="ReqIF"/> information type.
        /// </summary>
        /// <remarks>
        /// The value of the identifier must be a well-formed <code>xsd:ID</code>.
        /// </remarks>
        [ReqIfProperty(aggregation: AggregationKind.None, lowerValue: 1, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the last change of the information element. 
        /// This includes the creation of the information element. lastChange is of the XML Schema data type “dateTime” that specifies the time format as
        /// <code>YY-MM-DDThh:mm:ss</code> with optional time zone indicator as a suffix <code>±hh:mm</code>.
        /// </summary>
        /// <example>
        /// date time formatting: 2005-03-04T10:24:18+01:00 (MET time zone).
        /// </example>
        [ReqIfProperty(aggregation: AggregationKind.None, lowerValue: 1, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public DateTime LastChange { get; set; }

        /// <summary>
        /// Gets or sets the human-readable name for the information element.
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.None, lowerValue: 0, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public string LongName { get; set; }

        /// <summary>
        /// Gets or sets optional alternative identification element.
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.Composite, lowerValue: 0, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public AlternativeId AlternativeId { get; set; }

        /// <summary>
        /// Creates a new identifier that is a valid <code>xsd:ID</code>: an underscore followed by a new <see cref="Guid"/>
        /// </summary>
        /// <remarks>
        /// An <code>xsd:ID</code> must start with a letter or an underscore. A bare <see cref="Guid"/> often starts with a
        /// digit, which makes a written document invalid against the ReqIF schema.
        /// </remarks>
        /// <returns>
        /// a new, unique identifier
        /// </returns>
        public static string CreateIdentifier()
        {
            return "_" + Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Sets <see cref="LastChange"/> to the current UTC date and time
        /// </summary>
        /// <remarks>
        /// Call this method after changing the element, for example after changing one of the attribute values of a
        /// <see cref="SpecObject"/>. <see cref="LastChange"/> is not updated automatically.
        /// </remarks>
        public void MarkAsChanged()
        {
            this.LastChange = DateTime.UtcNow;
        }

        /// <summary>
        /// Gives a newly created element a new <see cref="Identifier"/>, sets its <see cref="LastChange"/> to the current
        /// UTC date and time and sets its <see cref="LongName"/>
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        internal void InitializeCreated(string longName)
        {
            this.Identifier = CreateIdentifier();
            this.LongName = longName;
            this.MarkAsChanged();
        }

        /// <summary>
        /// Checks the long name of an element for which the ReqIF specification makes it mandatory: attribute
        /// definitions, enum values and relation groups
        /// </summary>
        /// <param name="longName">
        /// The long name to check
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="longName"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="longName"/> is empty
        /// </exception>
        internal static void ValidateLongName(string longName)
        {
            if (longName == null)
            {
                throw new ArgumentNullException(nameof(longName), "The ReqIF specification makes the long name mandatory for this element");
            }

            if (longName.Length == 0)
            {
                throw new ArgumentException("The ReqIF specification makes the long name mandatory for this element; it may not be empty", nameof(longName));
            }
        }

        /// <summary>
        /// Generates a <see cref="AttributeDefinition"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal virtual void ReadXml(XmlReader reader)
        {
            this.Identifier = reader.GetAttribute("IDENTIFIER");

            var xmlLineInfo = reader as IXmlLineInfo;

            this.logger.LogTrace("read xml of {Typename}:{Identifier} at line:position {LineNumber}:{LinePosition}", this.GetType().Name, this.Identifier, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

            var lastChange = reader.GetAttribute("LAST-CHANGE");
            if (!string.IsNullOrWhiteSpace(lastChange))
            {
                try
                {
                    this.LastChange = XmlConvert.ToDateTime(lastChange, XmlDateTimeSerializationMode.RoundtripKind);
                }
                catch (Exception e)
                {
                    throw new SerializationException($"The Identifiable.LAST-CHANGE {lastChange} at line:position {xmlLineInfo?.LineNumber}:{xmlLineInfo?.LinePosition} could not be converted to a DATE", e);
                }
            }

            this.Description = reader.GetAttribute("DESC");
            this.LongName = reader.GetAttribute("LONG-NAME");
        }

        /// <summary>
        /// Converts a <see cref="AttributeDefinition"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Identifier"/> may not be null or empty
        /// </exception>
        internal virtual void WriteXml(XmlWriter writer)
        {
            if (string.IsNullOrEmpty(this.Identifier))
            {
                throw new SerializationException("The Identifier property of an Identifiable may not be null");
            }

            if (!string.IsNullOrEmpty(this.Description))
            {
                writer.WriteAttributeString("DESC", this.Description);
            }

            writer.WriteAttributeString("IDENTIFIER", this.Identifier);

            writer.WriteAttributeString("LAST-CHANGE", XmlConvert.ToString(this.LastChange, XmlDateTimeSerializationMode.RoundtripKind));

            if (!string.IsNullOrEmpty(this.LongName))
            {
                writer.WriteAttributeString("LONG-NAME", this.LongName);
            }

            this.WriteXmlAttributes(writer);

            if (this.AlternativeId != null)
            {
                writer.WriteStartElement("ALTERNATIVE-ID");
                this.AlternativeId.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeDefinition"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Identifier"/> may not be null or empty
        /// </exception>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal virtual async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(this.Identifier))
            {
                throw new SerializationException("The Identifier property of an Identifiable may not be null");
            }

            if (!string.IsNullOrEmpty(this.Description))
            {
                await writer.WriteAttributeStringAsync(null,"DESC", null, this.Description);
            }

            await writer.WriteAttributeStringAsync(null, "IDENTIFIER", null, this.Identifier);

            await writer.WriteAttributeStringAsync(null, "LAST-CHANGE",null, XmlConvert.ToString(this.LastChange, XmlDateTimeSerializationMode.RoundtripKind));

            if (!string.IsNullOrEmpty(this.LongName))
            {
                await writer.WriteAttributeStringAsync(null, "LONG-NAME", null, this.LongName);
            }

            await this.WriteXmlAttributesAsync(writer, token);

            if (this.AlternativeId != null)
            {
                await writer.WriteStartElementAsync(null, "ALTERNATIVE-ID", null);
                await this.AlternativeId.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }
        }

        /// <summary>
        /// Writes the XML attributes that a subclass adds to the element. It is called by <see cref="WriteXml"/>
        /// after the attributes of the <see cref="Identifiable"/> and before its ALTERNATIVE-ID element, since an
        /// attribute cannot be written after a child element.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal virtual void WriteXmlAttributes(XmlWriter writer)
        {
        }

        /// <summary>
        /// Asynchronously writes the XML attributes that a subclass adds to the element. It is called by
        /// <see cref="WriteXmlAsync"/> after the attributes of the <see cref="Identifiable"/> and before its
        /// ALTERNATIVE-ID element, since an attribute cannot be written after a child element.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal virtual Task WriteXmlAttributesAsync(XmlWriter writer, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Reads the ALTERNATIVE-ID
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected void ReadAlternativeId(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var alternativeId = new AlternativeId(this);

            using (var alternativeIdSubtree = reader.ReadSubtree())
            {
                alternativeId.ReadXml(alternativeIdSubtree, this.logger);
            }
        }

        /// <summary>
        /// Reads the ALTERNATIVE-ID
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        protected Task ReadAlternativeIdAsync(XmlReader reader, CancellationToken token)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return this.ReadAlternativeIdInternalAsync(reader, token);
        }

        /// <summary>
        /// Reads the ALTERNATIVE-ID
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task ReadAlternativeIdInternalAsync(XmlReader reader, CancellationToken token)
        {
            var alternativeId = new AlternativeId(this);

            using (var alternativeIdSubtree = reader.ReadSubtree())
            {
                await alternativeId.ReadXmlAsync(alternativeIdSubtree, token, this.logger);
            }
        }
    }
}
