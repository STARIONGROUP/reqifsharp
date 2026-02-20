// -------------------------------------------------------------------------------------------------
//  <copyright file="DatatypeDefinitionRealTestFixture.cs" company="Starion Group S.A.">
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
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    [TestFixture]
    public class DatatypeDefinitionRealTestFixture
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
            Assert.That(() => new DatatypeDefinitionReal(null), Throws.Nothing);
            Assert.That(() => new DatatypeDefinitionReal(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void Verify_that_when_MAX_LENGTH_is_too_large_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-REAL IDENTIFIER="_LjYOIAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:35:35.407+01:00" LONG-NAME="T_Real" ACCURACY="2" MAX="1E+309" MIN="-100.0">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_LjYOIAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-REAL>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionReal = new DatatypeDefinitionReal(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionReal.ReadXml(xmlReader), Throws.Nothing);

            Assert.That(datatypeDefinitionReal.Max, Is.EqualTo(double.PositiveInfinity));
        }

        [Test]
        public void Verify_that_when_MAX_LENGTH_is_invalid_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-REAL IDENTIFIER="_LjYOIAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:35:35.407+01:00" LONG-NAME="T_Real" ACCURACY="2" MAX="not-a-real" MIN="-100.0">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_LjYOIAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-REAL>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionReal = new DatatypeDefinitionReal(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionReal.ReadXml(xmlReader), Throws.InstanceOf<SerializationException>());
        }

        [Test]
        public void Verify_that_when_MIN_is_too_large_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-REAL IDENTIFIER="_LjYOIAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:35:35.407+01:00" LONG-NAME="T_Real" ACCURACY="2" MAX="100.1" MIN="-1.7976931348623157E+309">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_LjYOIAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-REAL>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionReal = new DatatypeDefinitionReal(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionReal.ReadXml(xmlReader), Throws.Nothing);

            Assert.That(datatypeDefinitionReal.Min, Is.EqualTo(double.NegativeInfinity));
        }

        [Test]
        public void Verify_that_when_MIN_is_invalid_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-REAL IDENTIFIER="_LjYOIAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:35:35.407+01:00" LONG-NAME="T_Real" ACCURACY="2" MAX="100.1" MIN="not-a-real">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_LjYOIAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-REAL>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionReal = new DatatypeDefinitionReal(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionReal.ReadXml(xmlReader), Throws.InstanceOf<SerializationException>());
        }

        [Test]
        public void Verify_that_when_ACCURACY_is_too_large_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-REAL IDENTIFIER="_LjYOIAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:35:35.407+01:00" LONG-NAME="T_Real" ACCURACY="9223372036854775809" MAX="100.1" MIN="-1.7976931348623157">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_LjYOIAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-REAL>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionReal = new DatatypeDefinitionReal(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionReal.ReadXml(xmlReader), Throws.Nothing);

            Assert.That(datatypeDefinitionReal.Accuracy, Is.EqualTo(long.MaxValue));
        }

        [Test]
        public void Verify_that_when_ACCURACY_is_invalid_exception_is_thrown()
        {
            var xml = """
                      <DATATYPE-DEFINITION-REAL IDENTIFIER="_LjYOIAfhEeelU71CdMk83g" LAST-CHANGE="2017-03-13T12:35:35.407+01:00" LONG-NAME="T_Real" ACCURACY="not-an-integer" MAX="100.1" MIN="-1.7976931348623157">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_LjYOIAfhEeelU71CdMk83g"/>
                          </ALTERNATIVE-ID>
                      </DATATYPE-DEFINITION-REAL>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionReal = new DatatypeDefinitionReal(NullLoggerFactory.Instance);

            Assert.That(() => datatypeDefinitionReal.ReadXml(xmlReader), Throws.InstanceOf<SerializationException>());
        }
    }
}
