// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionEnumeration.cs" company="RHEA System S.A.">
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
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// The purpose of the <see cref="AttributeDefinitionEnumeration"/> class is to define a enumeration attribute.
    /// </summary>
    /// <remarks>
    /// An <see cref="AttributeDefinitionEnumeration"/> element relates an <see cref="AttributeValueEnumeration"/> element to a
    /// <see cref="DatatypeDefinitionEnumeration"/> element via its <see cref="Type"/> attribute.
    /// An <see cref="AttributeDefinitionEnumeration"/> element MAY contain a default value that represents the value that is used as an
    /// attribute value if no attribute value is supplied by the user of the requirements authoring tool.
    /// </remarks>
    public class AttributeDefinitionEnumeration : AttributeDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionEnumeration"/> class.
        /// </summary>
        public AttributeDefinitionEnumeration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionEnumeration"/> class.
        /// </summary>
        /// <param name="specType">
        /// The owning <see cref="SpecType"/>.
        /// </param>
        internal AttributeDefinitionEnumeration(SpecType specType) 
            : base(specType)
        {            
        }

        /// <summary>
        /// Gets or sets the owned default value that is used if no attribute value is supplied 
        /// by the user of the requirements authoring tool.
        /// </summary>
        public AttributeValueEnumeration DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        public DatatypeDefinitionEnumeration Type { get; set; }

        /// <summary>
        /// Gets the <see cref="DatatypeDefinition"/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="DatatypeDefinitionEnumeration"/>
        /// </returns>
        protected override DatatypeDefinition GetDatatypeDefinition()
        {
            return this.Type;
        }

        /// <summary>
        /// Sets the <see cref="DatatypeDefinitionEnumeration"/>
        /// </summary>
        /// <param name="datatypeDefinition">
        /// The instance of <see cref="DatatypeDefinitionEnumeration"/> that is to be set.
        /// </param>
        protected override void SetDatatypeDefinition(DatatypeDefinition datatypeDefinition)
        {
            if (datatypeDefinition.GetType() != typeof(DatatypeDefinitionEnumeration))
            {
                throw new ArgumentException("datatypeDefinition must of type DatatypeDefinitionEnumeration");
            }

            this.Type = (DatatypeDefinitionEnumeration)datatypeDefinition;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user of a requirement authoring tool can pick
        /// one or more of the values in the set of specified values as an enumeration attribute value
        /// </summary>
        /// <value>If set to true, this means that the user of a requirements authoring tool can pick one or more than one of the values in
        /// the set of specified values as an enumeration attribute value.
        /// </value>
        /// <value> 
        /// If set to false, this means that the user of a requirements authoring tool can pick exactly one of the values in the set of
        /// specified values as an enumeration attribute value.
        /// </value>
        public bool IsMultiValued { get; set; }

        /// <summary>
        /// Generates a <see cref="AttributeDefinitionEnumeration"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            if (reader.GetAttribute("MULTI-VALUED") == "true")
            {
                this.IsMultiValued = true;
            }

            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "ALTERNATIVE-ID":
                            var alternativeId = new AlternativeId(this);
                            alternativeId.ReadXml(reader);
                            break;
                        case "DATATYPE-DEFINITION-ENUMERATION-REF":
                            var reference = reader.ReadElementContentAsString();
                            var datatypeDefinition = (DatatypeDefinitionEnumeration)this.SpecType.ReqIFContent.DataTypes.SingleOrDefault(x => x.Identifier == reference);
                            this.Type = datatypeDefinition;
                            break;
                        case "ATTRIBUTE-VALUE-ENUMERATION":
                            this.DefaultValue = new AttributeValueEnumeration(this);
                            using (var valuesubtree = reader.ReadSubtree())
                            {
                                valuesubtree.MoveToContent();
                                this.DefaultValue.ReadXml(valuesubtree);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeDefinitionEnumeration"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Type"/> may not be null
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            if (this.Type == null)
            {
                throw new SerializationException($"The Type property of AttributeDefinitionEnumeration {this.Identifier}:{this.LongName} may not be null");
            }

            writer.WriteAttributeString("MULTI-VALUED", this.IsMultiValued ? "true" : "false");

            if (this.DefaultValue != null)
            {
                writer.WriteStartElement("DEFAULT-VALUE");
                    writer.WriteStartElement("ATTRIBUTE-VALUE-ENUMERATION");
                        writer.WriteStartElement("DEFINITION");
                            writer.WriteElementString("ATTRIBUTE-DEFINITION-ENUMERATION-REF", this.DefaultValue.Definition.Identifier);
                        writer.WriteEndElement();
                        writer.WriteStartElement("VALUES");
                            foreach (var defaultValue in this.DefaultValue.Values)
                            {
                                writer.WriteElementString("ENUM-VALUE-REF", defaultValue.Identifier);
                            }
                        writer.WriteEndElement();
                    writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteStartElement("TYPE");
            writer.WriteElementString("DATATYPE-DEFINITION-ENUMERATION-REF", this.Type.Identifier);
            writer.WriteEndElement();
        }
    }
}
