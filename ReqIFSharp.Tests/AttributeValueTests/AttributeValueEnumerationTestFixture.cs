// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueEnumerationTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
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
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeValueEnumeration"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueEnumerationTestFixture
    {
        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();

            var attributeValueEnumeration = new AttributeValueEnumeration();
            attributeValueEnumeration.Definition = attributeDefinitionEnumeration;

            var attributeValue = (AttributeValue)attributeValueEnumeration;

            Assert.That(attributeValue.AttributeDefinition, Is.EqualTo(attributeDefinitionEnumeration));
            
            attributeValue.AttributeDefinition = attributeDefinitionEnumeration;

            Assert.That(attributeValue.AttributeDefinition, Is.EqualTo(attributeDefinitionEnumeration));
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var attributeDefinitionString = new AttributeDefinitionString();
            var attributeValueEnumeration = new AttributeValueEnumeration();
            var attributeValue = (AttributeValue)attributeValueEnumeration;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void Verify_That_WriteXml_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeValueEnumeration = new AttributeValueEnumeration();

            Assert.That(() => attributeValueEnumeration.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeValueEnumeration = new AttributeValueEnumeration();

            var cts = new CancellationTokenSource();

            Assert.That(async () => await attributeValueEnumeration.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_when_cancelled()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeValueEnumeration = new AttributeValueEnumeration
            {
                Definition = new AttributeDefinitionEnumeration()
            };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await attributeValueEnumeration.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Verify_Convenience_Value_Property()
        {
            var attributeValue = new AttributeValueEnumeration();

            var val = new List<EnumValue> { new EnumValue() };
            attributeValue.ObjectValue = val;
            
            Assert.That(attributeValue.Values.Count, Is.EqualTo(1));
        }

        [Test]
        public void Verify_that_when_ObjectValue_is_not_real_an_exception_is_thrown()
        {
            var attributeValue = new AttributeValueEnumeration();

            Assert.That(
                () => attributeValue.ObjectValue = "true",
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("The value to set is not an IEnumerable<EnumValue>."));
        }
    }
}
