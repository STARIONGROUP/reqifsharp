// -------------------------------------------------------------------------------------------------
// <copyright file="AlternativeId.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
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
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The purpose of the <see cref="AlternativeId"/> class is to provide an alternative, tool-specific identification.
    /// </summary>
    public class AlternativeId : IXmlSerializable
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
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("IDENTIFIER", this.Identifier);
        }

        /// <summary>
        /// Generates a <see cref="AlternativeId"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public void ReadXml(XmlReader reader)
        {
            this.Identifier = reader.GetAttribute("IDENTIFIER");
        }

        /// <summary>
        /// This method is reserved and should not be used.
        /// </summary>
        /// <returns>returns null</returns>
        /// <remarks>
        /// When implementing the IXmlSerializable interface, you should return null
        /// </remarks>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
    }
}