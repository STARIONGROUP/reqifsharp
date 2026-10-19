// -------------------------------------------------------------------------------------------------
// <copyright file="MinifiedDocumentTestFixture.cs" company="Starion Group S.A.">
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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// Verifies that a document without whitespace between its elements (minified XML) is read
    /// the same as the indented original.
    /// </summary>
    [TestFixture]
    public class MinifiedDocumentTestFixture
    {
        private const string FirstSpecRelationIdentifier = "specobject_1-specobject_2";

        private const string SecondSpecRelationIdentifier = "second-spec-relation";

        private const string MinifiedHeaderDocument =
            "<REQ-IF xmlns=\"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd\">" +
            "<THE-HEADER><REQ-IF-HEADER IDENTIFIER=\"header\">" +
            "<COMMENT>a comment</COMMENT>" +
            "<CREATION-TIME>2017-03-13T10:15:09.017+01:00</CREATION-TIME>" +
            "<REPOSITORY-ID>a repository</REPOSITORY-ID>" +
            "<REQ-IF-TOOL-ID>a tool</REQ-IF-TOOL-ID>" +
            "<REQ-IF-VERSION>1.0</REQ-IF-VERSION>" +
            "<SOURCE-TOOL-ID>a source tool</SOURCE-TOOL-ID>" +
            "<TITLE>a title</TITLE>" +
            "</REQ-IF-HEADER></THE-HEADER>" +
            "<CORE-CONTENT><REQ-IF-CONTENT/></CORE-CONTENT>" +
            "</REQ-IF>";

        private const string MinifiedHeaderDocumentWithEmptyElements =
            "<REQ-IF xmlns=\"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd\">" +
            "<THE-HEADER><REQ-IF-HEADER IDENTIFIER=\"header\">" +
            "<COMMENT/>" +
            "<CREATION-TIME>2017-03-13T10:15:09.017+01:00</CREATION-TIME>" +
            "<REPOSITORY-ID></REPOSITORY-ID>" +
            "<REQ-IF-TOOL-ID>a tool</REQ-IF-TOOL-ID>" +
            "<REQ-IF-VERSION>1.0</REQ-IF-VERSION>" +
            "<SOURCE-TOOL-ID>a source tool</SOURCE-TOOL-ID>" +
            "<TITLE>a title</TITLE>" +
            "</REQ-IF-HEADER></THE-HEADER>" +
            "<CORE-CONTENT><REQ-IF-CONTENT/></CORE-CONTENT>" +
            "</REQ-IF>";

        [Test]
        public void Verify_that_every_header_field_of_a_minified_document_is_read()
        {
            var reqif = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(MinifiedHeaderDocument, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif).Single();

            AssertHeader(reqif.TheHeader);
        }

        [Test]
        public async Task Verify_that_every_header_field_of_a_minified_document_is_read_async()
        {
            var reqif = (await new ReqIFDeserializer().DeserializeAsync(TestDocuments.ToStream(MinifiedHeaderDocument, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif, CancellationToken.None)).Single();

            AssertHeader(reqif.TheHeader);
        }

        [TestCaseSource(typeof(TestDocuments), nameof(TestDocuments.ComparableFiles))]
        public void Verify_that_a_minified_document_is_read_the_same_as_the_original(string fileName)
        {
            var original = TestDocuments.Read(fileName);
            var minified = TestDocuments.Minify(original);

            foreach (var kind in TestDocuments.Kinds)
            {
                var deserializer = new ReqIFDeserializer();

                var expected = deserializer.Deserialize(TestDocuments.ToStream(original, kind), kind).ToList();
                var actual = deserializer.Deserialize(TestDocuments.ToStream(minified, kind), kind).ToList();

                TestDocuments.AssertSameGraph(expected, actual);
            }
        }

        [TestCaseSource(typeof(TestDocuments), nameof(TestDocuments.ComparableFiles))]
        public async Task Verify_that_a_minified_document_is_read_the_same_as_the_original_async(string fileName)
        {
            var original = TestDocuments.Read(fileName);
            var minified = TestDocuments.Minify(original);

            foreach (var kind in TestDocuments.Kinds)
            {
                var deserializer = new ReqIFDeserializer();

                var expected = (await deserializer.DeserializeAsync(TestDocuments.ToStream(original, kind), kind, CancellationToken.None)).ToList();
                var actual = (await deserializer.DeserializeAsync(TestDocuments.ToStream(minified, kind), kind, CancellationToken.None)).ToList();

                TestDocuments.AssertSameGraph(expected, actual);
            }
        }

        [Test]
        public void Verify_that_the_spec_relations_of_a_relation_group_in_a_minified_document_are_read()
        {
            var original = CreateDocumentWithSpecRelationsInRelationGroup();
            var minified = TestDocuments.Minify(original);

            foreach (var kind in TestDocuments.Kinds)
            {
                var deserializer = new ReqIFDeserializer();

                var expected = deserializer.Deserialize(TestDocuments.ToStream(original, kind), kind).ToList();
                var actual = deserializer.Deserialize(TestDocuments.ToStream(minified, kind), kind).ToList();

                Assert.That(expected[0].CoreContent.SpecRelationGroups.Single().SpecRelations, Has.Count.EqualTo(2), "the original must be read with both spec relations");

                TestDocuments.AssertSameGraph(expected, actual);
            }
        }

        [Test]
        public async Task Verify_that_the_spec_relations_of_a_relation_group_in_a_minified_document_are_read_async()
        {
            var original = CreateDocumentWithSpecRelationsInRelationGroup();
            var minified = TestDocuments.Minify(original);

            foreach (var kind in TestDocuments.Kinds)
            {
                var deserializer = new ReqIFDeserializer();

                var expected = (await deserializer.DeserializeAsync(TestDocuments.ToStream(original, kind), kind, CancellationToken.None)).ToList();
                var actual = (await deserializer.DeserializeAsync(TestDocuments.ToStream(minified, kind), kind, CancellationToken.None)).ToList();

                Assert.That(expected[0].CoreContent.SpecRelationGroups.Single().SpecRelations, Has.Count.EqualTo(2), "the original must be read with both spec relations");

                TestDocuments.AssertSameGraph(expected, actual);
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_the_header_fields_after_an_empty_header_field_are_read(bool async)
        {
            var header = (await Deserialize(MinifiedHeaderDocumentWithEmptyElements, SupportedFileExtensionKind.Reqif, async)).Single().TheHeader;

            Assert.Multiple(() =>
            {
                Assert.That(header.Comment, Is.Empty);
                Assert.That(header.CreationTime, Is.EqualTo(XmlConvert.ToDateTime("2017-03-13T10:15:09.017+01:00", XmlDateTimeSerializationMode.RoundtripKind)));
                Assert.That(header.RepositoryId, Is.Empty);
                Assert.That(header.ReqIFToolId, Is.EqualTo("a tool"));
                Assert.That(header.ReqIFVersion, Is.EqualTo("1.0"));
                Assert.That(header.SourceToolId, Is.EqualTo("a source tool"));
                Assert.That(header.Title, Is.EqualTo("a title"));
            });
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_the_elements_after_the_spec_relations_of_a_relation_group_are_read(bool async)
        {
            var original = CreateDocumentWithSpecRelationsInRelationGroup(insertBefore: "TYPE");

            foreach (var xml in new[] { original, TestDocuments.Minify(original) })
            {
                foreach (var kind in TestDocuments.Kinds)
                {
                    var relationGroup = (await Deserialize(xml, kind, async)).Single().CoreContent.SpecRelationGroups.Single();

                    Assert.Multiple(() =>
                    {
                        Assert.That(relationGroup.SpecRelations.Select(x => x.Identifier), Is.EqualTo(new[] { FirstSpecRelationIdentifier, SecondSpecRelationIdentifier }));
                        Assert.That(relationGroup.Type?.Identifier, Is.EqualTo("relationgrouptype"));
                        Assert.That(relationGroup.SourceSpecification?.Identifier, Is.EqualTo("specification-1"));
                        Assert.That(relationGroup.TargetSpecification?.Identifier, Is.EqualTo("specification-2"));
                    });
                }
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_an_empty_spec_relations_element_of_a_relation_group_is_read(bool async)
        {
            var original = CreateDocumentWithSpecRelationsInRelationGroup(insertBefore: "TARGET-SPECIFICATION", empty: true);

            Assert.That(original, Does.Contain("<SPEC-RELATIONS />"), "the document must contain an empty SPEC-RELATIONS element");

            foreach (var xml in new[] { original, TestDocuments.Minify(original) })
            {
                foreach (var kind in TestDocuments.Kinds)
                {
                    var relationGroup = (await Deserialize(xml, kind, async)).Single().CoreContent.SpecRelationGroups.Single();

                    Assert.Multiple(() =>
                    {
                        Assert.That(relationGroup.SpecRelations, Is.Empty);
                        Assert.That(relationGroup.SourceSpecification?.Identifier, Is.EqualTo("specification-1"));
                        Assert.That(relationGroup.TargetSpecification?.Identifier, Is.EqualTo("specification-2"));
                    });
                }
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_an_unsupported_child_of_spec_relations_is_skipped_with_a_warning(bool async)
        {
            var ns = TestDocuments.ReqIFNamespace;

            // the SPEC-RELATION-REF inside the unsupported element is not a direct child of SPEC-RELATIONS and must not be read
            var unsupported = new XElement(ns + "SPEC-OBJECT-REF", new XElement(ns + "SPEC-RELATION-REF", FirstSpecRelationIdentifier));

            var original = CreateDocumentWithSpecRelationsInRelationGroup(insertBefore: "TYPE", unsupportedChild: unsupported);

            foreach (var xml in new[] { original, TestDocuments.Minify(original) })
            {
                foreach (var kind in TestDocuments.Kinds)
                {
                    var testLogEventSink = new TestLogEventSink();
                    var logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(testLogEventSink).CreateLogger();

                    using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(logger, dispose: true));

                    var relationGroup = (await Deserialize(xml, kind, async, loggerFactory)).Single().CoreContent.SpecRelationGroups.Single();

                    var warnings = testLogEventSink.Events
                        .Where(e => e.Level == LogEventLevel.Warning && e.MessageTemplate.Text.Contains("is not supported in SPEC-RELATIONS"))
                        .Select(e => e.RenderMessage())
                        .ToList();

                    Assert.Multiple(() =>
                    {
                        Assert.That(relationGroup.SpecRelations.Select(x => x.Identifier), Is.EqualTo(new[] { FirstSpecRelationIdentifier, SecondSpecRelationIdentifier }));
                        Assert.That(relationGroup.Type?.Identifier, Is.EqualTo("relationgrouptype"));
                        Assert.That(warnings, Has.Count.EqualTo(1));
                        Assert.That(warnings.FirstOrDefault(), Does.Contain("SPEC-OBJECT-REF").And.Contain("relationgroup-1"));
                    });
                }
            }
        }

        private static async Task<List<ReqIF>> Deserialize(string xml, SupportedFileExtensionKind kind, bool async, ILoggerFactory loggerFactory = null)
        {
            var deserializer = new ReqIFDeserializer(loggerFactory);
            var stream = TestDocuments.ToStream(xml, kind);

            return async
                ? (await deserializer.DeserializeAsync(stream, kind, CancellationToken.None)).ToList()
                : deserializer.Deserialize(stream, kind).ToList();
        }

        /// <summary>
        /// None of the test documents has spec relations in a relation group: this returns reqifsharpgenerated.reqif
        /// with a second spec relation and a relation group that refers to both, indented.
        /// </summary>
        /// <param name="insertBefore">
        /// The local name of the relation group child before which SPEC-RELATIONS is inserted; null to add it as the last child
        /// </param>
        /// <param name="empty">
        /// true to write an empty SPEC-RELATIONS element, without references
        /// </param>
        /// <param name="unsupportedChild">
        /// An element that is placed between the two references, or null
        /// </param>
        private static string CreateDocumentWithSpecRelationsInRelationGroup(string insertBefore = null, bool empty = false, XElement unsupportedChild = null)
        {
            var ns = TestDocuments.ReqIFNamespace;

            var document = XDocument.Parse(TestDocuments.Read("reqifsharpgenerated.reqif"));

            var specRelation = document.Descendants(ns + "SPEC-RELATION").Single();
            Assert.That(specRelation.Attribute("IDENTIFIER")!.Value, Is.EqualTo(FirstSpecRelationIdentifier));

            var copy = new XElement(specRelation);
            copy.SetAttributeValue("IDENTIFIER", SecondSpecRelationIdentifier);
            specRelation.AddAfterSelf(copy);

            var specRelations = new XElement(ns + "SPEC-RELATIONS");

            if (!empty)
            {
                specRelations.Add(
                    new XElement(ns + "SPEC-RELATION-REF", FirstSpecRelationIdentifier),
                    unsupportedChild,
                    new XElement(ns + "SPEC-RELATION-REF", SecondSpecRelationIdentifier));
            }

            var relationGroup = document.Descendants(ns + "RELATION-GROUP").Single();

            if (insertBefore == null)
            {
                relationGroup.Add(specRelations);
            }
            else
            {
                relationGroup.Element(ns + insertBefore)!.AddBeforeSelf(specRelations);
            }

            return document.ToString();
        }

        private static void AssertHeader(ReqIFHeader header)
        {
            Assert.Multiple(() =>
            {
                Assert.That(header.Identifier, Is.EqualTo("header"));
                Assert.That(header.Comment, Is.EqualTo("a comment"));
                Assert.That(header.CreationTime, Is.EqualTo(XmlConvert.ToDateTime("2017-03-13T10:15:09.017+01:00", XmlDateTimeSerializationMode.RoundtripKind)));
                Assert.That(header.RepositoryId, Is.EqualTo("a repository"));
                Assert.That(header.ReqIFToolId, Is.EqualTo("a tool"));
                Assert.That(header.ReqIFVersion, Is.EqualTo("1.0"));
                Assert.That(header.SourceToolId, Is.EqualTo("a source tool"));
                Assert.That(header.Title, Is.EqualTo("a title"));
            });
        }
    }
}
