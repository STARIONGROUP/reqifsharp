// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueRealTestFixture.cs" company="RHEA System S.A.">
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

namespace ReqIFLib.Tests
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeValueReal"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueRealTestFixture
    {
        private ILoggerFactory loggerFactory;

        [SetUp]
        public void SetUp()
        {
            this.loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        }

        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var attributeDefinitionReal = new AttributeDefinitionReal();

            var attributeValueReal = new AttributeValueReal();
            attributeValueReal.Definition = attributeDefinitionReal;

            var attributeValue = (AttributeValue)attributeValueReal;

            Assert.AreEqual(attributeDefinitionReal, attributeValue.AttributeDefinition);

            attributeValue.AttributeDefinition = attributeDefinitionReal;

            Assert.AreEqual(attributeDefinitionReal, attributeValue.AttributeDefinition);
        }

        [Test]
        public void Verify_Constructor_with_parameters()
        {
            var reqIFContent = new ReqIFContent();

            var specObjectType = new SpecObjectType(reqIFContent, this.loggerFactory);

            var attributeDefinitionReal = new AttributeDefinitionReal(specObjectType, this.loggerFactory);

            var attributeValueReal = new AttributeValueReal(attributeDefinitionReal, this.loggerFactory);

            Assert.That(attributeValueReal.OwningDefinition, Is.EqualTo(attributeDefinitionReal));
        }

        [Test]
        public void Verify_That_WriteXml_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeValueReal = new AttributeValueReal();

            Assert.That(() => attributeValueReal.WriteXml(writer), 
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeValueReal = new AttributeValueReal();

            var cancellationTokenSource = new CancellationTokenSource();

            Assert.That(() => attributeValueReal.WriteXmlAsync(writer, cancellationTokenSource.Token),
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_when_cancelled()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeValueReal = new AttributeValueReal
            {
                Definition = new AttributeDefinitionReal()
            };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await attributeValueReal.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var attributeDefinitionString = new AttributeDefinitionString();
            var attributeValueReal = new AttributeValueReal();
            var attributeValue = (AttributeValue)attributeValueReal;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void Verify_Convenience_Value_Property()
        {
            var attributeValue = new AttributeValueReal();

            var val = 3.66;
            attributeValue.ObjectValue = val;

            Assert.AreEqual(attributeValue.TheValue, val);
            Assert.AreEqual(attributeValue.ObjectValue, val);
        }

        [Test]
        public void Verify_that_when_ObjectValue_is_not_real_an_exception_is_thrown()
        {
            var attributeValue = new AttributeValueReal();

            Assert.That(
                () => attributeValue.ObjectValue = "true",
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("Cannot use true as value for this AttributeValueDouble."));
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
