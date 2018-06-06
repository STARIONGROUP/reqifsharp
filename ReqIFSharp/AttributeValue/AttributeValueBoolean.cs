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
    using System.Linq;
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
        /// Instantiated a new instance of the <see cref="AttributeValueBoolean"/> class
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinitionBoolean"/> for which this is the default value</param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinitionBoolean"/>
        /// </remarks>
        internal AttributeValueBoolean(AttributeDefinitionBoolean attributeDefinition)
            : base(attributeDefinition)
        {
            this.OwningDefinition = attributeDefinition;
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
        public bool? TheValue { get; set; }

        /// <summary>
        /// Gets or sets the value of this <see cref="AttributeValue"/>
        /// </summary>
        /// <remarks>
        /// This is a convenience property to get/set TheValue or Values in concrete implementation
        /// </remarks>
        public override object ObjectValue
        {
            get => this.TheValue.Value;
            set
            {
                if (!(value is bool castValue))
                {
                    throw new InvalidOperationException($"Cannot use {value} as value for this AttributeValueBoolean.");
                }

                this.TheValue = castValue;
            }
        }

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
            if (value == "1" || value == "true")
            {
                this.TheValue = true;
            }
            else if (value == "0" || value == "false")
            {
                this.TheValue = false;
            }

            while (reader.Read())
            {
                if (reader.ReadToDescendant("ATTRIBUTE-DEFINITION-BOOLEAN-REF"))
                {
                    var reference = reader.ReadElementContentAsString();

                    this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionBoolean>().SingleOrDefault(x => x.Identifier == reference);
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
                return;
            }

            if (this.TheValue.HasValue)
            {
                writer.WriteAttributeString("THE-VALUE", this.TheValue.Value ? "true" : "false");
            }

            writer.WriteStartElement("DEFINITION");
            if (!string.IsNullOrEmpty(this.Definition.Identifier))
            {
                writer.WriteElementString("ATTRIBUTE-DEFINITION-BOOLEAN-REF", this.Definition.Identifier);
            }

            writer.WriteEndElement();
        }
    }
}