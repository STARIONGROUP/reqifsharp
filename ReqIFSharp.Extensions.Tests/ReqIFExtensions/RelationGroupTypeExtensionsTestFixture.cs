// -------------------------------------------------------------------------------------------------
// <copyright file="RelationGroupTypeExtensionsTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="RelationGroupTypeExtensions"/> class.
    /// </summary>
    [TestFixture]
    public class RelationGroupTypeExtensionsTestFixture
    {
        private ReqIF reqIf;

        [SetUp]
        public void SetUp()
        {
            var reqIfTestDataCreator = new ReqIFTestDataCreator();

            this.reqIf = reqIfTestDataCreator.Create();
        }

        [Test]
        public void Verify_that_QueryReferencingRelationGroups_returns_expected_results()
        {
            var relationGroupType = (RelationGroupType)this.reqIf.CoreContent.SpecTypes.Single(x => x.Identifier == "relationgrouptype");

            var specObjects = relationGroupType.QueryReferencingRelationGroups();

            Assert.That(specObjects.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Verify_that_QueryReferencingRelationGroups_returns_empty_when_none_reference_type()
        {
            var reqIf = new ReqIF
            {
                CoreContent = new ReqIFContent()
            };

            var relationGroupType = new RelationGroupType { ReqIFContent = reqIf.CoreContent };
            var otherRelationGroupType = new RelationGroupType { ReqIFContent = reqIf.CoreContent };

            var relationGroup = new RelationGroup(reqIf.CoreContent, null)
            {
                Type = otherRelationGroupType
            };

            reqIf.CoreContent.SpecRelationGroups.Add(relationGroup);

            var result = relationGroupType.QueryReferencingRelationGroups();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Verify_that_QueryReferencingRelationGroups_throws_when_relationGroupType_is_null()
        {
            Assert.That(() => RelationGroupTypeExtensions.QueryReferencingRelationGroups(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_on_QueryReferencingRelationGroups_NullReferenceException_is_thrown_when_owning_ReqIFContent_is_not_set()
        {
            var relationGroupType = new RelationGroupType();

            Assert.That(() => relationGroupType.QueryReferencingRelationGroups(),
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("The owning ReqIFContent of the RelationGroupType is not set."));
        }
    }
}
