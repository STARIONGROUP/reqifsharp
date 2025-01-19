// -------------------------------------------------------------------------------------------------
// <copyright file="SpecificationTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
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
    using System.Threading;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="Specification"/>
    /// </summary>
    [TestFixture]
    public class SpecificationTestFixture
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
            var specificationType = new SpecificationType();

            var specification = new Specification();
            specification.Type = specificationType;

            var specElementWithAttributes = (SpecElementWithAttributes)specification;

            Assert.That(specElementWithAttributes.SpecType, Is.EqualTo(specificationType));

            var otherspecificationType = new SpecificationType();

            specElementWithAttributes.SpecType = otherspecificationType;

            Assert.That(specification.SpecType, Is.EqualTo(otherspecificationType));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenInvalidTypeIsSet()
        {
            var relationGroupType = new RelationGroupType();
            var spectObject = new SpecObject();
            var specElementWithAttributes = (SpecElementWithAttributes)spectObject;

            Assert.Throws<ArgumentException>(() => specElementWithAttributes.SpecType = relationGroupType);
        }

        [Test]
        public void Verify_that_When_Type_is_null_WriteXml_throws_exception()
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var specification = new Specification();

            Assert.That( () => specification.WriteXml(writer),
                Throws.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_that_When_Type_is_null_WriteXmlAsync_throws_exception()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var specification = new Specification();

            Assert.That(async () => await specification.WriteXmlAsync(writer, cancellationTokenSource.Token),
                Throws.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_that_when_specType_is_not_SpecificationType_an_exception_is_thrown()
        {
            var specObjectType = new SpecObjectType();
            
            var specification = new Specification();
            Assert.Throws<ArgumentException>(() => specification.SpecType = specObjectType);
        }

        [Test]
        public void Verify_that_When_Identifier_is_null_WriteXml_throws_exception()
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var specificationType = new SpecificationType();
            var specification = new Specification();
            specification.Type = specificationType;

            Assert.That(() => specification.WriteXml(writer),
                Throws.TypeOf<SerializationException>()
                    .With.Message.Contains("The Identifier property of an Identifiable may not be null"));
        }
    }
}
