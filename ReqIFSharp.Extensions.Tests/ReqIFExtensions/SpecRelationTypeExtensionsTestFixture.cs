//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecRelationTypeExtensionsTestFixture.cs" company="RHEA System S.A.">
// 
//    Copyright 2017-2024 RHEA System S.A.
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
    using System;
    using System.Linq;

    using NUnit.Framework;

    using ReqIFSharp.Extensions.ReqIFExtensions;
    using ReqIFSharp.Extensions.Tests.TestData;

    /// <summary>
    /// Suite of tests for the <see cref="SpecRelationTypeExtensions"/> class.
    /// </summary>
    [TestFixture]
    public class SpecRelationTypeExtensionsTestFixture
    {
        private ReqIF reqIf;

        [SetUp]
        public void SetUp()
        {
            var reqIfTestDataCreator = new ReqIFTestDataCreator();

            this.reqIf = reqIfTestDataCreator.Create();
        }

        [Test]
        public void Verify_that_QueryReferencingSpecRelations_returns_expected_results()
        {
            var specRelationType = (SpecRelationType)this.reqIf.CoreContent.SpecTypes.Single(x => x.Identifier == "specificationrelation");

            var specObjects = specRelationType.QueryReferencingSpecRelations();

            Assert.That(specObjects.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Verify_that_on_QueryReferencingSpecRelations_NullReferenceException_is_thrown_when_owning_ReqIFContent_is_not_set()
        {
            var specObjectType = new SpecRelationType();

            Assert.That(() => specObjectType.QueryReferencingSpecRelations(),
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("The owning ReqIFContent of the SpecRelationType is not set."));
        }
    }
}
