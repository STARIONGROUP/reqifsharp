// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFHeaderTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="ReqIFHeader"/> class
    /// </summary>
    [TestFixture]
    public class ReqIFHeaderTestFixture
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
        public void Verify_that_ReadXmlAsync_throws_exception_when_cancelled()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Datatype-Demo.reqif");

            var cts = new CancellationTokenSource();
            cts.Cancel();

            using var fileStream = File.OpenRead(reqifPath);
            using var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings { Async = true });
            
            var reqIfHeader = new ReqIFHeader(this.loggerFactory);

            Assert.That( async () => await reqIfHeader.ReadXmlAsync(xmlReader, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_throws_exception_when_cancelled()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var reqIfHeader = new ReqIFHeader(this.loggerFactory);

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await reqIfHeader.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Verify_that_when_unsupported_element_ReqIFHeader_ReadXml_logs_warning()
        {
            var xml = """
                      <THE-HEADER>
                          <REQ-IF-HEADER IDENTIFIER="_jgCysQfNEeeAO8RifBaE-g">
                              <COMMENT>Created by: jastram</COMMENT>
                              <CREATION-TIME>2017-03-13T10:15:09.017+01:00</CREATION-TIME>
                              <REPOSITORY-ID>repos-id</REPOSITORY-ID>
                              <REQ-IF-TOOL-ID>fmStudio (http://formalmind.com/studio)</REQ-IF-TOOL-ID>
                              <REQ-IF-VERSION>1.0</REQ-IF-VERSION>
                              <SOURCE-TOOL-ID>ProR (http://pror.org)</SOURCE-TOOL-ID>
                              <TITLE>Specification Title</TITLE>
                              <UNSUPPORTED-ELEMENT />
                          </REQ-IF-HEADER>
                      </THE-HEADER>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            var reqIfHeader = new ReqIFHeader(this.loggerFactory);

            Assert.That(() => reqIfHeader.ReadXml(reader), Throws.Nothing);

            var warningEvent = this.testLogEventSink.Events
                .Where(e => e.Level == LogEventLevel.Warning)
                .ToList().First();

            Assert.That(warningEvent.Properties["LocalName"].ToString().Trim('"'),
                Is.EqualTo("UNSUPPORTED-ELEMENT"));
        }

        [Test]
        public async Task Verify_that_when_unsupported_element_ReqIFHeader_ReadXmlAsync_logs_warning()
        {
            var xml = """
                      <THE-HEADER>
                          <REQ-IF-HEADER IDENTIFIER="_jgCysQfNEeeAO8RifBaE-g">
                              <COMMENT>Created by: jastram</COMMENT>
                              <CREATION-TIME>2017-03-13T10:15:09.017+01:00</CREATION-TIME>
                              <REPOSITORY-ID>repos-id</REPOSITORY-ID>
                              <REQ-IF-TOOL-ID>fmStudio (http://formalmind.com/studio)</REQ-IF-TOOL-ID>
                              <REQ-IF-VERSION>1.0</REQ-IF-VERSION>
                              <SOURCE-TOOL-ID>ProR (http://pror.org)</SOURCE-TOOL-ID>
                              <TITLE>Specification Title</TITLE>
                              <UNSUPPORTED-ELEMENT />
                          </REQ-IF-HEADER>
                      </THE-HEADER>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var reqIfHeader = new ReqIFHeader(this.loggerFactory);

            await reqIfHeader.ReadXmlAsync(reader, CancellationToken.None);

            var warningEvent = this.testLogEventSink.Events
                .Where(e => e.Level == LogEventLevel.Warning)
                .ToList().First();

            Assert.That(warningEvent.Properties["LocalName"].ToString().Trim('"'),
                Is.EqualTo("UNSUPPORTED-ELEMENT"));
        }

        [Test]
        public void Verify_that_when_invalid_date_ReqIFHeader_ReadXml_throws_exception()
        {
            var xml = """
                      <THE-HEADER>
                          <REQ-IF-HEADER IDENTIFIER="_abc">
                              <CREATION-TIME>invalid-date</CREATION-TIME>
                          </REQ-IF-HEADER>
                      </THE-HEADER>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = false });
            reader.MoveToContent();

            var reqIfHeader = new ReqIFHeader(this.loggerFactory);

            Assert.That(() => reqIfHeader.ReadXml(reader), Throws.TypeOf<SerializationException>());
        }

        [Test]
        public async Task Verify_that_when_invalid_date_ReqIFHeader_ReadXmlAsync_throws_exception()
        {
            var xml = """
                      <THE-HEADER>
                          <REQ-IF-HEADER IDENTIFIER="_abc">
                              <CREATION-TIME>invalid-date</CREATION-TIME>
                          </REQ-IF-HEADER>
                      </THE-HEADER>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var reqIfHeader = new ReqIFHeader(this.loggerFactory);

            await Assert.ThatAsync(() => reqIfHeader.ReadXmlAsync(reader, CancellationToken.None), Throws.TypeOf<SerializationException>());
        }
    }
}
