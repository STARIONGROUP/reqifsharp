// -------------------------------------------------------------------------------------------------
//  <copyright file="SpecObjectTypeTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2025 Starion Group S.A.
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

namespace ReqIFSharp.Tests.SpecTypes
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;
    using Serilog.Events;

    [TestFixture]
    public class SpecObjectTypeTestFixture
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
        public void Verify_that_constructor_does_not_throw_exception()
        {
            Assert.That(() => new SpecObjectType(null), Throws.Nothing);
            Assert.That(() => new SpecObjectType(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void Verify_that_when_argument_null_exception_is_thrown()
        {
            ReqIFContent content = null;

            Assert.That(() => new SpecObjectType(content, NullLoggerFactory.Instance), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_when_constructed_no_exception_is_thrown()
        {
            var content = new ReqIFContent();

            Assert.That(() => new SpecObjectType(content, NullLoggerFactory.Instance), Throws.Nothing);

            Assert.That(() => new SpecObjectType(NullLoggerFactory.Instance), Throws.Nothing);
        }

        [Test]
        public void ReadXml_UnsupportedElement_EmitsLogWarning()
        {
            var xml = """
                      <UNSUPPORTED-ELEMENT />
                      """;

            var content = new ReqIFContent();
            var specObjectType = new SpecObjectType(content, this.loggerFactory);

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = false, IgnoreWhitespace = false });

            specObjectType.ReadXml(reader);

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
            var specObjectType = new SpecObjectType(content, this.loggerFactory);

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true, IgnoreWhitespace = false });

            await specObjectType.ReadXmlAsync(reader, CancellationToken.None);

            var warningEvent = this.testLogEventSink.Events
                .Where(e => e.Level == LogEventLevel.Warning)
                .ToList().First();

            Assert.That(warningEvent.Properties["LocalName"].ToString().Trim('"'),
                Is.EqualTo("UNSUPPORTED-ELEMENT"));
        }
    }
}
