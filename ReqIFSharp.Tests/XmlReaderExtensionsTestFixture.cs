// -------------------------------------------------------------------------------------------------
// <copyright file="XmlReaderExtensionsTestFixture.cs" company="Starion Group S.A.">
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
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

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
