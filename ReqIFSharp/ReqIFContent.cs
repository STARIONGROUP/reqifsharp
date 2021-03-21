// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFContent.cs" company="RHEA System S.A.">
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
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="ReqIFContent"/> class represents the mandatory content of a ReqIF Exchange Document.
    /// </summary>
    public class ReqIFContent : IXmlSerializable
    {
        /// <summary>
        /// Backing field for the <see cref="DataTypes"/> property.
        /// </summary>
        private readonly List<DatatypeDefinition> dataTypes = new List<DatatypeDefinition>();

        /// <summary>
        /// Backing field for the <see cref="SpecTypes"/> property.
        /// </summary>
        private readonly List<SpecType> specTypes = new List<SpecType>();

        /// <summary>
        /// Backing field for the <see cref="SpecObjects"/> property.
        /// </summary>
        private readonly List<SpecObject> specObjects = new List<SpecObject>();

        /// <summary>
        /// Backing field for the <see cref="SpecRelations"/> property.
        /// </summary>
        private readonly List<SpecRelation> specRelations = new List<SpecRelation>();

        /// <summary>
        /// Backing field for the <see cref="Specifications"/> property.
        /// </summary>
        private readonly List<Specification> specifications = new List<Specification>();

        /// <summary>
        /// Backing field for the <see cref="SpecRelationGroups"/> property.
        /// </summary>
        private readonly List<RelationGroup> specRelationGroups = new List<RelationGroup>();

        /// <summary>
        /// Gets the <see cref="DatatypeDefinition"/>s
        /// </summary>        
        public List<DatatypeDefinition> DataTypes 
        {
                get
                {
                    return this.dataTypes;
                }
        }

        /// <summary>
        /// Gets the <see cref="SpecType"/>s
        /// </summary>        
        public List<SpecType> SpecTypes 
        {
            get
            {
                return this.specTypes;
            }
        }

        /// <summary>
        /// Gets the <see cref="SpecObject"/>
        /// </summary>
        public List<SpecObject> SpecObjects 
        {
            get
            {
                return this.specObjects;
            }
        }

        /// <summary>
        /// Gets the <see cref="SpecRelation"/>
        /// </summary>
        public List<SpecRelation> SpecRelations 
        {
            get
            {
                return this.specRelations;
            }
        }

        /// <summary>
        /// Gets the <see cref="Specification"/>
        /// </summary>
        public List<Specification> Specifications 
        {
            get
            {
                return this.specifications;
            } 
        }

        /// <summary>
        /// Gets the <see cref="RelationGroup"/>
        /// </summary>
        public List<RelationGroup> SpecRelationGroups 
        {
            get
            {
                return this.specRelationGroups;
            }
        }

        /// <summary>
        /// Gets or sets the document root element.
        /// </summary>
        public ReqIF DocumentRoot { get; set; }

        /// <summary>
        /// Generates a <see cref="ReqIFContent"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "DATATYPES":
                            using (var dataTypesSubtree = reader.ReadSubtree())
                            {
                                dataTypesSubtree.MoveToContent();
                                this.DeserializeDataTypes(dataTypesSubtree);
                            }
                            break;
                        case "SPEC-TYPES":
                            using (var specTypesSubtree = reader.ReadSubtree())
                            {
                                specTypesSubtree.MoveToContent();
                                this.DeserializeSpecTypes(specTypesSubtree);
                            }
                            break;
                        case "SPEC-OBJECTS":
                            using (var specObjectsSubtree = reader.ReadSubtree())
                            {
                                specObjectsSubtree.MoveToContent();
                                this.DeserializeSpectObjects(specObjectsSubtree);
                            }
                            break;
                        case "SPEC-RELATIONS":
                            using (var specRelationsSubtree = reader.ReadSubtree())
                            {
                                specRelationsSubtree.MoveToContent();
                                this.DeserializeSpecRelations(specRelationsSubtree);
                            }
                            break;
                        case "SPECIFICATIONS":
                            using (var specificationsSubtree = reader.ReadSubtree())
                            {
                                specificationsSubtree.MoveToContent();
                                this.DeserializeSpecifications(specificationsSubtree);
                            }
                            break;
                        case "SPEC-RELATION-GROUPS":
                            using (var specRelationGroupsSubtree = reader.ReadSubtree())
                            {
                                specRelationGroupsSubtree.MoveToContent();
                                this.DeserializeRelationGroups(specRelationGroupsSubtree);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Deserialize the <see cref="DatatypeDefinition"/>s contained by the <code>DATATYPES</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void DeserializeDataTypes(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName.StartsWith("DATATYPE-DEFINITION-"))
                {
                    var datatypeDefinition = ReqIfFactory.DatatypeDefinitionConstruct(reader.LocalName, this);
                    datatypeDefinition.ReadXml(reader);
                }
            }
        }

        /// <summary>
        /// Deserialize the <see cref="SpecType"/>s contained by the <code>SPEC-TYPES</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void DeserializeSpecTypes(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element)
                {
                    var xmlname = reader.LocalName;

                    if (xmlname == "SPEC-OBJECT-TYPE" || xmlname == "SPECIFICATION-TYPE"
                        || xmlname == "SPEC-RELATION-TYPE" || xmlname == "RELATION-GROUP-TYPE")
                    {
                        using (var subtree = reader.ReadSubtree())
                        {
                            subtree.MoveToContent();

                            var specType = ReqIfFactory.SpecTypeConstrcut(xmlname, this);
                            specType.ReadXml(subtree);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deserialize the <see cref="SpecObject"/>s contained by the <code>SPEC-OBJECTS</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void DeserializeSpectObjects(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SPEC-OBJECT")
                {
                    using (var subtree = reader.ReadSubtree())
                    {
                        subtree.MoveToContent();

                        var specObject = new SpecObject(this);
                        specObject.ReadXml(subtree);
                    }                                            
                }
            }
        }

        /// <summary>
        /// Deserialize the <see cref="SpecRelation"/>s contained by the <code>SPEC-RELATIONS</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void DeserializeSpecRelations(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SPEC-RELATION")
                {
                    using (var subtree = reader.ReadSubtree())
                    {
                        subtree.MoveToContent();

                        var specRelation = new SpecRelation(this);
                        specRelation.ReadXml(subtree);
                    }                        
                }
            }
        }

        /// <summary>
        /// Deserialize the <see cref="Specification"/>s contained by the <code>SPECIFICATIONS</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void DeserializeSpecifications(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SPECIFICATION")
                {
                    using (var subtree = reader.ReadSubtree())
                    {
                        subtree.MoveToContent();

                        var specification = new Specification(this);
                        specification.ReadXml(subtree);                        
                    }                        
                }
            }
        }

        /// <summary>
        /// Deserialize the <see cref="RelationGroup"/>s contained by the <code>SPEC-RELATION-GROUPS</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void DeserializeRelationGroups(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "RELATION-GROUP")
                {
                    using (var subtree = reader.ReadSubtree())
                    {
                        subtree.MoveToContent();

                        var relationGroup = new RelationGroup(this);
                        relationGroup.ReadXml(subtree);                        
                    }
                }
            }
        }
        
        /// <summary>
        /// Converts a <see cref="ReqIFContent"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public void WriteXml(XmlWriter writer)
        {
            this.WriteDataDefinitions(writer);
            this.WriteSpecTypes(writer);
            this.WriteSpecObjects(writer);
            this.WriteSpecRelations(writer);
            this.WriteSpecifications(writer);
            this.WriteRelationGroup(writer);
        }

        /// <summary>
        /// This method is reserved and should not be used.
        /// </summary>
        /// <returns>returns null</returns>
        /// <remarks>
        /// When implementing the IXmlSerializable interface, you should return null
        /// </remarks>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Write the <see cref="DatatypeDefinition"/>s
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteDataDefinitions(XmlWriter writer)
        {
            if (this.dataTypes.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("DATATYPES");

            foreach (var datatypeDefinition in this.dataTypes)
            {
                var xmlElementNAme = ReqIfFactory.XmlName(datatypeDefinition);
                writer.WriteStartElement(xmlElementNAme);
                datatypeDefinition.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the <see cref="SpecType"/>s
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteSpecTypes(XmlWriter writer)
        {
            if (this.specTypes.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("SPEC-TYPES");

            foreach (var specType in this.specTypes)
            {
                var xmlElementNAme = ReqIfFactory.XmlName(specType);
                writer.WriteStartElement(xmlElementNAme);
                specType.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the <see cref="SpecObject"/>s
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteSpecObjects(XmlWriter writer)
        {
            if (this.specObjects.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("SPEC-OBJECTS");

            foreach (var specObject in this.specObjects)
            {
                writer.WriteStartElement("SPEC-OBJECT");
                specObject.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the <see cref="SpecRelation"/>s
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteSpecRelations(XmlWriter writer)
        {
            if (this.specRelations.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("SPEC-RELATIONS");

            foreach (var specRelation in this.specRelations)
            {
                writer.WriteStartElement("SPEC-RELATION");
                specRelation.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write the <see cref="Specification"/>s
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteSpecifications(XmlWriter writer)
        {
            if (this.specifications.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("SPECIFICATIONS");

            foreach (var specification in this.specifications)
            {
                writer.WriteStartElement("SPECIFICATION");
                specification.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
        
        /// <summary>
        /// Write the <see cref="RelationGroup"/>s in the <see cref="SpecRelationGroups"/> property
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteRelationGroup(XmlWriter writer)
        {
            if (this.specRelationGroups.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("SPEC-RELATION-GROUPS");

            foreach (var relationGroup in this.specRelationGroups)
            {
                writer.WriteStartElement("RELATION-GROUP");
                relationGroup.WriteXml(writer);
                writer.WriteEndElement();    
            }

            writer.WriteEndElement();
        }
    }
}
