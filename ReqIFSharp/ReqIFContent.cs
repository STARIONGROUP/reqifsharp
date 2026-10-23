// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFContent.cs" company="Starion Group S.A.">
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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The <see cref="ReqIFContent"/> class represents the mandatory content of a ReqIF Exchange Document.
    /// </summary>
    [ReqIfClass(name: "REQ-IF-CONTENT", isAbstract: false)]
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
        [ReqIfProperty(aggregation: AggregationKind.Composite, lowerValue: 0, upperValue: int.MaxValue, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public List<DatatypeDefinition> DataTypes => this.dataTypes;

        /// <summary>
        /// Gets the <see cref="SpecType"/>s
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.Composite, lowerValue: 0, upperValue: int.MaxValue, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public List<SpecType> SpecTypes => this.specTypes;

        /// <summary>
        /// Gets the <see cref="SpecObject"/>
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.Composite, lowerValue: 0, upperValue: int.MaxValue, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public List<SpecObject> SpecObjects => this.specObjects;

        /// <summary>
        /// Gets the <see cref="SpecRelation"/>
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.Composite, lowerValue: 0, upperValue: int.MaxValue, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public List<SpecRelation> SpecRelations => this.specRelations;

        /// <summary>
        /// Gets the <see cref="Specification"/>
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.Composite, lowerValue: 0, upperValue: int.MaxValue, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public List<Specification> Specifications => this.specifications;

        /// <summary>
        /// Gets the <see cref="RelationGroup"/>
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.Composite, lowerValue: 0, upperValue: int.MaxValue, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public List<RelationGroup> SpecRelationGroups => this.specRelationGroups;

        /// <summary>
        /// Gets or sets the document root element.
        /// </summary>
        public ReqIF DocumentRoot { get; set; }

        /// <summary>
        /// Checks that an element passed to a create method does not belong to another <see cref="ReqIFContent"/>, so
        /// that the new element only refers to elements of its own document. An element that belongs to no content is
        /// accepted.
        /// </summary>
        /// <param name="content">
        /// The <see cref="ReqIFContent"/> the new element is created in
        /// </param>
        /// <param name="elementContent">
        /// The <see cref="ReqIFContent"/> of the element that is passed
        /// </param>
        /// <param name="element">
        /// The element that is passed
        /// </param>
        /// <param name="paramName">
        /// The name of the parameter the element is passed in
        /// </param>
        /// <exception cref="ArgumentException">
        /// The element belongs to another <see cref="ReqIFContent"/>
        /// </exception>
        internal static void EnsureSameContent(ReqIFContent content, ReqIFContent elementContent, Identifiable element, string paramName)
        {
            if (content != null && elementContent != null && !ReferenceEquals(content, elementContent))
            {
                throw new ArgumentException($"{element.Identifier} belongs to another ReqIF document and cannot be referenced from this one", paramName);
            }
        }

        /// <summary>
        /// Creates a <see cref="DatatypeDefinitionBoolean"/> and adds it to <see cref="DataTypes"/>
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <returns>
        /// The new <see cref="DatatypeDefinitionBoolean"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="DatatypeDefinition.ReqIFContent"/> set to this content
        /// </returns>
        public DatatypeDefinitionBoolean CreateDatatypeDefinitionBoolean(string longName)
        {
            var datatypeDefinition = new DatatypeDefinitionBoolean(this, this.loggerFactory);
            datatypeDefinition.InitializeCreated(longName);
            return datatypeDefinition;
        }

        /// <summary>
        /// Creates a <see cref="DatatypeDefinitionDate"/> and adds it to <see cref="DataTypes"/>
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <returns>
        /// The new <see cref="DatatypeDefinitionDate"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="DatatypeDefinition.ReqIFContent"/> set to this content
        /// </returns>
        public DatatypeDefinitionDate CreateDatatypeDefinitionDate(string longName)
        {
            var datatypeDefinition = new DatatypeDefinitionDate(this, this.loggerFactory);
            datatypeDefinition.InitializeCreated(longName);
            return datatypeDefinition;
        }

        /// <summary>
        /// Creates a <see cref="DatatypeDefinitionEnumeration"/> and adds it to <see cref="DataTypes"/>. Add its literals
        /// with <see cref="DatatypeDefinitionEnumeration.CreateEnumValue"/>.
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <returns>
        /// The new <see cref="DatatypeDefinitionEnumeration"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="DatatypeDefinition.ReqIFContent"/> set to this content
        /// </returns>
        public DatatypeDefinitionEnumeration CreateDatatypeDefinitionEnumeration(string longName)
        {
            var datatypeDefinition = new DatatypeDefinitionEnumeration(this, this.loggerFactory);
            datatypeDefinition.InitializeCreated(longName);
            return datatypeDefinition;
        }

        /// <summary>
        /// Creates a <see cref="DatatypeDefinitionInteger"/> and adds it to <see cref="DataTypes"/>
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <param name="min">
        /// The smallest allowed value
        /// </param>
        /// <param name="max">
        /// The largest allowed value
        /// </param>
        /// <returns>
        /// The new <see cref="DatatypeDefinitionInteger"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="DatatypeDefinition.ReqIFContent"/> set to this content
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="min"/> is larger than <paramref name="max"/>
        /// </exception>
        public DatatypeDefinitionInteger CreateDatatypeDefinitionInteger(string longName, long min, long max)
        {
            if (min > max)
            {
                throw new ArgumentException($"The min {min} is larger than the max {max}", nameof(min));
            }

            var datatypeDefinition = new DatatypeDefinitionInteger(this, this.loggerFactory)
            {
                Min = min,
                Max = max
            };

            datatypeDefinition.InitializeCreated(longName);
            return datatypeDefinition;
        }

        /// <summary>
        /// Creates a <see cref="DatatypeDefinitionReal"/> and adds it to <see cref="DataTypes"/>
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <param name="accuracy">
        /// The number of digits after the decimal point
        /// </param>
        /// <param name="min">
        /// The smallest allowed value
        /// </param>
        /// <param name="max">
        /// The largest allowed value
        /// </param>
        /// <returns>
        /// The new <see cref="DatatypeDefinitionReal"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="DatatypeDefinition.ReqIFContent"/> set to this content
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="accuracy"/> is negative, or <paramref name="min"/> is larger than <paramref name="max"/>
        /// </exception>
        public DatatypeDefinitionReal CreateDatatypeDefinitionReal(string longName, long accuracy, double min, double max)
        {
            if (accuracy < 0)
            {
                throw new ArgumentException($"The accuracy {accuracy} is negative", nameof(accuracy));
            }

            if (min > max)
            {
                throw new ArgumentException($"The min {min} is larger than the max {max}", nameof(min));
            }

            var datatypeDefinition = new DatatypeDefinitionReal(this, this.loggerFactory)
            {
                Accuracy = accuracy,
                Min = min,
                Max = max
            };

            datatypeDefinition.InitializeCreated(longName);
            return datatypeDefinition;
        }

        /// <summary>
        /// Creates a <see cref="DatatypeDefinitionString"/> and adds it to <see cref="DataTypes"/>
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <param name="maxLength">
        /// The largest allowed number of characters
        /// </param>
        /// <returns>
        /// The new <see cref="DatatypeDefinitionString"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="DatatypeDefinition.ReqIFContent"/> set to this content
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="maxLength"/> is negative
        /// </exception>
        public DatatypeDefinitionString CreateDatatypeDefinitionString(string longName, long maxLength)
        {
            if (maxLength < 0)
            {
                throw new ArgumentException($"The max length {maxLength} is negative", nameof(maxLength));
            }

            var datatypeDefinition = new DatatypeDefinitionString(this, this.loggerFactory)
            {
                MaxLength = maxLength
            };

            datatypeDefinition.InitializeCreated(longName);
            return datatypeDefinition;
        }

        /// <summary>
        /// Creates a <see cref="DatatypeDefinitionXHTML"/> and adds it to <see cref="DataTypes"/>
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <returns>
        /// The new <see cref="DatatypeDefinitionXHTML"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="DatatypeDefinition.ReqIFContent"/> set to this content
        /// </returns>
        public DatatypeDefinitionXHTML CreateDatatypeDefinitionXHTML(string longName)
        {
            var datatypeDefinition = new DatatypeDefinitionXHTML(this, this.loggerFactory);
            datatypeDefinition.InitializeCreated(longName);
            return datatypeDefinition;
        }

        /// <summary>
        /// Creates a <see cref="SpecObjectType"/> and adds it to <see cref="SpecTypes"/>. Add its attributes with the
        /// <see cref="SpecType.CreateAttributeDefinition(DatatypeDefinitionString, string)"/> overloads.
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <returns>
        /// The new <see cref="SpecObjectType"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="SpecType.ReqIFContent"/> set to this content
        /// </returns>
        public SpecObjectType CreateSpecObjectType(string longName)
        {
            var specType = new SpecObjectType(this, this.loggerFactory);
            specType.InitializeCreated(longName);
            return specType;
        }

        /// <summary>
        /// Creates a <see cref="SpecificationType"/> and adds it to <see cref="SpecTypes"/>
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <returns>
        /// The new <see cref="SpecificationType"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="SpecType.ReqIFContent"/> set to this content
        /// </returns>
        public SpecificationType CreateSpecificationType(string longName)
        {
            var specType = new SpecificationType(this, this.loggerFactory);
            specType.InitializeCreated(longName);
            return specType;
        }

        /// <summary>
        /// Creates a <see cref="SpecRelationType"/> and adds it to <see cref="SpecTypes"/>
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <returns>
        /// The new <see cref="SpecRelationType"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="SpecType.ReqIFContent"/> set to this content
        /// </returns>
        public SpecRelationType CreateSpecRelationType(string longName)
        {
            var specType = new SpecRelationType(this, this.loggerFactory);
            specType.InitializeCreated(longName);
            return specType;
        }

        /// <summary>
        /// Creates a <see cref="RelationGroupType"/> and adds it to <see cref="SpecTypes"/>
        /// </summary>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <returns>
        /// The new <see cref="RelationGroupType"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="SpecType.ReqIFContent"/> set to this content
        /// </returns>
        public RelationGroupType CreateRelationGroupType(string longName)
        {
            var specType = new RelationGroupType(this, this.loggerFactory);
            specType.InitializeCreated(longName);
            return specType;
        }

        /// <summary>
        /// Creates a <see cref="SpecObject"/> of the <paramref name="type"/> and adds it to <see cref="SpecObjects"/>.
        /// Add its values with the <see cref="SpecElementWithAttributes.CreateAttributeValue(AttributeDefinitionString, string)"/>
        /// overloads.
        /// </summary>
        /// <param name="type">
        /// The <see cref="SpecObjectType"/> of the new <see cref="SpecObject"/>
        /// </param>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <returns>
        /// The new <see cref="SpecObject"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="SpecElementWithAttributes.ReqIFContent"/> set to this content
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="type"/> belongs to another <see cref="ReqIFContent"/>
        /// </exception>
        public SpecObject CreateSpecObject(SpecObjectType type, string longName = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            EnsureSameContent(this, type.ReqIFContent, type, nameof(type));

            var specObject = new SpecObject(this, this.loggerFactory)
            {
                Type = type
            };

            specObject.InitializeCreated(longName);
            return specObject;
        }

        /// <summary>
        /// Creates a <see cref="Specification"/> of the <paramref name="type"/> and adds it to <see cref="Specifications"/>.
        /// Add its spec objects with <see cref="Specification.AddChild"/>.
        /// </summary>
        /// <param name="type">
        /// The <see cref="SpecificationType"/> of the new <see cref="Specification"/>
        /// </param>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <returns>
        /// The new <see cref="Specification"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="SpecElementWithAttributes.ReqIFContent"/> set to this content
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="type"/> belongs to another <see cref="ReqIFContent"/>
        /// </exception>
        public Specification CreateSpecification(SpecificationType type, string longName = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            EnsureSameContent(this, type.ReqIFContent, type, nameof(type));

            var specification = new Specification(this, this.loggerFactory)
            {
                Type = type
            };

            specification.InitializeCreated(longName);
            return specification;
        }

        /// <summary>
        /// Creates a <see cref="SpecRelation"/> of the <paramref name="type"/> from <paramref name="source"/> to
        /// <paramref name="target"/> and adds it to <see cref="SpecRelations"/>
        /// </summary>
        /// <param name="type">
        /// The <see cref="SpecRelationType"/> of the new <see cref="SpecRelation"/>
        /// </param>
        /// <param name="source">
        /// The <see cref="SpecObject"/> the relation starts from
        /// </param>
        /// <param name="target">
        /// The <see cref="SpecObject"/> the relation points to
        /// </param>
        /// <param name="longName">
        /// The human-readable name, or null
        /// </param>
        /// <returns>
        /// The new <see cref="SpecRelation"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="SpecElementWithAttributes.ReqIFContent"/> set to this content
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/>, <paramref name="source"/> or <paramref name="target"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="type"/>, <paramref name="source"/> or <paramref name="target"/> belongs to another <see cref="ReqIFContent"/>
        /// </exception>
        public SpecRelation CreateSpecRelation(SpecRelationType type, SpecObject source, SpecObject target, string longName = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            EnsureSameContent(this, type.ReqIFContent, type, nameof(type));
            EnsureSameContent(this, source.ReqIFContent, source, nameof(source));
            EnsureSameContent(this, target.ReqIFContent, target, nameof(target));

            var specRelation = new SpecRelation(this, this.loggerFactory)
            {
                Type = type,
                Source = source,
                Target = target
            };

            specRelation.InitializeCreated(longName);
            return specRelation;
        }

        /// <summary>
        /// Creates a <see cref="RelationGroup"/> of the <paramref name="type"/> between two specifications and adds it to
        /// <see cref="SpecRelationGroups"/>. Add its relations to <see cref="RelationGroup.SpecRelations"/>.
        /// </summary>
        /// <param name="type">
        /// The <see cref="RelationGroupType"/> of the new <see cref="RelationGroup"/>
        /// </param>
        /// <param name="sourceSpecification">
        /// The <see cref="Specification"/> the relations of the group start from
        /// </param>
        /// <param name="targetSpecification">
        /// The <see cref="Specification"/> the relations of the group point to
        /// </param>
        /// <param name="longName">
        /// The human-readable name, mandatory for a relation group
        /// </param>
        /// <returns>
        /// The new <see cref="RelationGroup"/>, with a new identifier, <see cref="Identifiable.LastChange"/>
        /// set to now and <see cref="RelationGroup.CoreContent"/> set to this content
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/>, <paramref name="sourceSpecification"/>, <paramref name="targetSpecification"/> or
        /// <paramref name="longName"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="longName"/> is empty, or <paramref name="type"/>, <paramref name="sourceSpecification"/> or
        /// <paramref name="targetSpecification"/> belongs to another <see cref="ReqIFContent"/>
        /// </exception>
        public RelationGroup CreateRelationGroup(RelationGroupType type, Specification sourceSpecification, Specification targetSpecification, string longName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (sourceSpecification == null)
            {
                throw new ArgumentNullException(nameof(sourceSpecification));
            }

            if (targetSpecification == null)
            {
                throw new ArgumentNullException(nameof(targetSpecification));
            }

            Identifiable.ValidateLongName(longName);
            EnsureSameContent(this, type.ReqIFContent, type, nameof(type));
            EnsureSameContent(this, sourceSpecification.ReqIFContent, sourceSpecification, nameof(sourceSpecification));
            EnsureSameContent(this, targetSpecification.ReqIFContent, targetSpecification, nameof(targetSpecification));

            var relationGroup = new RelationGroup(this, this.loggerFactory)
            {
                Type = type,
                SourceSpecification = sourceSpecification,
                TargetSpecification = targetSpecification
            };

            relationGroup.InitializeCreated(longName);
            return relationGroup;
        }

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
                    if (reader.SkipForeignElement(this.logger))
                    {
                        continue;
                    }

                    var xmlLineInfo = reader as IXmlLineInfo;

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
                            this.logger.LogWarning("The {LocalName} element at line:position {LineNumber}:{LinePosition} is not supported", reader.LocalName, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);
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
                token.ThrowIfCancellationRequested();

                if (await reader.MoveToContentAsync() == XmlNodeType.Element)
                {
                    if (await reader.SkipForeignElementAsync(this.logger, token))
                    {
                        continue;
                    }

                    var xmlLineInfo = reader as IXmlLineInfo;

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
                                await this.DeserializeSpecObjectsAsync(specObjectsSubtree, token);
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
                            this.logger.LogWarning("The {LocalName} element at line:position {LineNumber}:{LinePosition} is not supported", reader.LocalName, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);
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
                if (reader.SkipForeignElement(this.logger))
                {
                    continue;
                }

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
                token.ThrowIfCancellationRequested();

                if (await reader.SkipForeignElementAsync(this.logger, token))
                {
                    continue;
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
                    if (reader.SkipForeignElement(this.logger))
                    {
                        continue;
                    }

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
                token.ThrowIfCancellationRequested();

                if (await reader.MoveToContentAsync() == XmlNodeType.Element)
                {
                    if (await reader.SkipForeignElementAsync(this.logger, token))
                    {
                        continue;
                    }

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
                if (reader.SkipForeignElement(this.logger))
                {
                    continue;
                }

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
        private async Task DeserializeSpecObjectsAsync(XmlReader reader, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                token.ThrowIfCancellationRequested();

                if (await reader.SkipForeignElementAsync(this.logger, token))
                {
                    continue;
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
                if (reader.SkipForeignElement(this.logger))
                {
                    continue;
                }

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
                token.ThrowIfCancellationRequested();

                if (await reader.SkipForeignElementAsync(this.logger, token))
                {
                    continue;
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
                if (reader.SkipForeignElement(this.logger))
                {
                    continue;
                }

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
                token.ThrowIfCancellationRequested();

                if (await reader.SkipForeignElementAsync(this.logger, token))
                {
                    continue;
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
                if (reader.SkipForeignElement(this.logger))
                {
                    continue;
                }

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
                if (await reader.SkipForeignElementAsync(this.logger, token))
                {
                    continue;
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
            token.ThrowIfCancellationRequested();

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
            token.ThrowIfCancellationRequested();

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
            token.ThrowIfCancellationRequested();

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
            token.ThrowIfCancellationRequested();

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
            token.ThrowIfCancellationRequested();

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
            token.ThrowIfCancellationRequested();

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
            token.ThrowIfCancellationRequested();

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
