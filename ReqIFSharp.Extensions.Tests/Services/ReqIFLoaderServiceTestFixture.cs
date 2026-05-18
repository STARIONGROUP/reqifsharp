// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFLoaderServiceTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2017-2026 Starion Group S.A.
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
// -------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Schema;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.ReqIFExtensions;
    using ReqIFSharp.Extensions.Services;

    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIFLoaderService"/> class
    /// </summary>
    [TestFixture]
    public class ReqIFLoaderServiceTestFixture
    {
        private ReqIFLoaderService reqIfLoaderService;

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

        [SetUp]
        public void Setup()
        {
            var reqIfDeserializer = new ReqIFDeserializer(this.loggerFactory);
            this.reqIfLoaderService = new ReqIFLoaderService(reqIfDeserializer);
        }

        [Test]
        public async Task Verify_that_ReqIF_data_is_loaded_and_set_to_ReqIFData()
        {
            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            await using var fileStream = new FileStream(reqifPath, FileMode.Open);
            await this.reqIfLoaderService.LoadAsync(fileStream, supportedFileExtensionKind, cts.Token);

            Assert.That(this.reqIfLoaderService.ReqIFData, Is.Not.Empty);

            var reqIF = this.reqIfLoaderService.ReqIFData.First();

            Assert.That(reqIF.TheHeader.Title, Is.EqualTo("Traceability Template"));
        }

        [Test]
        public async Task Verify_that_ReqIF_data_is_loaded_and_can_be_disposed()
        {
            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            await using var fileStream = new FileStream(reqifPath, FileMode.Open);
            await this.reqIfLoaderService.LoadAsync(fileStream, supportedFileExtensionKind, cts.Token);

            Assert.That(() => this.reqIfLoaderService.Dispose(), Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_ReqIF_data_with_objects_is_loaded_and_set_to_ReqIFData()
        {
            var sw = Stopwatch.StartNew();

            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "requirements-and-objects.reqifz");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            await using var fileStream = new FileStream(reqifPath, FileMode.Open);
            await this.reqIfLoaderService.LoadAsync(fileStream, supportedFileExtensionKind, cts.Token);

            Console.WriteLine($"requirements-and-objects.reqifz desserialized in {sw.ElapsedMilliseconds} [ms]");

            Assert.That(this.reqIfLoaderService.ReqIFData, Is.Not.Empty);

            var reqIF = this.reqIfLoaderService.ReqIFData.First();

            Assert.That(reqIF.TheHeader.Title, Is.EqualTo("Subset026"));

            Assert.That(() => this.reqIfLoaderService.Dispose(), Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_ExternalObject_image_can_be_Queried()
        {
            var sw = Stopwatch.StartNew();

            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "requirements-and-objects.reqifz");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            await using var fileStream = new FileStream(reqifPath, FileMode.Open);
            await this.reqIfLoaderService.LoadAsync(fileStream, supportedFileExtensionKind, cts.Token);

            Console.WriteLine($"requirements-and-objects.reqifz desserialized in {sw.ElapsedMilliseconds} [ms]");

            var reqIF = this.reqIfLoaderService.ReqIFData.First();

            var externalObjects = reqIF.CoreContent.QueryExternalObjects().ToList();

            // firs iteration to assert that is retrieved from stream
            foreach (var externalObject in externalObjects)
            {
                sw.Restart();

                var image = await this.reqIfLoaderService.QueryDataAsync(externalObject, cts.Token);

                Console.WriteLine($"image extracted in {sw.ElapsedMilliseconds} [ms]");

                Assert.That(image, Is.Not.Null.Or.Empty);
            }

            // second iteration to assert that is retrieve from cache
            foreach (var externalObject in externalObjects)
            {
                sw.Restart();

                var image = await this.reqIfLoaderService.QueryDataAsync(externalObject, cts.Token);

                Console.WriteLine($"image extracted in {sw.ElapsedMilliseconds} [ms]");

                Assert.That(image, Is.Not.Null.Or.Empty);
            }
        }

        [Test]
        public async Task Verify_that_reloading_disposes_the_previous_source_stream()
        {
            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            await using (var firstStream = new FileStream(reqifPath, FileMode.Open))
            {
                await this.reqIfLoaderService.LoadAsync(firstStream, supportedFileExtensionKind, cts.Token);
            }

            var previousSourceStream = GetSourceStream(this.reqIfLoaderService);
            Assert.That(previousSourceStream, Is.Not.Null);

            await using (var secondStream = new FileStream(reqifPath, FileMode.Open))
            {
                await this.reqIfLoaderService.LoadAsync(secondStream, supportedFileExtensionKind, cts.Token);
            }

            Assert.That(() => _ = previousSourceStream.Position, Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(this.reqIfLoaderService.ReqIFData, Is.Not.Empty);
        }

        [Test]
        public async Task Verify_that_Reset_disposes_the_source_stream()
        {
            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            await using (var fileStream = new FileStream(reqifPath, FileMode.Open))
            {
                await this.reqIfLoaderService.LoadAsync(fileStream, supportedFileExtensionKind, cts.Token);
            }

            var sourceStream = GetSourceStream(this.reqIfLoaderService);
            Assert.That(sourceStream, Is.Not.Null);

            this.reqIfLoaderService.Reset();

            Assert.That(() => _ = sourceStream.Position, Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(this.reqIfLoaderService.ReqIFData, Is.Null);
        }

        [Test]
        public async Task Verify_that_a_failing_deserializer_propagates_and_leaves_service_usable()
        {
            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            var failingLoaderService = new ReqIFLoaderService(new ThrowingDeSerializer());

            await using (var fileStream = new FileStream(reqifPath, FileMode.Open))
            {
                Assert.That(
                    async () => await failingLoaderService.LoadAsync(fileStream, supportedFileExtensionKind, cts.Token),
                    Throws.InstanceOf<InvalidOperationException>());
            }

            Assert.That(failingLoaderService.ReqIFData, Is.Null);

            await using (var fileStream = new FileStream(reqifPath, FileMode.Open))
            {
                await this.reqIfLoaderService.LoadAsync(fileStream, supportedFileExtensionKind, cts.Token);
            }

            Assert.That(this.reqIfLoaderService.ReqIFData, Is.Not.Empty);
        }

        /// <summary>
        /// Reads the private <c>sourceStream</c> field of the <see cref="ReqIFLoaderService"/> for white-box assertions.
        /// </summary>
        private static Stream GetSourceStream(ReqIFLoaderService service)
        {
            var field = typeof(ReqIFLoaderService).GetField("sourceStream", BindingFlags.NonPublic | BindingFlags.Instance);
            return (Stream)field?.GetValue(service);
        }

        /// <summary>
        /// An <see cref="IReqIFDeSerializer"/> that always throws, used to exercise the error path of
        /// <see cref="ReqIFLoaderService"/>.
        /// </summary>
        private sealed class ThrowingDeSerializer : IReqIFDeSerializer
        {
            public IEnumerable<ReqIF> Deserialize(string fileUri, bool validate = false, ValidationEventHandler validationEventHandler = null)
                => throw new InvalidOperationException("deserialization failed");

            public IEnumerable<ReqIF> Deserialize(Stream stream, SupportedFileExtensionKind fileExtensionKind, bool validate = false, ValidationEventHandler validationEventHandler = null)
                => throw new InvalidOperationException("deserialization failed");

            public Task<IEnumerable<ReqIF>> DeserializeAsync(string fileUri, CancellationToken token, bool validate = false, ValidationEventHandler validationEventHandler = null)
                => throw new InvalidOperationException("deserialization failed");

            public Task<IEnumerable<ReqIF>> DeserializeAsync(Stream stream, SupportedFileExtensionKind fileExtensionKind, CancellationToken token, bool validate = false, ValidationEventHandler validationEventHandler = null)
                => throw new InvalidOperationException("deserialization failed");
        }
    }
}