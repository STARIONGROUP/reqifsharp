// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFHeaderTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
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
    using System.Threading;
    using System.Xml;

    using NUnit.Framework;
    using System.Runtime.Serialization;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIFHeader"/> class
    /// </summary>
    [TestFixture]
    public class ReqIFHeaderTestFixture
    {
        [Test]
        public void Verify_that_GetSchema_returns_null()
        {
            var reqIfHeader = new ReqIFHeader();

            Assert.That(reqIfHeader.GetSchema(), Is.Null);
        }

        [Test]
        public void Verify_that_ReadXmlAsync_throws_exception_when_cancelled()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Datatype-Demo.reqif");

            var cts = new CancellationTokenSource();
            cts.Cancel();

            using var fileStream = File.OpenRead(reqifPath);
            using var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings { Async = true });
            
            var reqIfHeader = new ReqIFHeader();

            Assert.That( async () => await reqIfHeader.ReadXmlAsync(xmlReader, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_throws_exception_when_cancelled()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var reqIfHeader = new ReqIFHeader();

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await reqIfHeader.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }
    }
}
