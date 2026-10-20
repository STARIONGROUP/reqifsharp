// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueEnumerationTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System;
    using System.Collections.Generic;
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
    /// Suite of tests for the <see cref="AttributeValueEnumeration"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueEnumerationTestFixture
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
            Assert.That(() => new AttributeValueEnumeration(null), Throws.Nothing);
            Assert.That(() => new AttributeValueEnumeration(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();

            var attributeValueEnumeration = new AttributeValueEnumeration();
            attributeValueEnumeration.Definition = attributeDefinitionEnumeration;

            var attributeValue = (AttributeValue)attributeValueEnumeration;

            Assert.That(attributeValue.AttributeDefinition, Is.EqualTo(attributeDefinitionEnumeration));
            
            attributeValue.AttributeDefinition = attributeDefinitionEnumeration;

            Assert.That(attributeValue.AttributeDefinition, Is.EqualTo(attributeDefinitionEnumeration));
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var attributeDefinitionString = new AttributeDefinitionString();
            var attributeValueEnumeration = new AttributeValueEnumeration();
            var attributeValue = (AttributeValue)attributeValueEnumeration;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_AttributeDefinition_Is_Null()
        {
            var attributeValueEnumeration = new AttributeValueEnumeration();
            var attributeValue = (AttributeValue)attributeValueEnumeration;

            Assert.That(
                () => attributeValue.AttributeDefinition = null,
                Throws.Exception.TypeOf<ArgumentNullException>()
                    .With.Property("ParamName").EqualTo("attributeDefinition"));
        }

        [Test]
        public void Verify_That_WriteXml_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeValueEnumeration = new AttributeValueEnumeration();

            Assert.That(() => attributeValueEnumeration.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Without_Definition_Set_Throws_SerializationException()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeValueEnumeration = new AttributeValueEnumeration();

            var cts = new CancellationTokenSource();

            Assert.That(async () => await attributeValueEnumeration.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_when_cancelled()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var attributeValueEnumeration = new AttributeValueEnumeration
            {
                Definition = new AttributeDefinitionEnumeration()
            };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await attributeValueEnumeration.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Verify_Convenience_Value_Property()
        {
            var attributeValue = new AttributeValueEnumeration();

            var val = new List<EnumValue> { new EnumValue() };
            attributeValue.ObjectValue = val;

            using (Assert.EnterMultipleScope())
            {
                Assert.That(attributeValue.Values.Count, Is.EqualTo(1));
                Assert.That(attributeValue.ObjectValue, Is.EqualTo(val));
            }
        }

        [Test]
        public void Verify_that_when_ObjectValue_is_not_real_an_exception_is_thrown()
        {
            var attributeValue = new AttributeValueEnumeration();

            Assert.That(
                () => attributeValue.ObjectValue = "true",
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("The value to set is not an IEnumerable<EnumValue>."));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_an_unknown_ReqIF_element_in_an_enumeration_value_is_reported_as_not_supported(bool async)
        {
            const string xml = "<ATTRIBUTE-VALUE-ENUMERATION xmlns=\"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd\"><DEFINITION/><VALUES><UNKNOWN/></VALUES></ATTRIBUTE-VALUE-ENUMERATION>";

            var sink = new TestLogEventSink();
            var serilogLogger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(sink).CreateLogger();

            using var testLoggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(serilogLogger, dispose: true));
            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = async });
            reader.MoveToContent();

            var attributeValueEnumeration = new AttributeValueEnumeration(testLoggerFactory);

            if (async)
            {
                await attributeValueEnumeration.ReadXmlAsync(reader, CancellationToken.None);
            }
            else
            {
                attributeValueEnumeration.ReadXml(reader);
            }

            var warnings = sink.Events.Where(e => e.Level == LogEventLevel.Warning).Select(e => e.RenderMessage()).ToList();

            // also the positive control of Verify_that_reading_enumeration_values_logs_no_warnings, which filters on the same SourceContext
            var fromEnumeration = sink.Events.Where(e => e.Level == LogEventLevel.Warning && e.Properties.TryGetValue("SourceContext", out var source) && source.ToString().Contains(nameof(AttributeValueEnumeration))).ToList();

            Assert.That(warnings, Has.Count.EqualTo(1));
            Assert.That(warnings[0], Does.Contain("UNKNOWN").And.Contain("is not supported"));
            Assert.That(fromEnumeration, Has.Count.EqualTo(1));
        }

        [TestCase("Datatype-Demo.reqif", false)]
        [TestCase("Datatype-Demo.reqif", true)]
        [TestCase("reqifsharpgenerated.reqif", false)]
        [TestCase("reqifsharpgenerated.reqif", true)]
        public async Task Verify_that_reading_enumeration_values_logs_no_warnings(string fileName, bool async)
        {
            var sink = new TestLogEventSink();
            var serilogLogger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(sink).CreateLogger();

            using var testLoggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(serilogLogger, dispose: true));
            using var stream = File.OpenRead(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", fileName));

            var deserializer = new ReqIFDeserializer(testLoggerFactory);

            var reqifs = async
                ? (await deserializer.DeserializeAsync(stream, SupportedFileExtensionKind.Reqif, CancellationToken.None)).ToList()
                : deserializer.Deserialize(stream, SupportedFileExtensionKind.Reqif).ToList();

            var enumerationValues = reqifs.SelectMany(x => x.CoreContent.SpecObjects).SelectMany(x => x.Values).OfType<AttributeValueEnumeration>().ToList();

            var warnings = sink.Events
                .Where(e => e.Level == LogEventLevel.Warning && e.Properties.TryGetValue("SourceContext", out var source) && source.ToString().Contains(nameof(AttributeValueEnumeration)))
                .Select(e => e.RenderMessage())
                .ToList();

            Assert.That(enumerationValues, Is.Not.Empty, "the document must contain enumeration values");
            Assert.That(warnings, Is.Empty);
        }
    }
}
