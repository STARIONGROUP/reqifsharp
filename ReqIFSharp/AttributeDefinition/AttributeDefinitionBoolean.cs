// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionBoolean.cs" company="RHEA System S.A.">
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
    /// The purpose of the <see cref="AttributeDefinitionBoolean"/> class is to define a <see cref="bool"/> attribute.
    /// </summary>
    /// <remarks>
    /// An <see cref="AttributeDefinitionBoolean"/> element relates an <see cref="AttributeValueBoolean"/> element to a
    /// <see cref="DatatypeDefinitionBoolean"/> element via its <see cref="Type"/> attribute.
    /// An <see cref="AttributeDefinitionBoolean"/> element MAY contain a default value that represents the value that is used as an attribute
    /// value if no attribute value is supplied by the user of the requirements authoring tool
    /// </remarks>
    public class AttributeDefinitionBoolean : AttributeDefinitionSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionBoolean"/> class.
        /// </summary>
        public AttributeDefinitionBoolean()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionBoolean"/> class.
        /// </summary>
        /// <param name="specType">
        /// The owning <see cref="SpecType"/>.
        /// </param>
        internal AttributeDefinitionBoolean(SpecType specType)
            : base(specType)
        {
        }

        /// <summary>
        /// Gets or sets the owned default value that is used if no attribute value is supplied 
        /// by the user of the requirements authoring tool.
        /// </summary>        
        public AttributeValueBoolean DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>                
        public DatatypeDefinitionBoolean Type { get; set; }

        /// <summary>
        /// Gets the <see cref="DatatypeDefinitionBoolean"/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="DatatypeDefinitionBoolean"/>
        /// </returns>
        protected override DatatypeDefinition GetDatatypeDefinition()
        {
            return this.Type;
        }

        /// <summary>
        /// Sets the <see cref="DatatypeDefinitionBoolean"/>
        /// </summary>
        /// <param name="datatypeDefinition">
        /// The instance of <see cref="DatatypeDefinitionBoolean"/> that is to be set.
        /// </param>
        protected override void SetDatatypeDefinition(DatatypeDefinition datatypeDefinition)
        {
            if (datatypeDefinition.GetType() != typeof(DatatypeDefinitionBoolean))
            {
                throw new ArgumentException("datatypeDefinition must of type DatatypeDefinitionBoolean");
            }

            this.Type = (DatatypeDefinitionBoolean)datatypeDefinition;
        }

        /// <summary>
        /// Generates a <see cref="AttributeDefinitionBoolean"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

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
                        case "DATATYPE-DEFINITION-BOOLEAN-REF":
                            var reference = reader.ReadElementContentAsString();
                            var datatypeDefinition = (DatatypeDefinitionBoolean)this.SpecType.ReqIFContent.DataTypes.SingleOrDefault(x => x.Identifier == reference);
                            this.Type = datatypeDefinition;
                            break;
                        case "ATTRIBUTE-VALUE-BOOLEAN":
                            this.DefaultValue = new AttributeValueBoolean(this);
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
        /// Converts a <see cref="AttributeDefinitionBoolean"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Type"/> may not be null
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            if (this.Type == null)
            {
                throw new SerializationException($"The Type property of AttributeDefinitionBoolean {this.Identifier}:{this.LongName} may not be null");
            }

            base.WriteXml(writer);

            if (this.DefaultValue != null)
            {
                writer.WriteStartElement("DEFAULT-VALUE");
                    writer.WriteStartElement("ATTRIBUTE-VALUE-BOOLEAN");
                    writer.WriteAttributeString("THE-VALUE", this.DefaultValue.TheValue ? "true" : "false");
                        writer.WriteStartElement("DEFINITION");
                            writer.WriteElementString("ATTRIBUTE-DEFINITION-BOOLEAN-REF", this.DefaultValue.Definition.Identifier);
                        writer.WriteEndElement();
                    writer.WriteEndElement();
                writer.WriteEndElement();
            }

            writer.WriteStartElement("TYPE");
            writer.WriteElementString("DATATYPE-DEFINITION-BOOLEAN-REF", this.Type.Identifier);
            writer.WriteEndElement();
        }
    }
}
