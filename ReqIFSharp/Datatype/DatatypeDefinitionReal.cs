// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionReal.cs" company="Starion Group S.A.">
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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// This element defines a data type for the representation of Real data values in the Exchange Document.
    /// </summary>
    public class DatatypeDefinitionReal : DatatypeDefinitionSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionReal"/> class.
        /// </summary>
        public DatatypeDefinitionReal()
        {
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionReal"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal DatatypeDefinitionReal(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
        }

        /// <summary>
        /// Gets or sets a value that Denotes the supported maximum precision of real numbers represented by this data type.
        /// </summary>
        public int Accuracy { get; set; }

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

            var accuracyValue = reader.GetAttribute("ACCURACY");
            if (!string.IsNullOrEmpty(accuracyValue))
            {
                this.Accuracy = XmlConvert.ToInt32(accuracyValue);
            }

            var maxValue = reader.GetAttribute("MAX");
            if (!string.IsNullOrEmpty(maxValue))
            {
                this.Max = XmlConvert.ToDouble(maxValue);
            }

            var minValue = reader.GetAttribute("MIN"); 
            if (!string.IsNullOrEmpty(minValue)) 
            { 
                this.Min = XmlConvert.ToDouble(minValue);
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

            var accuracyValue = reader.GetAttribute("ACCURACY");
            if (!string.IsNullOrEmpty(accuracyValue))
            {
                this.Accuracy = XmlConvert.ToInt32(accuracyValue);
            }

            var maxValue = reader.GetAttribute("MAX");
            if (!string.IsNullOrEmpty(maxValue))
            {
                this.Max = XmlConvert.ToDouble(maxValue);
            }

            var minValue = reader.GetAttribute("MIN");
            if (!string.IsNullOrEmpty(minValue))
            {
                this.Min = XmlConvert.ToDouble(minValue);
            }

            await this.ReadAlternativeIdAsync(reader, token);
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
