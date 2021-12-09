// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueXHTMLTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
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

namespace ReqIFLib.Tests
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeValueXHTML"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueXHTMLTestFixture
    {
        [Test]
        public void VerifyThatTheAttributeDefinitionCanBeSetOrGet()
        {
            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();

            var attributeValueXhtml = new AttributeValueXHTML();
            attributeValueXhtml.Definition = attributeDefinitionXhtml;

            var attributeValue = (AttributeValue)attributeValueXhtml;

            Assert.AreEqual(attributeDefinitionXhtml, attributeValue.AttributeDefinition);

            attributeValue.AttributeDefinition = attributeDefinitionXhtml;

            Assert.AreEqual(attributeDefinitionXhtml, attributeValue.AttributeDefinition);
        }

        [Test]
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var attributeDefinitionString = new AttributeDefinitionReal();
            var attributeValueXhtml = new AttributeValueXHTML();
            var attributeValue = (AttributeValue)attributeValueXhtml;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void Verify_that_GetSchema_returns_null()
        {
            var attributeValue = new AttributeValueXHTML();
            Assert.That(attributeValue.GetSchema(), Is.Null);
        }

        [Test]
        public void VerifyThatWriteXmlWithoutDefinitionSetThrowsSerializationException()
        {
            using (var fs = new FileStream("test.xml", FileMode.Create))
            {
                using (var writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true }))
                {
                    var attributeValueXhtml = new AttributeValueXHTML();
                    Assert.Throws<SerializationException>(() => attributeValueXhtml.WriteXml(writer));
                }
            }     
        }

        [Test]
        public void VerifyConvenienceValueProperty()
        {
            var attributeValue = new AttributeValueXHTML();

            var val = "testetestes";
            attributeValue.ObjectValue = val;

            Assert.AreEqual(attributeValue.TheValue, val);
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
    }
}
