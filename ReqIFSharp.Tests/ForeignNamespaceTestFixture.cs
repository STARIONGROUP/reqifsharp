// -------------------------------------------------------------------------------------------------
// <copyright file="ForeignNamespaceTestFixture.cs" company="Starion Group S.A.">
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

namespace ReqIFSharp.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// Verifies that an element with the local name of a ReqIF element, but in another namespace, is not read as
    /// a ReqIF element: it is skipped, with its content, and a warning is logged.
    /// </summary>
    [TestFixture]
    public class ForeignNamespaceTestFixture
    {
        private const string FileName = "reqifsharpgenerated.reqif";

        private const string WarningText = "it is not a ReqIF element and has been skipped";

        private static readonly XNamespace Other = "http://example.com/other";

        /// <summary>
        /// Test documents that can be read; the ones that cannot be read on purpose are left out
        /// </summary>
        private static readonly string[] ReadableFiles =
        {
            "Datatype-Demo.reqif",
            "DefaultValueDemo.reqif",
            "ProR_Traceability-Template-v1.0.reqif",
            "output.reqif",
            "reqifsharpgenerated.reqif",
            "testreqif.reqif",
            "Spielwiese.reqifz",
            "requirements-and-objects.reqifz",
            "test-multiple-reqif.reqifz"
        };

        /// <summary>
        /// The places where a foreign element is added, with the test document it is added to. Each foreign element is
        /// a copy of a ReqIF element (or a new element) that would change the object graph when it was read as a ReqIF element.
        /// </summary>
        private static readonly object[] Places =
        {
            new object[] { FileName, "root child" },
            new object[] { FileName, "header field" },
            new object[] { FileName, "content section" },
            new object[] { FileName, "datatype" },
            new object[] { FileName, "enum value" },
            new object[] { FileName, "embedded value" },
            new object[] { FileName, "spec type" },
            new object[] { FileName, "spec type child" },
            new object[] { FileName, "attribute definition" },
            new object[] { FileName, "attribute definition child BOOLEAN" },
            new object[] { FileName, "attribute definition child DATE" },
            new object[] { FileName, "attribute definition child ENUMERATION" },
            new object[] { FileName, "attribute definition child INTEGER" },
            new object[] { FileName, "attribute definition child REAL" },
            new object[] { FileName, "attribute definition child STRING" },
            new object[] { FileName, "attribute definition child XHTML" },
            new object[] { FileName, "spec object" },
            new object[] { FileName, "empty spec object" },
            new object[] { FileName, "attribute value" },
            new object[] { FileName, "enumeration value reference" },
            new object[] { "DefaultValueDemo.reqif", "default value enumeration value reference" },
            new object[] { FileName, "xhtml value" },
            new object[] { FileName, "specification" },
            new object[] { FileName, "spec hierarchy" },
            new object[] { FileName, "spec hierarchy object" },
            new object[] { FileName, "nested spec hierarchy" },
            new object[] { FileName, "spec relation" },
            new object[] { FileName, "relation group" },
            new object[] { FileName, "relation group spec relations" },
            new object[] { FileName, "relation group spec relation ref" },
            new object[] { FileName, "alternative id" },
            new object[] { FileName, "tool extension" },
            new object[] { FileName, "spec object type reference before" },
            new object[] { FileName, "spec object type reference after" },
            new object[] { FileName, "attribute value definition reference before" },
            new object[] { FileName, "attribute value definition reference after" },
            new object[] { FileName, "xhtml value definition reference before" },
            new object[] { FileName, "xhtml value definition reference after" },
            new object[] { FileName, "spec relation source reference before" },
            new object[] { FileName, "spec relation target reference after" },
            new object[] { FileName, "spec hierarchy object reference before" },
            new object[] { FileName, "specification type reference before" },
            new object[] { FileName, "relation group source specification reference before" },
            new object[] { FileName, "relation group type reference after" },
            new object[] { FileName, "spec hierarchy object reference after" },
            new object[] { FileName, "specification type reference after" },
            new object[] { FileName, "spec relation type reference before" },
            new object[] { FileName, "spec relation type reference after" },
            new object[] { FileName, "attribute value definition reference before BOOLEAN" },
            new object[] { FileName, "attribute value definition reference before DATE" },
            new object[] { FileName, "attribute value definition reference before INTEGER" },
            new object[] { FileName, "attribute value definition reference before REAL" },
            new object[] { FileName, "attribute value definition reference after ENUMERATION" },
            new object[] { FileName, "spec relation source wrapper" },
            new object[] { FileName, "relation group target specification wrapper" },
            new object[] { FileName, "attribute value definition wrapper BOOLEAN" },
            new object[] { FileName, "attribute value definition wrapper DATE" },
            new object[] { FileName, "attribute value definition wrapper INTEGER" },
            new object[] { FileName, "attribute value definition wrapper REAL" },
            new object[] { FileName, "attribute value definition wrapper STRING" },
            new object[] { FileName, "attribute definition type reference after BOOLEAN" },
            new object[] { FileName, "attribute definition type reference after DATE" },
            new object[] { FileName, "attribute definition type reference after ENUMERATION" },
            new object[] { FileName, "attribute definition type reference after INTEGER" },
            new object[] { FileName, "attribute definition type reference after REAL" },
            new object[] { FileName, "attribute definition type reference after STRING" },
            new object[] { FileName, "attribute definition type reference after XHTML" },
            new object[] { FileName, "empty element before the xhtml value" },
            new object[] { FileName, "empty element between the xhtml values" }
        };

        [TestCaseSource(nameof(ReadableFiles))]
        public async Task Verify_that_no_element_of_a_test_document_is_skipped(string fileName)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", fileName);
            var sources = new List<(string Name, Func<Stream> Open, SupportedFileExtensionKind Kind)>();

            if (fileName.EndsWith(".reqifz", StringComparison.Ordinal))
            {
                sources.Add(("original", () => File.OpenRead(path), SupportedFileExtensionKind.Reqifz));
            }
            else
            {
                var original = TestDocuments.Read(fileName);

                foreach (var (name, xml) in new[]
                         {
                             ("original", original),
                             ("minified", TestDocuments.Minify(original)),
                             ("prefixed", TestDocuments.Prefix(original)),
                             ("without namespace", TestDocuments.RemoveNamespace(original))
                         })
                {
                    sources.Add((name, () => TestDocuments.ToStream(xml, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif));
                }
            }

            foreach (var source in sources)
            {
                foreach (var async in new[] { false, true })
                {
                    using var stream = source.Open();

                    var (reqifs, warnings) = await Deserialize(stream, source.Kind, async);

                    Assert.That(reqifs, Is.Not.Empty);
                    Assert.That(warnings, Is.Empty, $"{source.Name}, async: {async}");
                }
            }
        }

        [TestCaseSource(nameof(Places))]
        public async Task Verify_that_a_foreign_element_is_skipped_with_a_warning(string fileName, string place)
        {
            var (original, withForeignElement) = CreateDocuments(fileName, place);

            Assert.That(withForeignElement, Does.Contain("xmlns:other="), "the document must contain a foreign element");

            foreach (var (name, expectedXml, actualXml) in new[]
                     {
                         ("indented", original, withForeignElement),
                         ("minified", TestDocuments.Minify(original), TestDocuments.Minify(withForeignElement))
                     })
            {
                foreach (var kind in TestDocuments.Kinds)
                {
                    foreach (var async in new[] { false, true })
                    {
                        var because = $"{place}, {name}, {kind}, async: {async}";

                        var (expected, _) = await Deserialize(TestDocuments.ToStream(expectedXml, kind), kind, async);
                        var (actual, warnings) = await Deserialize(TestDocuments.ToStream(actualXml, kind), kind, async);

                        TestDocuments.AssertSameGraph(expected, actual);

                        Assert.That(warnings, Has.Count.EqualTo(1), because);
                        Assert.That(warnings[0], Does.Contain("other:").And.Contain(Other.NamespaceName), because);
                    }
                }
            }
        }

        // the emptied container comes before the other one in the test document, so a search that runs past its end finds the other reference
        [TestCase("SPEC-RELATION", "TARGET", "SPEC-OBJECT-REF", false)]
        [TestCase("SPEC-RELATION", "TARGET", "SPEC-OBJECT-REF", true)]
        [TestCase("RELATION-GROUP", "SOURCE-SPECIFICATION", "SPECIFICATION-REF", false)]
        [TestCase("RELATION-GROUP", "SOURCE-SPECIFICATION", "SPECIFICATION-REF", true)]
        public async Task Verify_that_a_reference_container_with_only_a_foreign_reference_is_read_as_empty(string element, string container, string reference, bool async)
        {
            var ns = TestDocuments.ReqIFNamespace;
            var document = XDocument.Parse(TestDocuments.Read(FileName));
            document.Root!.SetAttributeValue(XNamespace.Xmlns + "other", Other.NamespaceName);

            var owner = document.Root.Descendants(ns + element).First();
            var identifier = owner.Attribute("IDENTIFIER")!.Value;

            // the other reference container of the element (SOURCE for TARGET, TARGET-SPECIFICATION for SOURCE-SPECIFICATION), after the emptied one
            var otherContainer = owner.Elements().Single(x => x.Name != ns + container && x.Element(ns + reference) != null);
            Assert.That(owner.Element(ns + container)!.IsBefore(otherContainer), Is.True, "the emptied container must come first");
            var otherReference = otherContainer.Element(ns + reference)!.Value;

            // only a foreign reference, to the element the other container refers to
            owner.Element(ns + container)!.Element(ns + reference)!.ReplaceWith(new XElement(Other + reference, otherReference));

            var xml = document.ToString();

            foreach (var copy in new[] { xml, TestDocuments.Minify(xml) })
            {
                var (reqifs, warnings) = await Deserialize(TestDocuments.ToStream(copy, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif, async);
                var content = reqifs.Single().CoreContent;

                Assert.Multiple(() =>
                {
                    if (element == "SPEC-RELATION")
                    {
                        var specRelation = content.SpecRelations.Single(x => x.Identifier == identifier);
                        Assert.That(specRelation.Target, Is.Null);
                        Assert.That(specRelation.Source?.Identifier, Is.EqualTo(otherReference));
                    }
                    else
                    {
                        var relationGroup = content.SpecRelationGroups.Single(x => x.Identifier == identifier);
                        Assert.That(relationGroup.SourceSpecification, Is.Null);
                        Assert.That(relationGroup.TargetSpecification?.Identifier, Is.EqualTo(otherReference));
                    }

                    Assert.That(warnings, Has.Count.EqualTo(1));
                });
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_the_xhtml_content_of_a_default_value_is_read_and_not_skipped(bool async)
        {
            var ns = TestDocuments.ReqIFNamespace;
            XNamespace xhtml = "http://www.w3.org/1999/xhtml";

            var document = XDocument.Parse(TestDocuments.Read(FileName));
            var definition = document.Root!.Descendants(ns + "ATTRIBUTE-DEFINITION-XHTML").First();
            var identifier = definition.Attribute("IDENTIFIER")!.Value;

            // as the serializer writes it
            definition.Add(new XElement(ns + "DEFAULT-VALUE",
                new XElement(ns + "ATTRIBUTE-VALUE-XHTML",
                    new XElement(ns + "THE-VALUE", new XElement(xhtml + "div", new XElement(xhtml + "p", "default"), new XElement(xhtml + "br"))),
                    new XElement(ns + "DEFINITION", new XElement(ns + "ATTRIBUTE-DEFINITION-XHTML-REF", identifier)))));

            var xml = document.ToString();

            foreach (var copy in new[] { xml, TestDocuments.Minify(xml) })
            {
                var (reqifs, warnings) = await Deserialize(TestDocuments.ToStream(copy, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif, async);

                var defaultValue = reqifs.Single().CoreContent.SpecTypes.SelectMany(x => x.SpecAttributes).OfType<AttributeDefinitionXHTML>().Single(x => x.Identifier == identifier).DefaultValue;

                Assert.That(defaultValue?.TheValue, Does.Contain("default").And.Contain(":br"));
                Assert.That(warnings, Is.Empty);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_an_attribute_value_created_with_a_logger_factory_logs_a_skipped_element(bool async)
        {
            const string enumerationXml = "<ATTRIBUTE-VALUE-ENUMERATION xmlns=\"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd\"><VALUES><other:ENUM-VALUE-REF xmlns:other=\"http://example.com/other\">x</other:ENUM-VALUE-REF></VALUES></ATTRIBUTE-VALUE-ENUMERATION>";
            const string stringXml = "<ATTRIBUTE-VALUE-STRING xmlns=\"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd\" THE-VALUE=\"x\"><DEFINITION><other:ATTRIBUTE-DEFINITION-STRING-REF xmlns:other=\"http://example.com/other\">x</other:ATTRIBUTE-DEFINITION-STRING-REF></DEFINITION></ATTRIBUTE-VALUE-STRING>";
            const string xhtmlXml = "<ATTRIBUTE-VALUE-XHTML xmlns=\"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd\"><other:THE-VALUE xmlns:other=\"http://example.com/other\">x</other:THE-VALUE></ATTRIBUTE-VALUE-XHTML>";

            foreach (var (xml, create) in new (string, Func<ILoggerFactory, AttributeValue>)[]
                     {
                         (enumerationXml, factory => new AttributeValueEnumeration(factory)),
                         (xhtmlXml, factory => new AttributeValueXHTML(factory)),
                         (stringXml, factory => new AttributeValueString(factory))
                     })
            {
                var sink = new TestLogEventSink();
                var serilogLogger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(sink).CreateLogger();

                using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(serilogLogger, dispose: true));
                using var reader = System.Xml.XmlReader.Create(new StringReader(xml), new System.Xml.XmlReaderSettings { Async = async });
                reader.MoveToContent();

                var attributeValue = create(loggerFactory);

                if (async)
                {
                    await attributeValue.ReadXmlAsync(reader, CancellationToken.None);
                }
                else
                {
                    attributeValue.ReadXml(reader);
                }

                Assert.That(sink.Events.Count(e => e.Level == LogEventLevel.Warning && e.MessageTemplate.Text.Contains(WarningText)), Is.EqualTo(1), attributeValue.GetType().Name);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_an_element_in_no_namespace_in_a_document_in_the_ReqIF_namespace_is_read(bool async)
        {
            // elements in no namespace are read as ReqIF elements in any document
            var document = XDocument.Parse(TestDocuments.Read(FileName));
            var title = document.Root!.Descendants(TestDocuments.ReqIFNamespace + "TITLE").Single();
            title.AddAfterSelf(new XElement(XNamespace.None + "TITLE", "title in no namespace"));

            var xml = document.ToString();

            Assert.That(xml, Does.Contain("<TITLE xmlns=\"\">"), "the document must contain a TITLE in no namespace");

            foreach (var copy in new[] { xml, TestDocuments.Minify(xml) })
            {
                var (reqifs, warnings) = await Deserialize(TestDocuments.ToStream(copy, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif, async);

                Assert.That(reqifs.Single().TheHeader.Title, Is.EqualTo("title in no namespace"));
                Assert.That(warnings, Is.Empty);
            }
        }

        /// <summary>
        /// Returns the test document without and with one foreign element at the given place. Both are prepared
        /// the same way first, when the place needs ReqIF elements that the test document does not have.
        /// </summary>
        private static (string Expected, string Actual) CreateDocuments(string fileName, string place)
        {
            var ns = TestDocuments.ReqIFNamespace;
            var document = XDocument.Parse(TestDocuments.Read(fileName));

            if (place == "relation group spec relation ref")
            {
                var specRelation = document.Root!.Descendants(ns + "SPEC-RELATION").First().Attribute("IDENTIFIER")!.Value;
                document.Root.Descendants(ns + "RELATION-GROUP").First().Add(new XElement(ns + "SPEC-RELATIONS", new XElement(ns + "SPEC-RELATION-REF", specRelation)));
            }

            var expected = document.ToString();

            return (expected, AddForeignElement(expected, place));
        }

        /// <summary>
        /// Returns a copy of the document with one foreign element at the given place
        /// </summary>
        private static string AddForeignElement(string original, string place)
        {
            var ns = TestDocuments.ReqIFNamespace;
            var document = XDocument.Parse(original);
            var root = document.Root!;

            root.SetAttributeValue(XNamespace.Xmlns + "other", Other.NamespaceName);

            XElement First(string localName) => root.Descendants(ns + localName).First();

            // a copy of the element, in the foreign namespace, with a new identifier
            XElement ForeignCopy(XElement element)
            {
                var copy = new XElement(element) { Name = Other + element.Name.LocalName };

                if (copy.Attribute("IDENTIFIER") != null)
                {
                    copy.SetAttributeValue("IDENTIFIER", "foreign-" + copy.Attribute("IDENTIFIER")!.Value);
                }

                return copy;
            }

            switch (place)
            {
                case "header field":
                    First("TITLE").AddAfterSelf(new XElement(Other + "TITLE", "foreign title"));
                    break;
                case "content section":
                    // a ReqIF spec object with a new identifier, in a foreign SPEC-OBJECTS
                    var specObject = new XElement(First("SPEC-OBJECT"));
                    specObject.SetAttributeValue("IDENTIFIER", "foreign-spec-object");
                    First("SPEC-OBJECTS").AddAfterSelf(new XElement(Other + "SPEC-OBJECTS", specObject));
                    break;
                case "datatype":
                    First("DATATYPE-DEFINITION-STRING").AddAfterSelf(ForeignCopy(First("DATATYPE-DEFINITION-STRING")));
                    break;
                case "enum value":
                    First("ENUM-VALUE").AddAfterSelf(ForeignCopy(First("ENUM-VALUE")));
                    break;
                case "spec type":
                    First("SPEC-OBJECT-TYPE").AddAfterSelf(ForeignCopy(First("SPEC-OBJECT-TYPE")));
                    break;
                case "attribute definition":
                    First("ATTRIBUTE-DEFINITION-STRING").AddAfterSelf(ForeignCopy(First("ATTRIBUTE-DEFINITION-STRING")));
                    break;
                case "spec object":
                    First("SPEC-OBJECT").AddAfterSelf(ForeignCopy(First("SPEC-OBJECT")));
                    break;
                case "empty spec object":
                    First("SPEC-OBJECT").AddAfterSelf(new XElement(Other + "SPEC-OBJECT", new XAttribute("IDENTIFIER", "foreign-empty")));
                    break;
                case "attribute value":
                    var stringValue = First("ATTRIBUTE-VALUE-STRING");
                    var foreignValue = ForeignCopy(stringValue);
                    foreignValue.SetAttributeValue("THE-VALUE", "foreign value");
                    stringValue.AddAfterSelf(foreignValue);
                    break;
                case "enumeration value reference":
                    var enumValueRef = First("ENUM-VALUE-REF");
                    var otherEnumValue = root.Descendants(ns + "ENUM-VALUE").Select(x => x.Attribute("IDENTIFIER")!.Value).First(x => x != enumValueRef.Value);
                    enumValueRef.AddAfterSelf(new XElement(Other + "ENUM-VALUE-REF", otherEnumValue));
                    break;
                case "xhtml value":
                    First("ATTRIBUTE-VALUE-XHTML").Element(ns + "THE-VALUE")!.AddAfterSelf(new XElement(Other + "THE-VALUE", new XElement((XNamespace)"http://www.w3.org/1999/xhtml" + "p", "foreign")));
                    break;
                case "spec hierarchy":
                    First("SPEC-HIERARCHY").AddAfterSelf(ForeignCopy(First("SPEC-HIERARCHY")));
                    break;
                case "spec relation":
                    First("SPEC-RELATION").AddAfterSelf(ForeignCopy(First("SPEC-RELATION")));
                    break;
                case "relation group spec relations":
                    First("RELATION-GROUP").Add(new XElement(Other + "SPEC-RELATIONS", new XElement(ns + "SPEC-RELATION-REF", First("SPEC-RELATION").Attribute("IDENTIFIER")!.Value)));
                    break;
                case "alternative id":
                    var alternativeId = First("ALTERNATIVE-ID").Element(ns + "ALTERNATIVE-ID")!;
                    // after the ReqIF one, which it would replace when it was read
                    alternativeId.AddAfterSelf(new XElement(Other + "ALTERNATIVE-ID", new XAttribute("IDENTIFIER", "foreign-alternative-id")));
                    break;
                case "tool extension":
                    root.Add(new XElement(ns + "TOOL-EXTENSIONS",
                        new XElement(Other + "REQ-IF-TOOL-EXTENSION", new XElement(Other + "SETTING", "foreign"))));
                    break;
                case "root child":
                    // a header with another title, in a foreign THE-HEADER
                    var header = new XElement(First("REQ-IF-HEADER"));
                    header.Element(ns + "TITLE")!.Value = "foreign title";
                    First("THE-HEADER").AddAfterSelf(new XElement(Other + "THE-HEADER", header));
                    break;
                case "embedded value":
                    First("EMBEDDED-VALUE").AddAfterSelf(new XElement(Other + "EMBEDDED-VALUE", new XAttribute("KEY", "2"), new XAttribute("OTHER-CONTENT", "foreign")));
                    break;
                case "spec type child":
                    // an attribute definition with a new identifier, in a foreign SPEC-ATTRIBUTES
                    var attributeDefinition = new XElement(First("ATTRIBUTE-DEFINITION-STRING"));
                    attributeDefinition.SetAttributeValue("IDENTIFIER", "foreign-attribute-definition");
                    First("SPEC-OBJECT-TYPE").Add(new XElement(Other + "SPEC-ATTRIBUTES", attributeDefinition));
                    break;
                case var _ when place.StartsWith("attribute definition child ", StringComparison.Ordinal):
                    // an alternative id in a foreign ALTERNATIVE-ID, after the ReqIF one if there is one
                    var kind = place.Substring("attribute definition child ".Length);
                    First("ATTRIBUTE-DEFINITION-" + kind).Add(new XElement(Other + "ALTERNATIVE-ID", new XElement(ns + "ALTERNATIVE-ID", new XAttribute("IDENTIFIER", "foreign-alternative-id"))));
                    break;
                case "default value enumeration value reference":
                    var defaultValueRef = root.Descendants(ns + "DEFAULT-VALUE").Descendants(ns + "ENUM-VALUE-REF").First();
                    var otherDefaultValue = root.Descendants(ns + "ENUM-VALUE").Select(x => x.Attribute("IDENTIFIER")!.Value).First(x => x != defaultValueRef.Value);
                    defaultValueRef.AddAfterSelf(new XElement(Other + "ENUM-VALUE-REF", otherDefaultValue));
                    break;
                case "specification":
                    First("SPECIFICATION").AddAfterSelf(ForeignCopy(First("SPECIFICATION")));
                    break;
                case "spec hierarchy object":
                    var specHierarchy = First("SPEC-HIERARCHY");
                    var objectRef = specHierarchy.Element(ns + "OBJECT")!.Element(ns + "SPEC-OBJECT-REF")!.Value;
                    var otherSpecObject = root.Descendants(ns + "SPEC-OBJECT").Select(x => x.Attribute("IDENTIFIER")!.Value).First(x => x != objectRef);
                    specHierarchy.Element(ns + "OBJECT")!.AddAfterSelf(new XElement(Other + "OBJECT", new XElement(ns + "SPEC-OBJECT-REF", otherSpecObject)));
                    break;
                case "nested spec hierarchy":
                    var nested = root.Descendants(ns + "SPEC-HIERARCHY").First(x => x.Parent!.Parent!.Name == ns + "SPEC-HIERARCHY");
                    nested.AddAfterSelf(ForeignCopy(nested));
                    break;
                case "relation group":
                    First("RELATION-GROUP").AddAfterSelf(ForeignCopy(First("RELATION-GROUP")));
                    break;
                case "relation group spec relation ref":
                    // CreateDocuments added a SPEC-RELATIONS with a reference to the spec relation; a second reference would add it twice
                    var specRelationRef = First("RELATION-GROUP").Element(ns + "SPEC-RELATIONS")!.Element(ns + "SPEC-RELATION-REF")!;
                    specRelationRef.AddAfterSelf(new XElement(Other + "SPEC-RELATION-REF", specRelationRef.Value));
                    break;
                case var _ when place.Contains(" reference "):
                    AddForeignReference(root, place);
                    break;
                case "empty element before the xhtml value":
                    SimplifiedXhtmlValue(root).Element(ns + "THE-VALUE")!.AddBeforeSelf(new XElement(Other + "EMPTY"));
                    break;
                case "empty element between the xhtml values":
                    SimplifiedXhtmlValue(root).Element(ns + "THE-VALUE")!.AddAfterSelf(new XElement(Other + "EMPTY"));
                    break;
                case "spec relation source wrapper":
                    AddForeignCopyWithOtherReference(root, First("SPEC-RELATION").Element(ns + "SOURCE")!, "SPEC-OBJECT-REF", Identifiers(root, "SPEC-OBJECT"));
                    break;
                case var _ when place.StartsWith("attribute value definition wrapper ", StringComparison.Ordinal):
                    // a foreign DEFINITION after the ReqIF one, as a direct child of the attribute value
                    var valueKind = place.Substring("attribute value definition wrapper ".Length);
                    var valueDefinition = root.Descendants(ns + "SPEC-OBJECT").Descendants(ns + "ATTRIBUTE-VALUE-" + valueKind).First().Element(ns + "DEFINITION")!;
                    AddForeignCopyWithOtherReference(root, valueDefinition, $"ATTRIBUTE-DEFINITION-{valueKind}-REF", Identifiers(root, "ATTRIBUTE-DEFINITION-" + valueKind));
                    break;
                case "relation group target specification wrapper":
                    AddForeignCopyWithOtherReference(root, First("RELATION-GROUP").Element(ns + "TARGET-SPECIFICATION")!, "SPECIFICATION-REF", Identifiers(root, "SPECIFICATION"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(place), place, "unknown place");
            }

            return document.ToString();
        }

        /// <summary>
        /// Adds a foreign reference element before or after the ReqIF reference in a reference container
        /// (TYPE, DEFINITION, SOURCE, TARGET, OBJECT, ...). It refers to another element of the same kind, so
        /// it would change the object graph (or make reading fail) when it was read.
        /// </summary>
        private static void AddForeignReference(XElement root, string place)
        {
            var ns = TestDocuments.ReqIFNamespace;

            var index = place.LastIndexOf(" reference ", StringComparison.Ordinal);
            var words = place.Substring(index + " reference ".Length).Split(' ');
            var before = words[0] == "before";
            var kind = words.Length > 1 ? words[1] : "STRING";

            (XElement Container, string Reference, IEnumerable<string> Candidates) target = place.Substring(0, index) switch
            {
                "spec object type" => (root.Descendants(ns + "SPEC-OBJECT").First().Element(ns + "TYPE")!, "SPEC-OBJECT-TYPE-REF", Identifiers(root, "SPEC-OBJECT-TYPE", "SPECIFICATION-TYPE")),
                "attribute value definition" => (root.Descendants(ns + "SPEC-OBJECT").Descendants(ns + "ATTRIBUTE-VALUE-" + kind).First().Element(ns + "DEFINITION")!, $"ATTRIBUTE-DEFINITION-{kind}-REF", Identifiers(root, "ATTRIBUTE-DEFINITION-" + kind)),
                "attribute definition type" => (root.Descendants(ns + "ATTRIBUTE-DEFINITION-" + kind).First().Element(ns + "TYPE")!, $"DATATYPE-DEFINITION-{kind}-REF", Identifiers(root, "DATATYPE-DEFINITION-" + kind, "DATATYPE-DEFINITION-BOOLEAN", "DATATYPE-DEFINITION-STRING")),
                "spec relation type" => (root.Descendants(ns + "SPEC-RELATION").First().Element(ns + "TYPE")!, "SPEC-RELATION-TYPE-REF", Identifiers(root, "SPEC-RELATION-TYPE", "SPEC-OBJECT-TYPE")),
                "xhtml value definition" => (SimplifiedXhtmlValue(root).Element(ns + "DEFINITION")!, "ATTRIBUTE-DEFINITION-XHTML-REF", Identifiers(root, "ATTRIBUTE-DEFINITION-XHTML")),
                "spec relation source" => (root.Descendants(ns + "SPEC-RELATION").First().Element(ns + "SOURCE")!, "SPEC-OBJECT-REF", Identifiers(root, "SPEC-OBJECT")),
                "spec relation target" => (root.Descendants(ns + "SPEC-RELATION").First().Element(ns + "TARGET")!, "SPEC-OBJECT-REF", Identifiers(root, "SPEC-OBJECT")),
                "spec hierarchy object" => (root.Descendants(ns + "SPEC-HIERARCHY").First().Element(ns + "OBJECT")!, "SPEC-OBJECT-REF", Identifiers(root, "SPEC-OBJECT")),
                "specification type" => (root.Descendants(ns + "SPECIFICATION").First().Element(ns + "TYPE")!, "SPECIFICATION-TYPE-REF", Identifiers(root, "SPECIFICATION-TYPE", "SPEC-OBJECT-TYPE")),
                "relation group source specification" => (root.Descendants(ns + "RELATION-GROUP").First().Element(ns + "SOURCE-SPECIFICATION")!, "SPECIFICATION-REF", Identifiers(root, "SPECIFICATION")),
                "relation group type" => (root.Descendants(ns + "RELATION-GROUP").First().Element(ns + "TYPE")!, "RELATION-GROUP-TYPE-REF", Identifiers(root, "RELATION-GROUP-TYPE", "SPEC-OBJECT-TYPE")),
                _ => throw new ArgumentOutOfRangeException(nameof(place), place, "unknown reference place")
            };

            var reference = target.Container.Element(ns + target.Reference)!;
            var other = target.Candidates.First(x => x != reference.Value);
            var foreign = new XElement(Other + target.Reference, other);

            if (before)
            {
                reference.AddBeforeSelf(foreign);
            }
            else
            {
                reference.AddAfterSelf(foreign);
            }
        }

        /// <summary>
        /// Adds, after a reference container, a foreign copy of it that refers to another element; it would replace
        /// the reference when it was read
        /// </summary>
        private static void AddForeignCopyWithOtherReference(XElement root, XElement container, string reference, IEnumerable<string> candidates)
        {
            var ns = TestDocuments.ReqIFNamespace;
            var copy = new XElement(container) { Name = Other + container.Name.LocalName };
            var referenceElement = copy.Element(ns + reference)!;
            referenceElement.Value = candidates.First(x => x != referenceElement.Value);
            container.AddAfterSelf(copy);
        }

        /// <summary>
        /// The identifiers of the elements with the given local names, in that order
        /// </summary>
        private static IEnumerable<string> Identifiers(XElement root, params string[] localNames)
        {
            return localNames.SelectMany(name => root.Descendants(TestDocuments.ReqIFNamespace + name)).Select(x => x.Attribute("IDENTIFIER")!.Value);
        }

        /// <summary>
        /// The first XHTML attribute value that has a THE-ORIGINAL-VALUE
        /// </summary>
        private static XElement SimplifiedXhtmlValue(XElement root)
        {
            var ns = TestDocuments.ReqIFNamespace;

            return root.Descendants(ns + "ATTRIBUTE-VALUE-XHTML").First(x => x.Element(ns + "THE-ORIGINAL-VALUE") != null);
        }

        private static async Task<(List<ReqIF> ReqIFs, List<string> Warnings)> Deserialize(Stream stream, SupportedFileExtensionKind kind, bool async)
        {
            var sink = new TestLogEventSink();
            var logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(sink).CreateLogger();

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(logger, dispose: true));

            var deserializer = new ReqIFDeserializer(loggerFactory);

            // a reader that does not move on would loop forever: fail the test instead of blocking the run
            var reqifs = await Task.Run(async () => async
                    ? (await deserializer.DeserializeAsync(stream, kind, CancellationToken.None)).ToList()
                    : deserializer.Deserialize(stream, kind).ToList())
                .WaitAsync(TimeSpan.FromSeconds(60));

            var warnings = sink.Events
                .Where(e => e.Level == LogEventLevel.Warning && e.MessageTemplate.Text.Contains(WarningText))
                .Select(e => e.RenderMessage())
                .ToList();

            return (reqifs, warnings);
        }
    }
}
