// -------------------------------------------------------------------------------------------------
// <copyright file="SpecRelationTestFixture.cs" company="Starion Group S.A.">
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
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="SpecRelation"/>
    /// </summary>
    [TestFixture]
    public class SpecRelationTestFixture
    {
        private XmlWriterSettings settings;

        private ILoggerFactory loggerFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
        }

        [SetUp]
        public void SetUp()
        {
            this.settings = new XmlWriterSettings();
        }

        [Test]
        public void Verify_that_constructor_does_not_throw_exception()
        {
            Assert.That(() => new SpecRelation(null), Throws.Nothing);
            Assert.That(() => new SpecRelation(this.loggerFactory), Throws.Nothing);
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

        [Test]
        public async Task Verify_that_when_ReadXmlAsync_cancel_throws_OperationCanceledException()
        {
            var xml = """
                      <SPEC-RELATION IDENTIFIER="specobject_1-specobject_2" LAST-CHANGE="2015-12-01T00:00:00Z" LONG-NAME="A relationship between spec objects">
                        <VALUES />
                        <TYPE>
                          <SPEC-RELATION-TYPE-REF>specificationrelation</SPEC-RELATION-TYPE-REF>
                        </TYPE>
                        <TARGET>
                          <SPEC-OBJECT-REF>specobject_2</SPEC-OBJECT-REF>
                        </TARGET>
                        <SOURCE>
                          <SPEC-OBJECT-REF>specobject_1</SPEC-OBJECT-REF>
                        </SOURCE>
                      </SPEC-RELATION>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent();
            var specRelation = new SpecRelation(content, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await cts.CancelAsync();

            await Assert.ThatAsync(() => specRelation.ReadXmlAsync(reader, cts.Token), Throws.InstanceOf<OperationCanceledException>());
        }
    }
}
