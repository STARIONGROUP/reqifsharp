//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecElementWithAttributesExtensionTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="SpecElementWithAttributesExtensions"/>
    /// </summary>
    [TestFixture]
    public class SpecElementWithAttributesExtensionTestFixture
    {
        private ReqIF reqIf;

        [SetUp]
        public async Task SetUp()
        {
            var reqIfDeserializer = new ReqIFDeserializer();

            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            await using var fileStream = new FileStream(reqifPath, FileMode.Open);
            var reqIfLoaderService = new ReqIFLoaderService(reqIfDeserializer);
            await reqIfLoaderService.Load(fileStream, cts.Token);

            this.reqIf = reqIfLoaderService.ReqIFData.Single();
        }
    }
}
