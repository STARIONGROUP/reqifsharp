﻿// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFHeaderTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
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

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIFHeader"/> class
    /// </summary>
    [TestFixture]
    public class ReqIFHeaderTestFixture
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
    }
}
