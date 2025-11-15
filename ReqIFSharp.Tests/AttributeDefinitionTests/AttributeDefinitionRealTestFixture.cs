// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionRealTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="AttributeDefinitionReal"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionRealTestFixture
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
            Assert.That(() => new AttributeDefinitionReal(null), Throws.Nothing);
            Assert.That(() => new AttributeDefinitionReal(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var datatypeDefinitionReal = new DatatypeDefinitionReal();

            var attributeDefinitionReal = new AttributeDefinitionReal();
            attributeDefinitionReal.Type = datatypeDefinitionReal;

            var attributeDefinition = (AttributeDefinition)attributeDefinitionReal;

            Assert.That(attributeDefinition.DatatypeDefinition, Is.EqualTo(datatypeDefinitionReal));

            attributeDefinition.DatatypeDefinition = datatypeDefinitionReal;

            Assert.That(attributeDefinition.DatatypeDefinition, Is.EqualTo(datatypeDefinitionReal));
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var datatypeDefinitionString = new DatatypeDefinitionString();
            var attributeDefinitionReal = new AttributeDefinitionReal();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionReal;

            Assert.Throws<ArgumentException>(() => attributeDefinition.DatatypeDefinition = datatypeDefinitionString);
        }

        [Test]
        public void Verify_That_WriteXml_Throws_Exception_When_Type_Is_Null()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeDefinitionReal = new AttributeDefinitionReal();

            Assert.That(() => attributeDefinitionReal.WriteXml(writer), Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_When_Type_Is_Null()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, Async = true});
            var attributeDefinitionReal = new AttributeDefinitionReal();

            Assert.That(async () => await attributeDefinitionReal.WriteXmlAsync(writer, cancellationTokenSource.Token), Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_that_ReadXml_sets_references_and_properties()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-REAL IDENTIFIER="_onD_wAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:38:50.097+01:00" LONG-NAME="Real">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_onD_wAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-REAL THE-VALUE="1.6"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-REAL>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionReal = new AttributeDefinitionReal(specificationType, this.loggerFactory);

            attributeDefinitionReal.ReadXml(reader);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(attributeDefinitionReal.Identifier, Is.EqualTo("_onD_wAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionReal.LongName, Is.EqualTo("Real"));
                Assert.That(attributeDefinitionReal.AlternativeId.Identifier, Is.EqualTo("_onD_wAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionReal.DefaultValue.TheValue, Is.EqualTo(1.6));
            }
        }

        [Test]
        public async Task Verify_that_ReadXmlAsync_sets_references_and_properties()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-REAL IDENTIFIER="_onD_wAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:38:50.097+01:00" LONG-NAME="Real">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_onD_wAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-REAL THE-VALUE="1.6"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-REAL>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionReal = new AttributeDefinitionReal(specificationType, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await attributeDefinitionReal.ReadXmlAsync(reader, cts.Token);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(attributeDefinitionReal.Identifier, Is.EqualTo("_onD_wAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionReal.LongName, Is.EqualTo("Real"));
                Assert.That(attributeDefinitionReal.AlternativeId.Identifier, Is.EqualTo("_onD_wAfhEeelU71CdMk83g"));
                Assert.That(attributeDefinitionReal.DefaultValue.TheValue, Is.EqualTo(1.6));
            }
        }

        [Test]
        public async Task Verify_that_when_ReadXmlAsync_cancel_throws_OperationCanceledException()
        {
            var xml = """
                      <ATTRIBUTE-DEFINITION-REAL IDENTIFIER="_onD_wAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:38:50.097+01:00" LONG-NAME="Real">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_onD_wAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                          <DEFAULT-VALUE>
                              <ATTRIBUTE-VALUE-REAL THE-VALUE="1.6"/>
                          </DEFAULT-VALUE>
                          <TYPE />
                      </ATTRIBUTE-DEFINITION-REAL>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent(this.loggerFactory);
            var specificationType = new SpecificationType(content, this.loggerFactory);
            var attributeDefinitionReal = new AttributeDefinitionReal(specificationType, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await cts.CancelAsync();

            await Assert.ThatAsync(() => attributeDefinitionReal.ReadXmlAsync(reader, cts.Token), Throws.InstanceOf<OperationCanceledException>());
        }
    }
}
