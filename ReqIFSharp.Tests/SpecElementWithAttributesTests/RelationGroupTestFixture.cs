// -------------------------------------------------------------------------------------------------
// <copyright file="RelationGroupTestFixture.cs" company="RHEA System S.A.">
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

namespace ReqIFSharp.Tests
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;
    using NUnit.Framework;
    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="RelationGroup"/>
    /// </summary>
    [TestFixture]
    public class RelationGroupTestFixture
    {
        [Test]
        public void VerifyThatTheSpectTypeCanBeSet()
        {
            var relationGroupType = new RelationGroupType();
            var relationGroup = new RelationGroup();

            relationGroup.SpecType = relationGroupType;

            Assert.AreEqual(relationGroupType, relationGroup.SpecType);
        }

        [Test]
        public void VerifyThatSettingAnIncorrectSpecTypeThrowsException()
        {
            var specificationType = new SpecificationType();
            var relationGroup = new RelationGroup();

            Assert.That(() => relationGroup.SpecType = specificationType,
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("specType must of type RelationGroupType"));
        }
    }
}