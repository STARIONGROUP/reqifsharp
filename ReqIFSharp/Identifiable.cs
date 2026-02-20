// -------------------------------------------------------------------------------------------------
// <copyright file="Identifiable.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
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
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The <see cref="Identifiable"/> Abstract base class provides an identification concept for <see cref="ReqIF"/> elements.
    /// </summary>
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
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the lifetime immutable identifier for an instance of a <see cref="ReqIF"/> information type.
        /// </summary>
        /// <remarks>
        /// The value of the identifier must be a well-formed <code>xsd:ID</code>.
        /// </remarks>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the last change of the information element. 
        /// This includes the creation of the information element. lastChange is of the XML Schema data type “dateTime” that specifies the time format as
        /// <code>YY-MM-DDThh:mm:ss</code> with optional time zone indicator as a suffix <code>±hh:mm</code>.
        /// </summary>
        /// <example>
        /// date time formatting: 2005-03-04T10:24:18+01:00 (MET time zone).
        /// </example>
        public DateTime LastChange { get; set; }

        /// <summary>
        /// Gets or sets the human-readable name for the information element.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// Gets or sets optional alternative identification element.
        /// </summary>
        public AlternativeId AlternativeId { get; set; }

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

            if (this.AlternativeId != null)
            {
                await writer.WriteStartElementAsync(null, "ALTERNATIVE-ID", null);
                await this.AlternativeId.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }
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
                alternativeId.ReadXml(alternativeIdSubtree);
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
                await alternativeId.ReadXmlAsync(alternativeIdSubtree, token);
            }
        }
    }
}
