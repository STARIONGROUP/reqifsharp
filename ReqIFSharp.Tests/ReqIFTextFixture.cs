// -------------------------------------------------------------------------------------------------
//  <copyright file="ReqIFTextFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2026 Starion Group S.A.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  -------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;
    using Serilog.Events;

    [TestFixture]
    public class ReqIFTextFixture
    {
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
        }

        [Test]
        public async Task ReqIF_ReadXmlAsync_RestoresHeaderAndToolExtensions()
        {
            var reqIf = CreateReqIfDocument();

            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                writer.WriteStartElement("REQ-IF");
                reqIf.WriteXml(writer);
                writer.WriteEndElement();
            }

            using var reader = XmlReader.Create(new StringReader(builder.ToString()), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var roundTrip = new ReqIF();
            await roundTrip.ReadXmlAsync(reader, CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(roundTrip.Lang, Is.EqualTo(reqIf.Lang));
                Assert.That(roundTrip.TheHeader.Title, Is.EqualTo(reqIf.TheHeader.Title));
                Assert.That(roundTrip.ToolExtension, Has.Count.EqualTo(1));
                Assert.That(roundTrip.CoreContent.DataTypes.Count, Is.EqualTo(2));
            });
        }

        private static ReqIF CreateReqIfDocument()
        {
            var reqIf = new ReqIF
            {
                Lang = "en",
                TheHeader = new ReqIFHeader
                {
                    Identifier = "header",
                    Comment = "comment",
                    CreationTime = new DateTime(2024, 01, 01, 12, 00, 00, DateTimeKind.Utc),
                    RepositoryId = "repo",
                    ReqIFToolId = "tool",
                    ReqIFVersion = "1.0",
                    SourceToolId = "source",
                    Title = "title"
                },
                CoreContent = new ReqIFContent()
            };

            var enumeration = new DatatypeDefinitionEnumeration(reqIf.CoreContent, null)
            {
                Identifier = "enum"
            };

            var enumValue = new EnumValue(enumeration, null)
            {
                Identifier = "enum-value"
            };

            _ = new EmbeddedValue(enumValue, null)
            {
                Key = 5,
                OtherContent = "red"
            };

            var xhtmlDatatype = new DatatypeDefinitionXHTML(reqIf.CoreContent, null)
            {
                Identifier = "xhtml"
            };

            reqIf.ToolExtension.Add(new ReqIFToolExtension { InnerXml = "<extension />" });

            return reqIf;
        }

        [Test]
        public void ReadXml_UnsupportedElement_EmitsLogWarning()
        {
            var xml = """
                      <UNSUPPORTED-ELEMENT />
                      """;

            var reqif = new ReqIF(this.loggerFactory);

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = false, IgnoreWhitespace = false });

            reqif.ReadXml(reader);

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

            var reqif = new ReqIF(this.loggerFactory);

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true, IgnoreWhitespace = false });

            await reqif.ReadXmlAsync(reader, CancellationToken.None);

            var warningEvent = this.testLogEventSink.Events
                .Where(e => e.Level == LogEventLevel.Warning)
                .ToList().First();

            Assert.That(warningEvent.Properties["LocalName"].ToString().Trim('"'),
                Is.EqualTo("UNSUPPORTED-ELEMENT"));
        }
    }
}
