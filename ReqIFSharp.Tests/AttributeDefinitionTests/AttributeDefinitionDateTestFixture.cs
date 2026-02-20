// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionDateTestFixture.cs" company="Starion Group S.A.">
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

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeDefinitionDate"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionDateTestFixture
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
            Assert.That(() => new AttributeDefinitionDate(null), Throws.Nothing);
            Assert.That(() => new AttributeDefinitionDate(this.loggerFactory), Throws.Nothing);
        }

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

        [Test]
        public void Verify_that_ReadXml_sets_references_and_properties()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-DATE IDENTIFIER="_eULasAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:41.309+01:00" LONG-NAME="Date">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_eULasAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-DATE THE-VALUE="1976-08-20T12:00:00.000+01:00"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-DATE>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionDate = new AttributeDefinitionDate(specificationType, this.loggerFactory);

            attributeDefinitionDate.ReadXml(reader);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(attributeDefinitionDate.Identifier, Is.EqualTo("_eULasAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionDate.LongName, Is.EqualTo("Date"));
                Assert.That(attributeDefinitionDate.AlternativeId.Identifier, Is.EqualTo("_eULasAfhEeelU71CdMk83g"));
            }
        }

        [Test]
        public async Task Verify_that_ReadXmlAsync_sets_references_and_properties()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-DATE IDENTIFIER="_eULasAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:41.309+01:00" LONG-NAME="Date">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_eULasAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-DATE THE-VALUE="1976-08-20T12:00:00.000+01:00"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-DATE>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionDate = new AttributeDefinitionDate(specificationType, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await attributeDefinitionDate.ReadXmlAsync(reader, cts.Token);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(attributeDefinitionDate.Identifier, Is.EqualTo("_eULasAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionDate.LongName, Is.EqualTo("Date"));
                Assert.That(attributeDefinitionDate.AlternativeId.Identifier, Is.EqualTo("_eULasAfhEeelU71CdMk83g"));
            }
        }

        [Test]
        public async Task Verify_that_when_ReadXmlAsync_cancel_throws_OperationCanceledException()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-DATE IDENTIFIER="_eULasAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:41.309+01:00" LONG-NAME="Date">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_eULasAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-DATE THE-VALUE="1976-08-20T12:00:00.000+01:00"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-DATE>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionDate = new AttributeDefinitionDate(specificationType, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await cts.CancelAsync();

            await Assert.ThatAsync(() => attributeDefinitionDate.ReadXmlAsync(reader, cts.Token), Throws.InstanceOf<OperationCanceledException>());
        }
    }
}
