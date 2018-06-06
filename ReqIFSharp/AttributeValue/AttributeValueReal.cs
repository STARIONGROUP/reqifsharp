// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueReal.cs" company="RHEA System S.A.">
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
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// The purpose of the <see cref="AttributeValueReal"/> class is to define a real attribute value.
    /// </summary>
    public class AttributeValueReal : AttributeValueSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueReal"/> class.
        /// </summary>
        public AttributeValueReal()
        {
        }

        /// <summary>
        /// Instantiated a new instance of the <see cref="AttributeValueReal"/> class
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinitionReal"/> for which this is the default value</param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinitionReal"/>
        /// </remarks>
        internal AttributeValueReal(AttributeDefinitionReal attributeDefinition)
            : base(attributeDefinition)
        {
            this.OwningDefinition = attributeDefinition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueReal"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        internal AttributeValueReal(SpecElementWithAttributes specElAt)
            : base(specElAt)
        {
        }

        /// <summary>
        /// Gets or sets the attribute value
        /// </summary>
        public double? TheValue { get; set; }

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
                if (!(value is double castValue))
                {
                    throw new InvalidOperationException($"Cannot use {value} as value for this AttributeValueDouble.");
                }

                this.TheValue = castValue;
            }
        }

        /// <summary>
        /// Gets or sets the reference to the value definition
        /// </summary>
        public AttributeDefinitionReal Definition { get; set; }

        /// <summary>
        /// Gets or sets the owning attribute definition.
        /// </summary>
        public AttributeDefinitionReal OwningDefinition { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition "/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="AttributeDefinitionReal"/>
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
            if (attributeDefinition.GetType() != typeof(AttributeDefinitionReal))
            {
                throw new ArgumentException("attributeDefinition must of type AttributeDefinitionReal");
            }

            this.Definition = (AttributeDefinitionReal)attributeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeValueReal"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            var value = reader["THE-VALUE"];
            if (double.TryParse(value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double theValue))
            {
                this.TheValue = theValue;
            }

            while (reader.Read())
            {
                if (reader.ReadToDescendant("ATTRIBUTE-DEFINITION-REAL-REF"))
                {
                    var reference = reader.ReadElementContentAsString();

                    this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionReal>().SingleOrDefault(x => x.Identifier == reference);
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeValueReal"/> object into its XML representation.
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
                writer.WriteAttributeString("THE-VALUE", this.TheValue.Value.ToString(NumberFormatInfo.InvariantInfo));
            }

            writer.WriteStartElement("DEFINITION");
            writer.WriteElementString("ATTRIBUTE-DEFINITION-REAL-REF", this.Definition.Identifier);
            writer.WriteEndElement();
        }
    }
}