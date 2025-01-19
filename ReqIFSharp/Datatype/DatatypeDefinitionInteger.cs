// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionInteger.cs" company="Starion Group S.A.">
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
// -------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The purpose of the <see cref="DatatypeDefinitionInteger"/> class is to define the primitive Integer data type
    /// </summary>
    /// <remarks>
    /// This element defines a data type for the representation of Integer data values in the Exchange Document.
    /// The representation of data values shall comply with the definitions in http://www.w3.org/TR/xmlschema-2/#integer
    /// ReqIfSharp supports 64 bit signed integers (long) with the following range: -9223372036854775808 to 9223372036854775807
    /// </remarks>
    public class DatatypeDefinitionInteger : DatatypeDefinitionSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionInteger"/> class.
        /// </summary>
        public DatatypeDefinitionInteger()
        {
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionInteger"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal DatatypeDefinitionInteger(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
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

            var maxValue = reader.GetAttribute("MAX"); 
            if (!string.IsNullOrEmpty(maxValue)) 
            { 
                this.Max = XmlConvert.ToInt64(maxValue);
            }
            
            var minValue = reader.GetAttribute("MIN"); 
            if (!string.IsNullOrEmpty(minValue)) 
            { 
                this.Min = XmlConvert.ToInt64(minValue);
            }

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

            var maxValue = reader.GetAttribute("MAX");
            if (!string.IsNullOrEmpty(maxValue))
            {
                this.Max = XmlConvert.ToInt64(maxValue);
            }

            var minValue = reader.GetAttribute("MIN");
            if (!string.IsNullOrEmpty(minValue))
            {
                this.Min = XmlConvert.ToInt64(minValue);
            }

            await this.ReadAlternativeIdAsync(reader, token);
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
