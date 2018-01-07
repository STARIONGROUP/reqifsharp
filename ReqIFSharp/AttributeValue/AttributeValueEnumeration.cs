// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueEnumeration.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    
    /// <summary>
    /// The purpose of the <see cref="AttributeValueEnumeration"/> class is to define an enumeration attribute value.
    /// </summary>
    public class AttributeValueEnumeration : AttributeValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueEnumeration"/> class.
        /// </summary>
        public AttributeValueEnumeration()
        {
        }

        /// <summary>
        /// Instantiated a new instance of the <see cref="AttributeValueEnumeration"/> class
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinitionEnumeration"/> for which this is the default value</param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinitionEnumeration"/>
        /// </remarks>
        internal AttributeValueEnumeration(AttributeDefinitionEnumeration attributeDefinition)
            : base(attributeDefinition)
        {
            this.OwningDefinition = attributeDefinition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueEnumeration"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        internal AttributeValueEnumeration(SpecElementWithAttributes specElAt)
            : base(specElAt)
        {
        }

        /// <summary>
        /// Backing field for the <see cref="Values"/> property.
        /// </summary>
        private readonly List<EnumValue> values = new List<EnumValue>();

        /// <summary>
        /// Gets <see cref="EnumValue"/>s that are chosen from a set of specified values
        /// </summary>
        public List<EnumValue> Values 
        {
            get
            {
                return this.values;
            }
        }

        /// <summary>
        /// Gets or sets the value of this <see cref="AttributeValue"/>
        /// </summary>
        /// <remarks>
        /// This is a convenience property to get/set TheValue or Values in concrete implementation
        /// </remarks>
        public override object ObjectValue
        {
            get => this.Values;
            set
            {
                if (!(value is IEnumerable<EnumValue> enumValues))
                {
                    throw new InvalidOperationException($"The value to set is not an IEnumerable<EnumValue>.");
                }

                this.values.Clear();
                this.values.AddRange(enumValues);
            }
        }

        /// <summary>
        /// Gets or sets the Reference to the attribute definition that relates the value to its data type.
        /// </summary>
        public AttributeDefinitionEnumeration Definition { get; set; }

        /// <summary>
        /// Gets or sets the owning attribute definition.
        /// </summary>
        public AttributeDefinitionEnumeration OwningDefinition { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition "/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="AttributeDefinitionEnumeration"/>
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
            if (attributeDefinition.GetType() != typeof(AttributeDefinitionEnumeration))
            {
                throw new ArgumentException("attributeDefinition must of type AttributeDefinitionEnumeration");
            }

            this.Definition = (AttributeDefinitionEnumeration)attributeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeValueEnumeration"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            using (var subtree = reader.ReadSubtree())
            {
                while (subtree.Read())
                {
                    if (subtree.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ATTRIBUTE-DEFINITION-ENUMERATION-REF")
                    {
                        var reference = reader.ReadElementContentAsString();

                        this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionEnumeration>().SingleOrDefault(x => x.Identifier == reference);
                        if (this.Definition == null)
                        {
                            throw new InvalidOperationException(string.Format("The attribute-definition Enumeration {0} could not be found for the value.", reference));
                        }
                    }

                    if (subtree.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ENUM-VALUE-REF")
                    {
                        var reference = reader.ReadElementContentAsString();

                        var enumValue = this.ReqIFContent.DataTypes.OfType<DatatypeDefinitionEnumeration>().SelectMany(x => x.SpecifiedValues).SingleOrDefault(x => x.Identifier == reference);
                        if (enumValue != null)
                        {
                            this.values.Add(enumValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeValueEnumeration"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            if (this.Definition == null)
            {
                throw new SerializationException("The Definition property of an AttributeValueEnumeration may not be null");
            }

            this.WriteDefinition(writer);

            this.WriteValues(writer);
        }

        /// <summary>
        /// Writes the <see cref="Definition"/> object.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteDefinition(XmlWriter writer)
        {
            writer.WriteStartElement("DEFINITION");
            writer.WriteElementString("ATTRIBUTE-DEFINITION-ENUMERATION-REF", this.Definition.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the <see cref="EnumValue"/> objects in the <see cref="Values"/> property.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteValues(XmlWriter writer)
        {
            if (this.values.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("VALUES");

            foreach (var enumValue in this.values)
            {
                writer.WriteElementString("ENUM-VALUE-REF", enumValue.Identifier);
            }

            writer.WriteEndElement();
        }
    }
}