// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFContent.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The <see cref="ReqIFContent"/> class represents the mandatory content of a ReqIF Exchange Document.
    /// </summary>
    public class ReqIFContent
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<ReqIFContent> logger;

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
        /// Initializes a new instance of the <see cref="ReqIFContent"/> class
        /// </summary>
        public ReqIFContent()
        {
            this.logger = NullLogger<ReqIFContent>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIFContent"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal ReqIFContent(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            this.logger = loggerFactory == null ? NullLogger<ReqIFContent>.Instance : loggerFactory.CreateLogger<ReqIFContent>();
        }

        /// <summary>
        /// Gets the <see cref="DatatypeDefinition"/>s
        /// </summary>        
        public List<DatatypeDefinition> DataTypes => this.dataTypes;

        /// <summary>
        /// Gets the <see cref="SpecType"/>s
        /// </summary>        
        public List<SpecType> SpecTypes => this.specTypes;

        /// <summary>
        /// Gets the <see cref="SpecObject"/>
        /// </summary>
        public List<SpecObject> SpecObjects => this.specObjects;

        /// <summary>
        /// Gets the <see cref="SpecRelation"/>
        /// </summary>
        public List<SpecRelation> SpecRelations => this.specRelations;

        /// <summary>
        /// Gets the <see cref="Specification"/>
        /// </summary>
        public List<Specification> Specifications => this.specifications;

        /// <summary>
        /// Gets the <see cref="RelationGroup"/>
        /// </summary>
        public List<RelationGroup> SpecRelationGroups => this.specRelationGroups;

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
        internal void ReadXml(XmlReader reader)
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
                        default:
                            this.logger.LogWarning("The {LocalName} is not supported", reader.LocalName);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="ReqIFContent"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (await reader.MoveToContentAsync() == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "DATATYPES":
                            using (var dataTypesSubtree = reader.ReadSubtree())
                            {
                                await dataTypesSubtree.MoveToContentAsync();
                                await this.DeserializeDataTypesAsync(dataTypesSubtree, token);
                            }
                            break;
                        case "SPEC-TYPES":
                            using (var specTypesSubtree = reader.ReadSubtree())
                            {
                                await specTypesSubtree.MoveToContentAsync();
                                await this.DeserializeSpecTypesAsync(specTypesSubtree, token);
                            }
                            break;
                        case "SPEC-OBJECTS":
                            using (var specObjectsSubtree = reader.ReadSubtree())
                            {
                                await specObjectsSubtree.MoveToContentAsync();
                                await this.DeserializeSpectObjectsAsync(specObjectsSubtree, token);
                            }
                            break;
                        case "SPEC-RELATIONS":
                            using (var specRelationsSubtree = reader.ReadSubtree())
                            {
                                await specRelationsSubtree.MoveToContentAsync();
                                await this.DeserializeSpecRelationsAsync(specRelationsSubtree, token);
                            }
                            break;
                        case "SPECIFICATIONS":
                            using (var specificationsSubtree = reader.ReadSubtree())
                            {
                                await specificationsSubtree.MoveToContentAsync();
                                await this.DeserializeSpecificationsAsync(specificationsSubtree, token);
                            }
                            break;
                        case "SPEC-RELATION-GROUPS":
                            using (var specRelationGroupsSubtree = reader.ReadSubtree())
                            {
                                await specRelationGroupsSubtree.MoveToContentAsync();
                                await this.DeserializeRelationGroupsAsync(specRelationGroupsSubtree, token);
                            }
                            break;
                        default:
                            this.logger.LogWarning("The {LocalName} is not supported", reader.LocalName);
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
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName.StartsWith("DATATYPE-DEFINITION-", StringComparison.Ordinal))
                {
                    var datatypeDefinition = ReqIfFactory.DatatypeDefinitionConstruct(reader.LocalName, this, this.loggerFactory);
                    datatypeDefinition.ReadXml(reader);
                }
            }
        }

        /// <summary>
        /// Asynchronously deserialize the <see cref="DatatypeDefinition"/>s contained by the <code>DATATYPES</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task DeserializeDataTypesAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName.StartsWith("DATATYPE-DEFINITION-", StringComparison.Ordinal))
                {
                    var datatypeDefinition = ReqIfFactory.DatatypeDefinitionConstruct(reader.LocalName, this, this.loggerFactory);
                    await datatypeDefinition.ReadXmlAsync(reader, token);
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

                            var specType = ReqIfFactory.SpecTypeConstruct(xmlname, this, this.loggerFactory);
                            specType.ReadXml(subtree);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously deserialize the <see cref="SpecType"/>s contained by the <code>SPEC-TYPES</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task DeserializeSpecTypesAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (await reader.MoveToContentAsync() == XmlNodeType.Element)
                {
                    var xmlname = reader.LocalName;

                    if (xmlname == "SPEC-OBJECT-TYPE" || xmlname == "SPECIFICATION-TYPE"
                                                      || xmlname == "SPEC-RELATION-TYPE" || xmlname == "RELATION-GROUP-TYPE")
                    {
                        using (var subtree = reader.ReadSubtree())
                        {
                            await subtree.MoveToContentAsync();

                            var specType = ReqIfFactory.SpecTypeConstruct(xmlname, this, this.loggerFactory);
                            await specType.ReadXmlAsync(subtree, token);
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

                        var specObject = new SpecObject(this, this.loggerFactory);
                        specObject.ReadXml(subtree);
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously deserialize the <see cref="SpecObject"/>s contained by the <code>SPEC-OBJECTS</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task DeserializeSpectObjectsAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName == "SPEC-OBJECT")
                {
                    using (var subtree = reader.ReadSubtree())
                    {
                        await subtree.MoveToContentAsync();

                        var specObject = new SpecObject(this, this.loggerFactory);
                        await specObject.ReadXmlAsync(subtree, token);
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

                        var specRelation = new SpecRelation(this, this.loggerFactory);
                        specRelation.ReadXml(subtree);
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously deserialize the <see cref="SpecRelation"/>s contained by the <code>SPEC-RELATIONS</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task DeserializeSpecRelationsAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName == "SPEC-RELATION")
                {
                    using (var subtree = reader.ReadSubtree())
                    {
                        await subtree.MoveToContentAsync();

                        var specRelation = new SpecRelation(this, this.loggerFactory);
                        await specRelation.ReadXmlAsync(subtree, token);
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

                        var specification = new Specification(this, this.loggerFactory);
                        specification.ReadXml(subtree);                        
                    }                        
                }
            }
        }

        /// <summary>
        /// Asynchronously deserialize the <see cref="Specification"/>s contained by the <code>SPECIFICATIONS</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task DeserializeSpecificationsAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName == "SPECIFICATION")
                {
                    using (var subtree = reader.ReadSubtree())
                    {
                        await subtree.MoveToContentAsync();

                        var specification = new Specification(this, this.loggerFactory);
                        await specification.ReadXmlAsync(subtree, token);
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

                        var relationGroup = new RelationGroup(this, this.loggerFactory);
                        relationGroup.ReadXml(subtree);                        
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously deserialize the <see cref="RelationGroup"/>s contained by the <code>SPEC-RELATION-GROUPS</code> element.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task DeserializeRelationGroupsAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName == "RELATION-GROUP")
                {
                    using (var subtree = reader.ReadSubtree())
                    {
                        await subtree.MoveToContentAsync();

                        var relationGroup = new RelationGroup(this, this.loggerFactory);
                        await relationGroup.ReadXmlAsync(subtree, token);
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
        internal void WriteXml(XmlWriter writer)
        {
            this.WriteDataDefinitions(writer);
            this.WriteSpecTypes(writer);
            this.WriteSpecObjects(writer);
            this.WriteSpecRelations(writer);
            this.WriteSpecifications(writer);
            this.WriteRelationGroup(writer);
        }

        /// <summary>
        /// Asynchronously converts a <see cref="ReqIFContent"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            await this.WriteDataDefinitionsAsync(writer, token);
            await this.WriteSpecTypesAsync(writer, token);
            await this.WriteSpecObjectsAsync(writer, token);
            await this.WriteSpecRelationsAsync(writer, token);
            await this.WriteSpecificationsAsync(writer, token);
            await this.WriteRelationGroupAsync(writer, token);
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
        /// Asynchronously writes the <see cref="DatatypeDefinition"/>s
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteDataDefinitionsAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (this.dataTypes.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "DATATYPES", null);

            foreach (var datatypeDefinition in this.dataTypes)
            {
                var xmlElementNAme = ReqIfFactory.XmlName(datatypeDefinition);
                await writer.WriteStartElementAsync(null, xmlElementNAme, null);
                await datatypeDefinition.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
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
        /// Asynchronously writes the <see cref="SpecType"/>s
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteSpecTypesAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (this.specTypes.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "SPEC-TYPES", null);

            foreach (var specType in this.specTypes)
            {
                var xmlElementNAme = ReqIfFactory.XmlName(specType);
                await writer.WriteStartElementAsync(null, xmlElementNAme, null);
                await specType.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
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
        /// Asynchronously writes the <see cref="SpecObject"/>s
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteSpecObjectsAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (this.specObjects.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "SPEC-OBJECTS", null);

            foreach (var specObject in this.specObjects)
            {
                await writer.WriteStartElementAsync(null, "SPEC-OBJECT", null);
                await specObject.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
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
        /// Asynchronously writes the <see cref="SpecRelation"/>s
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteSpecRelationsAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (this.specRelations.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "SPEC-RELATIONS", null);

            foreach (var specRelation in this.specRelations)
            {
                await writer.WriteStartElementAsync(null, "SPEC-RELATION", null);
                await specRelation.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
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
        /// Asynchronously writes the <see cref="Specification"/>s
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteSpecificationsAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (this.specifications.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "SPECIFICATIONS", null);

            foreach (var specification in this.specifications)
            {
                await writer.WriteStartElementAsync(null, "SPECIFICATION", null);
                await specification.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
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

        /// <summary>
        /// Asynchronously writes the <see cref="RelationGroup"/>s in the <see cref="SpecRelationGroups"/> property
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteRelationGroupAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (this.specRelationGroups.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "SPEC-RELATION-GROUPS", null);

            foreach (var relationGroup in this.specRelationGroups)
            {
                await writer.WriteStartElementAsync(null, "RELATION-GROUP", null);
                await relationGroup.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
        }
    }
}
