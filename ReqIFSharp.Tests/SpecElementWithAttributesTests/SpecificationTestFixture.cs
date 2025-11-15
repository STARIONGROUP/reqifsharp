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
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="Specification"/>
    /// </summary>
    [TestFixture]
    public class SpecificationTestFixture
    {
        private XmlWriterSettings settings;

        private ILoggerFactory loggerFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder => { builder.AddSerilog(); });
        }

        [SetUp]
        public void SetUp()
        {
            this.settings = new XmlWriterSettings();
        }

        [Test]
        public void Verify_that_constructor_does_not_throw_exception()
        {
            Assert.That(() => new Specification(null), Throws.Nothing);
            Assert.That(() => new Specification(this.loggerFactory), Throws.Nothing);
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

            Assert.That(() => specification.WriteXml(writer),
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

        [Test]
        public void Verify_that_ReadXml_sets_references_and_properties()
        {
            var xml = """
                      <SPECIFICATION IDENTIFIER="_jgCyugfNEeeAO8RifBaE-g" LAST-CHANGE="2017-03-13T10:15:09.017+01:00" LONG-NAME="Specification Document">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_jgCyugfNEeeAO8RifBaE-g"/>
                          </ALTERNATIVE-ID>
                          <VALUES />
                          <TYPE>
                              <SPECIFICATION-TYPE-REF>_jgCytgfNEeeAO8RifBaE-g</SPECIFICATION-TYPE-REF>
                          </TYPE>
                          <CHILDREN>
                              <SPEC-HIERARCHY IDENTIFIER="_jgCyvAfNEeeAO8RifBaE-g" LAST-CHANGE="2017-03-13T10:15:09.017+01:00">
                                  <ALTERNATIVE-ID>
                                      <ALTERNATIVE-ID IDENTIFIER="_jgCyvAfNEeeAO8RifBaE-g"/>
                                  </ALTERNATIVE-ID>
                                  <OBJECT>
                                      <SPEC-OBJECT-REF>_jgCyuAfNEeeAO8RifBaE-g</SPEC-OBJECT-REF>
                                  </OBJECT>
                              </SPEC-HIERARCHY>
                              <SPEC-HIERARCHY IDENTIFIER="_PDc0EAgMEee0LJx6M8ERew" LAST-CHANGE="2017-03-13T17:43:47.242+01:00">
                                  <ALTERNATIVE-ID>
                                      <ALTERNATIVE-ID IDENTIFIER="_PDc0EAgMEee0LJx6M8ERew"/>
                                  </ALTERNATIVE-ID>
                                  <OBJECT>
                                      <SPEC-OBJECT-REF>_PDEZkAgMEee0LJx6M8ERew</SPEC-OBJECT-REF>
                                  </OBJECT>
                              </SPEC-HIERARCHY>
                              <SPEC-HIERARCHY IDENTIFIER="_1qcsAAgtEeeQEdG1aamkjg" LAST-CHANGE="2017-03-13T21:44:19.841+01:00">
                                  <ALTERNATIVE-ID>
                                      <ALTERNATIVE-ID IDENTIFIER="_1qcsAAgtEeeQEdG1aamkjg"/>
                                  </ALTERNATIVE-ID>
                                  <OBJECT>
                                      <SPEC-OBJECT-REF>_1qST8AgtEeeQEdG1aamkjg</SPEC-OBJECT-REF>
                                  </OBJECT>
                              </SPEC-HIERARCHY>
                          </CHILDREN>
                      </SPECIFICATION>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            reader.MoveToContent();

            var content = new ReqIFContent();
            var specification = new Specification(content, this.loggerFactory);

            specification.ReadXml(reader);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(specification.Identifier, Is.EqualTo("_jgCyugfNEeeAO8RifBaE-g"));
                Assert.That(specification.LongName, Is.EqualTo("Specification Document"));
            }
        }

        [Test]
        public async Task Verify_that_ReadXmlAsync_sets_references_and_properties()
        {
            var xml = """
                      <SPECIFICATION IDENTIFIER="_jgCyugfNEeeAO8RifBaE-g" LAST-CHANGE="2017-03-13T10:15:09.017+01:00" LONG-NAME="Specification Document">
                          <ALTERNATIVE-ID>
                              <ALTERNATIVE-ID IDENTIFIER="_jgCyugfNEeeAO8RifBaE-g"/>
                          </ALTERNATIVE-ID>
                          <VALUES />
                          <TYPE>
                              <SPECIFICATION-TYPE-REF>_jgCytgfNEeeAO8RifBaE-g</SPECIFICATION-TYPE-REF>
                          </TYPE>
                          <CHILDREN>
                              <SPEC-HIERARCHY IDENTIFIER="_jgCyvAfNEeeAO8RifBaE-g" LAST-CHANGE="2017-03-13T10:15:09.017+01:00">
                                  <ALTERNATIVE-ID>
                                      <ALTERNATIVE-ID IDENTIFIER="_jgCyvAfNEeeAO8RifBaE-g"/>
                                  </ALTERNATIVE-ID>
                                  <OBJECT>
                                      <SPEC-OBJECT-REF>_jgCyuAfNEeeAO8RifBaE-g</SPEC-OBJECT-REF>
                                  </OBJECT>
                              </SPEC-HIERARCHY>
                              <SPEC-HIERARCHY IDENTIFIER="_PDc0EAgMEee0LJx6M8ERew" LAST-CHANGE="2017-03-13T17:43:47.242+01:00">
                                  <ALTERNATIVE-ID>
                                      <ALTERNATIVE-ID IDENTIFIER="_PDc0EAgMEee0LJx6M8ERew"/>
                                  </ALTERNATIVE-ID>
                                  <OBJECT>
                                      <SPEC-OBJECT-REF>_PDEZkAgMEee0LJx6M8ERew</SPEC-OBJECT-REF>
                                  </OBJECT>
                              </SPEC-HIERARCHY>
                              <SPEC-HIERARCHY IDENTIFIER="_1qcsAAgtEeeQEdG1aamkjg" LAST-CHANGE="2017-03-13T21:44:19.841+01:00">
                                  <ALTERNATIVE-ID>
                                      <ALTERNATIVE-ID IDENTIFIER="_1qcsAAgtEeeQEdG1aamkjg"/>
                                  </ALTERNATIVE-ID>
                                  <OBJECT>
                                      <SPEC-OBJECT-REF>_1qST8AgtEeeQEdG1aamkjg</SPEC-OBJECT-REF>
                                  </OBJECT>
                              </SPEC-HIERARCHY>
                          </CHILDREN>
                      </SPECIFICATION>
                      """;

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var content = new ReqIFContent();
            var specification = new Specification(content, this.loggerFactory);

            var cts = new CancellationTokenSource();

            await specification.ReadXmlAsync(reader, cts.Token);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(specification.Identifier, Is.EqualTo("_jgCyugfNEeeAO8RifBaE-g"));
                Assert.That(specification.LongName, Is.EqualTo("Specification Document"));
            }
        }

        [Test]
        public async Task Verify_that_when_ReadXmlAsync_cancel_throws_OperationCanceledException()
        {
            var xml = """
                      <SPECIFICATION IDENTIFIER="_jgCyugfNEeeAO8RifBaE-g" LAST-CHANGE="2017-03-13T10:15:09.017+01:00" LONG-NAME="Specification Document">
                          <VALUES />
                          <TYPE>
                              <SPECIFICATION-TYPE-REF>_jgCytgfNEeeAO8RifBaE-g</SPECIFICATION-TYPE-REF>
                          </TYPE>
                          <CHILDREN />
                          
                      </SPECIFICATION>
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
