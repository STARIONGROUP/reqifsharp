// -------------------------------------------------------------------------------------------------
// <copyright file="RelationGroup.cs" company="RHEA System S.A.">
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
    using System.Threading.Tasks;
    using System.Xml;
    
    /// <summary>
    /// The <see cref="RelationGroup"/> class represents a group of relations.
    /// </summary>
    /// <remarks>
    /// The <see cref="RelationGroup"/> class represents a group of relations between a source <see cref="Specification"/> and a target <see cref="Specification"/>.
    /// </remarks>
    /// <example>
    /// a <see cref="RelationGroup"/> instance may represent a set of relations between a customer requirements <see cref="Specification"/> and a system requirements <see cref="Specification"/>.
    /// </example>
    public class RelationGroup : SpecElementWithAttributes
    {
        /// <summary>
        /// Backing field for the <see cref="SpecRelations"/> property
        /// </summary>
        private readonly List<SpecRelation> specRelations = new List<SpecRelation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationGroup"/> class.
        /// </summary>
        public RelationGroup()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationGroup"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="reqIfContent"/>
        /// </param>
        internal RelationGroup(ReqIFContent reqIfContent) : base(reqIfContent)
        {
            this.CoreContent = reqIfContent;
            this.CoreContent.SpecRelationGroups.Add(this);
        }

        /// <summary>
        /// Gets or sets the <see cref="RelationGroupType"/> reference
        /// </summary>
        public RelationGroupType Type { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Specification"/> that contains <see cref="SpecObject"/> instances that are source objects of the relations.
        /// </summary>
        public Specification SourceSpecification { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Specification"/> that contains <see cref="SpecObject"/> instances that are target objects of the relations.
        /// </summary>
        public Specification TargetSpecification { get; set; }

        /// <summary>
        /// Gets the grouped <see cref="SpecRelation"/>s
        /// </summary>
        public List<SpecRelation> SpecRelations 
        {
            get
            {
                return this.specRelations;
            }
        }

        /// <summary>
        /// Gets or sets the owning <see cref="ReqIFContent"/> element.
        /// </summary>
        public ReqIFContent CoreContent { get; set; }

        /// <summary>
        /// Gets the <see cref="SpecType"/>class
        /// </summary>
        /// <returns>
        /// an instance of <see cref="SpecType"/>
        /// </returns>
        protected override SpecType GetSpecType()
        {
            return this.Type;
        }

        /// <summary>
        /// Sets the <see cref="SpecType"/> 
        /// </summary>
        /// <param name="specType">
        /// The <see cref="SpecType"/> to set.
        /// </param>
        protected override void SetSpecType(SpecType specType)
        {
            if (specType.GetType() != typeof(RelationGroupType))
            {
                throw new ArgumentException("specType must of type RelationGroupType");
            }

            this.Type = (RelationGroupType)specType;
        }

        /// <summary>
        /// Reads the concrete <see cref="SpecElementWithAttributes"/> elements
        /// </summary>
        /// <param name="reader">The current <see cref="XmlReader"/></param>
        /// <remarks>
        /// In this case, read the source, target and spec-relations
        /// </remarks>
        protected override void ReadObjectSpecificElements(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "SOURCE-SPECIFICATION":
                        if (reader.ReadToDescendant("SPECIFICATION-REF"))
                        {
                            var reference = reader.ReadElementContentAsString();
                            var specification = this.CoreContent.Specifications.SingleOrDefault(x => x.Identifier == reference);
                            this.SourceSpecification = specification
                                                       ?? new Specification
                                                       {
                                                           Identifier = reference,
                                                           Description = "This specification was not found in the source file."
                                                       };
                        }
                        break;
                    case "TARGET-SPECIFICATION":
                        if (reader.ReadToDescendant("SPECIFICATION-REF"))
                        {
                            var reference = reader.ReadElementContentAsString();
                            var specification = this.CoreContent.Specifications.SingleOrDefault(x => x.Identifier == reference);
                            this.TargetSpecification = specification
                                                       ?? new Specification
                                                       {
                                                           Identifier = reference,
                                                           Description = "This specification was not found in the source file."
                                                       };
                        }
                        break;
                    case "SPEC-RELATIONS":
                        reader.ReadStartElement();

                        this.DeserializeSpecRelations(reader);

                        reader.ReadEndElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Asynchronously reads the concrete <see cref="SpecElementWithAttributes"/> elements
        /// </summary>
        /// <param name="reader">The current <see cref="XmlReader"/></param>
        /// <remarks>
        /// In this case, read the source, target and spec-relations
        /// </remarks>
        protected override async Task ReadObjectSpecificElementsAsync(XmlReader reader)
        {
            if (await reader.MoveToContentAsync() == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "SOURCE-SPECIFICATION":
                        if (reader.ReadToDescendant("SPECIFICATION-REF"))
                        {
                            var reference = await reader.ReadElementContentAsStringAsync();
                            var specification = this.CoreContent.Specifications.SingleOrDefault(x => x.Identifier == reference);
                            this.SourceSpecification = specification
                                                       ?? new Specification
                                                       {
                                                           Identifier = reference,
                                                           Description = "This specification was not found in the source file."
                                                       };
                        }
                        break;
                    case "TARGET-SPECIFICATION":
                        if (reader.ReadToDescendant("SPECIFICATION-REF"))
                        {
                            var reference = await reader.ReadElementContentAsStringAsync();
                            var specification = this.CoreContent.Specifications.SingleOrDefault(x => x.Identifier == reference);
                            this.TargetSpecification = specification
                                                       ?? new Specification
                                                       {
                                                           Identifier = reference,
                                                           Description = "This specification was not found in the source file."
                                                       };
                        }
                        break;
                    case "SPEC-RELATIONS":
                        reader.ReadStartElement();

                        await this.DeserializeSpecRelationsAsync(reader);

                        reader.ReadEndElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="RelationGroup"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected override void ReadSpecType(XmlReader reader)
        {
            if (reader.ReadToDescendant("RELATION-GROUP-TYPE-REF"))
            {
                var reference = reader.ReadElementContentAsString();
                var specType = this.ReqIFContent.SpecTypes.SingleOrDefault(x => x.Identifier == reference);
                this.Type = (RelationGroupType)specType;
            }
        }

        /// <summary>
        /// Asynchronously reads the <see cref="SpecType"/> which is specific to the <see cref="RelationGroup"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected override async Task ReadSpecTypeAsync(XmlReader reader)
        {
            if (reader.ReadToDescendant("RELATION-GROUP-TYPE-REF"))
            {
                var reference = await reader.ReadElementContentAsStringAsync();
                var specType = this.ReqIFContent.SpecTypes.SingleOrDefault(x => x.Identifier == reference);
                this.Type = (RelationGroupType)specType;
            }
        }

        /// <summary>
        /// Read the Hierarchy
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected override void ReadHierarchy(XmlReader reader)
        {
            throw new InvalidOperationException("RelationGroup does not have a hierarchy");
        }

        /// <summary>
        /// Asynchronously reads the Hierarchy
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected override Task ReadHierarchyAsync(XmlReader reader)
        {
            throw new InvalidOperationException("RelationGroup does not have a hierarchy");
        }

        /// <summary>
        /// Deserialize the <see cref="SpecRelation"/>s contained by the <code>SPEC-RELATIONS</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void DeserializeSpecRelations(XmlReader reader)
        {
            while (reader.Read() && reader.MoveToContent() == XmlNodeType.Element && reader.LocalName.StartsWith("SPEC-RELATION-REF"))
            {
                var reference = reader.ReadElementContentAsString();
                var specRelation = this.CoreContent.SpecRelations.SingleOrDefault(x => x.Identifier == reference);
                if (specRelation != null)
                {
                    this.specRelations.Add(specRelation);
                }
            }
        }

        /// <summary>
        /// Asynchronously deserialize the <see cref="SpecRelation"/>s contained by the <code>SPEC-RELATIONS</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private async Task DeserializeSpecRelationsAsync(XmlReader reader)
        {
            while (await reader.ReadAsync() && await reader.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName.StartsWith("SPEC-RELATION-REF"))
            {
                var reference = await reader.ReadElementContentAsStringAsync();
                var specRelation = this.CoreContent.SpecRelations.SingleOrDefault(x => x.Identifier == reference);
                if (specRelation != null)
                {
                    this.specRelations.Add(specRelation);
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="RelationGroup"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The Object property may not be null.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            if (this.Type == null)
            {
                throw new SerializationException($"The Type property of RelationGroup {this.Identifier}:{this.LongName} may not be null");
            }

            if (this.SourceSpecification == null)
            {
                throw new SerializationException($"The SourceSpecification property of RelationGroup {this.Identifier}:{this.LongName} may not be null");
            }

            if (this.TargetSpecification == null)
            {
                throw new SerializationException($"The TargetSpecification property of RelationGroup {this.Identifier}:{this.LongName} may not be null");
            }

            base.WriteXml(writer);

            this.WriteType(writer);
            this.WriteSourceSpecification(writer);
            this.WriteTargetSpecification(writer);
            this.WriteSpecRelations(writer);
        }

        /// <summary>
        /// Asynchronously converts a <see cref="RelationGroup"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The Object property may not be null.
        /// </exception>
        public override async Task WriteXmlAsync(XmlWriter writer)
        {
            if (this.Type == null)
            {
                throw new SerializationException($"The Type property of RelationGroup {this.Identifier}:{this.LongName} may not be null");
            }

            if (this.SourceSpecification == null)
            {
                throw new SerializationException($"The SourceSpecification property of RelationGroup {this.Identifier}:{this.LongName} may not be null");
            }

            if (this.TargetSpecification == null)
            {
                throw new SerializationException($"The TargetSpecification property of RelationGroup {this.Identifier}:{this.LongName} may not be null");
            }

            await base.WriteXmlAsync(writer);

            await this.WriteTypeAsync(writer);
            await this.WriteSourceSpecificationAsync(writer);
            await this.WriteTargetSpecificationAsync(writer);
            await this.WriteSpecRelationsAsync(writer);
        }

        /// <summary>
        /// Writes the <see cref="Type"/>
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteType(XmlWriter writer)
        {
            writer.WriteStartElement("TYPE");
            writer.WriteElementString("RELATION-GROUP-TYPE-REF", this.Type.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="Type"/>
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private async Task WriteTypeAsync(XmlWriter writer)
        {
            await writer.WriteStartElementAsync(null,"TYPE", null);
            await writer.WriteElementStringAsync(null, "RELATION-GROUP-TYPE-REF", null, this.Type.Identifier);
            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// Writes the <see cref="SourceSpecification"/>
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteSourceSpecification(XmlWriter writer)
        {
            writer.WriteStartElement("SOURCE-SPECIFICATION");
            writer.WriteElementString("SPECIFICATION-REF", this.SourceSpecification.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="SourceSpecification"/>
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private async Task WriteSourceSpecificationAsync(XmlWriter writer)
        {
            await writer.WriteStartElementAsync(null, "SOURCE-SPECIFICATION", null);
            await writer.WriteElementStringAsync(null, "SPECIFICATION-REF",null, this.SourceSpecification.Identifier);
            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// Writes the <see cref="SourceSpecification"/>
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteTargetSpecification(XmlWriter writer)
        {
            writer.WriteStartElement("TARGET-SPECIFICATION");
            writer.WriteElementString("SPECIFICATION-REF", this.TargetSpecification.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="SourceSpecification"/>
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private async Task WriteTargetSpecificationAsync(XmlWriter writer)
        {
            await writer.WriteStartElementAsync(null, "TARGET-SPECIFICATION", null);
            await writer.WriteElementStringAsync(null, "SPECIFICATION-REF",null, this.TargetSpecification.Identifier);
            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// Writes the <see cref="SpecRelation"/> in the <see cref="SpecRelations"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteSpecRelations(XmlWriter writer)
        {
            if (this.SpecRelations.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("SPEC-RELATIONS");
            
            foreach (var specRelation in this.SpecRelations)
            {
                writer.WriteElementString("SPEC-RELATION-REF", specRelation.Identifier);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="SpecRelation"/> in the <see cref="SpecRelations"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private async Task WriteSpecRelationsAsync(XmlWriter writer)
        {
            if (this.SpecRelations.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "SPEC-RELATIONS", null);

            foreach (var specRelation in this.SpecRelations)
            {
                await writer.WriteElementStringAsync(null, "SPEC-RELATION-REF", null, specRelation.Identifier);
            }

            await writer.WriteEndElementAsync();
        }
    }
}
