// -------------------------------------------------------------------------------------------------
// <copyright file="EmbeddedValue.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2022 RHEA System S.A.
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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The <see cref="EmbeddedValue"/> class represents additional information related to enumeration literals.
    /// </summary>
    public class EmbeddedValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedValue"/> class.
        /// </summary>
        public EmbeddedValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedValue"/> class.
        /// </summary>
        /// <param name="enumValue">
        /// The owning <see cref="EnumValue"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal EmbeddedValue(EnumValue enumValue, ILoggerFactory loggerFactory)
        {
            this.EnumValue = enumValue;
            this.EnumValue.Properties = this;
        }

        /// <summary>
        /// Gets or sets the numerical value corresponding to the enumeration literal.
        /// </summary>
        public int Key { get; set; }

        /// <summary>
        /// Gets or sets Arbitrary additional information related to the enumeration literal.
        /// </summary>
        /// <example>
        /// example: a color
        /// </example>
        public string OtherContent { get; set; }

        /// <summary>
        /// Gets or sets the owning <see cref="EnumValue"/>
        /// </summary>
        public EnumValue EnumValue { get; set; }

        /// <summary>
        /// Generates a <see cref="EmbeddedValue"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal void ReadXml(XmlReader reader)
        {
            var key = reader.GetAttribute("KEY");

            if (key != null)
            {
                this.Key = XmlConvert.ToInt32(key);
            }

            this.OtherContent = reader.GetAttribute("OTHER-CONTENT");
        }

        /// <summary>
        /// Converts a <see cref="EmbeddedValue"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("KEY", XmlConvert.ToString(this.Key));
            writer.WriteAttributeString("OTHER-CONTENT", this.OtherContent);
        }

        /// <summary>
        /// Asynchronously converts a <see cref="EmbeddedValue"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            await writer.WriteAttributeStringAsync(null,"KEY", null, XmlConvert.ToString(this.Key));
            await writer.WriteAttributeStringAsync(null, "OTHER-CONTENT", null, this.OtherContent);
        }
    }
}
