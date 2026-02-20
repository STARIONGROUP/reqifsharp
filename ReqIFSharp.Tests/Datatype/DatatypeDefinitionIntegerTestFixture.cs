// -------------------------------------------------------------------------------------------------
//  <copyright file="DatatypeDefinitionIntegerTestFixture.cs" company="Starion Group S.A.">
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
    using System.Runtime.Serialization;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    [TestFixture]
    public class DatatypeDefinitionIntegerTestFixture
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
            Assert.That(() => new DatatypeDefinitionInteger(null), Throws.Nothing);
            Assert.That(() => new DatatypeDefinitionInteger(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void Verify_that_when_MAX_is_too_large_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-INTEGER IDENTIFIER="_IrT00AfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:35:17.083+01:00" LONG-NAME="T_Int" MAX="9223372036854775808" MIN="-100">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_IrT00AfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-INTEGER>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionInteger = new DatatypeDefinitionInteger(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionInteger.ReadXml(xmlReader), Throws.Nothing);

            Assert.That(datatypeDefinitionInteger.Max, Is.EqualTo(Int64.MaxValue));
        }

        [Test]
        public void Verify_that_when_MAX_is_invalid_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-INTEGER IDENTIFIER="_IrT00AfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:35:17.083+01:00" LONG-NAME="T_Int" MAX="not-an-integer" MIN="-100">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_IrT00AfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-INTEGER>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionInteger = new DatatypeDefinitionInteger(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionInteger.ReadXml(xmlReader), Throws.InstanceOf<SerializationException>());
        }

        [Test]
        public void Verify_that_when_MIN_is_too_large_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-INTEGER IDENTIFIER="_IrT00AfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:35:17.083+01:00" LONG-NAME="T_Int" MAX="100" MIN="-9223372036854775809">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_IrT00AfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-INTEGER>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionInteger = new DatatypeDefinitionInteger(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionInteger.ReadXml(xmlReader), Throws.Nothing);

            Assert.That(datatypeDefinitionInteger.Min, Is.EqualTo(Int64.MinValue));
        }

        [Test]
        public void Verify_that_when_MIN_is_invalid_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-INTEGER IDENTIFIER="_IrT00AfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:35:17.083+01:00" LONG-NAME="T_Int" MAX="100" MIN="not-an-integer">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_IrT00AfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-INTEGER>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionInteger = new DatatypeDefinitionInteger(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionInteger.ReadXml(xmlReader), Throws.InstanceOf<SerializationException>());
        }
    }
}
