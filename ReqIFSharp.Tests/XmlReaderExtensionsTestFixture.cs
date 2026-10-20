// -------------------------------------------------------------------------------------------------
// <copyright file="XmlReaderExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// Suite of tests for the <see cref="XmlReaderExtensions"/> class
    /// </summary>
    [TestFixture]
    public class XmlReaderExtensionsTestFixture
    {
        [TestCase("<A>text</A>", "text")]
        [TestCase("<A>one<![CDATA[ two]]><!-- comment --> three</A>", "one two three")]
        [TestCase("<A></A>", "")]
        public void Verify_that_the_content_is_read_and_the_reader_is_left_on_the_end_tag(string element, string expected)
        {
            using var reader = CreateReaderOnFirstChild($"<ROOT>{element}<B/></ROOT>");

            Assert.That(reader.ReadElementContentAsStringToEndElement(), Is.EqualTo(expected));
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement));
            Assert.That(reader.LocalName, Is.EqualTo("A"));

            reader.Read();
            Assert.That(reader.LocalName, Is.EqualTo("B"), "the next Read must land on the next sibling");
        }

        [TestCase("<A>text</A>", "text")]
        [TestCase("<A>one<![CDATA[ two]]><!-- comment --> three</A>", "one two three")]
        [TestCase("<A></A>", "")]
        public async Task Verify_that_the_content_is_read_and_the_reader_is_left_on_the_end_tag_async(string element, string expected)
        {
            using var reader = CreateReaderOnFirstChild($"<ROOT>{element}<B/></ROOT>", true);

            Assert.That(await reader.ReadElementContentAsStringToEndElementAsync(), Is.EqualTo(expected));
            Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement));
            Assert.That(reader.LocalName, Is.EqualTo("A"));

            await reader.ReadAsync();
            Assert.That(reader.LocalName, Is.EqualTo("B"), "the next Read must land on the next sibling");
        }

        [Test]
        public async Task Verify_that_an_empty_element_gives_an_empty_string_and_the_reader_stays_on_it()
        {
            using (var reader = CreateReaderOnFirstChild("<ROOT><A/><B/></ROOT>"))
            {
                Assert.That(reader.ReadElementContentAsStringToEndElement(), Is.Empty);
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
                Assert.That(reader.LocalName, Is.EqualTo("A"));
            }

            using (var reader = CreateReaderOnFirstChild("<ROOT><A/><B/></ROOT>", true))
            {
                Assert.That(await reader.ReadElementContentAsStringToEndElementAsync(), Is.Empty);
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
                Assert.That(reader.LocalName, Is.EqualTo("A"));
            }
        }

        [TestCase("<ROOT><A><C/></A></ROOT>")]
        [TestCase("<ROOT><A>text<C/></A></ROOT>")]
        public void Verify_that_an_element_with_child_elements_throws(string xml)
        {
            using (var reader = CreateReaderOnFirstChild(xml))
            {
                Assert.That(() => reader.ReadElementContentAsStringToEndElement(),
                    Throws.TypeOf<XmlException>().With.Message.Contains("The A element may only contain text"));
            }

            using (var reader = CreateReaderOnFirstChild(xml, true))
            {
                Assert.That(async () => await reader.ReadElementContentAsStringToEndElementAsync(),
                    Throws.TypeOf<XmlException>().With.Message.Contains("The A element may only contain text"));
            }
        }

        [Test]
        public void Verify_that_a_reader_that_is_not_on_an_element_throws()
        {
            using (var reader = CreateReaderOnFirstChild("<ROOT><A>text</A></ROOT>"))
            {
                reader.Read();

                Assert.That(() => reader.ReadElementContentAsStringToEndElement(),
                    Throws.TypeOf<XmlException>().With.Message.Contains("must be positioned on an element"));
            }

            using (var reader = CreateReaderOnFirstChild("<ROOT><A>text</A></ROOT>", true))
            {
                reader.Read();

                Assert.That(async () => await reader.ReadElementContentAsStringToEndElementAsync(),
                    Throws.TypeOf<XmlException>().With.Message.Contains("must be positioned on an element"));
            }
        }

        [Test]
        public async Task Verify_that_a_reader_on_whitespace_before_the_element_moves_to_the_element()
        {
            using (var reader = CreateReaderOnFirstChild("<ROOT>  <A>text</A><B/></ROOT>"))
            {
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Whitespace), "the reader must start on the whitespace");

                Assert.That(reader.ReadElementContentAsStringToEndElement(), Is.EqualTo("text"));
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement));
                Assert.That(reader.LocalName, Is.EqualTo("A"));
            }

            using (var reader = CreateReaderOnFirstChild("<ROOT>  <A>text</A><B/></ROOT>", true))
            {
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Whitespace), "the reader must start on the whitespace");

                Assert.That(await reader.ReadElementContentAsStringToEndElementAsync(), Is.EqualTo("text"));
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement));
                Assert.That(reader.LocalName, Is.EqualTo("A"));
            }
        }

        [Test]
        public async Task Verify_that_the_exception_has_the_line_and_position_of_the_offending_node()
        {
            const string xml = "<ROOT>\n<A>\ntext<C/></A></ROOT>";

            using (var reader = CreateReaderOnFirstChild(xml))
            {
                reader.MoveToContent();

                var exception = Assert.Throws<XmlException>(() => reader.ReadElementContentAsStringToEndElement());

                Assert.That(exception.LineNumber, Is.EqualTo(3));
                Assert.That(exception.LinePosition, Is.EqualTo(6), "the position of the C element name");
            }

            using (var reader = CreateReaderOnFirstChild(xml, true))
            {
                reader.MoveToContent();

                var exception = await Assert.ThrowsAsync<XmlException>(async () => await reader.ReadElementContentAsStringToEndElementAsync());

                Assert.That(exception.LineNumber, Is.EqualTo(3));
                Assert.That(exception.LinePosition, Is.EqualTo(6), "the position of the C element name");
            }
        }

        [Test]
        public void Verify_that_the_exception_has_no_line_and_position_when_the_reader_has_no_line_information()
        {
            var document = new XmlDocument();
            document.LoadXml("<ROOT><A>text<C/></A></ROOT>");

            using var reader = new XmlNodeReader(document.DocumentElement!.FirstChild!);
            reader.Read();

            Assert.That(reader, Is.Not.InstanceOf<IXmlLineInfo>(), "the reader must not have line information");

            var exception = Assert.Throws<XmlException>(() => reader.ReadElementContentAsStringToEndElement());

            Assert.That(exception.Message, Does.Contain("The A element may only contain text"));
            Assert.That(exception.LineNumber, Is.Zero);
            Assert.That(exception.LinePosition, Is.Zero);
        }

        [TestCase("<ROOT><o:A xmlns:o=\"http://example.com/other\"><B>text</B></o:A><C/></ROOT>")]
        [TestCase("<ROOT><A xmlns=\"http://example.com/other\"><B>text</B></A><C/></ROOT>")]
        [TestCase("<ROOT><A xmlns=\"HTTP://WWW.OMG.ORG/SPEC/REQIF/20110401/REQIF.XSD\"><B>text</B></A><C/></ROOT>", Description = "namespaces are compared exactly: a different case is another namespace")]
        public async Task Verify_that_a_foreign_element_is_skipped_and_the_reader_is_left_on_its_end_tag(string xml)
        {
            using (var reader = CreateReaderOnFirstChild(xml))
            {
                Assert.That(reader.SkipForeignElement(null), Is.True);
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement));
                Assert.That(reader.LocalName, Is.EqualTo("A"));

                reader.Read();
                Assert.That(reader.LocalName, Is.EqualTo("C"), "the next Read must land on the next sibling");
            }

            using (var reader = CreateReaderOnFirstChild(xml, true))
            {
                Assert.That(await reader.SkipForeignElementAsync(null, CancellationToken.None), Is.True);
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement));
                Assert.That(reader.LocalName, Is.EqualTo("A"));

                await reader.ReadAsync();
                Assert.That(reader.LocalName, Is.EqualTo("C"), "the next Read must land on the next sibling");
            }
        }

        [Test]
        public async Task Verify_that_an_empty_foreign_element_is_skipped_and_the_reader_stays_on_it()
        {
            const string xml = "<ROOT><o:A xmlns:o=\"http://example.com/other\"/><C/></ROOT>";

            using (var reader = CreateReaderOnFirstChild(xml))
            {
                Assert.That(reader.SkipForeignElement(null), Is.True);
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
                Assert.That(reader.LocalName, Is.EqualTo("A"));

                reader.Read();
                Assert.That(reader.LocalName, Is.EqualTo("C"));
            }

            using (var reader = CreateReaderOnFirstChild(xml, true))
            {
                Assert.That(await reader.SkipForeignElementAsync(null, CancellationToken.None), Is.True);
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
                Assert.That(reader.LocalName, Is.EqualTo("A"));

                await reader.ReadAsync();
                Assert.That(reader.LocalName, Is.EqualTo("C"));
            }
        }

        [TestCase("<ROOT><A xmlns=\"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd\"><B/></A></ROOT>")]
        [TestCase("<ROOT><reqif:A xmlns:reqif=\"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd\"><B/></reqif:A></ROOT>")]
        [TestCase("<ROOT><A><B/></A></ROOT>")]
        public async Task Verify_that_an_element_in_the_ReqIF_namespace_or_in_no_namespace_is_not_skipped(string xml)
        {
            using (var reader = CreateReaderOnFirstChild(xml))
            {
                Assert.That(reader.SkipForeignElement(null), Is.False);
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
                Assert.That(reader.LocalName, Is.EqualTo("A"), "the reader must not move");
            }

            using (var reader = CreateReaderOnFirstChild(xml, true))
            {
                Assert.That(await reader.SkipForeignElementAsync(null, CancellationToken.None), Is.False);
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
                Assert.That(reader.LocalName, Is.EqualTo("A"), "the reader must not move");
            }
        }

        [Test]
        public async Task Verify_that_a_reader_that_is_not_on_an_element_is_not_skipped()
        {
            using (var reader = CreateReaderOnFirstChild("<ROOT><A>text</A></ROOT>"))
            {
                reader.Read();

                Assert.That(reader.SkipForeignElement(null), Is.False);
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Text));
            }

            using (var reader = CreateReaderOnFirstChild("<ROOT><A>text</A></ROOT>", true))
            {
                await reader.ReadAsync();

                Assert.That(await reader.SkipForeignElementAsync(null, CancellationToken.None), Is.False);
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Text));
            }
        }

        [Test]
        public async Task Verify_that_a_warning_with_the_name_namespace_and_line_is_logged_for_a_skipped_element()
        {
            const string xml = "<ROOT>\n  <o:A xmlns:o=\"http://example.com/other\"/></ROOT>";

            foreach (var async in new[] { false, true })
            {
                var sink = new TestLogEventSink();
                var serilogLogger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(sink).CreateLogger();

                using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(serilogLogger, dispose: true));
                var logger = loggerFactory.CreateLogger<XmlReaderExtensionsTestFixture>();

                using var reader = CreateReaderOnFirstChild(xml, async);

                var skipped = async ? await reader.SkipForeignElementAsync(logger, CancellationToken.None) : reader.SkipForeignElement(logger);

                var warning = sink.Events.Single(e => e.Level == LogEventLevel.Warning).RenderMessage();

                Assert.That(skipped, Is.True);
                Assert.That(warning, Does.Contain("o:A").And.Contain("2:4").And.Contain("http://example.com/other").And.Contain("has been skipped"), $"async: {async}");
            }
        }

        [Test]
        public void Verify_that_skipping_a_foreign_element_asynchronously_can_be_cancelled()
        {
            using var reader = CreateReaderOnFirstChild("<ROOT><o:A xmlns:o=\"http://example.com/other\"><B/><B/></o:A></ROOT>", true);

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(async () => await reader.SkipForeignElementAsync(null, cts.Token), Throws.InstanceOf<OperationCanceledException>());
        }

        [Test]
        public async Task Verify_that_ReadToReqIFDescendant_finds_a_ReqIF_descendant_and_skips_foreign_ones()
        {
            const string xml = "<ROOT><A><o:B xmlns:o=\"http://example.com/other\">x</o:B><C><B>y</B></C></A></ROOT>";

            foreach (var async in new[] { false, true })
            {
                var (logger, warnings) = CreateLogger();
                using var reader = CreateReaderOnFirstChild(xml, async);

                var found = async ? await reader.ReadToReqIFDescendantAsync("B", logger, CancellationToken.None) : reader.ReadToReqIFDescendant("B", logger);

                Assert.That(found, Is.True, $"async: {async}");
                Assert.That(reader.LocalName, Is.EqualTo("B"));
                Assert.That(reader.NamespaceURI, Is.Empty, "the reader must be on the ReqIF B, not on the foreign one");
                Assert.That(reader.Depth, Is.EqualTo(3), "the B in C");
                Assert.That(warnings(), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task Verify_that_ReadToReqIFDescendant_does_not_search_past_the_element_it_started_on()
        {
            const string xml = "<ROOT><A><o:B xmlns:o=\"http://example.com/other\"/></A><B/></ROOT>";

            foreach (var async in new[] { false, true })
            {
                var (logger, warnings) = CreateLogger();
                using var reader = CreateReaderOnFirstChild(xml, async);

                var found = async ? await reader.ReadToReqIFDescendantAsync("B", logger, CancellationToken.None) : reader.ReadToReqIFDescendant("B", logger);

                Assert.That(found, Is.False, $"async: {async}");
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement));
                Assert.That(reader.LocalName, Is.EqualTo("A"), "the reader must stop on the end tag of A, before its sibling B");
                Assert.That(warnings(), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task Verify_that_ReadToReqIFDescendant_on_an_empty_element_returns_false_and_does_not_move()
        {
            foreach (var async in new[] { false, true })
            {
                using var reader = CreateReaderOnFirstChild("<ROOT><A/><B/></ROOT>", async);

                var found = async ? await reader.ReadToReqIFDescendantAsync("B", null, CancellationToken.None) : reader.ReadToReqIFDescendant("B", null);

                Assert.That(found, Is.False, $"async: {async}");
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Element));
                Assert.That(reader.LocalName, Is.EqualTo("A"));
            }
        }

        [Test]
        public async Task Verify_that_ReadToReqIFDescendant_on_a_foreign_element_skips_it_and_returns_false()
        {
            const string xml = "<ROOT><o:A xmlns:o=\"http://example.com/other\"><B/></o:A><C/></ROOT>";

            foreach (var async in new[] { false, true })
            {
                var (logger, warnings) = CreateLogger();
                using var reader = CreateReaderOnFirstChild(xml, async);

                var found = async ? await reader.ReadToReqIFDescendantAsync("B", logger, CancellationToken.None) : reader.ReadToReqIFDescendant("B", logger);

                Assert.That(found, Is.False, $"async: {async}");
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement));
                Assert.That(reader.LocalName, Is.EqualTo("A"), "the reader must be on the end tag of the skipped element");
                Assert.That(warnings(), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task Verify_that_foreign_elements_to_the_end_of_a_subtree_are_skipped_with_a_warning()
        {
            const string xml = "<ROOT><A><B/><o:X xmlns:o=\"http://example.com/other\"><o:Y/></o:X><o:Z xmlns:o=\"http://example.com/other\"/></A><C/></ROOT>";

            foreach (var async in new[] { false, true })
            {
                var (logger, warnings) = CreateLogger();
                using var reader = CreateReaderOnFirstChild(xml, async);

                using (var subtree = reader.ReadSubtree())
                {
                    if (async)
                    {
                        await subtree.SkipForeignElementsToEndAsync(logger, CancellationToken.None);
                    }
                    else
                    {
                        subtree.SkipForeignElementsToEnd(logger);
                    }

                    Assert.That(subtree.EOF, Is.True, $"async: {async}");
                }

                Assert.That(warnings(), Is.EqualTo(2), "X and Z, not Y inside X");

                reader.Read();
                Assert.That(reader.LocalName, Is.EqualTo("C"), "the outer reader must continue after A");
            }
        }

        [Test]
        public async Task Verify_that_ReadToReqIFDescendant_on_an_end_tag_returns_false_and_does_not_move()
        {
            foreach (var async in new[] { false, true })
            {
                using var reader = CreateReaderOnFirstChild("<ROOT><A><B/></A><B/></ROOT>", async);

                reader.Read();
                reader.Read();
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement), "the reader must start on the end tag of A");

                var found = async ? await reader.ReadToReqIFDescendantAsync("B", null, CancellationToken.None) : reader.ReadToReqIFDescendant("B", null);

                Assert.That(found, Is.False, $"async: {async}");
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement));
                Assert.That(reader.LocalName, Is.EqualTo("A"), "the reader must not move to the B after A");
            }
        }

        [Test]
        public async Task Verify_that_ReadToReqIFDescendant_on_whitespace_searches_the_next_element()
        {
            foreach (var async in new[] { false, true })
            {
                using var reader = CreateReaderOnFirstChild("<ROOT>  <A><B/></A></ROOT>", async);

                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.Whitespace), "the reader must start on the whitespace");

                var found = async ? await reader.ReadToReqIFDescendantAsync("B", null, CancellationToken.None) : reader.ReadToReqIFDescendant("B", null);

                Assert.That(found, Is.True, $"async: {async}");
                Assert.That(reader.LocalName, Is.EqualTo("B"));
                Assert.That(reader.Depth, Is.EqualTo(2), "the B in A");
            }
        }

        [Test]
        public void Verify_that_the_asynchronous_reference_helpers_can_be_cancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            using (var reader = CreateReaderOnFirstChild("<ROOT><A><B/><C/></A></ROOT>", true))
            {
                Assert.That(async () => await reader.ReadToReqIFDescendantAsync("C", null, cts.Token), Throws.InstanceOf<OperationCanceledException>());
            }

            using (var reader = CreateReaderOnFirstChild("<ROOT><A><B/><C/></A></ROOT>", true))
            {
                using var subtree = reader.ReadSubtree();

                Assert.That(async () => await subtree.SkipForeignElementsToEndAsync(null, cts.Token), Throws.InstanceOf<OperationCanceledException>());
            }
        }

        /// <summary>
        /// Creates a logger and a function that counts the warnings logged to it
        /// </summary>
        private static (Microsoft.Extensions.Logging.ILogger Logger, Func<int> Warnings) CreateLogger()
        {
            var sink = new TestLogEventSink();
            var serilogLogger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(sink).CreateLogger();
            var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(serilogLogger, dispose: true));

            return (loggerFactory.CreateLogger<XmlReaderExtensionsTestFixture>(), () => sink.Events.Count(e => e.Level == LogEventLevel.Warning));
        }

        /// <summary>
        /// Creates a reader positioned on the first child element of the root
        /// </summary>
        private static XmlReader CreateReaderOnFirstChild(string xml, bool async = false)
        {
            var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = async });

            reader.MoveToContent();
            reader.Read();

            return reader;
        }
    }
}
