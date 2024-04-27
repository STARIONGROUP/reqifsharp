//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecElementWithAttributesExtensionTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2024 Starion Group S.A.
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

namespace ReqIFSharp.Extensions.Tests.ReqIFExtensions
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.ReqIFExtensions;
    using ReqIFSharp.Extensions.Services;

    /// <summary>
    /// Suite of tests for the <see cref="SpecElementWithAttributesExtensions"/>
    /// </summary>
    [TestFixture]
    public class SpecElementWithAttributesExtensionTestFixture
    {
        private ReqIF reqIf;

        private IReqIFLoaderService reqIFLoaderService;

        [SetUp]
        public async Task SetUp()
        {
            var reqIfDeserializer = new ReqIFDeserializer();

            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "requirements-and-objects.reqifz");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            await using var fileStream = new FileStream(reqifPath, FileMode.Open);
            this.reqIFLoaderService = new ReqIFLoaderService(reqIfDeserializer);
            await this.reqIFLoaderService.Load(fileStream, supportedFileExtensionKind, cts.Token);

            this.reqIf = this.reqIFLoaderService.ReqIFData.Single();
        }

        [Test]
        public void Verify_that_QueryExternalObjects_returns_expected_results()
        {
            var specObject = this.reqIf.CoreContent.SpecObjects.Single(x => x.Identifier == "_3.4.2.2.2_BrLeft_2_BrRight_._BrLeft_f_BrRight_1");

            var externalObjects = specObject.QueryExternalObjects();

            Assert.That(externalObjects.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Verify_that_QueryBase64Payloads_returns_expected_results()
        {
            var specObject = this.reqIf.CoreContent.SpecObjects.Single(x => x.Identifier == "_3.4.2.2.2_BrLeft_2_BrRight_._BrLeft_f_BrRight_1");

            var base64Payloads = await specObject.QueryBase64Payloads(this.reqIFLoaderService);

            var base64Payload = base64Payloads.Single();

            Assert.That(base64Payload.Item2, Does.StartWith("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAfgAAACfCAIAAACazFx+AAAAAXNSR0IArs4c6QAA"));
        }
    }
}
