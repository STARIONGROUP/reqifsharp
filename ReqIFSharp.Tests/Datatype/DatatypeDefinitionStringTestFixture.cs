// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionStringTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    [TestFixture]
    public class DatatypeDefinitionStringTestFixture
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
            Assert.That(() => new DatatypeDefinitionString(null), Throws.Nothing);
            Assert.That(() => new DatatypeDefinitionString(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void Verify_that_when_MAX_LENGTH_is_too_large_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-STRING IDENTIFIER="_jgCyswfNEeeAO8RifBaE-g" LAST-CHANGE="2017-03-13T10:15:09.017+01:00" LONG-NAME="T_String32k" MAX-LENGTH="9223372036854775809">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_jgCyswfNEeeAO8RifBaE-g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-STRING>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionString = new DatatypeDefinitionString(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionString.ReadXml(xmlReader), Throws.Nothing);

            Assert.That(datatypeDefinitionString.MaxLength, Is.EqualTo(long.MaxValue));
        }

        [Test]
        public void Verify_that_when_MAX_LENGTH_is_invalid_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-STRING IDENTIFIER="_jgCyswfNEeeAO8RifBaE-g" LAST-CHANGE="2017-03-13T10:15:09.017+01:00" LONG-NAME="T_String32k" MAX-LENGTH="not-an-integer">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_jgCyswfNEeeAO8RifBaE-g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-STRING>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionString = new DatatypeDefinitionString(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionString.ReadXml(xmlReader), Throws.InstanceOf<SerializationException>());
        }
    }
}
