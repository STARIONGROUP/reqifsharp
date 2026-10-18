// -------------------------------------------------------------------------------------------------
// <copyright file="PrefixedNamespaceTestFixture.cs" company="Starion Group S.A.">
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
    /// Verifies that the REQ-IF root and the elements below it are matched on their local name and namespace,
    /// so that a document in which the ReqIF namespace is bound to a prefix (<c>reqif:REQ-IF</c>) is read
    /// the same as the unprefixed original, see GH-87.
    /// </summary>
    [TestFixture]
    public class PrefixedNamespaceTestFixture
    {
        private static readonly string[] ForeignNamespaceDocuments =
        {
            "<other:REQ-IF xmlns:other=\"http://example.com/not-reqif\"><other:THE-HEADER/></other:REQ-IF>",
            "<REQ-IF xmlns=\"http://example.com/not-reqif\"><THE-HEADER/></REQ-IF>"
        };

        private TestLogEventSink testLogEventSink;

        private ILoggerFactory loggerFactory;

        [SetUp]
        public void SetUp()
        {
            this.testLogEventSink = new TestLogEventSink();

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(this.testLogEventSink)
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(logger, dispose: true));
        }

        [TearDown]
        public void TearDown()
        {
            this.loggerFactory.Dispose();
        }

        [TestCaseSource(typeof(TestDocuments), nameof(TestDocuments.ComparableFiles))]
        public void Verify_that_a_prefixed_document_is_read_the_same_as_the_original(string fileName)
        {
            var original = TestDocuments.Read(fileName);
            var prefixed = CreatePrefixedCopy(original);

            foreach (var kind in TestDocuments.Kinds)
            {
                var deserializer = new ReqIFDeserializer();

                var expected = deserializer.Deserialize(TestDocuments.ToStream(original, kind), kind).ToList();
                var actual = deserializer.Deserialize(TestDocuments.ToStream(prefixed, kind), kind).ToList();

                TestDocuments.AssertSameGraph(expected, actual);
            }
        }

        [TestCaseSource(typeof(TestDocuments), nameof(TestDocuments.ComparableFiles))]
        public async Task Verify_that_a_prefixed_document_is_read_the_same_as_the_original_async(string fileName)
        {
            var original = TestDocuments.Read(fileName);
            var prefixed = CreatePrefixedCopy(original);

            foreach (var kind in TestDocuments.Kinds)
            {
                var deserializer = new ReqIFDeserializer();

                var expected = (await deserializer.DeserializeAsync(TestDocuments.ToStream(original, kind), kind, CancellationToken.None)).ToList();
                var actual = (await deserializer.DeserializeAsync(TestDocuments.ToStream(prefixed, kind), kind, CancellationToken.None)).ToList();

                TestDocuments.AssertSameGraph(expected, actual);
            }
        }

        [Test]
        public void Verify_that_a_document_without_namespace_is_read_the_same_as_the_original()
        {
            var original = TestDocuments.Read("reqifsharpgenerated.reqif");
            var withoutNamespace = CreateCopyWithoutNamespace(original);

            foreach (var kind in TestDocuments.Kinds)
            {
                var deserializer = new ReqIFDeserializer();

                var expected = deserializer.Deserialize(TestDocuments.ToStream(original, kind), kind).ToList();
                var actual = deserializer.Deserialize(TestDocuments.ToStream(withoutNamespace, kind), kind).ToList();

                TestDocuments.AssertSameGraph(expected, actual);
            }
        }

        [Test]
        public async Task Verify_that_a_document_without_namespace_is_read_the_same_as_the_original_async()
        {
            var original = TestDocuments.Read("reqifsharpgenerated.reqif");
            var withoutNamespace = CreateCopyWithoutNamespace(original);

            foreach (var kind in TestDocuments.Kinds)
            {
                var deserializer = new ReqIFDeserializer();

                var expected = (await deserializer.DeserializeAsync(TestDocuments.ToStream(original, kind), kind, CancellationToken.None)).ToList();
                var actual = (await deserializer.DeserializeAsync(TestDocuments.ToStream(withoutNamespace, kind), kind, CancellationToken.None)).ToList();

                TestDocuments.AssertSameGraph(expected, actual);
            }
        }

        [TestCaseSource(nameof(ForeignNamespaceDocuments))]
        public void Verify_that_a_root_element_in_another_namespace_is_skipped_with_a_warning(string xml)
        {
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            var reqifs = deserializer.Deserialize(TestDocuments.ToStream(xml, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif);

            Assert.That(reqifs, Is.Empty);
            this.AssertSkippedWarningLogged();
        }

        [TestCaseSource(nameof(ForeignNamespaceDocuments))]
        public async Task Verify_that_a_root_element_in_another_namespace_is_skipped_with_a_warning_async(string xml)
        {
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            var reqifs = await deserializer.DeserializeAsync(TestDocuments.ToStream(xml, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif, CancellationToken.None);

            Assert.That(reqifs, Is.Empty);
            this.AssertSkippedWarningLogged();
        }

        private void AssertSkippedWarningLogged()
        {
            var warnings = this.testLogEventSink.Events
                .Where(e => e.Level == LogEventLevel.Warning && e.MessageTemplate.Text.Contains("has been skipped"))
                .ToList();

            Assert.That(warnings, Has.Count.EqualTo(1));
            Assert.That(warnings[0].RenderMessage(), Does.Contain("http://example.com/not-reqif"));
        }

        /// <summary>
        /// Returns a copy of the document in which every ReqIF element is written as reqif:...
        /// </summary>
        private static string CreatePrefixedCopy(string original)
        {
            // keep the original whitespace, so that the copy differs from the original only in its prefixes
            var document = XDocument.Parse(original, LoadOptions.PreserveWhitespace);

            RemoveReqIFNamespaceDeclarations(document);

            document.Root!.SetAttributeValue(XNamespace.Xmlns + "reqif", TestDocuments.ReqIFNamespace.NamespaceName);

            var prefixed = document.ToString(SaveOptions.DisableFormatting);

            Assert.That(prefixed, Does.Contain("<reqif:REQ-IF "), "the copy must use the reqif prefix");
            Assert.That(prefixed, Does.Not.Contain("<REQ-IF "), "the copy must not use the default namespace for ReqIF elements");

            return prefixed;
        }

        /// <summary>
        /// Returns a copy of the document in which the ReqIF elements are in no namespace
        /// </summary>
        private static string CreateCopyWithoutNamespace(string original)
        {
            var document = XDocument.Parse(original, LoadOptions.PreserveWhitespace);

            RemoveReqIFNamespaceDeclarations(document);

            foreach (var element in document.Descendants().Where(e => e.Name.Namespace == TestDocuments.ReqIFNamespace))
            {
                element.Name = XNamespace.None + element.Name.LocalName;
            }

            var withoutNamespace = document.ToString(SaveOptions.DisableFormatting);

            Assert.That(withoutNamespace, Does.Contain("<REQ-IF "), "the copy must contain an unprefixed root");
            Assert.That(withoutNamespace, Does.Not.Contain(TestDocuments.ReqIFNamespace.NamespaceName), "the copy must not declare the ReqIF namespace");

            return withoutNamespace;
        }

        private static void RemoveReqIFNamespaceDeclarations(XDocument document)
        {
            foreach (var element in document.Descendants())
            {
                element.Attributes()
                    .Where(a => a.IsNamespaceDeclaration && a.Value == TestDocuments.ReqIFNamespace.NamespaceName)
                    .Remove();
            }
        }
    }
}
