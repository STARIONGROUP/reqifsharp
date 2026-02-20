// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionEnumerationTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
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

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeDefinitionEnumeration"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionEnumerationTestFixture
    {
        private ILoggerFactory loggerFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
        }

        [Test]
        public void Verify_that_constructor_does_not_throw_exception()
        {
            Assert.That(() => new AttributeDefinitionEnumeration(null), Throws.Nothing);
            Assert.That(() => new AttributeDefinitionEnumeration(this.loggerFactory), Throws.Nothing);
        }

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

        [Test]
        public void Verify_that_ReadXml_sets_references_and_properties()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-ENUMERATION IDENTIFIER="_gelcwAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:56.048+01:00" LONG-NAME="Enum_Single" MULTI-VALUED="false">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_gelcwAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <TYPE>
                              <DATATYPE-DEFINITION-ENUMERATION-REF>_QIZRcAfhEeelU71CdMk83g</DATATYPE-DEFINITION-ENUMERATION-REF>
                          </TYPE>
                      </ATTRIBUTE-DEFINITION-ENUMERATION>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration(specificationType, this.loggerFactory);

            attributeDefinitionEnumeration.ReadXml(reader);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(attributeDefinitionEnumeration.Identifier, Is.EqualTo("_gelcwAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionEnumeration.LongName, Is.EqualTo("Enum_Single"));
                Assert.That(attributeDefinitionEnumeration.AlternativeId.Identifier, Is.EqualTo("_gelcwAfhEeelU71CdMk83g"));
            }
        }

        [Test]
        public async Task Verify_that_ReadXmlAsync_sets_references_and_properties()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-ENUMERATION IDENTIFIER="_gelcwAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:56.048+01:00" LONG-NAME="Enum_Single" MULTI-VALUED="false">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_gelcwAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <TYPE>
                              <DATATYPE-DEFINITION-ENUMERATION-REF>_QIZRcAfhEeelU71CdMk83g</DATATYPE-DEFINITION-ENUMERATION-REF>
                          </TYPE>
                      </ATTRIBUTE-DEFINITION-ENUMERATION>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration(specificationType, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await attributeDefinitionEnumeration.ReadXmlAsync(reader, cts.Token);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(attributeDefinitionEnumeration.Identifier, Is.EqualTo("_gelcwAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionEnumeration.LongName, Is.EqualTo("Enum_Single"));
                Assert.That(attributeDefinitionEnumeration.AlternativeId.Identifier, Is.EqualTo("_gelcwAfhEeelU71CdMk83g"));
            }
        }

        [Test]
        public async Task Verify_that_when_ReadXmlAsync_cancel_throws_OperationCanceledException()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-ENUMERATION IDENTIFIER="_gelcwAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:56.048+01:00" LONG-NAME="Enum_Single" MULTI-VALUED="false">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_gelcwAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <TYPE>
                              <DATATYPE-DEFINITION-ENUMERATION-REF>_QIZRcAfhEeelU71CdMk83g</DATATYPE-DEFINITION-ENUMERATION-REF>
                          </TYPE>
                      </ATTRIBUTE-DEFINITION-ENUMERATION>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration(specificationType, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await cts.CancelAsync();

            await Assert.ThatAsync(() => attributeDefinitionEnumeration.ReadXmlAsync(reader, cts.Token), Throws.InstanceOf<OperationCanceledException>());
        }
    }
}
