// -------------------------------------------------------------------------------------------------
// <copyright file="SpecRelationTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="SpecObject"/>
    /// </summary>
    [TestFixture]
    public class SpecRelationTestFixture
    {
        [Test]
        public void VerifyThatTheSpecTypeCanBeSetOrGet()
        {
            var specRelationType = new SpecRelationType();

            var specRelation = new SpecRelation();
            specRelation.Type = specRelationType;

            var specElementWithAttributes = (SpecElementWithAttributes)specRelation;

            Assert.AreEqual(specRelationType, specElementWithAttributes.SpecType);

            var relationType = new SpecRelationType();

            specElementWithAttributes.SpecType = relationType;

            Assert.AreEqual(relationType, specRelation.SpecType);
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenInvalidTypeIsSet()
        {
            var relationGroupType = new RelationGroupType();
            var specRelation = new SpecRelation();
            var specElementWithAttributes = (SpecElementWithAttributes)specRelation;

            Assert.Throws<ArgumentException>(() => specElementWithAttributes.SpecType = relationGroupType);
        }
    }
}
