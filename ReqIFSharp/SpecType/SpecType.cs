// -------------------------------------------------------------------------------------------------
// <copyright file="SpecType.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Xml;
    
    /// <summary>
    /// Contains a set of attribute definitions. By using an instance of a subclass of <see cref="SpecType"/>, multiple elements can be
    /// associated with the same set of attribute definitions (attribute names, default values, data types, etc.).
    /// </summary>
    public abstract class SpecType : Identifiable
    {
        /// <summary>
        /// Backing field for the <see cref="SpecAttributes"/> property
        /// </summary>
        private List<AttributeDefinition> specAttributes = new List<AttributeDefinition>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecType"/> class.
        /// </summary>
        protected SpecType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecType"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        internal SpecType(ReqIFContent reqIfContent)
        {
            this.ReqIFContent = reqIfContent;
            reqIfContent.SpecTypes.Add(this);
        }

        /// <summary>
        /// Gets the set of attribute definitions.
        /// </summary>        
        public List<AttributeDefinition> SpecAttributes 
        {
            get
            {
                return this.specAttributes;
            }
        }

        /// <summary>
        /// Gets or sets the owning <see cref="ReqIFContent"/> object.
        /// </summary>
        public ReqIFContent ReqIFContent { get; set; }

        /// <summary>
        /// Generates a <see cref="SpecType"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "SPEC-ATTRIBUTES")
                {
                    var specAttributesSubTree = reader.ReadSubtree();
                    
                    while (specAttributesSubTree.Read())
                    {
                        if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName.StartsWith("ATTRIBUTE-DEFINITION-"))
                        {
                            this.CreateAttributeDefinition(reader, reader.LocalName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates <see cref="AttributeDefinition"/> and adds it to the <see cref="SpecAttributes"/> list.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="xmlname">
        /// The XML Element name of the <see cref="AttributeDefinition"/>
        /// </param>
        private void CreateAttributeDefinition(XmlReader reader, string xmlname)
        {
            var attributeDefinition = ReqIfFactory.AttributeDefinitionConstruct(xmlname, this);
            if (attributeDefinition == null)
            {
                return;
            }

            using (var attributeDefTree = reader.ReadSubtree())
            {
                attributeDefTree.MoveToContent();
                attributeDefinition.ReadXml(attributeDefTree);
            }
        }

        /// <summary>
        /// Converts a <see cref="SpecType"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            this.WriteSpecAttributes(writer);
        }

        /// <summary>
        /// Writes the <see cref="AttributeDefinition"/> objects from the <see cref="SpecAttributes"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteSpecAttributes(XmlWriter writer)
        {
            if (this.specAttributes.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("SPEC-ATTRIBUTES");

            foreach (var attributeDefinition in this.specAttributes)
            {
                var xmlname = ReqIfFactory.XmlName(attributeDefinition);
                writer.WriteStartElement(xmlname);
                attributeDefinition.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
