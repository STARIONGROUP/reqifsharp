// -------------------------------------------------------------------------------------------------
// <copyright file="SpecHierarchyTestFixture.cs" company="Starion Group S.A.">
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

namespace ReqIFSharp.Tests.SpecElementWithAttributesTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// Suite of tests for the <see cref="SpecHierarchy"/> class
    /// </summary>
    [TestFixture]
    public class SpecHierarchyTestFixture
    {
        private XmlWriterSettings settings;

        private ILoggerFactory loggerFactory;

        private TestLogEventSink testLogEventSink;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.testLogEventSink = new TestLogEventSink();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
                .WriteTo.Sink(this.testLogEventSink)
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
        }

        [SetUp]
        public void SetUp()
        {
            this.testLogEventSink.Events.Clear();
            this.settings = new XmlWriterSettings();
        }

        [Test]
        public void Verify_that_constructor_does_not_throw_exception()
        {
            Assert.That(() => new SpecHierarchy(null), Throws.Nothing);
            Assert.That(() => new SpecHierarchy(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void Verify_that_When_Object_is_null_WriteXml_throws_exception()
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var specHierarchy = new SpecHierarchy
            {
                Identifier = "identifier",
                LongName = "longname"
            };

            Assert.That(
                () => specHierarchy.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Object property of SpecHierarchy identifier:longname may not be null"));
        }

        [Test]
        public void Verify_that_When_Object_is_null_WriteXmlAsync_throws_exception()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            this.settings.Async = true;

            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var specHierarchy = new SpecHierarchy
            {
                Identifier = "identifier",
                LongName = "longname"
            };

            Assert.That(
                async () => await specHierarchy.WriteXmlAsync(writer, cancellationTokenSource.Token),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Object property of SpecHierarchy identifier:longname may not be null"));
        }

        [Test]
        public void SpecHierarchy_ReadXml_InitialisesContainerAndChildren()
        {
            var content = new ReqIFContent();
            var rootSpecification = new Specification(content, null) { Identifier = "root" };
            var existingObject = new SpecObject(content, null) { Identifier = "existing-object" };

            var specHierarchy = new SpecHierarchy(rootSpecification, content, null)
            {
                Identifier = "hierarchy"
            };

            var xml = """
                      <SPEC-HIERARCHY IDENTIFIER="hierarchy" IS-TABLE-INTERNAL="true">
                        <ALTERNATIVE-ID>
                          <ALTERNATIVE-ID IDENTIFIER="an-alternative-id"/>
                        </ALTERNATIVE-ID>
                        <OBJECT>
                          <SPEC-OBJECT-REF>missing-object</SPEC-OBJECT-REF>
                        </OBJECT>
                        <CHILDREN>
                          <SPEC-HIERARCHY IDENTIFIER="child">
                            <OBJECT>
                              <SPEC-OBJECT-REF>existing-object</SPEC-OBJECT-REF>
                            </OBJECT>
                          </SPEC-HIERARCHY>
                        </CHILDREN>
                      </SPEC-HIERARCHY>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            specHierarchy.ReadXml(reader);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(specHierarchy.IsTableInternal, Is.True);
                Assert.That(specHierarchy.Object, Is.Null, "Missing objects should yield null reference");
                Assert.That(specHierarchy.Children, Has.Count.EqualTo(1));
                Assert.That(specHierarchy.Children[0].Object, Is.SameAs(existingObject));
                Assert.That(specHierarchy.Children[0].Container, Is.SameAs(specHierarchy));
                Assert.That(specHierarchy.AlternativeId.Identifier, Is.EqualTo("an-alternative-id"));
            }
        }

        [Test]
        public async Task SpecHierarchy_ReadXmlAsync_InitialisesContainerAndChildren()
        {
            var content = new ReqIFContent();
            var rootSpecification = new Specification(content, null) { Identifier = "root" };
            var existingObject = new SpecObject(content, null) { Identifier = "existing-object" };

            var specHierarchy = new SpecHierarchy(rootSpecification, content, null)
            {
                Identifier = "hierarchy"
            };

            var xml = """
                      <SPEC-HIERARCHY IDENTIFIER="hierarchy" IS-TABLE-INTERNAL="true">
                        <ALTERNATIVE-ID>
                          <ALTERNATIVE-ID IDENTIFIER="an-alternative-id"/>
                        </ALTERNATIVE-ID>
                        <OBJECT>
                          <SPEC-OBJECT-REF>missing-object</SPEC-OBJECT-REF>
                        </OBJECT>
                        <CHILDREN>
                          <SPEC-HIERARCHY IDENTIFIER="child">
                            <OBJECT>
                              <SPEC-OBJECT-REF>existing-object</SPEC-OBJECT-REF>
                            </OBJECT>
                          </SPEC-HIERARCHY>
                        </CHILDREN>
                      </SPEC-HIERARCHY>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            await specHierarchy.ReadXmlAsync(reader, CancellationToken.None);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(specHierarchy.IsTableInternal, Is.True);
                Assert.That(specHierarchy.Object, Is.Null, "Missing objects should yield null reference");
                Assert.That(specHierarchy.Children, Has.Count.EqualTo(1));
                Assert.That(specHierarchy.Children[0].Object, Is.SameAs(existingObject));
                Assert.That(specHierarchy.Children[0].Container, Is.SameAs(specHierarchy));
                Assert.That(specHierarchy.AlternativeId.Identifier, Is.EqualTo("an-alternative-id"));
            }
        }

        [Test]
        public async Task Verify_that_when_ReadXmlAsync_cancel_throws_OperationCanceledException()
        {
            var content = new ReqIFContent();
            var rootSpecification = new Specification(content, null) { Identifier = "root" };
            
            var specHierarchy = new SpecHierarchy(rootSpecification, content, null)
            {
                Identifier = "hierarchy"
            };

            var xml = """
                      <SPEC-HIERARCHY IDENTIFIER="hierarchy" IS-TABLE-INTERNAL="true">
                        <OBJECT>
                          <SPEC-OBJECT-REF>missing-object</SPEC-OBJECT-REF>
                        </OBJECT>
                        <CHILDREN>
                          <SPEC-HIERARCHY IDENTIFIER="child">
                            <OBJECT>
                              <SPEC-OBJECT-REF>existing-object</SPEC-OBJECT-REF>
                            </OBJECT>
                          </SPEC-HIERARCHY>
                        </CHILDREN>
                      </SPEC-HIERARCHY>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var cts = new CancellationTokenSource();

            await cts.CancelAsync();

            await Assert.ThatAsync(() => specHierarchy.ReadXmlAsync(reader, cts.Token), Throws.InstanceOf<OperationCanceledException>());
        }

        [Test]
        public void SpecHierarchy_ReadXml_Verify_that_when_attribute_is_incorrect_type_exception_is_thrown()
        {
            var content = new ReqIFContent();
            var rootSpecification = new Specification(content, null) { Identifier = "root" };
            
            var specHierarchy = new SpecHierarchy(rootSpecification, content, null)
            {
                Identifier = "hierarchy"
            };

            var xml = """
                      <SPEC-HIERARCHY IDENTIFIER="hierarchy" IS-TABLE-INTERNAL="INCORRECT-DATA-TYPE">
                        <ALTERNATIVE-ID>
                          <ALTERNATIVE-ID IDENTIFIER="an-alternative-id"/>
                        </ALTERNATIVE-ID>
                        <OBJECT>
                          <SPEC-OBJECT-REF>missing-object</SPEC-OBJECT-REF>
                        </OBJECT>
                        <CHILDREN>
                          <SPEC-HIERARCHY IDENTIFIER="child">
                            <OBJECT>
                              <SPEC-OBJECT-REF>existing-object</SPEC-OBJECT-REF>
                            </OBJECT>
                          </SPEC-HIERARCHY>
                        </CHILDREN>
                      </SPEC-HIERARCHY>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            Assert.That(() => specHierarchy.ReadXml(reader), Throws.InstanceOf<SerializationException>());
        }

        [Test]
        public void ReadXml_UnsupportedElement_EmitsLogWarning()
        {
            var xml = """
                      <UNSUPPORTED-ELEMENT />
                      """;

            var specHierarchy = new SpecHierarchy(this.loggerFactory);

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = false, IgnoreWhitespace = false });

            reader.MoveToContent();

            specHierarchy.ReadXml(reader);

            var warningEvent = this.testLogEventSink.Events
                .Where(e => e.Level == LogEventLevel.Warning)
                .ToList().First();

            Assert.That(warningEvent.Properties["LocalName"].ToString().Trim('"'),
                Is.EqualTo("UNSUPPORTED-ELEMENT"));
        }

        [Test]
        public async Task ReadXmlAsync_UnsupportedElement_EmitsLogWarning()
        {
            var xml = """
                      <UNSUPPORTED-ELEMENT />
                      """;

            var specHierarchy = new SpecHierarchy(this.loggerFactory);

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true, IgnoreWhitespace = false });

            await specHierarchy.ReadXmlAsync(reader, CancellationToken.None);

            var warningEvent = this.testLogEventSink.Events
                .Where(e => e.Level == LogEventLevel.Warning)
                .ToList().First();

            Assert.That(warningEvent.Properties["LocalName"].ToString().Trim('"'),
                Is.EqualTo("UNSUPPORTED-ELEMENT"));
        }

        [Test]
        public void Verify_that_when_is_editable_is_not_boolean_exception_is_raised()
        {
            var content = new ReqIFContent();
            var rootSpecification = new Specification(content, null) { Identifier = "root" };

            var specHierarchy = new SpecHierarchy(rootSpecification, content, null)
            {
                Identifier = "hierarchy"
            };

            var xml = """
                      <SPEC-HIERARCHY IDENTIFIER="hierarchy" IS-EDITABLE="INCORRECT-DATA-TYPE">
                        <OBJECT />
                        <CHILDREN />
                      </SPEC-HIERARCHY>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = false });
            reader.MoveToContent();

            Assert.That(() => specHierarchy.ReadXml(reader), Throws.InstanceOf<SerializationException>());
        }
    }
}
