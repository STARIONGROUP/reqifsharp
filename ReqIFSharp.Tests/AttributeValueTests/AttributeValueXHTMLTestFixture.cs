// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueXHTMLTestFixture.cs" company="Starion Group S.A.">
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
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeValueXHTML"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueXHTMLTestFixture
    {
        private ILoggerFactory loggerFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
        }

        [Test]
        public void Verify_that_constructor_does_not_throw_exception()
        {
            Assert.That(() => new AttributeValueXHTML(null), Throws.Nothing);
            Assert.That(() => new AttributeValueXHTML(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();

            var attributeValueXhtml = new AttributeValueXHTML();
            attributeValueXhtml.Definition = attributeDefinitionXhtml;

            var attributeValue = (AttributeValue)attributeValueXhtml;

            Assert.That(attributeValue.AttributeDefinition, Is.EqualTo(attributeDefinitionXhtml));

            attributeValue.AttributeDefinition = attributeDefinitionXhtml;

            Assert.That(attributeValue.AttributeDefinition, Is.EqualTo(attributeDefinitionXhtml));
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var attributeDefinitionString = new AttributeDefinitionReal();
            var attributeValueXhtml = new AttributeValueXHTML();
            var attributeValue = (AttributeValue)attributeValueXhtml;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_AttributeDefinition_Is_Null()
        {
            var attributeValueXhtml = new AttributeValueXHTML();
            var attributeValue = (AttributeValue)attributeValueXhtml;

            Assert.That(
                () => attributeValue.AttributeDefinition = null,
                Throws.Exception.TypeOf<ArgumentNullException>()
                    .With.Property("ParamName").EqualTo("attributeDefinition"));
        }

        [Test]
        public void Verify_That_WriteXml_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeValueXhtml = new AttributeValueXHTML();

            Assert.That(() => attributeValueXhtml.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeValueXhtml = new AttributeValueXHTML();

            var cancellationTokenSource = new CancellationTokenSource();

            Assert.That(() => attributeValueXhtml.WriteXmlAsync(writer, cancellationTokenSource.Token),
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_that_every_public_constructor_creates_an_empty_list_of_external_objects()
        {
            // GH-98: the ILoggerFactory constructor left ExternalObjects null
            Assert.Multiple(() =>
            {
                Assert.That(new AttributeValueXHTML().ExternalObjects, Is.Not.Null.And.Empty);
                Assert.That(new AttributeValueXHTML(this.loggerFactory).ExternalObjects, Is.Not.Null.And.Empty);
                Assert.That(new AttributeValueXHTML(null).ExternalObjects, Is.Not.Null.And.Empty);
            });
        }

        [Test]
        public async Task Verify_that_a_value_created_with_the_parameterless_constructor_can_be_read()
        {
            // GH-98: the parameterless constructor left the logger null, so reading threw an ArgumentNullException
            const string xml = "<ATTRIBUTE-VALUE-XHTML xmlns=\"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd\" IS-SIMPLIFIED=\"true\">" +
                               "<THE-VALUE><xhtml:p xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">text</xhtml:p></THE-VALUE>" +
                               "</ATTRIBUTE-VALUE-XHTML>";

            foreach (var async in new[] { false, true })
            {
                using var xmlReader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = async });
                xmlReader.MoveToContent();

                var attributeValueXhtml = new AttributeValueXHTML();

                if (async)
                {
                    await attributeValueXhtml.ReadXmlAsync(xmlReader, CancellationToken.None);
                }
                else
                {
                    attributeValueXhtml.ReadXml(xmlReader);
                }

                Assert.That(attributeValueXhtml.TheValue, Does.Contain("text"), $"async: {async}");
                Assert.That(attributeValueXhtml.IsSimplified, Is.True);
            }
        }

        [Test]
        public void Verify_that_ReadXmlAsync_of_a_value_created_with_the_parameterless_constructor_can_be_cancelled()
        {
            const string xml = "<ATTRIBUTE-VALUE-XHTML xmlns=\"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd\">" +
                               "<THE-VALUE><xhtml:p xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">text</xhtml:p></THE-VALUE>" +
                               "</ATTRIBUTE-VALUE-XHTML>";

            var cts = new CancellationTokenSource();
            cts.Cancel();

            using var xmlReader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            xmlReader.MoveToContent();

            Assert.That(async () => await new AttributeValueXHTML().ReadXmlAsync(xmlReader, cts.Token),
                Throws.Exception.InstanceOf<OperationCanceledException>());
        }

        [Test]
        public void Verify_that_ReadXmlAsync_throws_exception_when_cancelled()
        {
            const string xml = "<ATTRIBUTE-VALUE-XHTML xmlns=\"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd\">" +
                               "<THE-VALUE><xhtml:p xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">text</xhtml:p></THE-VALUE>" +
                               "</ATTRIBUTE-VALUE-XHTML>";

            var cts = new CancellationTokenSource();
            cts.Cancel();

            using var xmlReader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            xmlReader.MoveToContent();

            var attributeValueXhtml = new AttributeValueXHTML(this.loggerFactory);

            Assert.That(async () => await attributeValueXhtml.ReadXmlAsync(xmlReader, cts.Token),
                Throws.Exception.InstanceOf<OperationCanceledException>());

            Assert.That(attributeValueXhtml.TheValue, Is.Null, "nothing must be read after the cancellation");
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_when_cancelled()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeValueXhtml = new AttributeValueXHTML
            {
                Definition = new AttributeDefinitionXHTML()
            };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await attributeValueXhtml.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Verify_Convenience_Value_Property()
        {
            var attributeValue = new AttributeValueXHTML();

            var val = "testetestes";
            attributeValue.ObjectValue = val;

            Assert.That(val, Is.EqualTo(attributeValue.TheValue));
            Assert.That(val, Is.EqualTo(attributeValue.ObjectValue));
        }

        [Test]
        public void Verify_that_when_ObjectValue_is__set_to_null_exception_is_raised_when_setter_is_called()
        {
            var attributeValue = new AttributeValueXHTML();

            Assert.That(() => attributeValue.ObjectValue = null, Throws.TypeOf<ArgumentNullException>()); 
        }

        [Test]
        public void Verify_that_raw_text_can_be_extracted_from_xtml_value()
        {
            var attributeValue = new AttributeValueXHTML();

            attributeValue.TheValue = null;
            Assert.That(attributeValue.ExtractUnformattedTextFromValue(), Is.Empty);

            attributeValue.TheValue = "";
            Assert.That(attributeValue.ExtractUnformattedTextFromValue(), Is.Empty);
            
            var val = "<xhtml:div>Description of the SpecObject that includes formatted tables and/or style:<xhtml:ul class=\"noindent\"><xhtml:li>Element 1</xhtml:li><xhtml:li>Element 2</xhtml:li></xhtml:ul></xhtml:div>";
            attributeValue.ObjectValue = val;

            var unformattedText = attributeValue.ExtractUnformattedTextFromValue();

            Assert.That(unformattedText, Is.EqualTo("Description of the SpecObject that includes formatted tables and/or style: Element 1 Element 2"));
        }

        [Test]
        public void Verify_that_normalized_text_has_no_extra_spaces_around_inline_elements()
        {
            // the example of GH-92
            var attributeValue = new AttributeValueXHTML
            {
                TheValue = "<xhtml:div xmlns:xhtml=\"http://www.w3.org/1999/xhtml\"><xhtml:p>brew a cup in <xhtml:b>under 30 seconds</xhtml:b>.</xhtml:p></xhtml:div>"
            };

            Assert.That(attributeValue.ExtractNormalizedTextFromValue(), Is.EqualTo("brew a cup in under 30 seconds."));
            Assert.That(attributeValue.ExtractUnformattedTextFromValue(), Is.EqualTo("brew a cup in  under 30 seconds ."), "the existing method keeps its behaviour");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("<xhtml:div/>")]
        [TestCase("<xhtml:div>  \n\t </xhtml:div>")]
        public void Verify_that_the_normalized_text_of_a_value_without_text_is_empty(string value)
        {
            var attributeValue = new AttributeValueXHTML { TheValue = value };

            Assert.That(attributeValue.ExtractNormalizedTextFromValue(), Is.Empty);
            Assert.That(attributeValue.ExtractNormalizedTextFromValue(true), Is.Empty);
        }

        [Test]
        public void Verify_that_the_line_breaks_and_indentation_of_the_source_are_collapsed()
        {
            var attributeValue = new AttributeValueXHTML
            {
                TheValue = "<xhtml:div>\r\n    <xhtml:p>\r\n        The   system\tshall\r\n        <xhtml:em>log</xhtml:em>  every request.\r\n    </xhtml:p>\r\n</xhtml:div>"
            };

            Assert.That(attributeValue.ExtractNormalizedTextFromValue(), Is.EqualTo("The system shall log every request."));
            Assert.That(attributeValue.ExtractNormalizedTextFromValue(true), Is.EqualTo("The system shall log every request."));
        }

        [TestCase("<xhtml:p>10&#160;kg</xhtml:p>", "10\u00A0kg")]
        [TestCase("<xhtml:p>a&#160;&#160;b</xhtml:p>", "a\u00A0\u00A0b")]
        [TestCase("<xhtml:p>&#160;</xhtml:p>", "\u00A0")]
        [TestCase("<xhtml:p>x&#160;</xhtml:p>", "x\u00A0")]
        [TestCase("<xhtml:p>a &#160; b</xhtml:p>", "a \u00A0 b")]
        public void Verify_that_non_breaking_spaces_are_kept(string value, string expected)
        {
            var attributeValue = new AttributeValueXHTML { TheValue = value };

            Assert.That(attributeValue.ExtractNormalizedTextFromValue(), Is.EqualTo(expected));
            Assert.That(attributeValue.ExtractNormalizedTextFromValue(true), Is.EqualTo(expected));
        }

        [TestCase("  plain   text  ", "plain text", Description = "a value without elements")]
        [TestCase("<xhtml:p>a<?pi data?>b</xhtml:p>", "ab", Description = "processing instructions are not text")]
        [TestCase("<xhtml:p>see <xhtml:a href=\"http://x\" title=\"tip\">link</xhtml:a></xhtml:p>", "see link", Description = "attribute values are not text")]
        [TestCase("<xhtml:p>see <xhtml:a title=\"caf&eacute;\">x</xhtml:a></xhtml:p>", "see x", Description = "an HTML entity in an attribute value does not throw")]
        [TestCase("<xhtml:p xml:space=\"preserve\"><xhtml:b>x</xhtml:b> <xhtml:i>y</xhtml:i></xhtml:p>", "x y", Description = "xml:space=\"preserve\" gives significant whitespace, which is kept as one space")]
        [TestCase("<xhtml:p><xhtml:b>bold</xhtml:b> <xhtml:i>italic</xhtml:i>\n<xhtml:span>span</xhtml:span></xhtml:p>", "bold italic span")]
        [TestCase("<xhtml:pre><xhtml:b>a</xhtml:b>   <xhtml:i>b</xhtml:i></xhtml:pre>", "a   b")]
        public void Verify_that_whitespace_between_inline_elements_is_kept(string value, string expected)
        {
            // the whitespace between the elements is a text node of its own
            var attributeValue = new AttributeValueXHTML { TheValue = value };

            Assert.That(attributeValue.ExtractNormalizedTextFromValue(), Is.EqualTo(expected));
            Assert.That(attributeValue.ExtractNormalizedTextFromValue(true), Is.EqualTo(expected));
        }

        [TestCase("<xhtml:pre>a   b</xhtml:pre>", "a   b", "a   b")]
        [TestCase("<xhtml:pre>line 1\nline 2</xhtml:pre>", "line 1 line 2", "line 1\nline 2")]
        [TestCase("<xhtml:pre>line 1\r\nline 2</xhtml:pre>", "line 1 line 2", "line 1\nline 2")]
        [TestCase("<xhtml:pre>line 1\rline 2</xhtml:pre>", "line 1 line 2", "line 1\nline 2", Description = "a carriage return alone is a line break too")]
        [TestCase("<xhtml:pre>a<xhtml:pre>b</xhtml:pre>c   d</xhtml:pre>", "a b c   d", "a\nb\nc   d", Description = "the text after a nested pre is still kept as written")]
        [TestCase("<xhtml:pre>\n    code\n\tmore\n</xhtml:pre>", "    code \tmore", "    code\n\tmore")]
        [TestCase("<xhtml:pre>a\n\nb</xhtml:pre>", "a b", "a\n\nb")]
        [TestCase("<xhtml:pre>a\n  \nb</xhtml:pre>", "a b", "a\n  \nb", Description = "a line of spaces inside the block is kept as written")]
        [TestCase("<xhtml:pre>&#160;</xhtml:pre><xhtml:p>z</xhtml:p>", "  z", " \nz", Description = "a non-breaking space is not a blank line")]
        [TestCase("<xhtml:pre>a<xhtml:br/>b</xhtml:pre>", "a b", "a\nb")]
        [TestCase("<xhtml:pre>x  <xhtml:b>y</xhtml:b>  z</xhtml:pre>", "x  y  z", "x  y  z")]
        [TestCase("<xhtml:p>before   it</xhtml:p><xhtml:pre>  kept  </xhtml:pre><xhtml:p>after   it</xhtml:p>", "before it   kept   after it", "before it\n  kept  \nafter it")]
        [TestCase("<xhtml:pre>a</xhtml:pre><xhtml:pre>b</xhtml:pre>", "a b", "a\nb")]
        [TestCase("<xhtml:pre>  \n \t </xhtml:pre>", "", "")]
        [TestCase("<xhtml:pre/>", "", "")]
        [TestCase("<XHTML:PRE>a   b</XHTML:PRE>", "a   b", "a   b")]
        public void Verify_that_the_text_of_a_pre_element_is_kept_as_written(string value, string expected, string expectedWithLineBreaks)
        {
            var attributeValue = new AttributeValueXHTML { TheValue = value };

            Assert.That(attributeValue.ExtractNormalizedTextFromValue(), Is.EqualTo(expected));
            Assert.That(attributeValue.ExtractNormalizedTextFromValue(true), Is.EqualTo(expectedWithLineBreaks));
        }

        [TestCase("<xhtml:div><xhtml:p>first</xhtml:p><xhtml:p>second</xhtml:p></xhtml:div>", "first second", "first\nsecond")]
        [TestCase("<xhtml:div>intro<xhtml:ul><xhtml:li>one</xhtml:li><xhtml:li>two</xhtml:li></xhtml:ul>end</xhtml:div>", "intro one two end", "intro\none\ntwo\nend")]
        [TestCase("<xhtml:p>line one<xhtml:br/>line two</xhtml:p>", "line one line two", "line one\nline two")]
        [TestCase("<xhtml:h1>Title</xhtml:h1><xhtml:p>text</xhtml:p>", "Title text", "Title\ntext")]
        [TestCase("<xhtml:table><xhtml:tr><xhtml:td>a</xhtml:td><xhtml:td>b</xhtml:td></xhtml:tr><xhtml:tr><xhtml:th>c</xhtml:th><xhtml:th>d</xhtml:th></xhtml:tr></xhtml:table>", "a b c d", "a b\nc d")]
        [TestCase("<xhtml:div><xhtml:div><xhtml:p>nested</xhtml:p></xhtml:div></xhtml:div>", "nested", "nested")]
        [TestCase("<xhtml:p>a</xhtml:p><xhtml:hr/><xhtml:blockquote>b</xhtml:blockquote><xhtml:dl><xhtml:dt>c</xhtml:dt><xhtml:dd>d</xhtml:dd></xhtml:dl><xhtml:ol><xhtml:li>e</xhtml:li></xhtml:ol>", "a b c d e", "a\nb\nc\nd\ne")]
        [TestCase("<XHTML:P>x</XHTML:P><P>y</P>", "x y", "x\ny")]
        [TestCase("<xhtml:h2>A</xhtml:h2><xhtml:h3>B</xhtml:h3><xhtml:h4>C</xhtml:h4><xhtml:h5>D</xhtml:h5><xhtml:h6>E</xhtml:h6><xhtml:address>F</xhtml:address>", "A B C D E F", "A\nB\nC\nD\nE\nF")]
        [TestCase("<xhtml:table><xhtml:caption>cap</xhtml:caption><xhtml:thead>h</xhtml:thead><xhtml:tbody>b</xhtml:tbody><xhtml:tfoot>f</xhtml:tfoot></xhtml:table>", "cap h b f", "cap\nh\nb\nf")]
        public void Verify_that_block_elements_separate_lines(string value, string expected, string expectedWithLineBreaks)
        {
            var attributeValue = new AttributeValueXHTML { TheValue = value };

            Assert.That(attributeValue.ExtractNormalizedTextFromValue(), Is.EqualTo(expected));
            Assert.That(attributeValue.ExtractNormalizedTextFromValue(true), Is.EqualTo(expectedWithLineBreaks));
        }

        [Test]
        public void Verify_that_normalized_text_reads_every_top_level_element_cdata_and_no_comments()
        {
            var attributeValue = new AttributeValueXHTML
            {
                TheValue = "<xhtml:p>first <!-- not text --><![CDATA[a < b]]></xhtml:p><xhtml:p xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">second</xhtml:p><p>third</p>"
            };

            Assert.That(attributeValue.ExtractNormalizedTextFromValue(true), Is.EqualTo("first a < b\nsecond\nthird"));
        }

        [Test]
        public void Verify_that_the_normalized_text_of_a_value_that_is_not_well_formed_throws()
        {
            var attributeValue = new AttributeValueXHTML { TheValue = "<xhtml:p>unclosed" };

            Assert.That(() => attributeValue.ExtractNormalizedTextFromValue(), Throws.InstanceOf<XmlException>());
        }

        [Test]
        public void Verify_that_character_and_entity_references_are_expanded()
        {
            var attributeValue = new AttributeValueXHTML
            {
                TheValue = "<xhtml:p>a &amp; b &lt; c &gt; d &quot;e&quot; &apos;f&apos; &#x2014; &#8212; g</xhtml:p>"
            };

            Assert.That(attributeValue.ExtractNormalizedTextFromValue(), Is.EqualTo("a & b < c > d \"e\" 'f' — — g"));
        }

        [TestCase("<xhtml:p>a&nbsp;b</xhtml:p>", "a b")]
        [TestCase("<xhtml:p>caf&eacute; &hellip; &copy;</xhtml:p>", "café … ©")]
        [TestCase("<xhtml:p>&nbsp;x&nbsp;</xhtml:p>", " x ")]
        [TestCase("<xhtml:pre>a&nbsp;  &eacute;</xhtml:pre>", "a   é")]
        public void Verify_that_html_named_entities_are_decoded(string value, string expected)
        {
            // XHTML in ReqIF has no DTD that declares the HTML entities, so the reader does not expand them
            var attributeValue = new AttributeValueXHTML { TheValue = value };

            Assert.That(attributeValue.ExtractNormalizedTextFromValue(), Is.EqualTo(expected));
            Assert.That(attributeValue.ExtractNormalizedTextFromValue(true), Is.EqualTo(expected));
        }

        // the position is that of the entity name in the value, 1-based, on the first line and on later lines
        [TestCase("<xhtml:p>a &unknown; b</xhtml:p>", 1, 13)]
        [TestCase("<xhtml:p>x\n</xhtml:p>\n<xhtml:p>a &unknown; b</xhtml:p>", 3, 13)]
        public void Verify_that_an_unknown_entity_throws_with_its_position(string value, int lineNumber, int linePosition)
        {
            var attributeValue = new AttributeValueXHTML { TheValue = value };

            var exception = Assert.Throws<XmlException>(() => attributeValue.ExtractNormalizedTextFromValue());

            Assert.That(exception.Message, Does.Contain("&unknown;"));
            Assert.That(exception.LineNumber, Is.EqualTo(lineNumber));
            Assert.That(exception.LinePosition, Is.EqualTo(linePosition));
        }

        [Test]
        public void Verify_that_a_value_that_is_not_well_formed_throws_with_its_position_in_the_value()
        {
            // the name of the closing tag, on column 13, does not match the open element
            var attributeValue = new AttributeValueXHTML { TheValue = "<xhtml:p>a</xhtml:b>" };

            var exception = Assert.Throws<XmlException>(() => attributeValue.ExtractNormalizedTextFromValue());

            Assert.That(exception.LineNumber, Is.EqualTo(1));
            Assert.That(exception.LinePosition, Is.EqualTo(13));
        }

        [Test]
        public void Verify_that_a_document_type_declaration_in_the_value_is_rejected()
        {
            // the value is read as an XML fragment, in which a DOCTYPE is not allowed, so it is rejected whatever the
            // DTD setting; DtdProcessing.Prohibit stays as a second line of defence
            var attributeValue = new AttributeValueXHTML { TheValue = "<!DOCTYPE p [<!ENTITY e \"expanded\">]><p>&e;</p>" };

            Assert.That(() => attributeValue.ExtractNormalizedTextFromValue(), Throws.InstanceOf<XmlException>());
        }

        [Test]
        public void Verify_that_the_normalized_text_of_the_xhtml_values_of_a_test_document_is_normalized()
        {
            var reqif = new ReqIFDeserializer().Deserialize(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "reqifsharpgenerated.reqif")).Single();

            var values = reqif.CoreContent.SpecObjects.SelectMany(x => x.Values).OfType<AttributeValueXHTML>().ToList();

            Assert.That(values, Is.Not.Empty);

            foreach (var text in values.Select(x => x.ExtractNormalizedTextFromValue()))
            {
                Assert.That(text, Is.Not.Empty);
                Assert.That(text, Does.Not.Contain("  ").And.Not.Contain("\n").And.Not.StartWith(" ").And.Not.EndWith(" "));
                Assert.That(text, Does.StartWith("XhtmlPType").And.Contain("text before br text after br"));
                Assert.That(text, Does.Not.Contain("eclipse.org").And.Not.Contain("text-decoration"), "attribute values are not text");
            }
        }

        [Test]
        public void Verify_that_when_IsSimpe_is_incorrect_bool_exception_is_thrown()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Datatype-Demo-XhtML-Fault.reqif");
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            Assert.That(() => deserializer.Deserialize(reqifPath, false),
                Throws.TypeOf<SerializationException>()
                    .With.Message.StartsWith("The AttributeValueXHTML.IS-SIMPLIFIED"));

        }
    }
}
