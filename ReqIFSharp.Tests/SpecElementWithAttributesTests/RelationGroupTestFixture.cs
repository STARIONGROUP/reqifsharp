// -------------------------------------------------------------------------------------------------
// <copyright file="RelationGroupTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="RelationGroup"/>
    /// </summary>
    [TestFixture]
    public class RelationGroupTestFixture
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
            Assert.That(() => new RelationGroup(null), Throws.Nothing);
            Assert.That(() => new RelationGroup(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void Verify_That_WriteXml_Throws_Exception_When_Type_Is_Null()
        {
            var relationGroup = new RelationGroup();

            // Type is not set
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(() => relationGroup.WriteXml(writer),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The Type property of RelationGroup : may not be null"));
                }
            }

            // Source specification is not set
            var relationGroupType = new RelationGroupType();
            relationGroup.Type = relationGroupType;
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(() => relationGroup.WriteXml(writer),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The SourceSpecification property of RelationGroup : may not be null"));
                }
            }

            // target specification is not set
            var sourceSpecification = new Specification();
            relationGroup.SourceSpecification = sourceSpecification;
            
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(() => relationGroup.WriteXml(writer),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The TargetSpecification property of RelationGroup : may not be null"));
                }
            }
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_When_Type_Is_Null()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var relationGroup = new RelationGroup();

            // Type is not set
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(async () => await relationGroup.WriteXmlAsync(writer, cancellationTokenSource.Token),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The Type property of RelationGroup : may not be null"));
                }
            }

            // Source specification is not set
            var relationGroupType = new RelationGroupType();
            relationGroup.Type = relationGroupType;
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(async () => await relationGroup.WriteXmlAsync(writer, cancellationTokenSource.Token),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The SourceSpecification property of RelationGroup : may not be null"));
                }
            }

            // target specification is not set
            var sourceSpecification = new Specification();
            relationGroup.SourceSpecification = sourceSpecification;

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(async () => await relationGroup.WriteXmlAsync(writer, cancellationTokenSource.Token),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The TargetSpecification property of RelationGroup : may not be null"));
                }
            }
        }

        [Test]
        public void Verify_That_The_SpectType_Can_Be_Set()
        {
            var relationGroupType = new RelationGroupType();
            var relationGroup = new RelationGroup();

            relationGroup.SpecType = relationGroupType;

            Assert.That(relationGroup.SpecType, Is.EqualTo(relationGroupType));
        }

        [Test]
        public void Verify_That_Setting_An_Incorrect_SpecType_Throws_Exception()
        {
            var specificationType = new SpecificationType();
            var relationGroup = new RelationGroup();

            Assert.That(() => relationGroup.SpecType = specificationType,
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("specType must of type RelationGroupType")); 
        }

        [Test]
        public void Verify_that_ReadXml_sets_references_and_properties()
        {
            var xml = """
                      <RELATION-GROUP IDENTIFIER="relationgroup-1" LAST-CHANGE="2015-12-01T00:00:00Z" LONG-NAME="relationgroup 1">
                        <TYPE>
                          <RELATION-GROUP-TYPE-REF>relationgrouptype</RELATION-GROUP-TYPE-REF>
                        </TYPE>
                        <SOURCE-SPECIFICATION>
                          <SPECIFICATION-REF>specification-1</SPECIFICATION-REF>
                        </SOURCE-SPECIFICATION>
                        <TARGET-SPECIFICATION>
                          <SPECIFICATION-REF>specification-2</SPECIFICATION-REF>
                        </TARGET-SPECIFICATION>
                      </RELATION-GROUP>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            var content = new ReqIFContent();
            var relationGroup = new RelationGroup(content, this.loggerFactory);

            relationGroup.ReadXml(reader);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(relationGroup.Identifier, Is.EqualTo("relationgroup-1"));
                Assert.That(relationGroup.LongName, Is.EqualTo("relationgroup 1"));
                Assert.That(relationGroup.SourceSpecification.Identifier, Is.EqualTo("specification-1"));
                Assert.That(relationGroup.TargetSpecification.Identifier, Is.EqualTo("specification-2"));
            }
        }

        [Test]
        public async Task Verify_that_ReadXmlAsync_sets_references_and_properties()
        {
            var xml = """
                      <RELATION-GROUP IDENTIFIER="relationgroup-1" LAST-CHANGE="2015-12-01T00:00:00Z" LONG-NAME="relationgroup 1">
                        <TYPE>
                          <RELATION-GROUP-TYPE-REF>relationgrouptype</RELATION-GROUP-TYPE-REF>
                        </TYPE>
                        <SOURCE-SPECIFICATION>
                          <SPECIFICATION-REF>specification-1</SPECIFICATION-REF>
                        </SOURCE-SPECIFICATION>
                        <TARGET-SPECIFICATION>
                          <SPECIFICATION-REF>specification-2</SPECIFICATION-REF>
                        </TARGET-SPECIFICATION>
                      </RELATION-GROUP>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent();
            var relationGroup = new RelationGroup(content, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await relationGroup.ReadXmlAsync(reader, cts.Token);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(relationGroup.Identifier, Is.EqualTo("relationgroup-1"));
                Assert.That(relationGroup.LongName, Is.EqualTo("relationgroup 1"));
                Assert.That(relationGroup.SourceSpecification.Identifier, Is.EqualTo("specification-1"));
                Assert.That(relationGroup.TargetSpecification.Identifier, Is.EqualTo("specification-2"));
            }
        }

        [Test]
        public async Task Verify_that_when_ReadXmlAsync_cancel_throws_OperationCanceledException()
        {
            var xml = """
                      <RELATION-GROUP IDENTIFIER="relationgroup-1" LAST-CHANGE="2015-12-01T00:00:00Z" LONG-NAME="relationgroup 1">
                        <TYPE>
                          <RELATION-GROUP-TYPE-REF>relationgrouptype</RELATION-GROUP-TYPE-REF>
                        </TYPE>
                        <SOURCE-SPECIFICATION>
                          <SPECIFICATION-REF>specification-1</SPECIFICATION-REF>
                        </SOURCE-SPECIFICATION>
                        <TARGET-SPECIFICATION>
                          <SPECIFICATION-REF>specification-2</SPECIFICATION-REF>
                        </TARGET-SPECIFICATION>
                      </RELATION-GROUP>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent();
            var relationGroup = new RelationGroup(content, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await cts.CancelAsync();

            await Assert.ThatAsync(() => relationGroup.ReadXmlAsync(reader, cts.Token), Throws.InstanceOf<OperationCanceledException>());
        }
    }
}
