// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionBooleanTestFixture.cs" company="RHEA System S.A.">
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
    using System.Threading.Tasks;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeDefinitionBoolean"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionBooleanTestFixture
    {
        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var datatypeDefinitionBoolean = new DatatypeDefinitionBoolean();

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            attributeDefinitionBoolean.Type = datatypeDefinitionBoolean;

            var attributeDefinition = (AttributeDefinition)attributeDefinitionBoolean;

            Assert.That(attributeDefinition.DatatypeDefinition, Is.EqualTo(datatypeDefinitionBoolean));

            attributeDefinition.DatatypeDefinition = datatypeDefinitionBoolean;

            Assert.That(attributeDefinition.DatatypeDefinition, Is.EqualTo(datatypeDefinitionBoolean));
        }

        [Test]        
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var datatypeDefinitionString = new DatatypeDefinitionString();
            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionBoolean;

            Assert.Throws<ArgumentException>(() => attributeDefinition.DatatypeDefinition = datatypeDefinitionString) ;
        }

        [Test]
        public void Verify_That_WriteXml_Throws_Exception_When_Type_Is_Null()
        {
            using var fs = new FileStream("test.xml", FileMode.Create);
            using var writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true });
            var attributeDefinitionBoolean = new AttributeDefinitionBoolean()
            {
                Identifier = "identifier",
                LongName = "longname"
            };

            Assert.That(() => attributeDefinitionBoolean.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Type property of AttributeDefinitionBoolean identifier:longname may not be null"));
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_When_Type_Is_Null()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeDefinitionBoolean = new AttributeDefinitionBoolean()
            {
                Identifier = "identifier",
                LongName = "longname"
            };

            Assert.That(async () => await attributeDefinitionBoolean.WriteXmlAsync(writer, cancellationTokenSource.Token),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Type property of AttributeDefinitionBoolean identifier:longname may not be null"));
        }

        [Test]
        public void Verify_that_WriteXml_does_not_throw_exception()
        {
            var datatypeDefinitionBoolean = new DatatypeDefinitionBoolean
            {
                Identifier = "datatypeDefinitionBoolean"
            };
            
            var attributeValueBoolean = new AttributeValueBoolean();
            attributeValueBoolean.Definition = new AttributeDefinitionBoolean { Identifier = "default-identifier" };

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean
            {
                Identifier = "attributeDefinitionBoolean",
                Type = datatypeDefinitionBoolean,
                DefaultValue = attributeValueBoolean
            };

            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, ConformanceLevel = ConformanceLevel.Fragment});
            writer.WriteStartElement("TEST");

            Assert.That(() => attributeDefinitionBoolean.WriteXml(writer), Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_WriteXmlAsync_does_not_throw_exception()
        {
            var datatypeDefinitionBoolean = new DatatypeDefinitionBoolean
            {
                Identifier = "datatypeDefinitionBoolean"
            };

            var attributeValueBoolean = new AttributeValueBoolean();
            attributeValueBoolean.Definition = new AttributeDefinitionBoolean { Identifier = "default-identifier" };

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean
            {
                Identifier = "attributeDefinitionBoolean",
                Type = datatypeDefinitionBoolean,
                DefaultValue = attributeValueBoolean
            };

            await using var memoryStream = new MemoryStream();
            await using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, ConformanceLevel = ConformanceLevel.Fragment, Async = true});
            await writer.WriteStartElementAsync(null, "TEST", null);

            var cancellationTokenSource = new CancellationTokenSource();
            Assert.That(async () => await attributeDefinitionBoolean.WriteXmlAsync(writer, cancellationTokenSource.Token), Throws.Nothing);
        }
    }
}
