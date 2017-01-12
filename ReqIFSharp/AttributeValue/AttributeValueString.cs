// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueString.cs" company="RHEA System S.A.">
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
    /// The purpose of the <see cref="AttributeValueString"/> class is to define a <see cref="string"/> attribute value.
    /// </summary>
    public class AttributeValueString : AttributeValueSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueString"/> class.
        /// </summary>
        public AttributeValueString()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueString"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        internal AttributeValueString(SpecElementWithAttributes specElAt)
            : base(specElAt)
        {
        }

        /// <summary>
        /// Gets or sets the attribute value
        /// </summary>
        public string TheValue { get; set; }

        /// <summary>
        /// Gets or sets reference to the value definition
        /// </summary>
        public AttributeDefinitionString Definition { get; set; }

        /// <summary>
        /// Gets or sets the owning attribute definition.
        /// </summary>
        public AttributeDefinitionString OwningDefinition { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition "/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="AttributeDefinitionString"/>
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
            if (attributeDefinition.GetType() != typeof(AttributeDefinitionString))
            {
                throw new ArgumentException("attributeDefinition must of type AttributeDefinitionString");
            }

            this.Definition = (AttributeDefinitionString)attributeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeValueString"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            var value = reader["THE-VALUE"];
            this.TheValue = value;

            while (reader.Read())
            {
                if (reader.ReadToDescendant("ATTRIBUTE-DEFINITION-STRING-REF"))
                {
                    var reference = reader.ReadElementContentAsString();

                    AttributeDefinitionString attributeDefinitionString = null;
                    foreach (var specType in this.SpecElAt.ReqIfContent.SpecTypes)
                    {
                        foreach (var attributeDefinition in specType.SpecAttributes)
                        {
                            if (attributeDefinition.Identifier == reference)
                            {
                                attributeDefinitionString = (AttributeDefinitionString)attributeDefinition;
                                break;
                            }
                        }
                    }

                    this.Definition = attributeDefinitionString;

                    break;
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeValueString"/> object into its XML representation.
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
                throw new SerializationException(string.Format("The Definition property of an AttributeValueString may not be null"));
            }

            writer.WriteAttributeString("THE-VALUE", this.TheValue.ToString());
            writer.WriteStartElement("DEFINITION");
            writer.WriteElementString("ATTRIBUTE-DEFINITION-STRING-REF", this.Definition.Identifier);
            writer.WriteEndElement();
        }
    }
}