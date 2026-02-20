// -------------------------------------------------------------------------------------------------
// <copyright file="AlternativeId.cs" company="Starion Group S.A.">
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
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The purpose of the <see cref="AlternativeId"/> class is to provide an alternative, tool-specific identification.
    /// </summary>
    public class AlternativeId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlternativeId"/> class.
        /// </summary>
        public AlternativeId()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlternativeId"/> class.
        /// </summary>
        /// <param name="identifiable">
        /// The owning <see cref="Identifiable"/>
        /// </param>
        internal AlternativeId(Identifiable identifiable)
        {
            this.Ident = identifiable;
            identifiable.AlternativeId = this;
        }

        /// <summary>
        /// Gets or sets the optional alternative identifier, which may be a requirements management tool identifier or <see cref="ReqIF"/> tool identifier
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the owning <see cref="Identifiable"/>.
        /// </summary>
        public Identifiable Ident { get; set; }

        /// <summary>
        /// Converts a <see cref="AlternativeId"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Identifier"/> may not be null or empty
        /// </exception>
        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("ALTERNATIVE-ID");
            writer.WriteAttributeString("IDENTIFIER", this.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AlternativeId"/> object into its XML representation.
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
        internal async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await writer.WriteStartElementAsync(null, "ALTERNATIVE-ID", null);
            await writer.WriteAttributeStringAsync(null, "IDENTIFIER", null, this.Identifier);
            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// Generates a <see cref="AlternativeId"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "ALTERNATIVE-ID":
                            this.Identifier = reader.GetAttribute("IDENTIFIER");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Generates a <see cref="AlternativeId"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                if (await reader.MoveToContentAsync() == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "ALTERNATIVE-ID":
                            this.Identifier = reader.GetAttribute("IDENTIFIER");
                            break;
                    }
                }
            }
        }
    }
}
