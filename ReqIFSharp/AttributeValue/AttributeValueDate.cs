// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueDate.cs" company="RHEA System S.A.">
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
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// The purpose of the <see cref="AttributeValueDate"/> class is to define a <see cref="DateTime"/> attribute value.
    /// </summary>
    public class AttributeValueDate : AttributeValueSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueDate"/> class.
        /// </summary>
        public AttributeValueDate()
        {
        }

        /// <summary>
        /// Instantiated a new instance of the <see cref="AttributeValueDate"/> class
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinitionDate"/> for which this is the default value</param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinitionDate"/>
        /// </remarks>
        internal AttributeValueDate(AttributeDefinitionDate attributeDefinition)
            : base(attributeDefinition)
        {
            this.OwningDefinition = attributeDefinition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueDate"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        internal AttributeValueDate(SpecElementWithAttributes specElAt)
            : base(specElAt)
        {
        }

        /// <summary>
        /// Gets or sets the attribute value.
        /// </summary>
        public DateTime TheValue { get; set; }

        /// <summary>
        /// Gets or sets the Reference to the value definition.
        /// </summary>
        public AttributeDefinitionDate Definition { get; set; }

        /// <summary>
        /// Gets or sets the owning attribute definition.
        /// </summary>        
        public AttributeDefinitionDate OwningDefinition { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition "/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="AttributeDefinitionDate"/>
        /// </returns>
        protected override AttributeDefinition GetAttributeDefinition()
        {
            return this.Definition;
        }

        /// <summary>
        /// Sets the <see cref="AttributeDefinition"/>
        /// </summary>
        /// <param name="attributeDefinition">
        /// The <see cref="AttributeDefinition"/> to set
        /// </param>
        protected override void SetAttributeDefinition(AttributeDefinition attributeDefinition)
        {
            if (attributeDefinition.GetType() != typeof(AttributeDefinitionDate))
            {
                throw new ArgumentException("attributeDefinition must of type AttributeDefinitionDate");
            }

            this.Definition = (AttributeDefinitionDate)attributeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeValueDate"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            var value = reader["THE-VALUE"];
            this.TheValue = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);

            while (reader.Read())
            {
                if (reader.ReadToDescendant("ATTRIBUTE-DEFINITION-DATE-REF"))
                {
                    var reference = reader.ReadElementContentAsString();

                    this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionDate>().SingleOrDefault(x => x.Identifier == reference);
                    if (this.Definition == null)
                    {
                        throw new InvalidOperationException(string.Format("The attribute-definition Date {0} could not be found for the value.", reference));
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeValueDate"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Definition"/> may not be null
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            if (this.Definition == null)
            {
                throw new SerializationException("The Definition property of an AttributeValueDate may not be null");
            }

            writer.WriteAttributeString("THE-VALUE", this.TheValue.ToString("o"));
            writer.WriteStartElement("DEFINITION");
            writer.WriteElementString("ATTRIBUTE-DEFINITION-DATE-REF", this.Definition.Identifier);
            writer.WriteEndElement();
        }
    }
}