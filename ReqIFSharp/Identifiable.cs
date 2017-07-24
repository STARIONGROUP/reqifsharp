// -------------------------------------------------------------------------------------------------
// <copyright file="Identifiable.cs" company="RHEA System S.A.">
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
// -------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="Identifiable"/> Abstract base class provides an identification concept for <see cref="ReqIF"/> elements.
    /// </summary>
    public abstract class Identifiable : IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Identifiable"/> class.
        /// </summary>
        /// <remarks>
        /// The <see cref="LastChange"/> property is set to the current time.
        /// </remarks>
        protected Identifiable()
        {
            this.LastChange = DateTime.Now;
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
        /// <code>CCYY-MM-DDThh:mm:ss</code> with optional time zone indicator as a suffix <code>±hh:mm</code>.
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
        public virtual void ReadXml(XmlReader reader)
        {
            this.Identifier = reader.GetAttribute("IDENTIFIER");

            var lastChange = reader.GetAttribute("LAST-CHANGE");
            this.LastChange = XmlConvert.ToDateTime(lastChange, XmlDateTimeSerializationMode.Utc);

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
        public virtual void WriteXml(XmlWriter writer)
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

            writer.WriteAttributeString("LAST-CHANGE", XmlConvert.ToString(this.LastChange, XmlDateTimeSerializationMode.Utc));

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