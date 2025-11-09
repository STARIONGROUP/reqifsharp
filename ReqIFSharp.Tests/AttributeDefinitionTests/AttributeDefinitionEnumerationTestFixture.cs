// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionEnumerationTestFixture.cs" company="Starion Group S.A.">
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

using Microsoft.Extensions.Logging.Abstractions;

namespace ReqIFSharp.Tests
{
    using NUnit.Framework;
    using ReqIFSharp;
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeDefinitionEnumeration"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionEnumerationTestFixture
    {
        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var datatypeDefinitionEnumeration = new DatatypeDefinitionEnumeration();

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            attributeDefinitionEnumeration.Type = datatypeDefinitionEnumeration;

            var attributeDefinition = (AttributeDefinition)attributeDefinitionEnumeration;

            Assert.That(attributeDefinition.DatatypeDefinition, Is.EqualTo(datatypeDefinitionEnumeration));

            attributeDefinition.DatatypeDefinition = datatypeDefinitionEnumeration;

            Assert.That(attributeDefinition.DatatypeDefinition, Is.EqualTo(datatypeDefinitionEnumeration));
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var datatypeDefinitionString = new DatatypeDefinitionString();
            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionEnumeration;

            Assert.Throws<ArgumentException>(() => attributeDefinition.DatatypeDefinition = datatypeDefinitionString);
        }

        [Test]
        public void Verify_That_WriteXml_Throws_Exception_When_Type_I_sNull()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, ConformanceLevel = ConformanceLevel.Fragment });
            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();

            Assert.That(() => attributeDefinitionEnumeration.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_When_Type_I_sNull()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, Async = true, ConformanceLevel = ConformanceLevel.Fragment});
            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();

            Assert.That(async () => await attributeDefinitionEnumeration.WriteXmlAsync(writer, cancellationTokenSource.Token),
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_that_when_invalid_IsMultiValued_Exception_is_raised()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-ENUMERATION IDENTIFIER="AD1" MULTI-VALUED="not-a-bool" />
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var specType = new SpecificationType { ReqIFContent = new ReqIFContent() };
            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration(specType, NullLoggerFactory.Instance);

            Assert.That(() => attributeDefinitionEnumeration.ReadXml(xmlReader),
                Throws.InstanceOf<SerializationException>());
        }
    }
}
