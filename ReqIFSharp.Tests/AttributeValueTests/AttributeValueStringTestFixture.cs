// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueStringTestFixture.cs" company="RHEA System S.A.">
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
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeValueString"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueStringTestFixture
    {
        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var attributeDefinitionString = new AttributeDefinitionString();

            var attributeValueString = new AttributeValueString();
            attributeValueString.Definition = attributeDefinitionString;

            var attributeValue = (AttributeValue)attributeValueString;

            Assert.That(attributeValue.AttributeDefinition, Is.EqualTo(attributeDefinitionString));

            attributeValue.AttributeDefinition = attributeDefinitionString;

            Assert.That(attributeValue.AttributeDefinition, Is.EqualTo(attributeDefinitionString));
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var attributeDefinitionString = new AttributeDefinitionReal();
            var attributeValueString = new AttributeValueString();
            var attributeValue = (AttributeValue)attributeValueString;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void Verify_That_WriteXml_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeValueString = new AttributeValueString();

            Assert.That(() => attributeValueString.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeValueString = new AttributeValueString();

            var cancellationTokenSource = new CancellationTokenSource();

            Assert.That(() => attributeValueString.WriteXmlAsync(writer, cancellationTokenSource.Token),
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_when_cancelled()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeValueString = new AttributeValueString
            {
                Definition = new AttributeDefinitionString()
            };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await attributeValueString.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Verify_Convenience_Value_Property()
        {
            var attributeValue = new AttributeValueString();

            var val = "test";
            attributeValue.ObjectValue = val;

            Assert.That(val, Is.EqualTo(attributeValue.TheValue));
            Assert.That(val, Is.EqualTo(attributeValue.ObjectValue));
        }

        [Test]
        public void Verify_that_ReadXmlAsync_throws_exception_when_cancelled()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Datatype-Demo.reqif");

            var cts = new CancellationTokenSource();
            cts.Cancel();

            using var fileStream = File.OpenRead(reqifPath);
            using var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings { Async = true });

            var attributeValueReal = new AttributeValueReal();

            Assert.That(async () => await attributeValueReal.ReadXmlAsync(xmlReader, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }
    }
}
