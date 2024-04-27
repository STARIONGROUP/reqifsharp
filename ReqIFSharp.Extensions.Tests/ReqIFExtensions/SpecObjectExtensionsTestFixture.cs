//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecObjectExtensionsTestFixture.cs" company="Starion Group S.A.">
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
    using System;
    using System.Linq;

    using NUnit.Framework;

    using ReqIFSharp.Extensions.ReqIFExtensions;

    /// <summary>
    /// Suite of tests for the <see cref="SpecObjectExtensions"/> class
    /// </summary>
    [TestFixture]
    public class SpecObjectExtensionsTestFixture
    {
        private ReqIF reqIf;

        private SpecObject specObject;

        private Specification specification;

        private SpecRelation specRelation;

        [SetUp]
        public void SetUp()
        {
            this.reqIf = new ReqIF
            {
                CoreContent = new ReqIFContent()
            };

            this.specObject = new SpecObject(this.reqIf.CoreContent, null)
            {
                Identifier = "specObject"
            };
            
            this.specObject.ReqIFContent = this.reqIf.CoreContent;

            this.specification = new Specification(this.reqIf.CoreContent, null);

            var specHierarchy = new SpecHierarchy(this.specification, this.reqIf.CoreContent, null)
            {
                Object = this.specObject
            };

            this.specRelation = new SpecRelation(this.reqIf.CoreContent, null);
            this.specRelation.Source = this.specObject;
        }

        [Test]
        public void Verify_that_QueryContainerSpecifications_returns_expected_result()
        {
            var specifications = this.specObject.QueryContainerSpecifications();

            Assert.That(specifications.Single(), Is.EqualTo(this.specification));
        }

        [Test]
        public void Verify_that_when_ReqIFContent_is_null_exception_is_thrown()
        {
            this.specObject = new SpecObject();

            Assert.That(() => this.specObject.QueryContainerSpecifications(), 
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Verify_that_QuerySpecRelations_returns_expected_result()
        {
            var specRelations = this.specObject.QuerySpecRelations();

            Assert.That(specRelations.Single(), Is.EqualTo(this.specRelation));
        }
    }
}
