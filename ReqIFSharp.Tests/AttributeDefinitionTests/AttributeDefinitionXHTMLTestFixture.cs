// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionXHTMLTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="AttributeDefinitionXHTML"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionXHTMLTestFixture
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
            Assert.That(() => new AttributeDefinitionXHTML(null), Throws.Nothing);
            Assert.That(() => new AttributeDefinitionXHTML(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var datatypeDefinitionXhtml = new DatatypeDefinitionXHTML();

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            attributeDefinitionXhtml.Type = datatypeDefinitionXhtml;

            var attributeDefinition = (AttributeDefinition)attributeDefinitionXhtml;

            Assert.That(attributeDefinition.DatatypeDefinition, Is.EqualTo(datatypeDefinitionXhtml));

            attributeDefinition.DatatypeDefinition = datatypeDefinitionXhtml;

            Assert.That(attributeDefinition.DatatypeDefinition, Is.EqualTo(datatypeDefinitionXhtml));
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var datatypeDefinitionDate = new DatatypeDefinitionDate();
            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionXhtml;

            Assert.Throws<ArgumentException>(() => attributeDefinition.DatatypeDefinition = datatypeDefinitionDate);
        }

        [Test]
        public void Verify_That_WriteXml_Throws_Exception_When_Type_Is_Null()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            Assert.That(() => attributeDefinitionXhtml.WriteXml(writer), Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_When_Type_Is_Null()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, Async = true });

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            Assert.That(async () => await attributeDefinitionXhtml.WriteXmlAsync(writer, cancellationTokenSource.Token), Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_that_ReadXml_sets_references_and_properties()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-XHTML IDENTIFIER="_Y4OIUAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:03.641+01:00" LONG-NAME="XHTML">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_Y4OIUAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-XHTML THE-VALUE="xhtml string"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-XHTML>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionXhtml = new AttributeDefinitionXHTML(specificationType, this.loggerFactory);

            attributeDefinitionXhtml.ReadXml(reader);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(attributeDefinitionXhtml.Identifier, Is.EqualTo("_Y4OIUAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionXhtml.LongName, Is.EqualTo("XHTML"));
                Assert.That(attributeDefinitionXhtml.AlternativeId.Identifier, Is.EqualTo("_Y4OIUAfhEeelU71CdMk83g"));
            }
        }

        [Test]
        public async Task Verify_that_ReadXmlAsync_sets_references_and_properties()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-XHTML IDENTIFIER="_Y4OIUAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:03.641+01:00" LONG-NAME="XHTML">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_Y4OIUAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-XHTML THE-VALUE="xhtml string"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-XHTML>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionXhtml = new AttributeDefinitionXHTML(specificationType, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await attributeDefinitionXhtml.ReadXmlAsync(reader, cts.Token);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(attributeDefinitionXhtml.Identifier, Is.EqualTo("_Y4OIUAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionXhtml.LongName, Is.EqualTo("XHTML"));
                Assert.That(attributeDefinitionXhtml.AlternativeId.Identifier, Is.EqualTo("_Y4OIUAfhEeelU71CdMk83g"));
            }
        }

        [Test]
        public async Task Verify_that_when_ReadXmlAsync_cancel_throws_OperationCanceledException()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-XHTML IDENTIFIER="_Y4OIUAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:37:03.641+01:00" LONG-NAME="XHTML">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_Y4OIUAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-XHTML THE-VALUE="xhtml string"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-XHTML>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionXhtml = new AttributeDefinitionXHTML(specificationType, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await cts.CancelAsync();

            await Assert.ThatAsync(() => attributeDefinitionXhtml.ReadXmlAsync(reader, cts.Token), Throws.InstanceOf<OperationCanceledException>());
        }
    }
}
