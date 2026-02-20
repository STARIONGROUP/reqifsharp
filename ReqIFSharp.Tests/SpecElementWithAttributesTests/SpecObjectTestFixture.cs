// -------------------------------------------------------------------------------------------------
// <copyright file="SpecObjectTestFixture.cs" company="Starion Group S.A.">
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
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// Suite of tests for the <see cref="SpecObject"/>
    /// </summary>
    [TestFixture]
    public class SpecObjectTestFixture
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
            Assert.That(() => new SpecObject(null), Throws.Nothing);
            Assert.That(() => new SpecObject(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void VerifyThatTheSpecTypeCanBeSetOrGet()
        {
            var specObjectType = new SpecObjectType();

            var spectObject = new SpecObject();
            spectObject.Type = specObjectType;

            var specElementWithAttributes = (SpecElementWithAttributes)spectObject;

            Assert.That(specElementWithAttributes.SpecType, Is.EqualTo(specObjectType));

            var otherSpecObjectType = new SpecObjectType();

            specElementWithAttributes.SpecType = otherSpecObjectType;

            Assert.That(spectObject.SpecType, Is.EqualTo(otherSpecObjectType));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenInvalidTypeIsSet()
        {
            var relationGroupType = new RelationGroupType();
            var spectObject = new SpecObject();
            var specElementWithAttributes = (SpecElementWithAttributes)spectObject;

            Assert.Throws<ArgumentException>(() => specElementWithAttributes.SpecType = relationGroupType);
        }

        [Test]
        public void Verify_that_When_Type_is_null_WriteXml_throws_exception()
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var spectObject = new SpecObject
            {
                Identifier = "SpectObjectIdentifier",
                LongName = "SpectObjectLongName"
            };
            
            Assert.That(
                () => spectObject.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Type property of SpecObject SpectObjectIdentifier:SpectObjectLongName may not be null"));
        }

        [Test]
        public void Verify_that_When_Type_is_null_WriteXmlAsync_throws_exception()
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var spectObject = new SpecObject
            {
                Identifier = "SpectObjectIdentifier",
                LongName = "SpectObjectLongName"
            };

            var cts = new CancellationTokenSource();

            Assert.That(
                () => spectObject.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Type property of SpecObject SpectObjectIdentifier:SpectObjectLongName may not be null"));
        }

        [Test]
        public void Verify_that_ReadXml_sets_references_and_properties()
        {
            var xml = """
                      <SPEC-OBJECT IDENTIFIER="_jgCyuAfNEeeAO8RifBaE-g" LAST-CHANGE="2017-03-14T15:03:00.541+01:00" LONG-NAME="specobjectname">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_jgCyuAfNEeeAO8RifBaE-g"/>
                          </ALTERNATIVE-ID>
                          <VALUES />
                          <TYPE>
                              <SPEC-OBJECT-TYPE-REF>_jgCytAfNEeeAO8RifBaE-g</SPEC-OBJECT-TYPE-REF>
                          </TYPE>
                      </SPEC-OBJECT>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            var content = new ReqIFContent();
            var specObject = new SpecObject(content, this.loggerFactory);

            specObject.ReadXml(reader);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(specObject.Identifier, Is.EqualTo("_jgCyuAfNEeeAO8RifBaE-g"));
                Assert.That(specObject.LongName, Is.EqualTo("specobjectname"));
                Assert.That(specObject.AlternativeId.Identifier, Is.EqualTo("_jgCyuAfNEeeAO8RifBaE-g"));
            }
        }

        [Test]
        public async Task Verify_that_ReadXmlAsync_sets_references_and_properties()
        {
            var xml = """
                      <SPEC-OBJECT IDENTIFIER="_jgCyuAfNEeeAO8RifBaE-g" LAST-CHANGE="2017-03-14T15:03:00.541+01:00" LONG-NAME="specobjectname">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_jgCyuAfNEeeAO8RifBaE-g"/>
                          </ALTERNATIVE-ID>
                          <VALUES />
                          <TYPE>
                              <SPEC-OBJECT-TYPE-REF>_jgCytAfNEeeAO8RifBaE-g</SPEC-OBJECT-TYPE-REF>
                          </TYPE>
                      </SPEC-OBJECT>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent();
            var specObject = new SpecObject(content, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await specObject.ReadXmlAsync(reader, cts.Token);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(specObject.Identifier, Is.EqualTo("_jgCyuAfNEeeAO8RifBaE-g"));
                Assert.That(specObject.LongName, Is.EqualTo("specobjectname"));
                Assert.That(specObject.AlternativeId.Identifier, Is.EqualTo("_jgCyuAfNEeeAO8RifBaE-g"));
            }
        }

        [Test]
        public async Task Verify_that_when_ReadXmlAsync_cancel_throws_OperationCanceledException()
        {
            var xml = """
                      <SPEC-OBJECT IDENTIFIER="_jgCyuAfNEeeAO8RifBaE-g" LAST-CHANGE="2017-03-14T15:03:00.541+01:00" LONG-NAME="specobjectname">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_jgCyuAfNEeeAO8RifBaE-g"/>
                          </ALTERNATIVE-ID>
                          <VALUES />
                          <TYPE>
                              <SPEC-OBJECT-TYPE-REF>_jgCytAfNEeeAO8RifBaE-g</SPEC-OBJECT-TYPE-REF>
                          </TYPE>
                      </SPEC-OBJECT>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent();
            var specObject = new SpecObject(content, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await cts.CancelAsync();

            await Assert.ThatAsync(() => specObject.ReadXmlAsync(reader, cts.Token), Throws.InstanceOf<OperationCanceledException>());
        }

        [Test]
        public void ReadXml_UnsupportedElement_EmitsLogWarning()
        {
            var xml = """
                      <UNSUPPORTED-ELEMENT />
                      """;

            var content = new ReqIFContent();
            var specObject = new SpecObject(content, this.loggerFactory);

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = false, IgnoreWhitespace = false });

            reader.MoveToContent();

            specObject.ReadXml(reader);

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

            var content = new ReqIFContent();
            var specObject = new SpecObject(content, this.loggerFactory);

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true, IgnoreWhitespace = false });

            await reader.MoveToContentAsync();

            await specObject.ReadXmlAsync(reader, CancellationToken.None);

            var warningEvent = this.testLogEventSink.Events
                .Where(e => e.Level == LogEventLevel.Warning)
                .ToList().First();

            Assert.That(warningEvent.Properties["LocalName"].ToString().Trim('"'),
                Is.EqualTo("UNSUPPORTED-ELEMENT"));
        }

        [Test]
        public void Verify_that_when_invalid_date_Identifiable_ReadXml_throws_exception()
        {
            var xml = """
                      <SPEC-OBJECT IDENTIFIER="_jgCyuAfNEeeAO8RifBaE-g" LAST-CHANGE="invalid-date" LONG-NAME="specobjectname">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_jgCyuAfNEeeAO8RifBaE-g"/>
                          </ALTERNATIVE-ID>
                          <VALUES />
                          <TYPE>
                              <SPEC-OBJECT-TYPE-REF>_jgCytAfNEeeAO8RifBaE-g</SPEC-OBJECT-TYPE-REF>
                          </TYPE>
                      </SPEC-OBJECT>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            var content = new ReqIFContent();
            var specObject = new SpecObject(content, this.loggerFactory);

            Assert.That(() => specObject.ReadXml(reader), Throws.TypeOf<SerializationException>() );
        }
    }
}
