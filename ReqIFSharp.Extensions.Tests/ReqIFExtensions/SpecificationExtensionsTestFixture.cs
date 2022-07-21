//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecificationExtensionsTestFixture.cs" company="RHEA System S.A.">
// 
//    Copyright 2017-2022 RHEA System S.A.
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
    /// Suite of tests for the <see cref="SpecificationExtensions"/> class
    /// </summary>
    [TestFixture]
    public class SpecificationExtensionsTestFixture
    {
        private ReqIF reqIf;

        [SetUp]
        public async Task SetUp()
        {
            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            await using var fileStream = new FileStream(reqifPath, FileMode.Open);
            var reqIfDeserializer = new ReqIFDeserializer();
            var reqIfLoaderService = new ReqIFLoaderService(reqIfDeserializer);
            await reqIfLoaderService.Load(fileStream, supportedFileExtensionKind, cts.Token);

            this.reqIf = reqIfLoaderService.ReqIFData.Single();
        }

        [Test]
        public void Verify_that_QuerySpecHierarchies_returns_the_expected_results()
        {
            var specification = this.reqIf.CoreContent.Specifications.Single(x => x.Identifier == "_o7scS6dbEeafNduaIhMwQg");

            Assert.That(specification.QueryAllContainedSpecHierarchies().Count(), Is.EqualTo(13));
        }

        [Test]
        public void Verify_that_QueryAttributeDefinitions_returns_the_expected_results()
        {
            var specification = this.reqIf.CoreContent.Specifications.Single(x => x.Identifier == "_o7scS6dbEeafNduaIhMwQg");

            var attributeDefinitions = specification.QueryAttributeDefinitions().ToList();

            Assert.That(attributeDefinitions.Count, Is.EqualTo(3));

            Assert.That(attributeDefinitions.Single(x => x.Identifier == "_o7scPKdbEeafNduaIhMwQg"), Is.Not.Null);
            Assert.That(attributeDefinitions.Single(x => x.Identifier == "_o7scPadbEeafNduaIhMwQg"), Is.Not.Null);
            Assert.That(attributeDefinitions.Single(x => x.Identifier == "_o7scO6dbEeafNduaIhMwQg"), Is.Not.Null);
        }
    }
}