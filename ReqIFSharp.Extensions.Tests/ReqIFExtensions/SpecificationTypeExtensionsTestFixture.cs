﻿//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecificationTypeExtensionsTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2025 Starion Group S.A.
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
    /// Suite of tests for the <see cref="SpecObjectTypeExtensions"/> class.
    /// </summary>
    [TestFixture]
    public class SpecificationTypeExtensionsTestFixture
    {
        private ReqIF reqIf;

        [SetUp]
        public void SetUp()
        {
            var reqIfTestDataCreator = new ReqIFTestDataCreator();

            this.reqIf = reqIfTestDataCreator.Create();
        }

        [Test]
        public void Verify_that_QueryReferencingSpecifications_returns_expected_results()
        {
            var specificationType = (SpecificationType)this.reqIf.CoreContent.SpecTypes.Single(x => x.Identifier == "specificationtype");

            var specObjects = specificationType.QueryReferencingSpecifications();

            Assert.That(specObjects.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Verify_that_on_QueryReferencingSpecifications_NullReferenceException_is_thrown_when_owning_ReqIFContent_is_not_set()
        {
            var specificationType = new SpecificationType();

            Assert.That(() => specificationType.QueryReferencingSpecifications(),
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("The owning ReqIFContent of the SpecificationType is not set."));
        }
    }
}
