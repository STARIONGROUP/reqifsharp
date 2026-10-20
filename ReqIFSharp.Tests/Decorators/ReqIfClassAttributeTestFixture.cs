// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIfClassAttributeTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="ReqIfClassAttribute"/> class
    /// </summary>
    [TestFixture]
    public class ReqIfClassAttributeTestFixture
    {
        [Test]
        public void Verify_that_default_constructor_values_are_as_expected()
        {
            var classAttribute = new ReqIfClassAttribute();

            Assert.That(classAttribute.Name, Is.EqualTo(string.Empty));
            Assert.That(classAttribute.IsAbstract, Is.False);
        }

        [Test]
        public void Verify_that_constructor_sets_properties_as_expected()
        {
            var classAttribute = new ReqIfClassAttribute("SPEC-OBJECT", true);

            Assert.That(classAttribute.Name, Is.EqualTo("SPEC-OBJECT"));
            Assert.That(classAttribute.IsAbstract, Is.True);
        }

        [Test]
        public void Verify_that_properties_can_be_set()
        {
            var classAttribute = new ReqIfClassAttribute
            {
                Name = "SPEC-RELATION",
                IsAbstract = true
            };

            Assert.That(classAttribute.Name, Is.EqualTo("SPEC-RELATION"));
            Assert.That(classAttribute.IsAbstract, Is.True);
        }
    }
}
