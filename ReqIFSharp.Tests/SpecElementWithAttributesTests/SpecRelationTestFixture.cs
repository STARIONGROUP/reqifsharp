// -------------------------------------------------------------------------------------------------
// <copyright file="SpecRelationTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2024 RHEA System S.A.
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
    /// Suite of tests for the <see cref="SpecObject"/>
    /// </summary>
    [TestFixture]
    public class SpecRelationTestFixture
    {
        private XmlWriterSettings settings;

        [SetUp]
        public void SetUp()
        {
            this.settings = new XmlWriterSettings();
        }

        [Test]
        public void VerifyThatTheSpecTypeCanBeSetOrGet()
        {
            var specRelationType = new SpecRelationType();

            var specRelation = new SpecRelation();
            specRelation.Type = specRelationType;

            var specElementWithAttributes = (SpecElementWithAttributes)specRelation;

            Assert.That(specElementWithAttributes.SpecType, Is.EqualTo(specRelationType));

            var relationType = new SpecRelationType();

            specElementWithAttributes.SpecType = relationType;

            Assert.That(specRelation.SpecType, Is.EqualTo(relationType));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenInvalidTypeIsSet()
        {
            var relationGroupType = new RelationGroupType();
            var specRelation = new SpecRelation();
            var specElementWithAttributes = (SpecElementWithAttributes)specRelation;

            Assert.Throws<ArgumentException>(() => specElementWithAttributes.SpecType = relationGroupType);
        }

        [Test]
        public void Verify_that_When_Type_is_null_WriteXml_throws_exception()
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var specRelation = new SpecRelation
            {
                Identifier = "SpecRelationIdentifier",
                LongName = "SpecRelationLongName"
            };

            Assert.That(
                () => specRelation.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Type of SpecRelation SpecRelationIdentifier:SpecRelationLongName may not be null"));
        }

        [Test]
        public void Verify_that_When_Source_is_null_WriteXml_throws_exception()
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var specRelationType = new SpecRelationType();

            var specRelation = new SpecRelation
            {
                Identifier = "SpecRelationIdentifier",
                LongName = "SpecRelationLongName",
                Type = specRelationType
            };

            Assert.That(
                () => specRelation.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Source of SpecRelation SpecRelationIdentifier:SpecRelationLongName may not be null"));
        }

        [Test]
        public void Verify_that_When_Target_is_null_WriteXml_throws_exception()
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var specRelationType = new SpecRelationType();
            var spectObject = new SpecObject();

            var specRelation = new SpecRelation
            {
                Identifier = "SpecRelationIdentifier",
                LongName = "SpecRelationLongName",
                Type = specRelationType,
                Source = spectObject
            };

            Assert.That(
                () => specRelation.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Target of SpecRelation SpecRelationIdentifier:SpecRelationLongName may not be null"));
        }
    }
}
