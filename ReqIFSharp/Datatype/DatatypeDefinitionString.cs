// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionString.cs" company="Starion Group S.A.">
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
    /// The purpose of the <see cref="DatatypeDefinitionString"/> class is to define the primitive <see cref="string"/> data type
    /// </summary>
    /// <remarks>
    /// This element defines a data type for the representation of String data values in the Exchange Document.
    /// </remarks>
    public class DatatypeDefinitionString : DatatypeDefinitionSimple 
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<DatatypeDefinitionString> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionString"/> class.
        /// </summary>
        public DatatypeDefinitionString()
        {
            this.logger = NullLogger<DatatypeDefinitionString>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionReal"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public DatatypeDefinitionString(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<DatatypeDefinitionString>.Instance : this.loggerFactory.CreateLogger<DatatypeDefinitionString>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionString"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal DatatypeDefinitionString(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<DatatypeDefinitionString>.Instance : this.loggerFactory.CreateLogger<DatatypeDefinitionString>();
        }

        /// <summary>
        /// Gets or sets the maximum permissible string length
        /// </summary>
        public long MaxLength { get; set; }

        /// <summary>
        /// Generates a <see cref="DatatypeDefinitionReal"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            this.ReadXmlAttributes(reader);

            this.ReadAlternativeId(reader);
        }

        /// <summary>
        /// Asynchronously generates a <see cref="DatatypeDefinitionReal"/> object from its XML representation.
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

            this.ReadXmlAttributes(reader);

            await this.ReadAlternativeIdAsync(reader, token);
        }

        /// <summary>
        /// Reads the properties that are defined as XML Attributes (MAX-LENGTH)
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void ReadXmlAttributes(XmlReader reader)
        {
            var xmlLineInfo = reader as IXmlLineInfo;

            this.logger.LogTrace("reading MAX-LENGTH at line:position {LineNumber}:{LinePosition}", xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

            var maxLength = reader.GetAttribute("MAX-LENGTH");
            if (!string.IsNullOrWhiteSpace(maxLength))
            {
                try
                {
                    this.MaxLength = XmlConvert.ToInt64(maxLength);
                }
                catch (OverflowException)
                {
                    this.logger.LogWarning("The DatatypeDefinitionString.MAX-LENGTH: {Value} at line:position {LineNumber}:{LinePosition} could not be processed. MaxLength is set to Int64.MaxValue",
                        maxLength, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

                    this.MaxLength = long.MaxValue;
                }
                catch (Exception e)
                {
                    throw new SerializationException($"The DatatypeDefinitionString.MAX-LENGTH: {maxLength} at line:position {xmlLineInfo?.LineNumber}:{xmlLineInfo?.LinePosition} could not be converted to an INTEGER", e);
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="DatatypeDefinitionReal"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("MAX-LENGTH", XmlConvert.ToString(this.MaxLength));

            base.WriteXml(writer);
        }

        /// <summary>
        /// Asynchronously converts a <see cref="DatatypeDefinitionReal"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            await writer.WriteAttributeStringAsync(null, "MAX-LENGTH", null, XmlConvert.ToString(this.MaxLength));

            await base.WriteXmlAsync(writer, token);
        }
    }
}
