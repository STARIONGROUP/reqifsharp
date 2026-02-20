// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionInteger.cs" company="Starion Group S.A.">
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
    /// The purpose of the <see cref="DatatypeDefinitionInteger"/> class is to define the primitive Integer data type
    /// </summary>
    /// <remarks>
    /// This element defines a data type for the representation of Integer data values in the Exchange Document.
    /// The representation of data values shall comply with the definitions in http://www.w3.org/TR/xmlschema-2/#integer
    /// ReqIfSharp supports 64-bit signed integers (long) with the following range: -9223372036854775808 to 9223372036854775807
    /// </remarks>
    public class DatatypeDefinitionInteger : DatatypeDefinitionSimple
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<DatatypeDefinitionInteger> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionInteger"/> class.
        /// </summary>
        public DatatypeDefinitionInteger()
        {
            this.logger = NullLogger<DatatypeDefinitionInteger>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionInteger"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public DatatypeDefinitionInteger(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<DatatypeDefinitionInteger>.Instance : this.loggerFactory.CreateLogger<DatatypeDefinitionInteger>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionInteger"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal DatatypeDefinitionInteger(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<DatatypeDefinitionInteger>.Instance : this.loggerFactory.CreateLogger<DatatypeDefinitionInteger>();
        }

        /// <summary>
        /// Gets or sets a value that denotes the largest negative data value representable by this data type.
        /// </summary>
        public long Min { get; set; }

        /// <summary>
        /// Gets or sets a value that denotes the largest positive data value representable by this data type.
        /// </summary>
        public long Max { get; set; }

        /// <summary>
        /// Generates a <see cref="AttributeDefinition"/> object from its XML representation.
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
        /// Asynchronously generates a <see cref="AttributeDefinition"/> object from its XML representation.
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
        /// Reads the properties that are defined as XML Attributes (MAX, MIN)
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void ReadXmlAttributes(XmlReader reader)
        {
            var xmlLineInfo = reader as IXmlLineInfo;

            this.logger.LogTrace("reading MAX at line:position {LineNumber}:{LinePosition}", xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

            var maxValue = reader.GetAttribute("MAX");
            if (!string.IsNullOrWhiteSpace(maxValue))
            {
                try
                {
                    this.Max = XmlConvert.ToInt64(maxValue);
                }
                catch (OverflowException)
                {
                    this.logger.LogWarning("The DatatypeDefinitionInteger.MAX: {Value} at line:position {LineNumber}:{LinePosition} could not be processed. Max is set to Int64.MaxValue",
                        maxValue, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

                    this.Max = long.MaxValue;
                }
                catch (Exception e)
                {
                    throw new SerializationException($"The DatatypeDefinitionInteger.MAX {maxValue} at line:position {xmlLineInfo?.LineNumber}:{xmlLineInfo?.LinePosition} could not be converted to an INTEGER", e);
                }
            }

            this.logger.LogTrace("reading MIN at line:position {LineNumber}:{LinePosition}", xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

            var minValue = reader.GetAttribute("MIN");
            if (!string.IsNullOrWhiteSpace(minValue))
            {
                try
                {
                    this.Min = XmlConvert.ToInt64(minValue);
                }
                catch (OverflowException)
                {
                    this.logger.LogWarning("The DatatypeDefinitionInteger.MIN: {Value} at line:position {LineNumber}:{LinePosition} could not be processed. Min is set to Int64.MinValue",
                        minValue, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

                    this.Min = long.MinValue;
                }
                catch (Exception e)
                {
                    throw new SerializationException($"The DatatypeDefinitionInteger.MIN {minValue} at line:position {xmlLineInfo?.LineNumber}:{xmlLineInfo?.LinePosition} could not be converted to an INTEGER", e);
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeDefinition"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal override void WriteXml(XmlWriter writer)
        { 
            writer.WriteAttributeString("MIN", XmlConvert.ToString(this.Min)); 
            writer.WriteAttributeString("MAX", XmlConvert.ToString(this.Max));

            base.WriteXml(writer);
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeDefinition"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            await writer.WriteAttributeStringAsync(null, "MIN", null, XmlConvert.ToString(this.Min));
            await writer.WriteAttributeStringAsync(null, "MAX",null, XmlConvert.ToString(this.Max));

            await base.WriteXmlAsync(writer, token);
        }
    }
}
