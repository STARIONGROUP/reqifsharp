// -------------------------------------------------------------------------------------------------
// <copyright file="SpecObjectTypeExtensionsTestFixture.cs" company="Starion Group S.A.">
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
    public class SpecObjectTypeExtensionsTestFixture
    {
        private ReqIF reqIf;

        [SetUp]
        public void SetUp()
        {
            var reqIfTestDataCreator = new ReqIFTestDataCreator();

            this.reqIf = reqIfTestDataCreator.Create();
        }

        [Test]
        public void Verify_that_QueryReferencingSpecObject_returns_expected_results()
        {
            var specObjectType = (SpecObjectType)this.reqIf.CoreContent.SpecTypes.Single(x => x.Identifier == "requirement");

            var specObjects = specObjectType.QueryReferencingSpecObjects();

            Assert.That(specObjects.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Verify_that_QueryReferencingSpecObject_returns_empty_when_none_reference_type()
        {
            var reqIf = new ReqIF
            {
                CoreContent = new ReqIFContent()
            };

            var specObjectType = new SpecObjectType { ReqIFContent = reqIf.CoreContent };
            var otherSpecObjectType = new SpecObjectType { ReqIFContent = reqIf.CoreContent };

            var specObject = new SpecObject(reqIf.CoreContent, null)
            {
                Type = otherSpecObjectType
            };

            reqIf.CoreContent.SpecObjects.Add(specObject);

            var result = specObjectType.QueryReferencingSpecObjects();

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Verify_that_QueryReferencingSpecObject_throws_when_specObjectType_is_null()
        {
            Assert.That(() => SpecObjectTypeExtensions.QueryReferencingSpecObjects(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_on_QueryReferencingSpecifications_NullReferenceException_is_thrown_when_owning_ReqIFContent_is_not_set()
        {
            var specObjectType = new SpecObjectType();

            Assert.That(() => specObjectType.QueryReferencingSpecObjects(),
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("The owning ReqIFContent of the SpecObjectType is not set."));
        }
    }
}
