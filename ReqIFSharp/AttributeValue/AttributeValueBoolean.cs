// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueBoolean.cs" company="RHEA System S.A.">
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
    using System.Runtime.Serialization;
    using System.Xml;
    
    /// <summary>
    /// The purpose of the <see cref="AttributeValueBoolean"/> class is to define a <see cref="bool"/> attribute value.
    /// </summary>
    public class AttributeValueBoolean : AttributeValueSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueBoolean"/> class.
        /// </summary>
        public AttributeValueBoolean()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueBoolean"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        internal AttributeValueBoolean(SpecElementWithAttributes specElAt)
            : base(specElAt)
        {
        }

        /// <summary>
        /// Gets or sets the attribute value.
        /// </summary>
        public bool TheValue { get; set; }

        /// <summary>
        /// Gets or sets a reference to the value definition
        /// </summary>
        public AttributeDefinitionBoolean Definition { get; set; }

        /// <summary>
        /// Gets or sets the owning attribute definition.
        /// </summary>
        public AttributeDefinitionBoolean OwningDefinition { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition "/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="AttributeDefinitionBoolean"/>
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
            if (attributeDefinition.GetType() != typeof(AttributeDefinitionBoolean))
            {
                throw new ArgumentException("attributeDefinition must of type AttributeDefinitionBoolean");
            }

            this.Definition = (AttributeDefinitionBoolean)attributeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeValueBoolean"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            var value = reader["THE-VALUE"];
            this.TheValue = XmlConvert.ToBoolean(value);

            while (reader.Read())
            {
                if (reader.ReadToDescendant("ATTRIBUTE-DEFINITION-BOOLEAN-REF"))
                {
                    var reference = reader.ReadElementContentAsString();

                    AttributeDefinitionBoolean attributeDefinitionBoolean = null;
                    foreach (var specType in this.SpecElAt.ReqIfContent.SpecTypes)
                    {
                        foreach (var attributeDefinition in specType.SpecAttributes)
                        {
                            if (attributeDefinition.Identifier == reference)
                            {
                                attributeDefinitionBoolean = (AttributeDefinitionBoolean)attributeDefinition;
                                break;
                            }
                        }
                    }

                    this.Definition = attributeDefinitionBoolean;

                    break;
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeValueBoolean"/> object into its XML representation.
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
                throw new SerializationException(string.Format("The Definition property of an AttributeValueBoolean may not be null"));
            }

            writer.WriteAttributeString("THE-VALUE", this.TheValue ? "true" : "false");
            writer.WriteStartElement("DEFINITION");
            writer.WriteElementString("ATTRIBUTE-DEFINITION-BOOLEAN-REF", this.Definition.Identifier);
            writer.WriteEndElement();
        }
    }
}