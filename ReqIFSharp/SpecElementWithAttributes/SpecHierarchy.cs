// -------------------------------------------------------------------------------------------------
// <copyright file="SpecHierarchy.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The <see cref="SpecHierarchy"/> class represents a node in a hierarchically structured requirements specification.
    /// </summary>
    /// <remarks>
    /// The nodes of the tree that constitutes the structure of <see cref="SpecObject"/>s. 
    /// The tree is created by references of <see cref="SpecHierarchy"/> instances to other <see cref="SpecHierarchy"/> instances.
    /// Each node has additionally a reference to a <see cref="SpecObject"/> resulting in a hierarchical structure of <see cref="SpecObject"/>s
    /// </remarks>
    public class SpecHierarchy : AccessControlledElement
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<SpecHierarchy> logger;

        /// <summary>
        /// Backing field for the <see cref="Children"/> property.
        /// </summary>
        private readonly List<SpecHierarchy> children = new List<SpecHierarchy>();

        /// <summary>
        /// Backing field for the <see cref="EditableAtts"/> property.
        /// </summary>
        private readonly List<AttributeDefinition> editableAtts = new List<AttributeDefinition>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecHierarchy"/> class.
        /// </summary>
        public SpecHierarchy()
        {
            this.logger = NullLogger<SpecHierarchy>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecHierarchy"/> class.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="reqIfContent">
        /// The requirement core information content.
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        protected internal SpecHierarchy(Specification root, ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<SpecHierarchy>.Instance : this.loggerFactory.CreateLogger<SpecHierarchy>();

            this.Initialize(null, root, reqIfContent);
            this.Root.Children.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecHierarchy"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="reqIfContent">
        /// The requirement core information content.
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal SpecHierarchy(SpecHierarchy container, Specification root, ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<SpecHierarchy>.Instance : this.loggerFactory.CreateLogger<SpecHierarchy>();

            this.Initialize(container, root, reqIfContent);
            this.Container.Children.Add(this);
        }

        /// <summary>
        /// Gets or sets the owning <see cref="ReqIFContent"/>
        /// </summary>
        public ReqIFContent ReqIfContent { get; set; }

        /// <summary>
        /// Gets the Down links to next level of owned SpecHierarchy.
        /// </summary>
        public List<SpecHierarchy> Children => this.children;

        /// <summary>
        /// Gets the attributes whose values are editable for the <see cref="SpecHierarchy"/> by a tool user
        /// </summary>
        public List<AttributeDefinition> EditableAtts => this.editableAtts;

        /// <summary>
        /// Gets or sets the reference to the associated <see cref="SpecObject"/>
        /// </summary>
        public SpecObject Object { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the table is internal
        /// </summary>
        /// <remarks>
        /// Some requirements authoring tools enable the user to use tables as part of a requirement’s content, where parts of
        /// the table represent requirements as well. If that is the case, this attribute needs to be set to true for the root node of
        /// the table hierarchy and all descendant SpecHierarchy nodes.
        /// The root node of the table hierarchy is related to the SpecObject element that is the root of the table by the object
        /// association.
        /// </remarks>
        public bool IsTableInternal { get; set; }

        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        public Specification Root { get; set; }

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        public SpecHierarchy Container { get; set; }

        /// <summary>
        /// Generates a <see cref="SpecHierarchy"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            using (var speccHierarchySubtree = reader.ReadSubtree())
            {
                while (speccHierarchySubtree.Read())
                {
                    if (speccHierarchySubtree.MoveToContent() == XmlNodeType.Element)
                    {
                        switch (speccHierarchySubtree.LocalName)
                        {
                            case "ALTERNATIVE-ID":
                                this.ReadAlternativeId(speccHierarchySubtree);
                                break;
                            case "OBJECT":
                                using (var subtree = speccHierarchySubtree.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    this.DeserializeObject(subtree);
                                }
                                break;
                            case "CHILDREN":
                                using (var subtree = speccHierarchySubtree.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    this.DeserializeSpecHierarchy(subtree);
                                }
                                break;
                            default:
                                this.logger.LogWarning("The {LocalName} is not supported", speccHierarchySubtree.LocalName);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="SpecHierarchy"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            base.ReadXml(reader);

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
                        case "ALTERNATIVE-ID":
                            await this.ReadAlternativeIdAsync(reader, token);
                            break;
                        case "OBJECT":
                            using (var subtree = reader.ReadSubtree())
                            {
                                await subtree.MoveToContentAsync();
                                await this.DeserializeObjectAsync(subtree, token);
                            }
                            break;
                        case "CHILDREN":
                            using (var subtree = reader.ReadSubtree())
                            {
                                await subtree.MoveToContentAsync();
                                await this.DeserializeSpecHierarchyAsync(subtree, token);
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
        /// Converts a <see cref="AttributeDefinition"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The Object property may not be null.
        /// </exception>
        internal override void WriteXml(XmlWriter writer)
        {
            if (this.Object == null)
            {
                throw new SerializationException($"The Object property of SpecHierarchy {this.Identifier}:{this.LongName} may not be null");
            }

            base.WriteXml(writer);

            if (this.IsTableInternal)
            {
                writer.WriteAttributeString("IS-TABLE-INTERNAL", "true");
            }

            this.WriteObject(writer);

            this.WriteEditableAtts(writer);

            this.WriteChildren(writer);
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeDefinition"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The Object property may not be null.
        /// </exception>
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            if (this.Object == null)
            {
                throw new SerializationException($"The Object property of SpecHierarchy {this.Identifier}:{this.LongName} may not be null");
            }

            await base.WriteXmlAsync(writer, token);

            if (this.IsTableInternal)
            {
                await writer.WriteAttributeStringAsync(null, "IS-TABLE-INTERNAL",null, "true");
            }

            await this.WriteObjectAsync(writer, token);

            await this.WriteEditableAttsAsync(writer, token);

            await this.WriteChildrenAsync(writer, token);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <param name="reqIfContent">
        /// The requirement core information content.
        /// </param>
        private void Initialize(SpecHierarchy container, Specification root, ReqIFContent reqIfContent)
        {
            this.Container = container;
            this.Root = root;
            this.ReqIfContent = reqIfContent;
        }

        /// <summary>
        /// The deserialize object.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        private void DeserializeObject(XmlReader reader)
        {
            if (reader.ReadToDescendant("SPEC-OBJECT-REF"))
            {
                var reference = reader.ReadElementContentAsString();
                var specObject = this.ReqIfContent.SpecObjects.SingleOrDefault(x => x.Identifier == reference);
                this.Object = specObject;

                if (specObject == null)
                {
                    this.logger.LogTrace("The SpecObject:{Reference} could not be found and has been set to null on SpecHierarchy:{Identifier}", reference, Identifier);
                }
            }
        }

        /// <summary>
        /// The deserialize object.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task DeserializeObjectAsync(XmlReader reader, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (reader.ReadToDescendant("SPEC-OBJECT-REF"))
            {
                var reference = await reader.ReadElementContentAsStringAsync();
                var specObject = this.ReqIfContent.SpecObjects.SingleOrDefault(x => x.Identifier == reference);
                this.Object = specObject;

                if (specObject == null)
                {
                    this.logger.LogTrace("The SpecObject:{Reference} could not be found and has been set to null on SpecHierarchy:{Identifier}", reference, Identifier);
                }
            }
        }

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void DeserializeSpecHierarchy(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SPEC-HIERARCHY")
                {
                    if (bool.TryParse(reader.GetAttribute("IS-TABLE-INTERNAL"), out var isTableInternal))
                    {
                        this.IsTableInternal = isTableInternal;
                    }

                    using (var subtree = reader.ReadSubtree())
                    {
                        subtree.MoveToContent();
                        var specHierarchy = new SpecHierarchy(this, this.Root, this.ReqIfContent, this.loggerFactory);
                        specHierarchy.ReadXml(subtree);
                    }
                }
            }
        }

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task DeserializeSpecHierarchyAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName == "SPEC-HIERARCHY")
                {
                    if (bool.TryParse(reader.GetAttribute("IS-TABLE-INTERNAL"), out var isTableInternal))
                    {
                        this.IsTableInternal = isTableInternal;
                    }

                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    using (var subtree = reader.ReadSubtree())
                    {
                        await subtree.MoveToContentAsync();
                        var specHierarchy = new SpecHierarchy(this, this.Root, this.ReqIfContent, this.loggerFactory);
                        await specHierarchy.ReadXmlAsync(subtree, token);
                    }
                }
            }
        }

        /// <summary>
        /// Writes the <see cref="Object"/>.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteObject(XmlWriter writer)
        {
            writer.WriteStartElement("OBJECT");
            writer.WriteElementString("SPEC-OBJECT-REF", this.Object.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="Object"/>.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteObjectAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            await writer.WriteStartElementAsync(null, "OBJECT", null);
            await writer.WriteElementStringAsync(null,"SPEC-OBJECT-REF", null, this.Object.Identifier);
            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// Writes the <see cref="AttributeDefinition"/> objects from the <see cref="EditableAtts"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteEditableAtts(XmlWriter writer)
        {
            if (this.editableAtts.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("EDITABLE-ATTS");

            foreach (var attributeDefinition in this.editableAtts)
            {
                if (attributeDefinition is AttributeDefinitionBoolean)
                {
                    writer.WriteStartElement("ATTRIBUTE-DEFINITION-BOOLEAN");
                    attributeDefinition.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeDefinition is AttributeDefinitionDate)
                {
                    writer.WriteStartElement("ATTRIBUTE-DEFINITION-DATE");
                    attributeDefinition.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeDefinition is AttributeDefinitionEnumeration)
                {
                    writer.WriteStartElement("ATTRIBUTE-DEFINITION-ENUMERATION");
                    attributeDefinition.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeDefinition is AttributeDefinitionInteger)
                {
                    writer.WriteStartElement("ATTRIBUTE-DEFINITION-INTEGER");
                    attributeDefinition.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeDefinition is AttributeDefinitionReal)
                {
                    writer.WriteStartElement("ATTRIBUTE-DEFINITION-REAL");
                    attributeDefinition.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeDefinition is AttributeDefinitionString)
                {
                    writer.WriteStartElement("ATTRIBUTE-DEFINITION-STRING");
                    attributeDefinition.WriteXml(writer);
                    writer.WriteEndElement();
                }

                if (attributeDefinition is AttributeDefinitionXHTML)
                {
                    writer.WriteStartElement("ATTRIBUTE-DEFINITION-XHTML");
                    attributeDefinition.WriteXml(writer);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="AttributeDefinition"/> objects from the <see cref="EditableAtts"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteEditableAttsAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (this.editableAtts.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "EDITABLE-ATTS", null);

            foreach (var attributeDefinition in this.editableAtts)
            {
                if (attributeDefinition is AttributeDefinitionBoolean)
                {
                    await writer.WriteStartElementAsync(null,"ATTRIBUTE-DEFINITION-BOOLEAN", null);
                    await attributeDefinition.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeDefinition is AttributeDefinitionDate)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-DEFINITION-DATE", null);
                    await attributeDefinition.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeDefinition is AttributeDefinitionEnumeration)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-DEFINITION-ENUMERATION", null);
                    await attributeDefinition.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeDefinition is AttributeDefinitionInteger)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-DEFINITION-INTEGER", null);
                    await attributeDefinition.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeDefinition is AttributeDefinitionReal)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-DEFINITION-REAL", null);
                    await attributeDefinition.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeDefinition is AttributeDefinitionString)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-DEFINITION-STRING", null);
                    await attributeDefinition.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeDefinition is AttributeDefinitionXHTML)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-DEFINITION-XHTML", null);
                    await attributeDefinition.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }
            }

            await writer.WriteEndElementAsync();
        }

        /// <summary>
        /// Writes the <see cref="SpecHierarchy"/> objects from the <see cref="Children"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteChildren(XmlWriter writer)
        {
            if (this.children.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("CHILDREN");

            foreach (var specHierarchy in this.children)
            {
                writer.WriteStartElement("SPEC-HIERARCHY");
                specHierarchy.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="SpecHierarchy"/> objects from the <see cref="Children"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteChildrenAsync(XmlWriter writer, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (this.children.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "CHILDREN", null);

            foreach (var specHierarchy in this.children)
            {
                await writer.WriteStartElementAsync(null, "SPEC-HIERARCHY", null);
                await specHierarchy.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
        }
    }
}
