// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueIntegerTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2022 RHEA System S.A.
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
    using System.Threading;
    using System.Runtime.Serialization;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeValueInteger"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueIntegerTestFixture
    {
        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var attributeDefinitionInteger = new AttributeDefinitionInteger();

            var attributeValueInteger = new AttributeValueInteger();
            attributeValueInteger.Definition = attributeDefinitionInteger;

            var attributeValue = (AttributeValue)attributeValueInteger;

            Assert.AreEqual(attributeDefinitionInteger, attributeValue.AttributeDefinition);

            attributeValue.AttributeDefinition = attributeDefinitionInteger;

            Assert.AreEqual(attributeDefinitionInteger, attributeValue.AttributeDefinition);
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var attributeDefinitionString = new AttributeDefinitionString();
            var attributeValueInteger = new AttributeValueInteger();
            var attributeValue = (AttributeValue)attributeValueInteger;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void Verify_That_WriteXml_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeValueInteger = new AttributeValueInteger();
            Assert.That(() => attributeValueInteger.WriteXml(writer), 
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Without_Definition_Set_Throws_SerializationException()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeValueInteger = new AttributeValueInteger();

            Assert.That(async () => await attributeValueInteger.WriteXmlAsync(writer, cancellationTokenSource.Token), 
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_when_cancelled()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeValueInteger = new AttributeValueInteger
            {
                Definition = new AttributeDefinitionInteger()
            };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await attributeValueInteger.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Verify_Convenience_Value_Property()
        {
            var attributeValue = new AttributeValueInteger();

            long val = 3;
            attributeValue.ObjectValue = val;

            Assert.That(attributeValue.TheValue, Is.EqualTo(val));
            Assert.That(attributeValue.ObjectValue, Is.EqualTo(val));
        }

        [Test]
        public void Verify_that_when_ObjectValue_is_not_integer_an_exception_is_thrown()
        {
            var attributeValue = new AttributeValueInteger();

            Assert.That(
                () => attributeValue.ObjectValue = "true",
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("Cannot use true as value for this AttributeValueInteger."));
        }

        [Test]
        public void Verify_that_ReadXmlAsync_throws_exception_when_cancelled()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Datatype-Demo.reqif");

            var cts = new CancellationTokenSource();
            cts.Cancel();

            using var fileStream = File.OpenRead(reqifPath);
            using var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings { Async = true });

            var attributeValueInteger = new AttributeValueInteger();

            Assert.That(async () => await attributeValueInteger.ReadXmlAsync(xmlReader, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }
    }
}