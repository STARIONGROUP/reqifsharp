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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Contains a set of attribute definitions. By using an instance of a subclass of <see cref="SpecType"/>, multiple elements can be
    /// associated with the same set of attribute definitions (attribute names, default values, data types, etc.).
    /// </summary>
    public abstract class SpecType : Identifiable
    {
        /// <summary>
        /// Backing field for the <see cref="SpecAttributes"/> property
        /// </summary>
        private readonly List<AttributeDefinition> specAttributes = new List<AttributeDefinition>();

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
        protected SpecType(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.ReqIFContent = reqIfContent;
            reqIfContent.SpecTypes.Add(this);
        }

        /// <summary>
        /// Gets the set of attribute definitions.
        /// </summary>        
        public List<AttributeDefinition> SpecAttributes => this.specAttributes;

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
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case "ALTERNATIVE-ID":
                            using (var subtree = reader.ReadSubtree())
                            {
                                var alternativeId = new AlternativeId(this);
                                subtree.MoveToContent();
                                alternativeId.ReadXml(subtree);
                            }

                            break;
                        case "SPEC-ATTRIBUTES":

                            using (var specAttributesSubTree = reader.ReadSubtree())
                            {
                                while (specAttributesSubTree.Read())
                                {
                                    if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName.StartsWith("ATTRIBUTE-DEFINITION-"))
                                    {
                                        this.CreateAttributeDefinition(reader, reader.LocalName);
                                    }
                                }
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="SpecType"/> object from its XML representation.
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
                            using (var subtree = reader.ReadSubtree())
                            {
                                var alternativeId = new AlternativeId(this);
                                await subtree.MoveToContentAsync();
                                alternativeId.ReadXml(reader);
                            }

                            break;
                        case "SPEC-ATTRIBUTES":

                            using (var specAttributesSubTree = reader.ReadSubtree())
                            {
                                while (await specAttributesSubTree.ReadAsync())
                                {
                                    if (token.IsCancellationRequested)
                                    {
                                        token.ThrowIfCancellationRequested();
                                    }

                                    if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName.StartsWith("ATTRIBUTE-DEFINITION-"))
                                    {
                                        await this.CreateAttributeDefinitionAsync(reader, reader.LocalName, token);
                                    }
                                }
                            }

                            break;
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
            var attributeDefinition = ReqIfFactory.AttributeDefinitionConstruct(xmlname, this, this.loggerFactory);
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
        /// Asynchronously creates <see cref="AttributeDefinition"/> and adds it to the <see cref="SpecAttributes"/> list.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="xmlname">
        /// The XML Element name of the <see cref="AttributeDefinition"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task CreateAttributeDefinitionAsync(XmlReader reader, string xmlname, CancellationToken token)
        {
            var attributeDefinition = ReqIfFactory.AttributeDefinitionConstruct(xmlname, this, this.loggerFactory);
            if (attributeDefinition == null)
            {
                return;
            }

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            using (var attributeDefTree = reader.ReadSubtree())
            {
                await attributeDefTree.MoveToContentAsync();
                await attributeDefinition.ReadXmlAsync(attributeDefTree, token);
            }
        }

        /// <summary>
        /// Converts a <see cref="SpecType"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            this.WriteSpecAttributes(writer);
        }

        /// <summary>
        /// Asynchronously converts a <see cref="SpecType"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            await base.WriteXmlAsync(writer, token);
            
            await this.WriteSpecAttributesAsync(writer, token);
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

        /// <summary>
        /// Asynchronously writes the <see cref="AttributeDefinition"/> objects from the <see cref="SpecAttributes"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteSpecAttributesAsync(XmlWriter writer, CancellationToken token)
        {
            if (this.specAttributes.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "SPEC-ATTRIBUTES", null);

            foreach (var attributeDefinition in this.specAttributes)
            {
                var xmlname = ReqIfFactory.XmlName(attributeDefinition);
                await writer.WriteStartElementAsync(null, xmlname, null);
                await attributeDefinition.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
        }
    }
}
