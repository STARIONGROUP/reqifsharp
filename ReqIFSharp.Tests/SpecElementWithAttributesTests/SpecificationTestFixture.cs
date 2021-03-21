// -------------------------------------------------------------------------------------------------
// <copyright file="SpecificationTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
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

namespace ReqIFLib.Tests
{
    using System;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="Specification"/>
    /// </summary>
    [TestFixture]
    public class SpecificationTestFixture
    {
        [Test]
        public void VerifyThatTheSpecTypeCanBeSetOrGet()
        {
            var specificationType = new SpecificationType();

            var specification = new Specification();
            specification.Type = specificationType;

            var specElementWithAttributes = (SpecElementWithAttributes)specification;

            Assert.AreEqual(specificationType, specElementWithAttributes.SpecType);

            var otherspecificationType = new SpecificationType();

            specElementWithAttributes.SpecType = otherspecificationType;

            Assert.AreEqual(otherspecificationType, specification.SpecType);
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenInvalidTypeIsSet()
        {
            var relationGroupType = new RelationGroupType();
            var spectObject = new SpecObject();
            var specElementWithAttributes = (SpecElementWithAttributes)spectObject;

            Assert.Throws<ArgumentException>(() => specElementWithAttributes.SpecType = relationGroupType);
        }
    }
}
