// -------------------------------------------------------------------------------------------------
// <copyright file="SpecElementWithAttributes.cs" company="Starion Group S.A.">
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
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// An abstract super class for elements that can own attributes.
    /// </summary>
    /// <remarks>
    /// Any element that can own attributes, like a requirement, a specification, or a relation between requirements needs to be
    /// an instance of a concrete subclass of this abstract class.
    /// While this class aggregates the values of the attributes, the association to the attributes’ types that define the acceptable
    /// values for the attributes is realized by concrete sub classes of this class.
    /// </remarks>
    [ReqIfClass(name: "SPEC-ELEMENT-WITH-ATTRIBUTES", isAbstract: true)]
    public abstract class SpecElementWithAttributes : Identifiable
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<SpecElementWithAttributes> logger;

        /// <summary>
        /// Backing field for the <see cref="Values"/> property.
        /// </summary>
        private readonly List<AttributeValue> values = new List<AttributeValue>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecElementWithAttributes"/> class.
        /// </summary>
        protected SpecElementWithAttributes()
        {
            this.logger = NullLogger<SpecElementWithAttributes>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecElementWithAttributes"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected SpecElementWithAttributes(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<SpecElementWithAttributes>.Instance : this.loggerFactory.CreateLogger<SpecElementWithAttributes>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecElementWithAttributes"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected SpecElementWithAttributes(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.ReqIFContent = reqIfContent;

            this.logger = this.loggerFactory == null ? NullLogger<SpecElementWithAttributes>.Instance : this.loggerFactory.CreateLogger<SpecElementWithAttributes>();
        }

        /// <summary>
        /// Gets the values of the attributes owned by the element.
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.Composite, lowerValue: 0, upperValue: int.MaxValue, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public List<AttributeValue> Values => this.values;

        /// <summary>
        /// Gets or sets the owning <see cref="ReqIFContent"/>
        /// </summary>
        public ReqIFContent ReqIFContent { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SpecType"/>
        /// </summary>
        public SpecType SpecType 
        {
            get => this.GetSpecType();

            set => this.SetSpecType(value);
        }

        /// <summary>
        /// Gets the <see cref="SpecType"/> from the subclass
        /// </summary>
        /// <returns>
        /// an instance of <see cref="SpecType"/>
        /// </returns>
        protected abstract SpecType GetSpecType();

        /// <summary>
        /// Sets the <see cref="SpecType"/> to the subclass
        /// </summary>
        /// <param name="specType">
        /// The <see cref="SpecType"/> to set.
        /// </param>
        protected abstract void SetSpecType(SpecType specType);

        /// <summary>
        /// Creates an <see cref="AttributeValueBoolean"/> for the <paramref name="definition"/> and adds it to <see cref="Values"/>
        /// </summary>
        /// <param name="definition">
        /// The <see cref="AttributeDefinitionBoolean"/> the value is for. It must be an attribute of the <see cref="SpecType"/>
        /// of this element.
        /// </param>
        /// <param name="value">
        /// The value
        /// </param>
        /// <returns>
        /// The new <see cref="AttributeValueBoolean"/>, with <see cref="AttributeValue.SpecElAt"/> set to this element
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definition"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This element has no <see cref="SpecType"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="definition"/> has no datatype or is not an attribute of the <see cref="SpecType"/> of this element, or this
        /// element already has a value for it
        /// </exception>
        public AttributeValueBoolean CreateAttributeValue(AttributeDefinitionBoolean definition, bool value)
        {
            this.EnsureValueCanBeCreated(definition);

            return new AttributeValueBoolean(this, this.loggerFactory)
            {
                Definition = definition,
                TheValue = value
            };
        }

        /// <summary>
        /// Creates an <see cref="AttributeValueDate"/> for the <paramref name="definition"/> and adds it to <see cref="Values"/>
        /// </summary>
        /// <param name="definition">
        /// The <see cref="AttributeDefinitionDate"/> the value is for. It must be an attribute of the <see cref="SpecType"/>
        /// of this element.
        /// </param>
        /// <param name="value">
        /// The value
        /// </param>
        /// <returns>
        /// The new <see cref="AttributeValueDate"/>, with <see cref="AttributeValue.SpecElAt"/> set to this element
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definition"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This element has no <see cref="SpecType"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="definition"/> has no datatype or is not an attribute of the <see cref="SpecType"/> of this element, or this
        /// element already has a value for it
        /// </exception>
        public AttributeValueDate CreateAttributeValue(AttributeDefinitionDate definition, DateTime value)
        {
            this.EnsureValueCanBeCreated(definition);

            return new AttributeValueDate(this, this.loggerFactory)
            {
                Definition = definition,
                TheValue = value
            };
        }

        /// <summary>
        /// Creates an <see cref="AttributeValueInteger"/> for the <paramref name="definition"/> and adds it to <see cref="Values"/>
        /// </summary>
        /// <param name="definition">
        /// The <see cref="AttributeDefinitionInteger"/> the value is for. It must be an attribute of the <see cref="SpecType"/>
        /// of this element.
        /// </param>
        /// <param name="value">
        /// The value
        /// </param>
        /// <returns>
        /// The new <see cref="AttributeValueInteger"/>, with <see cref="AttributeValue.SpecElAt"/> set to this element
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definition"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This element has no <see cref="SpecType"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="definition"/> has no datatype or is not an attribute of the <see cref="SpecType"/> of this element, this
        /// element already has a value for it, or the <paramref name="value"/> is outside the <see cref="DatatypeDefinitionInteger.Min"/> and
        /// <see cref="DatatypeDefinitionInteger.Max"/> of its datatype
        /// </exception>
        public AttributeValueInteger CreateAttributeValue(AttributeDefinitionInteger definition, long value)
        {
            this.EnsureValueCanBeCreated(definition);

            var datatype = definition.Type;

            if (value < datatype.Min || value > datatype.Max)
            {
                throw new ArgumentException($"The value {value} is outside the range {datatype.Min} to {datatype.Max} of the datatype {datatype.Identifier}", nameof(value));
            }

            return new AttributeValueInteger(this, this.loggerFactory)
            {
                Definition = definition,
                TheValue = value
            };
        }

        /// <summary>
        /// Creates an <see cref="AttributeValueReal"/> for the <paramref name="definition"/> and adds it to <see cref="Values"/>
        /// </summary>
        /// <param name="definition">
        /// The <see cref="AttributeDefinitionReal"/> the value is for. It must be an attribute of the <see cref="SpecType"/>
        /// of this element.
        /// </param>
        /// <param name="value">
        /// The value
        /// </param>
        /// <returns>
        /// The new <see cref="AttributeValueReal"/>, with <see cref="AttributeValue.SpecElAt"/> set to this element
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definition"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This element has no <see cref="SpecType"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="definition"/> has no datatype or is not an attribute of the <see cref="SpecType"/> of this element, this
        /// element already has a value for it, or the <paramref name="value"/> is outside the <see cref="DatatypeDefinitionReal.Min"/> and
        /// <see cref="DatatypeDefinitionReal.Max"/> of its datatype, or is not a number
        /// </exception>
        public AttributeValueReal CreateAttributeValue(AttributeDefinitionReal definition, double value)
        {
            this.EnsureValueCanBeCreated(definition);

            var datatype = definition.Type;

            if (!(value >= datatype.Min && value <= datatype.Max))
            {
                throw new ArgumentException($"The value {value.ToString(CultureInfo.InvariantCulture)} is outside the range {datatype.Min.ToString(CultureInfo.InvariantCulture)} to {datatype.Max.ToString(CultureInfo.InvariantCulture)} of the datatype {datatype.Identifier}", nameof(value));
            }

            return new AttributeValueReal(this, this.loggerFactory)
            {
                Definition = definition,
                TheValue = value
            };
        }

        /// <summary>
        /// Creates an <see cref="AttributeValueString"/> for the <paramref name="definition"/> and adds it to <see cref="Values"/>
        /// </summary>
        /// <param name="definition">
        /// The <see cref="AttributeDefinitionString"/> the value is for. It must be an attribute of the <see cref="SpecType"/>
        /// of this element.
        /// </param>
        /// <param name="value">
        /// The value; use an empty string for no text, as the ReqIF schema requires a value
        /// </param>
        /// <returns>
        /// The new <see cref="AttributeValueString"/>, with <see cref="AttributeValue.SpecElAt"/> set to this element
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definition"/> or <paramref name="value"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This element has no <see cref="SpecType"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="definition"/> has no datatype or is not an attribute of the <see cref="SpecType"/> of this element, this
        /// element already has a value for it, or the <paramref name="value"/> is longer than the <see cref="DatatypeDefinitionString.MaxLength"/>
        /// of its datatype
        /// </exception>
        public AttributeValueString CreateAttributeValue(AttributeDefinitionString definition, string value)
        {
            this.EnsureValueCanBeCreated(definition);

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var datatype = definition.Type;

            if (value.Length > datatype.MaxLength)
            {
                throw new ArgumentException($"The value is {value.Length} characters long, more than the max length {datatype.MaxLength} of the datatype {datatype.Identifier}", nameof(value));
            }

            return new AttributeValueString(this, this.loggerFactory)
            {
                Definition = definition,
                TheValue = value
            };
        }

        /// <summary>
        /// Creates an <see cref="AttributeValueXHTML"/> for the <paramref name="definition"/> and adds it to <see cref="Values"/>
        /// </summary>
        /// <param name="definition">
        /// The <see cref="AttributeDefinitionXHTML"/> the value is for. It must be an attribute of the <see cref="SpecType"/>
        /// of this element.
        /// </param>
        /// <param name="xhtml">
        /// The XHTML content: well-formed XML with one root element, an <c>xhtml:div</c> or <c>xhtml:p</c>, and no text
        /// outside it. The <c>xhtml</c> prefix may be used without declaring it. The content of the root element is not
        /// validated against the XHTML schema.
        /// </param>
        /// <returns>
        /// The new <see cref="AttributeValueXHTML"/>, with <see cref="AttributeValue.SpecElAt"/> set to this element and an
        /// <see cref="ExternalObject"/> in <see cref="AttributeValueXHTML.ExternalObjects"/> for each <c>xhtml:object</c>.
        /// When the <paramref name="xhtml"/> uses the <c>xhtml</c> prefix without declaring it, its
        /// <see cref="AttributeValueXHTML.TheValue"/> has <c>xmlns:xhtml</c> added to the root element, so the value is
        /// complete on its own, like a value read from a document, and is written well-formed in any document.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definition"/> or <paramref name="xhtml"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This element has no <see cref="SpecType"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="definition"/> has no datatype or is not an attribute of the <see cref="SpecType"/> of this element, this
        /// element already has a value for it, or the <paramref name="xhtml"/> does not meet the rules above
        /// </exception>
        public AttributeValueXHTML CreateAttributeValue(AttributeDefinitionXHTML definition, string xhtml)
        {
            this.EnsureValueCanBeCreated(definition);

            if (xhtml == null)
            {
                throw new ArgumentNullException(nameof(xhtml));
            }

            AttributeValueXHTML.EnsureValidValue(xhtml);
            var theValue = AttributeValueXHTML.DeclareXhtmlPrefix(xhtml);

            var attributeValue = new AttributeValueXHTML(this, this.loggerFactory)
            {
                Definition = definition,
                TheValue = theValue
            };

            attributeValue.ExternalObjects.AddRange(attributeValue.CreateExternalObjects(theValue));
            return attributeValue;
        }

        /// <summary>
        /// Creates an <see cref="AttributeValueEnumeration"/> for the <paramref name="definition"/> and adds it to <see cref="Values"/>
        /// </summary>
        /// <param name="definition">
        /// The <see cref="AttributeDefinitionEnumeration"/> the value is for. It must be an attribute of the
        /// <see cref="SpecType"/> of this element.
        /// </param>
        /// <param name="values">
        /// The selected literals. They must be literals of the enumeration that is the type of the
        /// <paramref name="definition"/>; select more than one only when the definition is multi-valued.
        /// </param>
        /// <returns>
        /// The new <see cref="AttributeValueEnumeration"/>, with <see cref="AttributeValue.SpecElAt"/> set to this element
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definition"/> or <paramref name="values"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This element has no <see cref="SpecType"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="definition"/> has no datatype or is not an attribute of the <see cref="SpecType"/> of this element, this element
        /// already has a value for it, a selected literal is null or not a literal of its enumeration, or more than one
        /// literal is selected for a definition that is not multi-valued
        /// </exception>
        public AttributeValueEnumeration CreateAttributeValue(AttributeDefinitionEnumeration definition, params EnumValue[] values)
        {
            this.EnsureValueCanBeCreated(definition);

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length > 1 && !definition.IsMultiValued)
            {
                throw new ArgumentException($"The attribute definition {definition.Identifier} is not multi-valued, but {values.Length} values are selected", nameof(values));
            }

            foreach (var enumValue in values)
            {
                if (enumValue == null)
                {
                    throw new ArgumentException("A selected value is null", nameof(values));
                }

                if (!definition.Type.SpecifiedValues.Contains(enumValue))
                {
                    throw new ArgumentException($"The enum value {enumValue.Identifier} is not a literal of the enumeration {definition.Type.Identifier}", nameof(values));
                }
            }

            var attributeValue = new AttributeValueEnumeration(this, this.loggerFactory)
            {
                Definition = definition
            };

            attributeValue.Values.AddRange(values);
            return attributeValue;
        }

        /// <summary>
        /// Checks that a value for the <paramref name="definition"/> can be added to this element
        /// </summary>
        /// <param name="definition">
        /// The <see cref="AttributeDefinition"/> of the value to create
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="definition"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This element has no <see cref="SpecType"/>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The <paramref name="definition"/> has no datatype or is not an attribute of the <see cref="SpecType"/> of this element, or this
        /// element already has a value for it
        /// </exception>
        private void EnsureValueCanBeCreated(AttributeDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            var specType = this.SpecType;

            if (specType == null)
            {
                throw new InvalidOperationException($"{this.Identifier} has no spec type, so it cannot have attribute values");
            }

            if (definition.DatatypeDefinition == null)
            {
                throw new ArgumentException($"The attribute definition {definition.Identifier} has no datatype", nameof(definition));
            }

            if (!specType.SpecAttributes.Contains(definition))
            {
                throw new ArgumentException($"The attribute definition {definition.Identifier} is not an attribute of the spec type {specType.Identifier} of {this.Identifier}", nameof(definition));
            }

            if (this.Values.Any(x => x.AttributeDefinition == definition))
            {
                throw new ArgumentException($"{this.Identifier} already has a value for the attribute definition {definition.Identifier}", nameof(definition));
            }
        }

        /// <summary>
        /// Generates a <see cref="SpecElementWithAttributes"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            using (var specElementWithAttributesReader = reader.ReadSubtree())
            {
                while (specElementWithAttributesReader.Read())
                {
                    var xmlLineInfo = reader as IXmlLineInfo;

                    if (specElementWithAttributesReader.MoveToContent() == XmlNodeType.Element)
                    {
                        if (specElementWithAttributesReader.SkipForeignElement(this.logger))
                        {
                            continue;
                        }

                        switch (specElementWithAttributesReader.LocalName)
                        {
                            case "ALTERNATIVE-ID":
                                this.ReadAlternativeId(specElementWithAttributesReader);
                                break;
                            case "TYPE":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    this.ReadSpecType(subtree);
                                }
                                break;
                            case "CHILDREN":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    this.ReadHierarchy(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-BOOLEAN":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueBoolean = new AttributeValueBoolean(this, this.loggerFactory);
                                    attributeValueBoolean.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-DATE":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueDate = new AttributeValueDate(this, this.loggerFactory);
                                    attributeValueDate.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-ENUMERATION":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueEnumeration = new AttributeValueEnumeration(this, this.loggerFactory);
                                    attributeValueEnumeration.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-INTEGER":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueInteger = new AttributeValueInteger(this, this.loggerFactory);
                                    attributeValueInteger.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-REAL":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueReal = new AttributeValueReal(this, this.loggerFactory);
                                    attributeValueReal.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-STRING":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueString = new AttributeValueString(this, this.loggerFactory);
                                    attributeValueString.ReadXml(subtree);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-XHTML":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    subtree.MoveToContent();
                                    var attributeValueXhtml = new AttributeValueXHTML(this, this.loggerFactory);
                                    attributeValueXhtml.ReadXml(subtree);
                                }
                                break;
                            default:
                                this.logger.LogWarning("The {LocalName} element at line:position {LineNumber}:{LinePosition} is not supported", specElementWithAttributesReader.LocalName, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);
                                break;
                        }
                    }

                    this.ReadObjectSpecificElements(specElementWithAttributesReader);
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="SpecElementWithAttributes"/> object from its XML representation.
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
            
            using (var specElementWithAttributesReader = reader.ReadSubtree())
            {
                while (await specElementWithAttributesReader.ReadAsync())
                {
                    token.ThrowIfCancellationRequested();

                    if (await specElementWithAttributesReader.MoveToContentAsync() == XmlNodeType.Element)
                    {
                        if (await specElementWithAttributesReader.SkipForeignElementAsync(this.logger, token))
                        {
                            continue;
                        }

                        var xmlLineInfo = reader as IXmlLineInfo;

                        switch (specElementWithAttributesReader.LocalName)
                        {
                            case "ALTERNATIVE-ID":
                                await this.ReadAlternativeIdAsync(reader, token);
                                break;
                            case "TYPE":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    await this.ReadSpecTypeAsync(subtree, token);
                                }
                                break;
                            case "CHILDREN":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    await this.ReadHierarchyAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-BOOLEAN":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueBoolean = new AttributeValueBoolean(this, this.loggerFactory);
                                    await attributeValueBoolean.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-DATE":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueDate = new AttributeValueDate(this, this.loggerFactory);
                                    await attributeValueDate.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-ENUMERATION":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueEnumeration = new AttributeValueEnumeration(this, this.loggerFactory);
                                    await attributeValueEnumeration.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-INTEGER":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueInteger = new AttributeValueInteger(this, this.loggerFactory);
                                    await attributeValueInteger.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-REAL":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueReal = new AttributeValueReal(this, this.loggerFactory);
                                    await attributeValueReal.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-STRING":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueString = new AttributeValueString(this, this.loggerFactory);
                                    await attributeValueString.ReadXmlAsync(subtree, token);
                                }
                                break;
                            case "ATTRIBUTE-VALUE-XHTML":
                                using (var subtree = specElementWithAttributesReader.ReadSubtree())
                                {
                                    await subtree.MoveToContentAsync();
                                    var attributeValueXhtml = new AttributeValueXHTML(this, this.loggerFactory);
                                    await attributeValueXhtml.ReadXmlAsync(subtree, token);
                                }
                                break;
                            default:
                                this.logger.LogWarning("The {LocalName} element at line:position {LineNumber}:{LinePosition} is not supported", specElementWithAttributesReader.LocalName, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);
                                break;
                        }
                    }

                    await this.ReadObjectSpecificElementsAsync(specElementWithAttributesReader, token);
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
        /// Asynchronously reads the concrete <see cref="SpecElementWithAttributes"/> elements
        /// </summary>
        /// <param name="reader">
        /// The current <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        protected virtual Task ReadObjectSpecificElementsAsync(XmlReader reader, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="SpecElementWithAttributes"/> sub class
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected abstract void ReadSpecType(XmlReader reader);

        /// <summary>
        /// Asynchronously reads the <see cref="SpecType"/> which is specific to the <see cref="SpecElementWithAttributes"/> sub class
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        protected abstract Task ReadSpecTypeAsync(XmlReader reader, CancellationToken token);

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="SpecElementWithAttributes"/> sub class
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected abstract void ReadHierarchy(XmlReader reader);

        /// <summary>
        /// Asynchronously reads the <see cref="SpecType"/> which is specific to the <see cref="SpecElementWithAttributes"/> sub class
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        protected abstract Task ReadHierarchyAsync(XmlReader reader, CancellationToken token);

        /// <summary>
        /// Converts a <see cref="SpecElementWithAttributes"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            this.WriteValues(writer);
        }

        /// <summary>
        /// Asynchronously converts a <see cref="SpecElementWithAttributes"/> object into its XML representation.
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

            await this.WriteValuesAsync(writer, token);
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

        /// <summary>
        /// Asynchronously writes the <see cref="AttributeValue"/> objects from the <see cref="Values"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteValuesAsync(XmlWriter writer, CancellationToken token)
        {
            if (this.values.Count == 0)
            {
                return;
            }

            await writer.WriteStartElementAsync(null,"VALUES", null);

            foreach (var attributeValue in this.values)
            {
                if (attributeValue is AttributeValueBoolean)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-BOOLEAN", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueDate)
                {
                    await writer.WriteStartElementAsync(null,"ATTRIBUTE-VALUE-DATE", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueEnumeration)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-ENUMERATION", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueInteger)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-INTEGER", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueReal)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-REAL", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueString)
                {
                    await writer.WriteStartElementAsync(null, "ATTRIBUTE-VALUE-STRING", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }

                if (attributeValue is AttributeValueXHTML)
                {
                    await writer.WriteStartElementAsync(null,"ATTRIBUTE-VALUE-XHTML", null);
                    await attributeValue.WriteXmlAsync(writer, token);
                    await writer.WriteEndElementAsync();
                }
            }

            await writer.WriteEndElementAsync();
        }
    }
}
