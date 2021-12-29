// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueBooleanTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="AttributeValueBoolean"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueBooleanTestFixture
    {
        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var atrAttributeDefinitionBoolean = new AttributeDefinitionBoolean();

            var attributeValueBoolean = new AttributeValueBoolean();
            attributeValueBoolean.Definition = atrAttributeDefinitionBoolean;

            var attributeValue = (AttributeValue)attributeValueBoolean;

            Assert.AreEqual(atrAttributeDefinitionBoolean, attributeValue.AttributeDefinition);

            attributeValue.AttributeDefinition = atrAttributeDefinitionBoolean;

            Assert.AreEqual(atrAttributeDefinitionBoolean, attributeValue.AttributeDefinition);
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var attributeDefinitionString = new AttributeDefinitionString();
            var attributeValueBoolean = new AttributeValueBoolean();
            var attributeValue = (AttributeValue)attributeValueBoolean;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void Verify_That_WriteXml_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeValueReal = new AttributeValueBoolean();

            Assert.That(
                () => attributeValueReal.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Definition property of an AttributeValueBoolean may not be null"));
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeValueReal = new AttributeValueBoolean();

            var cts = new CancellationTokenSource();

            Assert.That(
                async () => await attributeValueReal.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Definition property of an AttributeValueBoolean may not be null"));
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_when_cancelled()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeValueReal = new AttributeValueBoolean
            {
                Definition = new AttributeDefinitionBoolean()
            };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await attributeValueReal.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Verify_Convenience_Value_Property()
        {
            var attributeValueBoolean = new AttributeValueBoolean();
            attributeValueBoolean.ObjectValue = true;

            Assert.That(attributeValueBoolean.TheValue, Is.True);
            Assert.That(attributeValueBoolean.ObjectValue, Is.True);
        }

        [Test]
        public void Verify_that_when_ObjectValue_is_not_boolean_an_exception_is_thrown()
        {
            var attributeValueBoolean = new AttributeValueBoolean();

            Assert.That(
                () => attributeValueBoolean.ObjectValue = "true",
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("Cannot use true as value for this AttributeValueBoolean."));
        }

        [Test]
        public void Verify_that_ReadXmlAsync_throws_exception_when_cancelled()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Datatype-Demo.reqif");

            var cts = new CancellationTokenSource();
            cts.Cancel();

            using var fileStream = File.OpenRead(reqifPath);
            using var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings { Async = true });

            var attributeValueBoolean = new AttributeValueBoolean();

            Assert.That(async () => await attributeValueBoolean.ReadXmlAsync(xmlReader, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }
    }
}
