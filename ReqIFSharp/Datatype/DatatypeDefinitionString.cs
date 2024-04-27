// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionString.cs" company="Starion Group S.A.">
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
    /// The purpose of the <see cref="DatatypeDefinitionString"/> class is to define the primitive <see cref="string"/> data type
    /// </summary>
    /// <remarks>
    /// This element defines a data type for the representation of String data values in the Exchange Document.
    /// </remarks>
    public class DatatypeDefinitionString : DatatypeDefinitionSimple 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionString"/> class.
        /// </summary>
        public DatatypeDefinitionString()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionString"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal DatatypeDefinitionString(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
        }

        /// <summary>
        /// Gets or sets the maximum permissible string length
        /// </summary>        
        public int MaxLength { get; set; }

        /// <summary>
        /// Generates a <see cref="DatatypeDefinitionReal"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        { 
            base.ReadXml(reader);

            var value = reader.GetAttribute("MAX-LENGTH"); 
            if ( !string.IsNullOrEmpty(value)) 
            { 
                this.MaxLength = XmlConvert.ToInt32(value);
            }

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

            var value = reader.GetAttribute("MAX-LENGTH");
            if (!string.IsNullOrEmpty(value))
            {
                this.MaxLength = XmlConvert.ToInt32(value);
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
