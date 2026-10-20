// -------------------------------------------------------------------------------------------------
// <copyright file="SpecificationTypeExtensionsTestFixture.cs" company="Starion Group S.A.">
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
        public void Verify_that_QueryReferencingSpecifications_returns_empty_when_none_reference_type()
        {
            var reqIf = new ReqIF
            {
                CoreContent = new ReqIFContent()
            };

            var specificationType = new SpecificationType { ReqIFContent = reqIf.CoreContent };
            var otherSpecificationType = new SpecificationType { ReqIFContent = reqIf.CoreContent };

            var specification = new Specification(reqIf.CoreContent, null)
            {
                Type = otherSpecificationType
            };

            reqIf.CoreContent.Specifications.Add(specification);

            var result = specificationType.QueryReferencingSpecifications();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Verify_that_QueryReferencingSpecifications_throws_when_specificationType_is_null()
        {
            Assert.That(() => SpecificationTypeExtensions.QueryReferencingSpecifications(null), Throws.ArgumentNullException);
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
