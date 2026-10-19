// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueXHTML.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="AttributeValueXHTML"/> class is to define an attribute value with XHTML contents.
    /// </summary>
    [ReqIfClass(name: "ATTRIBUTE-VALUE-XHTML", isAbstract: false)]
    public class AttributeValueXHTML : AttributeValue
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<AttributeValueXHTML> logger;

        /// <summary>
        /// The XHTML elements that start and end a line in <see cref="ExtractNormalizedTextFromValue"/>
        /// </summary>
        private static readonly HashSet<string> LineElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "address", "blockquote", "br", "caption", "dd", "div", "dl", "dt", "h1", "h2", "h3", "h4", "h5", "h6",
            "hr", "li", "ol", "p", "pre", "table", "tbody", "tfoot", "thead", "tr", "ul"
        };

        /// <summary>
        /// The XHTML elements that are separated by a space in <see cref="ExtractNormalizedTextFromValue"/>
        /// </summary>
        private static readonly HashSet<string> CellElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "td", "th"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueXHTML"/> class.
        /// </summary>
        public AttributeValueXHTML()
        {
            this.logger = NullLogger<AttributeValueXHTML>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueXHTML"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public AttributeValueXHTML(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AttributeValueXHTML>.Instance : this.loggerFactory.CreateLogger<AttributeValueXHTML>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueXHTML"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal AttributeValueXHTML(SpecElementWithAttributes specElAt, ILoggerFactory loggerFactory)
            : base(specElAt, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AttributeValueXHTML>.Instance : this.loggerFactory.CreateLogger<AttributeValueXHTML>();
        }

        /// <summary>
        /// Instantiated a new instance of the <see cref="AttributeValueXHTML"/> class
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinitionXHTML"/> for which this is the default value</param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinitionXHTML"/>
        /// </remarks>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal AttributeValueXHTML(AttributeDefinitionXHTML attributeDefinition, ILoggerFactory loggerFactory)
            : base(attributeDefinition, loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AttributeValueXHTML>.Instance : this.loggerFactory.CreateLogger<AttributeValueXHTML>();

            this.OwningDefinition = attributeDefinition;
        }

        /// <summary>
        /// Gets or sets the XHTML Content
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.None, lowerValue: 1, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public string TheValue { get; set; }

        /// <summary>
        /// Gets or sets the Linkage to the original attribute value that has been saved if isSimplified is true.
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.None, lowerValue: 0, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public string TheOriginalValue { get; set; }

        /// <summary>
        /// Gets or sets the value of this <see cref="AttributeValue"/>
        /// </summary>
        /// <remarks>
        /// This is a convenience property to get/set TheValue or Values in concrete implementation
        /// </remarks>
        public override object ObjectValue
        {
            get => this.TheValue;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException($"{nameof(value)} cannot be null.");
                }

                this.TheValue = value.ToString();
            }
        }

        /// <summary>
        /// Gets the <see cref="List{ExternalObject}"/> that may be present as xhtml:object in the XHTML content
        /// </summary>
        public List<ExternalObject> ExternalObjects { get; private set; } = new List<ExternalObject>();

        /// <summary>
        /// Gets or sets the Reference to the attribute definition that relates the value to its data type.
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.None, lowerValue: 1, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public AttributeDefinitionXHTML Definition { get; set; }

        /// <summary>
        /// Gets or sets the owning attribute definition.
        /// </summary>
        public AttributeDefinitionXHTML OwningDefinition { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition "/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="AttributeDefinitionXHTML"/>
        /// </returns>
        protected override AttributeDefinition GetAttributeDefinition()
        {
            return this.Definition;
        }

        /// <summary>
        /// Sets the <see cref="AttributeDefinition"/>
        /// </summary>
        /// <param name="attributeDefinition">
        /// The <see cref="AttributeDefinition"/> to set
        /// </param>
        protected override void SetAttributeDefinition(AttributeDefinition attributeDefinition)
        {
            if (attributeDefinition == null)
            {
                throw new ArgumentNullException(nameof(attributeDefinition));
            }

            if (attributeDefinition.GetType() != typeof(AttributeDefinitionXHTML))
            {
                throw new ArgumentException($"{nameof(attributeDefinition)} must of type AttributeDefinitionXHTML");
            }

            this.Definition = (AttributeDefinitionXHTML)attributeDefinition;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute value is a simplified representation of the original value.
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.None, lowerValue: 0, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public bool IsSimplified { get; set; }

        /// <summary>
        /// extract text from the value of the removing all the formatting xhtml nodes 
        /// </summary>
        /// <returns>
        /// a string stripped of xhtml formatting elements
        /// </returns>
        public string ExtractUnformattedTextFromValue()
        {
            if (string.IsNullOrEmpty(this.TheValue))
            {
                return string.Empty;
            }

            var result = new List<string>();
            
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(this.TheValue)))
            {
                using (var xmlTextReader = new XmlTextReader(memoryStream))
                {
                    xmlTextReader.Namespaces = false;

                    xmlTextReader.MoveToContent();

                    using (var subtree = xmlTextReader.ReadSubtree())
                    {
                        while (subtree.Read())
                        {
                            if (subtree.MoveToContent() == XmlNodeType.Text)
                            {
                                result.Add(subtree.Value);
                            }
                        }
                    }
                }
            }

            return result.Any() ? string.Join(" ", result) : string.Empty;
        }

        /// <summary>
        /// Extracts the text of the value as plain text with normalized whitespace: outside <c>pre</c> elements, every run
        /// of whitespace (space, tab, carriage return and line feed) is collapsed to one space, and each line is trimmed.
        /// Text is joined as it is written in the XHTML, so no space is added around inline elements such as <c>b</c> or
        /// <c>span</c>.
        /// </summary>
        /// <remarks>
        /// Block elements (such as <c>p</c>, <c>div</c>, <c>li</c>, the headings and table rows) and <c>br</c> separate
        /// lines, and table cells are separated by a space. Unlike <see cref="ExtractUnformattedTextFromValue"/>, all
        /// top-level elements of the value are read. Non-breaking spaces (U+00A0, <c>&amp;#160;</c>) are kept as they
        /// are: they are not collapsed, not replaced and not trimmed. HTML named entities such as <c>&amp;nbsp;</c> and
        /// <c>&amp;eacute;</c>, which no DTD declares in ReqIF, are turned into their characters.
        /// <para>
        /// The text of a <c>pre</c> element is kept as written: its spaces, tabs and indentation are not collapsed, and
        /// each of its lines is a line of the result. Blank lines at the start and end of a <c>pre</c> element are left
        /// out, and blank lines inside it are kept only when <paramref name="keepLineBreaks"/> is true.
        /// </para>
        /// </remarks>
        /// <param name="keepLineBreaks">
        /// true to separate the lines with a line feed (<c>\n</c>); false to separate them with a space
        /// </param>
        /// <returns>
        /// The plain text; an empty string when the value is empty
        /// </returns>
        /// <exception cref="XmlException">
        /// The value is not well-formed XML, or it uses an entity that is neither an XML entity nor an HTML named entity
        /// </exception>
        public string ExtractNormalizedTextFromValue(bool keepLineBreaks = false)
        {
            if (string.IsNullOrEmpty(this.TheValue))
            {
                return string.Empty;
            }

            // every line, with whether it is part of a pre block, whose whitespace is kept as written
            var lines = new List<(StringBuilder Text, bool Preformatted)> { (new StringBuilder(), false) };
            var preDepth = 0;

            // the value is read as a fragment: it may hold several top-level elements and text, and prefixes without a
            // namespace declaration; line and position of an error are those in the value
            using (var xmlTextReader = new XmlTextReader(this.TheValue, XmlNodeType.Element, new XmlParserContext(null, null, null, XmlSpace.None)))
            {
                xmlTextReader.Namespaces = false;
                xmlTextReader.DtdProcessing = DtdProcessing.Prohibit;
                xmlTextReader.XmlResolver = null;
                xmlTextReader.WhitespaceHandling = WhitespaceHandling.All;

                while (xmlTextReader.Read())
                {
                    switch (xmlTextReader.NodeType)
                    {
                        case XmlNodeType.Text:
                        case XmlNodeType.CDATA:
                        case XmlNodeType.Whitespace:
                        case XmlNodeType.SignificantWhitespace:
                            AppendText(lines, xmlTextReader.Value, preDepth > 0);
                            break;
                        case XmlNodeType.EntityReference:
                            // an HTML named entity such as &nbsp;, which no DTD declares: the reader does not expand it
                            AppendText(lines, DecodeHtmlEntity(xmlTextReader), preDepth > 0);
                            break;
                        case XmlNodeType.Element:
                        case XmlNodeType.EndElement:
                            var name = xmlTextReader.Name.Substring(xmlTextReader.Name.LastIndexOf(':') + 1);

                            if (string.Equals(name, "pre", StringComparison.OrdinalIgnoreCase) && !xmlTextReader.IsEmptyElement)
                            {
                                preDepth += xmlTextReader.NodeType == XmlNodeType.Element ? 1 : -1;
                            }

                            if (LineElements.Contains(name))
                            {
                                lines.Add((new StringBuilder(), preDepth > 0));
                            }
                            else if (CellElements.Contains(name))
                            {
                                lines[lines.Count - 1].Text.Append(' ');
                            }

                            break;
                    }
                }
            }

            var result = new List<string>();
            var index = 0;

            while (index < lines.Count)
            {
                if (!lines[index].Preformatted)
                {
                    var line = CollapseWhitespace(lines[index].Text.ToString());

                    if (line.Length > 0)
                    {
                        result.Add(line);
                    }

                    index++;
                    continue;
                }

                // a pre block: its lines as written, without the blank lines at its start and end
                var block = new List<string>();

                while (index < lines.Count && lines[index].Preformatted)
                {
                    block.Add(lines[index].Text.ToString());
                    index++;
                }

                var first = block.FindIndex(x => !IsBlank(x));

                if (first < 0)
                {
                    continue;
                }

                var kept = block.GetRange(first, block.FindLastIndex(x => !IsBlank(x)) - first + 1);

                // on one line, the blank lines inside the block would only add spaces
                result.AddRange(keepLineBreaks ? kept : kept.Where(x => !IsBlank(x)));
            }

            return string.Join(keepLineBreaks ? "\n" : " ", result);
        }

        /// <summary>
        /// Appends text to the last line; in a pre block, every line break of the text starts a new line
        /// </summary>
        private static void AppendText(List<(StringBuilder Text, bool Preformatted)> lines, string text, bool preformatted)
        {
            if (!preformatted)
            {
                lines[lines.Count - 1].Text.Append(text);
                return;
            }

            var parts = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            lines[lines.Count - 1].Text.Append(parts[0]);

            for (var i = 1; i < parts.Length; i++)
            {
                lines.Add((new StringBuilder(parts[i]), true));
            }
        }

        /// <summary>
        /// Returns the character(s) of the HTML named entity that the reader is on, such as <c>nbsp</c> or <c>eacute</c>
        /// </summary>
        /// <exception cref="XmlException">
        /// The entity is not an HTML named entity
        /// </exception>
        private static string DecodeHtmlEntity(XmlTextReader reader)
        {
            var reference = $"&{reader.Name};";
            var decoded = WebUtility.HtmlDecode(reference);

            if (decoded == reference)
            {
                throw new XmlException($"The entity {reference} is not an XML or HTML entity", null, reader.LineNumber, reader.LinePosition);
            }

            return decoded;
        }

        /// <summary>
        /// Checks whether a line holds only XML whitespace (space, tab, carriage return, line feed), or nothing
        /// </summary>
        private static bool IsBlank(string line)
        {
            return CollapseWhitespace(line).Length == 0;
        }

        /// <summary>
        /// Collapses every run of XML whitespace (space, tab, carriage return, line feed) to one space and trims the result
        /// </summary>
        private static string CollapseWhitespace(string text)
        {
            var result = new StringBuilder(text.Length);
            var whitespace = false;

            foreach (var character in text)
            {
                if (character == ' ' || character == '\t' || character == '\r' || character == '\n')
                {
                    whitespace = true;
                    continue;
                }

                if (whitespace && result.Length > 0)
                {
                    result.Append(' ');
                }

                whitespace = false;
                result.Append(character);
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates a <see cref="AttributeValueXHTML"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <remarks>
        /// Only the direct children (DEFINITION, THE-VALUE and THE-ORIGINAL-VALUE) are read, in any order.
        /// <see cref="XmlReader.ReadInnerXml"/> and <see cref="XmlReader.Skip"/> already move the reader to the
        /// next node, so the loop does not call <see cref="XmlReader.Read"/> after them; otherwise the next sibling
        /// would be skipped when there is no whitespace between the elements.
        /// </remarks>
        internal override void ReadXml(XmlReader reader)
        {
            this.ReadXmlAttributes(reader);

            using (var subtree = reader.ReadSubtree())
            {
                subtree.MoveToContent();
                subtree.Read();

                while (!subtree.EOF)
                {
                    if (subtree.NodeType != XmlNodeType.Element || subtree.Depth != 1)
                    {
                        subtree.Read();
                        continue;
                    }

                    if (subtree.SkipForeignElement(this.logger))
                    {
                        subtree.Read();
                        continue;
                    }

                    switch (subtree.LocalName)
                    {
                        case "DEFINITION":
                            using (var definitionSubtree = subtree.ReadSubtree())
                            {
                                this.ReadDefinition(definitionSubtree);
                            }

                            subtree.Read();
                            break;
                        case "THE-VALUE":
                            this.TheValue = subtree.ReadInnerXml().Trim();

                            if (this.TheValue.Contains("xhtml:object") && this.TheValue.Contains("data"))
                            {
                                var externalObjects = this.CreateExternalObjects(this.TheValue);
                                this.ExternalObjects.AddRange(externalObjects);
                            }

                            break;
                        case "THE-ORIGINAL-VALUE":
                            this.TheOriginalValue = subtree.ReadInnerXml().Trim();
                            break;
                        default:
                            subtree.Skip();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="AttributeValueXHTML"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <remarks>
        /// Only the direct children (DEFINITION, THE-VALUE and THE-ORIGINAL-VALUE) are read, in any order.
        /// <see cref="XmlReader.ReadInnerXmlAsync"/> and <see cref="XmlReader.SkipAsync"/> already move the reader to the
        /// next node, so the loop does not call <see cref="XmlReader.ReadAsync"/> after them; otherwise the next sibling
        /// would be skipped when there is no whitespace between the elements.
        /// </remarks>
        internal override async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            this.ReadXmlAttributes(reader);

            using (var subtree = reader.ReadSubtree())
            {
                await subtree.MoveToContentAsync();
                await subtree.ReadAsync();

                while (!subtree.EOF)
                {
                    token.ThrowIfCancellationRequested();

                    if (subtree.NodeType != XmlNodeType.Element || subtree.Depth != 1)
                    {
                        await subtree.ReadAsync();
                        continue;
                    }

                    if (await subtree.SkipForeignElementAsync(this.logger, token))
                    {
                        await subtree.ReadAsync();
                        continue;
                    }

                    switch (subtree.LocalName)
                    {
                        case "DEFINITION":
                            using (var definitionSubtree = subtree.ReadSubtree())
                            {
                                await this.ReadDefinitionAsync(definitionSubtree, token);
                            }

                            await subtree.ReadAsync();
                            break;
                        case "THE-VALUE":
                            this.TheValue = (await subtree.ReadInnerXmlAsync()).Trim();

                            if (this.TheValue.Contains("xhtml:object") && this.TheValue.Contains("data"))
                            {
                                var externalObjects = await this.CreateExternalObjectsAsync(this.TheValue);
                                this.ExternalObjects.AddRange(externalObjects);
                            }

                            break;
                        case "THE-ORIGINAL-VALUE":
                            this.TheOriginalValue = (await subtree.ReadInnerXmlAsync()).Trim();
                            break;
                        default:
                            await subtree.SkipAsync();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Reads the <see cref="AttributeDefinitionXHTML"/> referenced by the DEFINITION element
        /// </summary>
        /// <param name="reader">
        /// an <see cref="XmlReader"/> over the subtree of the DEFINITION element
        /// </param>
        private void ReadDefinition(XmlReader reader)
        {
            reader.MoveToContent();

            if (reader.ReadToReqIFDescendant("ATTRIBUTE-DEFINITION-XHTML-REF", this.logger))
            {
                this.SetDefinition(reader.ReadElementContentAsStringToEndElement());
            }

            reader.SkipForeignElementsToEnd(this.logger);
        }

        /// <summary>
        /// Asynchronously reads the <see cref="AttributeDefinitionXHTML"/> referenced by the DEFINITION element
        /// </summary>
        /// <param name="reader">
        /// an <see cref="XmlReader"/> over the subtree of the DEFINITION element
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task ReadDefinitionAsync(XmlReader reader, CancellationToken token)
        {
            await reader.MoveToContentAsync();

            if (await reader.ReadToReqIFDescendantAsync("ATTRIBUTE-DEFINITION-XHTML-REF", this.logger, token))
            {
                this.SetDefinition(await reader.ReadElementContentAsStringToEndElementAsync());
            }

            await reader.SkipForeignElementsToEndAsync(this.logger, token);
        }

        /// <summary>
        /// Sets the <see cref="Definition"/> to the <see cref="AttributeDefinitionXHTML"/> with the referenced identifier
        /// </summary>
        /// <param name="reference">
        /// The identifier of the <see cref="AttributeDefinitionXHTML"/>
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="AttributeDefinitionXHTML"/> could not be found
        /// </exception>
        private void SetDefinition(string reference)
        {
            this.Definition = this.ReqIFContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionXHTML>().SingleOrDefault(x => x.Identifier == reference);

            if (this.Definition == null)
            {
                throw new InvalidOperationException($"The attribute-definition XHTML {reference} could not be found for the value.");
            }
        }

        /// <summary>
        /// Reads the properties that are defined as XML Attributes (IS-SIMPLIFIED)
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        private void ReadXmlAttributes(XmlReader reader)
        {
            var xmlLineInfo = reader as IXmlLineInfo;

            this.logger.LogTrace("reading IS-SIMPLIFIED at line:position {LineNumber}:{LinePosition}", xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

            var isSimplified = reader.GetAttribute("IS-SIMPLIFIED");
            if (!string.IsNullOrWhiteSpace(isSimplified))
            {
                try
                {
                    this.IsSimplified = XmlConvert.ToBoolean(isSimplified);
                }
                catch (Exception e)
                {
                    throw new SerializationException($"The AttributeValueXHTML.IS-SIMPLIFIED {isSimplified} at line:position {xmlLineInfo?.LineNumber}:{xmlLineInfo?.LinePosition} could not be converted to a BOOLEAN", e);
                }
            }
        }

        /// <summary>
        /// Creates <see cref="ExternalObject"/>s from provided XHTML
        /// </summary>
        /// <param name="xhtml">
        /// The XHTML that may contain <see cref="ExternalObject"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{ExternalObject}"/> which may be empty
        /// </returns>
        private IEnumerable<ExternalObject> CreateExternalObjects(string xhtml)
        {
            var sw = Stopwatch.StartNew();

            var result = new List<ExternalObject>();

            var settings = new XmlReaderSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Fragment
            };

            var xmlReader = XmlReader.Create(new StringReader(xhtml), settings);
            while (xmlReader.Read())
            {
                if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.LocalName == "object")
                {
                    var mimeTypeAttribute = xmlReader.GetAttribute("type");
                    var uriAttribute = xmlReader.GetAttribute("data");
                    if (!string.IsNullOrEmpty(uriAttribute))
                    {
                        uriAttribute = Uri.UnescapeDataString(uriAttribute);
                    }
                    var heightAttribute = xmlReader.GetAttribute("height");
                    var widthAttribute = xmlReader.GetAttribute("width");

                    var externalObject = new ExternalObject(this)
                    {
                        MimeType = mimeTypeAttribute,
                        Uri = uriAttribute
                    };

                    if (!string.IsNullOrEmpty(heightAttribute) && int.TryParse(heightAttribute, out int height))
                    {
                        externalObject.Height = height;
                    }

                    if (!string.IsNullOrEmpty(widthAttribute) && int.TryParse(widthAttribute, out int width))
                    {
                        externalObject.Width = width;
                    }

                    result.Add(externalObject);
                }
            }

            this.logger.LogTrace("a total of {Count} ExternalObjects have been created in {Time} [sw]", result.Count, sw.ElapsedMilliseconds);

            return result;
        }

        /// <summary>
        /// Creates <see cref="ExternalObject"/>s from provided XHTML
        /// </summary>
        /// <param name="xhtml">
        /// The XHTML that may contain <see cref="ExternalObject"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{ExternalObject}"/> which may be empty
        /// </returns>
        private async Task<IEnumerable<ExternalObject>> CreateExternalObjectsAsync(string xhtml)
        {
            var sw = Stopwatch.StartNew();

            var result = new List<ExternalObject>();

            var settings = new XmlReaderSettings
            {
                Async = true,
                ConformanceLevel = ConformanceLevel.Fragment
            };

            var xmlReader = XmlReader.Create(new StringReader(xhtml), settings);
            while (xmlReader.Read())
            {
                if (await xmlReader.MoveToContentAsync() == XmlNodeType.Element && xmlReader.LocalName == "object")
                {
                    var mimeTypeAttribute = xmlReader.GetAttribute("type");
                    var uriAttribute = xmlReader.GetAttribute("data");
                    if (!string.IsNullOrEmpty(uriAttribute))
                    {
                        uriAttribute = Uri.UnescapeDataString(uriAttribute);
                    }
                    var heightAttribute = xmlReader.GetAttribute("height");
                    var widthAttribute = xmlReader.GetAttribute("width");
                    
                    var externalObject = new ExternalObject(this)
                    {
                        MimeType = mimeTypeAttribute,
                        Uri = uriAttribute
                    };

                    if (!string.IsNullOrEmpty(heightAttribute) && int.TryParse(heightAttribute, out int height))
                    {
                        externalObject.Height = height;
                    }

                    if (!string.IsNullOrEmpty(widthAttribute) && int.TryParse(widthAttribute, out int width))
                    {
                        externalObject.Width = width;
                    }

                    result.Add(externalObject);
                }
            }
            
            this.logger.LogTrace("a total of {Count} ExternalObjects have been created in {Time} [sw]", result.Count, sw.ElapsedMilliseconds);

            return result;
        }

        /// <summary>
        /// Converts a <see cref="AttributeValueXHTML"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Definition"/> may not be null
        /// </exception>
        internal override void WriteXml(XmlWriter writer)
        {
            if (this.Definition == null)
            {
                throw new SerializationException("The Definition property of an AttributeValueXHTML may not be null");
            }

            if (this.IsSimplified)
            {
                writer.WriteAttributeString("IS-SIMPLIFIED", "true");
            }

            writer.WriteStartElement("DEFINITION");
            writer.WriteElementString("ATTRIBUTE-DEFINITION-XHTML-REF", this.Definition.Identifier);
            writer.WriteEndElement();

            writer.WriteStartElement("THE-VALUE");
            writer.WriteRaw(this.TheValue);
            writer.WriteEndElement();

            if (!string.IsNullOrEmpty(this.TheOriginalValue))
            {
                writer.WriteStartElement("THE-ORIGINAL-VALUE");
                writer.WriteRaw(this.TheOriginalValue);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeValueXHTML"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Definition"/> may not be null
        /// </exception>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            if (this.Definition == null)
            {
                throw new SerializationException("The Definition property of an AttributeValueXHTML may not be null");
            }

            token.ThrowIfCancellationRequested();

            if (this.IsSimplified)
            {
                await writer.WriteAttributeStringAsync(null, "IS-SIMPLIFIED", null, "true");
            }

            await writer.WriteStartElementAsync(null, "DEFINITION", null);
            await writer.WriteElementStringAsync(null, "ATTRIBUTE-DEFINITION-XHTML-REF", null, this.Definition.Identifier);
            await writer.WriteEndElementAsync();

            await writer.WriteStartElementAsync(null, "THE-VALUE", null);
            await writer.WriteRawAsync(this.TheValue);
            await writer.WriteEndElementAsync();

            if (!string.IsNullOrEmpty(this.TheOriginalValue))
            {
                await writer.WriteStartElementAsync(null, "THE-ORIGINAL-VALUE", null);
                await writer.WriteRawAsync(this.TheOriginalValue);
                await writer.WriteEndElementAsync();
            }
        }
    }
}
