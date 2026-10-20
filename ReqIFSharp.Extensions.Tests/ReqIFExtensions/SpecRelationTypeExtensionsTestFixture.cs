// -------------------------------------------------------------------------------------------------
// <copyright file="SpecRelationTypeExtensionsTestFixture.cs" company="Starion Group S.A.">
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
