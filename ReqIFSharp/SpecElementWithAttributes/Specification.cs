// -------------------------------------------------------------------------------------------------
// <copyright file="Specification.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// Represents a hierarchically structured requirements specification.
    /// It is the root node of the tree that hierarchically structures <see cref="SpecObject"/> instances.
    /// </summary>
    [ReqIfClass(name: "SPECIFICATION", isAbstract: false)]
    public class Specification : SpecElementWithAttributes
    {
        /// <summary>
        /// Backing field for the <see cref="Children"/> property.
        /// </summary>
        private readonly List<SpecHierarchy> children = new List<SpecHierarchy>();

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<Specification> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Specification"/> class.
        /// </summary>
        public Specification()
        {
            this.logger = NullLogger<Specification>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public Specification(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<Specification>.Instance : this.loggerFactory.CreateLogger<Specification>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal Specification(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<Specification>.Instance : this.loggerFactory.CreateLogger<Specification>();

            this.ReqIFContent.Specifications.Add(this);
        }

        /// <summary>
        /// Gets the links to next level of owned SpecHierarchy.
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.Composite, lowerValue: 0, upperValue: int.MaxValue, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public List<SpecHierarchy> Children => this.children;

        /// <summary>
        /// Gets or sets the <see cref="SpecificationType"/> reference.
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.None, lowerValue: 1, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public SpecificationType Type { get; set; }

        /// <summary>
        /// Adds the <paramref name="specObject"/> to the top level of this specification
        /// </summary>
        /// <param name="specObject">
        /// The <see cref="SpecObject"/> to add
        /// </param>
        /// <returns>
        /// The new <see cref="SpecHierarchy"/> that points to the <paramref name="specObject"/> and is added to
        /// <see cref="Children"/>, with a new identifier, <see cref="Identifiable.LastChange"/> set to now and its
        /// <see cref="SpecHierarchy.Root"/> and <see cref="SpecHierarchy.ReqIfContent"/> set
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="specObject"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="specObject"/> belongs to another <see cref="ReqIFContent"/>
        /// </exception>
        public SpecHierarchy AddChild(SpecObject specObject)
        {
            if (specObject == null)
            {
                throw new ArgumentNullException(nameof(specObject));
            }

            ReqIFContent.EnsureSameContent(this.ReqIFContent, specObject.ReqIFContent, specObject, nameof(specObject));

            var specHierarchy = new SpecHierarchy(this, this.ReqIFContent, this.loggerFactory)
            {
                Object = specObject
            };

            specHierarchy.InitializeCreated(null);
            return specHierarchy;
        }

        /// <summary>
        /// Converts a <see cref="Specification"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal override void WriteXml(XmlWriter writer)
        {
            if (this.Type == null)
            {
                throw new SerializationException($"The Type property of Specification {this.Identifier}:{this.LongName} may not be null");
            }

            base.WriteXml(writer);

            this.WriteType(writer);

            this.WriteChildren(writer);
        }

        /// <summary>
        /// Asynchronously converts a <see cref="Specification"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            if (this.Type == null)
            {
                throw new SerializationException($"The Type property of Specification {this.Identifier}:{this.LongName} may not be null");
            }

            await base.WriteXmlAsync(writer, token);

            await this.WriteTypeAsync(writer, token);

            await this.WriteChildrenAsync(writer, token);
        }

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
            if (specType == null)
            {
                throw new ArgumentNullException(nameof(specType));
            }

            if (specType.GetType() != typeof(SpecificationType))
            {
                throw new ArgumentException($"{nameof(specType)} must of type SpecificationType");
            }

            this.Type = (SpecificationType)specType;
        }

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected override void ReadSpecType(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.ReadToReqIFDescendant("SPECIFICATION-TYPE-REF", this.logger))
            {
                var reference = reader.ReadElementContentAsStringToEndElement();
                var specType = this.ReqIFContent.SpecTypes.SingleOrDefault(x => x.Identifier == reference);
                this.Type = (SpecificationType)specType;
            }

            reader.SkipForeignElementsToEnd(this.logger);
        }

        /// <summary>
        /// Asynchronously reads the <see cref="SpecType"/> which is specific to the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        protected override Task ReadSpecTypeAsync(XmlReader reader, CancellationToken token)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            token.ThrowIfCancellationRequested();

            return this.ReadSpecTypeInternalAsync(reader, token);
        }

        /// <summary>
        /// Asynchronously reads the <see cref="SpecType"/> which is specific to the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task ReadSpecTypeInternalAsync(XmlReader reader, CancellationToken token)
        {
            if (await reader.ReadToReqIFDescendantAsync("SPECIFICATION-TYPE-REF", this.logger, token))
            {
                var reference = await reader.ReadElementContentAsStringToEndElementAsync();
                var specType = this.ReqIFContent.SpecTypes.SingleOrDefault(x => x.Identifier == reference);
                this.Type = (SpecificationType)specType;
            }

            await reader.SkipForeignElementsToEndAsync(this.logger, token);
        }
        /// <summary>
        /// Reads the <see cref="SpecHierarchy"/> which is specific to the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected override void ReadHierarchy(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            while (reader.Read())
            {
                if (reader.SkipForeignElement(this.logger))
                {
                    continue;
                }

                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SPEC-HIERARCHY")
                {
                    using (var subtree = reader.ReadSubtree())
                    {
                        subtree.MoveToContent();

                        var specHierarchy = new SpecHierarchy(this, this.ReqIFContent, this.loggerFactory);
                        specHierarchy.ReadXml(subtree);
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously reads the <see cref="SpecHierarchy"/> which is specific to the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        protected override Task ReadHierarchyAsync(XmlReader reader, CancellationToken token)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            token.ThrowIfCancellationRequested();

            return this.ReadHierarchyInternalAsync(reader, token);
        }

        /// <summary>
        /// Asynchronously reads the <see cref="SpecHierarchy"/> which is specific to the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task ReadHierarchyInternalAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                token.ThrowIfCancellationRequested();

                if (await reader.SkipForeignElementAsync(this.logger, token))
                {
                    continue;
                }

                if (await reader.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName == "SPEC-HIERARCHY")
                {
                    token.ThrowIfCancellationRequested();

                    using (var subtree = reader.ReadSubtree())
                    {
                        await subtree.MoveToContentAsync();

                        var specHierarchy = new SpecHierarchy(this, this.ReqIFContent, this.loggerFactory);
                        await specHierarchy.ReadXmlAsync(subtree, token);
                    }
                }
            }
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
            writer.WriteElementString("SPECIFICATION-TYPE-REF", this.Type.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously writes the <see cref="Type"/>
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteTypeAsync(XmlWriter writer, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await writer.WriteStartElementAsync(null, "TYPE", null);
            await writer.WriteElementStringAsync(null, "SPECIFICATION-TYPE-REF",null, this.Type.Identifier);
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
