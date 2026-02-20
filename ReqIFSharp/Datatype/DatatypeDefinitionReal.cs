// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionReal.cs" company="Starion Group S.A.">
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
    /// This element defines a data type for the representation of Real data values in the Exchange Document.
    /// </summary>
    public class DatatypeDefinitionReal : DatatypeDefinitionSimple
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<DatatypeDefinitionReal> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionReal"/> class.
        /// </summary>
        public DatatypeDefinitionReal()
        {
            this.logger = NullLogger<DatatypeDefinitionReal>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionReal"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public DatatypeDefinitionReal(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<DatatypeDefinitionReal>.Instance : this.loggerFactory.CreateLogger<DatatypeDefinitionReal>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionReal"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal DatatypeDefinitionReal(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<DatatypeDefinitionReal>.Instance : this.loggerFactory.CreateLogger<DatatypeDefinitionReal>();
        }

        /// <summary>
        /// Gets or sets a value that Denotes the supported maximum precision of real numbers represented by this data type.
        /// </summary>
        public long Accuracy { get; set; }

        /// <summary>
        /// Gets or sets a value that denotes the largest negative data value representable by this data type.
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// Gets or sets a value that denotes the largest positive data value representable by this data type.
        /// </summary>
        public double Max { get; set; }

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
        /// Reads the properties that are defined as XML Attributes (ACCURACY, MAX, MIN)
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void ReadXmlAttributes(XmlReader reader)
        {
            var xmlLineInfo = reader as IXmlLineInfo;

            this.logger.LogTrace("reading DatatypeDefinitionReal.ACCURACY at line:position {LineNumber}:{LinePosition}", xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

            var accuracyValue = reader.GetAttribute("ACCURACY");
            if (!string.IsNullOrWhiteSpace(accuracyValue))
            {
                try
                {
                    this.Accuracy = XmlConvert.ToInt64(accuracyValue);
                }
                catch (OverflowException)
                {
                    this.logger.LogWarning("The DatatypeDefinitionReal.ACCURACY: {Value} at line:position {LineNumber}:{LinePosition} could not be processed. Accuracy is set to Int64.MaxValue",
                        accuracyValue, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

                    this.Accuracy = long.MaxValue;
                }
                catch (Exception e)
                {
                    throw new SerializationException($"The DatatypeDefinitionReal.ACCURACY {accuracyValue} at line:position {xmlLineInfo?.LineNumber}:{xmlLineInfo?.LinePosition} could not be converted to an INTEGER", e);
                }
            }

            this.logger.LogTrace("reading DatatypeDefinitionReal.MAX at line:position {LineNumber}:{LinePosition}", xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

            var maxValue = reader.GetAttribute("MAX");
            if (!string.IsNullOrWhiteSpace(maxValue))
            {
                try
                {
                    this.Max = XmlConvert.ToDouble(maxValue);
                }
                catch (Exception e)
                {
                    throw new SerializationException($"The DatatypeDefinitionReal.MAX {maxValue} at line:position {xmlLineInfo?.LineNumber}:{xmlLineInfo?.LinePosition} could not be converted to a REAL", e);
                }
            }

            this.logger.LogTrace("reading DatatypeDefinitionReal.MIN at line:position {LineNumber}:{LinePosition}", xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

            var minValue = reader.GetAttribute("MIN");
            if (!string.IsNullOrWhiteSpace(minValue))
            {
                try
                {
                    this.Min = XmlConvert.ToDouble(minValue);
                }
                catch (Exception e)
                {
                    throw new SerializationException($"The DatatypeDefinitionReal.MIN {minValue} at line:position {xmlLineInfo?.LineNumber}:{xmlLineInfo?.LinePosition} could not be converted to a REAL", e);
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
            writer.WriteAttributeString("ACCURACY", XmlConvert.ToString(this.Accuracy));
            writer.WriteAttributeString("MIN", XmlConvert.ToString(this.Min));
            writer.WriteAttributeString("MAX", XmlConvert.ToString(this.Max));

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
            await writer.WriteAttributeStringAsync(null,"ACCURACY", null, XmlConvert.ToString(this.Accuracy));
            await writer.WriteAttributeStringAsync(null, "MIN", null, XmlConvert.ToString(this.Min));
            await writer.WriteAttributeStringAsync(null, "MAX", null, XmlConvert.ToString(this.Max));

            await base.WriteXmlAsync(writer, token);
        }
    }
}
