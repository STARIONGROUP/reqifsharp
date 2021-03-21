// -------------------------------------------------------------------------------------------------
// <copyright file="EmbeddedValue.cs" company="RHEA System S.A.">
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
    /// The <see cref="EmbeddedValue"/> class represents additional information related to enumeration literals.
    /// </summary>
    public class EmbeddedValue : IXmlSerializable
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
        internal EmbeddedValue(EnumValue enumValue)
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
        public void ReadXml(XmlReader reader)
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
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("KEY", XmlConvert.ToString(this.Key));
            writer.WriteAttributeString("OTHER-CONTENT", this.OtherContent);
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
