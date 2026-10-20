// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFContentTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;
    using Serilog.Events;

    [TestFixture]
    public class ReqIFContentTestFixture
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
        public void ReadXml_UnsupportedElement_EmitsLogWarning()
        {
            var xml = """
                      <UNSUPPORTED-ELEMENT />
                      """;

            var reqIfContent = new ReqIFContent(this.loggerFactory);

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = false, IgnoreWhitespace = false });

            reqIfContent.ReadXml(reader);

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

            var reqIfContent = new ReqIFContent(this.loggerFactory);
            
            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true, IgnoreWhitespace = false });

            await reqIfContent.ReadXmlAsync(reader, CancellationToken.None);

            var warningEvent = this.testLogEventSink.Events
                .Where(e => e.Level == LogEventLevel.Warning)
                .ToList().First();

            Assert.That(warningEvent.Properties["LocalName"].ToString().Trim('"'),
                Is.EqualTo("UNSUPPORTED-ELEMENT"));
        }

        [Test]
        public async Task Verify_that_when_cancelled_ReadXmlAsync_throws_exception()
        {
            var xml = """
                      <REQ-IF-CONTENT />
                      """;

            var reqIfContent = new ReqIFContent(this.loggerFactory);

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true, IgnoreWhitespace = false });

            var cts = new CancellationTokenSource();

            await cts.CancelAsync();

            await Assert.ThatAsync(() => reqIfContent.ReadXmlAsync(reader, cts.Token),
                Throws.TypeOf<OperationCanceledException>());
        }
    }
}
