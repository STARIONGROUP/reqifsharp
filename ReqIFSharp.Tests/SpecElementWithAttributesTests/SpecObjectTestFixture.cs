// -------------------------------------------------------------------------------------------------
// <copyright file="SpecObjectTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2022 RHEA System S.A.
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
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="SpecObject"/>
    /// </summary>
    [TestFixture]
    public class SpecObjectTestFixture
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
            var specObjectType = new SpecObjectType();

            var spectObject = new SpecObject();
            spectObject.Type = specObjectType;

            var specElementWithAttributes = (SpecElementWithAttributes)spectObject;

            Assert.AreEqual(specObjectType, specElementWithAttributes.SpecType);

            var otherSpecObjectType = new SpecObjectType();

            specElementWithAttributes.SpecType = otherSpecObjectType;

            Assert.AreEqual(otherSpecObjectType, spectObject.SpecType);
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

            var spectObject = new SpecObject
            {
                Identifier = "SpectObjectIdentifier",
                LongName = "SpectObjectLongName"
            };
            
            Assert.That(
                () => spectObject.WriteXml(writer),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Type property of SpecObject SpectObjectIdentifier:SpectObjectLongName may not be null"));
        }

        [Test]
        public void Verify_that_When_Type_is_null_WriteXmlAsync_throws_exception()
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var spectObject = new SpecObject
            {
                Identifier = "SpectObjectIdentifier",
                LongName = "SpectObjectLongName"
            };

            var cts = new CancellationTokenSource();

            Assert.That(
                () => spectObject.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<SerializationException>()
                    .With.Message.Contains("The Type property of SpecObject SpectObjectIdentifier:SpectObjectLongName may not be null"));
        }
    }
}
