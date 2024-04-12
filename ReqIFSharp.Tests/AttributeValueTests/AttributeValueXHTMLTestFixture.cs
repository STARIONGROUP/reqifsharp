// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueXHTMLTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2024 RHEA System S.A.
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
    using System.Runtime.Serialization;
    using System.Threading;
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
