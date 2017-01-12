// -------------------------------------------------------------------------------------------------
// <copyright file="SpecElementWithAttributes.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Xml;
    
    /// <summary>
    /// An abstract super class for elements that can own attributes.
    /// </summary>
    /// <remarks>
    /// Any element that can own attributes, like a requirement, a specification, or a relation between requirements needs to be
    /// an instance of a concrete subclass of this abstract class.
    /// While this class aggregates the values of the attributes, the association to the attributes’ types that define the acceptable
    /// values for the attributes is realized by concrete sub classes of this class.
    /// </remarks>
    public abstract class SpecElementWithAttributes : Identifiable
    {
        /// <summary>
        /// Backing field for the <see cref="Values"/> property.
        /// </summary>
        private List<AttributeValue> values = new List<AttributeValue>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecElementWithAttributes"/> class.
        /// </summary>
        protected SpecElementWithAttributes()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecElementWithAttributes"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="reqIfContent"/>
        /// </param>
        internal SpecElementWithAttributes(ReqIFContent reqIfContent)
        {
            this.ReqIfContent = reqIfContent;
        }

        /// <summary>
        /// Gets the values of the attributes owned by the element.
        /// </summary>
        public List<AttributeValue> Values
        {
            get
            {
                return this.values;
            }
        }

        /// <summary>
        /// Gets or sets the owning <see cref="ReqIFContent"/>
        /// </summary>
        public ReqIFContent ReqIfContent { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SpecType"/>
        /// </summary>
        public SpecType SpecType 
        {
            get
            {
                return this.GetSpecType();
            }

            set
            {
                this.SetSpecType(value);
            }
        }

        /// <summary>
        /// Gets the <see cref="SpecType"/> from the sub class
        /// </summary>
        /// <returns>
        /// an instance of <see cref="SpecType"/>
        /// </returns>
        protected abstract SpecType GetSpecType();

        /// <summary>
        /// Sets the <see cref="SpecType"/> to the sub class
        /// </summary>
        /// <param name="specType">
        /// The <see cref="SpecType"/> to set.
        /// </param>
        protected abstract void SetSpecType(SpecType specType);

        /// <summary>
        /// Generates a <see cref="SpecElementWithAttributes"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            
            using (var specElementWithAttributesReader = reader.ReadSubtree())
            {
                while (specElementWithAttributesReader.Read())
                {
                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ALTERNATIVE-ID")
                    {
                        var alternativeId = new AlternativeId(this);
                        alternativeId.ReadXml(specElementWithAttributesReader);
                    }

                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "TYPE")
                    {
                        using (var subtree = specElementWithAttributesReader.ReadSubtree())
                        {
                            subtree.MoveToContent();
                            this.ReadSpecType(subtree);
                        }
                    }

                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "CHILDREN")
                    {
                        using (var subtree = specElementWithAttributesReader.ReadSubtree())
                        {
                            subtree.MoveToContent();
                            this.ReadHierarchy(subtree);
                        }
                    }

                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element && specElementWithAttributesReader.LocalName == "ATTRIBUTE-VALUE-BOOLEAN")
                    {
                        using (var subtree = specElementWithAttributesReader.ReadSubtree())
                        {
                            subtree.MoveToContent();
                            var attributeValueBoolean = new AttributeValueBoolean(this);
                            attributeValueBoolean.ReadXml(subtree);
                        }
                    }

                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element && specElementWithAttributesReader.LocalName == "ATTRIBUTE-VALUE-DATE")
                    {
                        using (var subtree = specElementWithAttributesReader.ReadSubtree())
                        {
                            subtree.MoveToContent();
                            var attributeValueDate = new AttributeValueDate(this);
                            attributeValueDate.ReadXml(subtree);
                        }
                    }

                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element && specElementWithAttributesReader.LocalName == "ATTRIBUTE-VALUE-ENUMERATION")
                    {
                        using (var subtree = specElementWithAttributesReader.ReadSubtree())
                        {
                            subtree.MoveToContent();
                            var attributeValueEnumeration = new AttributeValueEnumeration(this);
                            attributeValueEnumeration.ReadXml(subtree);
                        }
                    }

                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element && specElementWithAttributesReader.LocalName == "ATTRIBUTE-VALUE-INTEGER")
                    {
                        using (var subtree = specElementWithAttributesReader.ReadSubtree())
                        {
                            subtree.MoveToContent();
                            var attributeValueInteger = new AttributeValueInteger(this);
                            attributeValueInteger.ReadXml(subtree);
                        }
                    }

                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element && specElementWithAttributesReader.LocalName == "ATTRIBUTE-VALUE-REAL")
                    {
                        using (var subtree = specElementWithAttributesReader.ReadSubtree())
                        {
                            subtree.MoveToContent();
                            var attributeValueReal = new AttributeValueReal(this);
                            attributeValueReal.ReadXml(subtree);
                        }
                    }

                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element && specElementWithAttributesReader.LocalName == "ATTRIBUTE-VALUE-STRING")
                    {
                        using (var subtree = specElementWithAttributesReader.ReadSubtree())
                        {
                            subtree.MoveToContent();
                            var attributeValueString = new AttributeValueString(this);
                            attributeValueString.ReadXml(subtree);
                        }
                    }

                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element && specElementWithAttributesReader.LocalName == "ATTRIBUTE-VALUE-XHTML")
                    {
                        using (var subtree = specElementWithAttributesReader.ReadSubtree())
                        {
                            subtree.MoveToContent();
                            var attributeValueXhtml = new AttributeValueXHTML(this);
                            attributeValueXhtml.ReadXml(subtree);
                        }
                    }

                    this.ReadObjectSpecificElements(specElementWithAttributesReader);
                }
            }
        }

        /// <summary>
        /// Reads the concrete <see cref="SpecElementWithAttributes"/> elements
        /// </summary>
        /// <param name="reader">The current <see cref="XmlReader"/></param>
        protected virtual void ReadObjectSpecificElements(XmlReader reader)
        {
        }

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="SpecElementWithAttributes"/> sub class
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected abstract void ReadSpecType(XmlReader reader);

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="SpecElementWithAttributes"/> sub class
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected abstract void ReadHierarchy(XmlReader reader);

        /// <summary>
        /// Converts a <see cref="SpecElementWithAttributes"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            this.WriteValues(writer);
        }

        /// <summary>
        /// Writes the <see cref="AttributeValue"/> objects from the <see cref="Values"/> list.
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

            foreach (var attributeValue in this.values)
            {
                if (attributeValue is AttributeValueBoolean)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-BOOLEAN");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueDate)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-DATE");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueEnumeration)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-ENUMERATION");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueInteger)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-INTEGER");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueReal)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-REAL");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueString)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-STRING");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeValue is AttributeValueXHTML)
                {
                    writer.WriteStartElement("ATTRIBUTE-VALUE-XHTML");
                    attributeValue.WriteXml(writer);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }
    }
}