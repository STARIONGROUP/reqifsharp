// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueXHTMLTestFixture.cs" company="Starion Group S.A.">
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
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml;

    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="EmbeddedValue"/>
    /// </summary>
    [TestFixture]
    public class EmbeddedValueTestFixture
    {
        [Test]
        public void Verify_That_WriteXmlAsync_throws_exception_when_cancelled()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var embeddedValue = new EmbeddedValue();

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await embeddedValue.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Verify_that_when_KEY_is_too_large_exception_is_thrown()
        {
            var xml = """
                      <EMBEDDED-VALUE KEY="9223372036854775809" OTHER-CONTENT="1"/>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var embeddedValue = new EmbeddedValue();

            Assert.That(() => embeddedValue.ReadXml(xmlReader), Throws.Nothing);

            Assert.That(embeddedValue.Key, Is.EqualTo(0));
        }

        [Test]
        public void Verify_that_when_KEY_is_invalid_exception_is_thrown()
        {
            var xml = """
                      <EMBEDDED-VALUE KEY="not-an-integer" OTHER-CONTENT="1"/>
                      """;

            var xmlReader = XmlReader.Create(new StringReader(xml));
            xmlReader.MoveToContent();

            var datatypeDefinitionString = new EmbeddedValue();

            Assert.That(() => datatypeDefinitionString.ReadXml(xmlReader), Throws.InstanceOf<SerializationException>());
        }
    }
}
