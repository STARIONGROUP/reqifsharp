// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIfClassAttributeTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
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
