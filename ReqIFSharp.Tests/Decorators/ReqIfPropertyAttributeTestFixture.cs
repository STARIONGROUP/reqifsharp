// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIfPropertyAttributeTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests.Decorators
{
    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIfPropertyAttribute"/> class
    /// </summary>
    [TestFixture]
    public class ReqIfPropertyAttributeTestFixture
    {
        [Test]
        public void Verify_that_default_constructor_values_are_as_expected()
        {
            var propertyAttribute = new ReqIfPropertyAttribute();

            Assert.That(propertyAttribute.Aggregation, Is.EqualTo(AggregationKind.None));
            Assert.That(propertyAttribute.LowerValue, Is.EqualTo(1));
            Assert.That(propertyAttribute.UpperValue, Is.EqualTo(1));
            Assert.That(propertyAttribute.IsOrdered, Is.False);
            Assert.That(propertyAttribute.IsReadOnly, Is.False);
            Assert.That(propertyAttribute.IsDerived, Is.False);
            Assert.That(propertyAttribute.IsDerivedUnion, Is.False);
            Assert.That(propertyAttribute.IsUnique, Is.True);
            Assert.That(propertyAttribute.DefaultValue, Is.Null);
        }

        [Test]
        public void Verify_that_constructor_sets_properties_as_expected()
        {
            var propertyAttribute = new ReqIfPropertyAttribute(
                aggregation: AggregationKind.Composite,
                lowerValue: 0,
                upperValue: int.MaxValue,
                isOrdered: true,
                isReadOnly: true,
                isDerived: true,
                isDerivedUnion: true,
                isUnique: false,
                defaultValue: "default");

            Assert.That(propertyAttribute.Aggregation, Is.EqualTo(AggregationKind.Composite));
            Assert.That(propertyAttribute.LowerValue, Is.EqualTo(0));
            Assert.That(propertyAttribute.UpperValue, Is.EqualTo(int.MaxValue));
            Assert.That(propertyAttribute.IsOrdered, Is.True);
            Assert.That(propertyAttribute.IsReadOnly, Is.True);
            Assert.That(propertyAttribute.IsDerived, Is.True);
            Assert.That(propertyAttribute.IsDerivedUnion, Is.True);
            Assert.That(propertyAttribute.IsUnique, Is.False);
            Assert.That(propertyAttribute.DefaultValue, Is.EqualTo("default"));
        }
    }
}
