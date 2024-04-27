// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionDateTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
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
    using System.Threading.Tasks;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeDefinitionDate"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionDateTestFixture
    {
        [Test]
        public void Verify_That_The_Attribute_Definition_Can_Be_Set_Or_Get()
        {
            var datatypeDefinitionDate = new DatatypeDefinitionDate();

            var attributeDefinitionDate = new AttributeDefinitionDate();
            attributeDefinitionDate.Type = datatypeDefinitionDate;

            var attributeDefinition = (AttributeDefinition)attributeDefinitionDate;

            Assert.That(attributeDefinition.DatatypeDefinition, Is.EqualTo(datatypeDefinitionDate));

            attributeDefinition.DatatypeDefinition = datatypeDefinitionDate;

            Assert.That(attributeDefinition.DatatypeDefinition, Is.EqualTo(datatypeDefinitionDate));
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var datatypeDefinitionString = new DatatypeDefinitionString();
            var attributeDefinitionDate = new AttributeDefinitionDate();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionDate;

            Assert.Throws<ArgumentException>(() => attributeDefinition.DatatypeDefinition = datatypeDefinitionString);
        }

        [Test]
        public void Verify_That_WriteXml_Throws_Exception_When_Type_Is_Null()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeDefinitionDate = new AttributeDefinitionDate()
            {
                Identifier = "identifier",
                LongName = "longname"
            };

            Assert.That(() => attributeDefinitionDate.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Type property of AttributeDefinitionDate identifier:longname may not be null"));

            Assert.Throws<SerializationException>(() => attributeDefinitionDate.WriteXml(writer));
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_When_Type_Is_Null()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeDefinitionDate = new AttributeDefinitionDate()
            {
                Identifier = "identifier",
                LongName = "longname"
            };

            Assert.That(async () => await attributeDefinitionDate.WriteXmlAsync(writer, cancellationTokenSource.Token),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Type property of AttributeDefinitionDate identifier:longname may not be null"));
        }

        [Test]
        public void Verify_that_WriteXml_does_not_throw_exception()
        {
            var datatypeDefinitionDate = new DatatypeDefinitionDate
            {
                Identifier = "datatypeDefinitionDate"
            };

            var attributeValueDate = new AttributeValueDate();
            attributeValueDate.Definition = new AttributeDefinitionDate { Identifier = "default-identifier" };

            var attributeDefinitionDate = new AttributeDefinitionDate
            {
                Identifier = "attributeDefinitionDate",
                Type = datatypeDefinitionDate,
                DefaultValue = attributeValueDate
            };

            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, ConformanceLevel = ConformanceLevel.Fragment });
            writer.WriteStartElement("TEST");

            Assert.That(() => attributeDefinitionDate.WriteXml(writer),
                Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_WriteXmlAsync_does_not_throw_exception()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var datatypeDefinitionDate = new DatatypeDefinitionDate
            {
                Identifier = "datatypeDefinitionDate"
            };

            var attributeValueDate = new AttributeValueDate();
            attributeValueDate.Definition = new AttributeDefinitionDate { Identifier = "default-identifier" };

            var attributeDefinitionDate = new AttributeDefinitionDate
            {
                Identifier = "attributeDefinitionDate",
                Type = datatypeDefinitionDate,
                DefaultValue = attributeValueDate
            };

            await using var memoryStream = new MemoryStream();
            await using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, ConformanceLevel = ConformanceLevel.Fragment, Async = true});
            await writer.WriteStartElementAsync(null, "TEST", null);

            Assert.That(async () => await attributeDefinitionDate.WriteXmlAsync(writer, cancellationTokenSource.Token),
                Throws.Nothing);
        }
    }
}
