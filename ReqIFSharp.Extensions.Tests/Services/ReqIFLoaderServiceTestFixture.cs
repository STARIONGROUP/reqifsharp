// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFLoaderServiceTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2021 RHEA System S.A.
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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using NUnit.Framework;

    using ReqIFSharp;

    using ReqIFSharp.Extensions.ReqIFExtensions;
    using ReqIFSharp.Extensions.Services;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIFLoaderService"/> class
    /// </summary>
    public class ReqIFLoaderServiceTestFixture
    {
        private ReqIFLoaderService reqIfLoaderService;

        [SetUp]
        public void Setup()
        {
            var reqIfDeserializer = new ReqIFDeserializer();
            this.reqIfLoaderService = new ReqIFLoaderService(reqIfDeserializer);
        }

        [Test]
        public async Task Verify_that_ReqIF_data_is_loaded_and_set_to_ReqIFData()
        {
            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            await using var fileStream = new FileStream(reqifPath, FileMode.Open);
            await this.reqIfLoaderService.Load(fileStream, cts.Token);

            Assert.That(this.reqIfLoaderService.ReqIFData, Is.Not.Empty);

            var reqIF = this.reqIfLoaderService.ReqIFData.First();

            Assert.That(reqIF.TheHeader.Title, Is.EqualTo("Traceability Template"));
        }

        [Test]
        public async Task Verify_that_ReqIF_data_with_objects_is_loaded_and_set_to_ReqIFData()
        {
            var sw = Stopwatch.StartNew();

            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "requirements-and-objects.reqifz");

            await using var fileStream = new FileStream(reqifPath, FileMode.Open);
            await this.reqIfLoaderService.Load(fileStream, cts.Token);

            Console.WriteLine($"requirements-and-objects.reqifz desserialized in {sw.ElapsedMilliseconds} [ms]");

            Assert.That(this.reqIfLoaderService.ReqIFData, Is.Not.Empty);

            var reqIF = this.reqIfLoaderService.ReqIFData.First();

            Assert.That(reqIF.TheHeader.Title, Is.EqualTo("Subset026"));
        }

        [Test]
        public async Task Verify_that_ExternalObject_image_can_be_Queried()
        {
            var sw = Stopwatch.StartNew();

            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "requirements-and-objects.reqifz");

            await using var fileStream = new FileStream(reqifPath, FileMode.Open);
            await this.reqIfLoaderService.Load(fileStream, cts.Token);

            Console.WriteLine($"requirements-and-objects.reqifz desserialized in {sw.ElapsedMilliseconds} [ms]");

            var reqIF = this.reqIfLoaderService.ReqIFData.First();

            var externalObjects = reqIF.CoreContent.QueryExternalObjects().ToList();

            // firs iteration to assert that is retrieved from stream
            foreach (var externalObject in externalObjects)
            {
                sw.Restart();

                var image = await this.reqIfLoaderService.QueryData(externalObject, cts.Token);

                Console.WriteLine($"image extracted in {sw.ElapsedMilliseconds} [ms]");

                Assert.That(image, Is.Not.Null.Or.Empty);
            }

            // second iteration to assert that is retrieve from cache
            foreach (var externalObject in externalObjects)
            {
                sw.Restart();

                var image = await this.reqIfLoaderService.QueryData(externalObject, cts.Token);

                Console.WriteLine($"image extracted in {sw.ElapsedMilliseconds} [ms]");

                Assert.That(image, Is.Not.Null.Or.Empty);
            }
        }
    }
}