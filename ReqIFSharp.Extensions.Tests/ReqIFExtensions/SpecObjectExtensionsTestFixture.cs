// -------------------------------------------------------------------------------------------------
// <copyright file="SpecObjectExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.Tests.ReqIFExtensions
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    using ReqIFSharp;
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
        public void Verify_that_QueryContainerSpecifications_returns_empty_when_specObject_not_in_specification()
        {
            var specObject = new SpecObject
            {
                ReqIFContent = this.reqIf.CoreContent
            };

            Assert.That(specObject.QueryContainerSpecifications(), Is.Empty);
        }

        [Test]
        public void Verify_that_QueryContainerSpecifications_throws_when_specObject_is_null()
        {
            Assert.That(() => SpecObjectExtensions.QueryContainerSpecifications(null), Throws.ArgumentNullException);
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

        [Test]
        public void Verify_that_QuerySpecRelations_returns_results_when_specObject_is_target()
        {
            var targetSpecObject = new SpecObject(this.reqIf.CoreContent, null)
            {
                Identifier = "target"
            };

            var specRelation = new SpecRelation(this.reqIf.CoreContent, null)
            {
                Target = targetSpecObject
            };

            var specRelations = targetSpecObject.QuerySpecRelations();

            Assert.That(specRelations.Single(), Is.EqualTo(specRelation));
        }

        [Test]
        public void Verify_that_QuerySpecRelations_returns_empty_when_no_relations_exist()
        {
            var specObject = new SpecObject
            {
                ReqIFContent = this.reqIf.CoreContent
            };

            Assert.That(specObject.QuerySpecRelations(), Is.Empty);
        }

        [Test]
        public void Verify_that_QuerySpecRelations_throws_when_specObject_is_null()
        {
            Assert.That(() => SpecObjectExtensions.QuerySpecRelations(null), Throws.ArgumentNullException);
        }
    }
}
