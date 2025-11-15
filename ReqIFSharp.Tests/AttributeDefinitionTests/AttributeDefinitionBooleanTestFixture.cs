// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionBooleanTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="AttributeDefinitionBoolean"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionBooleanTestFixture
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
            Assert.That(() => new AttributeDefinitionBoolean(null), Throws.Nothing);
            Assert.That(() => new AttributeDefinitionBoolean(this.loggerFactory), Throws.Nothing);
        }

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

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean(this.loggerFactory)
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

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean(this.loggerFactory)
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
            var datatypeDefinitionBoolean = new DatatypeDefinitionBoolean(this.loggerFactory)
            {
                Identifier = "datatypeDefinitionBoolean"
            };
            
            var attributeValueBoolean = new AttributeValueBoolean();
            attributeValueBoolean.Definition = new AttributeDefinitionBoolean { Identifier = "default-identifier" };

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean(this.loggerFactory)
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
            var datatypeDefinitionBoolean = new DatatypeDefinitionBoolean(this.loggerFactory)
            {
                Identifier = "datatypeDefinitionBoolean"
            };

            var attributeValueBoolean = new AttributeValueBoolean(this.loggerFactory);
            attributeValueBoolean.Definition = new AttributeDefinitionBoolean { Identifier = "default-identifier" };

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean(this.loggerFactory)
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

        [Test]
        public void Verify_that_ReadXml_sets_references_and_properties()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-BOOLEAN IDENTIFIER="_b6gNkAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:26.006+01:00" LONG-NAME="Bool">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_b6gNkAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-BOOLEAN THE-VALUE="true"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-BOOLEAN>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionBoolean = new AttributeDefinitionBoolean(specificationType, this.loggerFactory);

            attributeDefinitionBoolean.ReadXml(reader);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(attributeDefinitionBoolean.Identifier, Is.EqualTo("_b6gNkAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionBoolean.LongName, Is.EqualTo("Bool"));
                Assert.That(attributeDefinitionBoolean.AlternativeId.Identifier, Is.EqualTo("_b6gNkAfhEeelU71CdMk83g"));
            }
        }

        [Test]
        public async Task Verify_that_ReadXmlAsync_sets_references_and_properties()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-BOOLEAN IDENTIFIER="_b6gNkAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:26.006+01:00" LONG-NAME="Bool">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_b6gNkAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-BOOLEAN THE-VALUE="true"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-BOOLEAN>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionBoolean = new AttributeDefinitionBoolean(specificationType, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await attributeDefinitionBoolean.ReadXmlAsync(reader, cts.Token);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(attributeDefinitionBoolean.Identifier, Is.EqualTo("_b6gNkAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionBoolean.LongName, Is.EqualTo("Bool"));
                Assert.That(attributeDefinitionBoolean.AlternativeId.Identifier, Is.EqualTo("_b6gNkAfhEeelU71CdMk83g"));
            }
        }

        [Test]
        public async Task Verify_that_when_ReadXmlAsync_cancel_throws_OperationCanceledException()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-BOOLEAN IDENTIFIER="_b6gNkAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:26.006+01:00" LONG-NAME="Bool">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_b6gNkAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-BOOLEAN THE-VALUE="true"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-BOOLEAN>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionBoolean = new AttributeDefinitionBoolean(specificationType, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await cts.CancelAsync();

            await Assert.ThatAsync(() => attributeDefinitionBoolean.ReadXmlAsync(reader, cts.Token), Throws.InstanceOf<OperationCanceledException>());
        }
    }
}
