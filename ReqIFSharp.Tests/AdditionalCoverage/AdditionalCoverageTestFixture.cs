// -------------------------------------------------------------------------------------------------
// <copyright file="AdditionalCoverageTestFixture.cs" company="Starion Group S.A.">
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
// -------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests.AdditionalCoverage
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Additional tests that exercise asynchronous and fallback behaviour in several core classes.
    /// </summary>
    [TestFixture]
    public class AdditionalCoverageTestFixture
    {
        [Test]
        public void RelationGroup_ReadXml_CreatesFallbacksForMissingReferences()
        {
            var content = new ReqIFContent();
            var relationGroupType = new RelationGroupType(content, null) { Identifier = "rgt" };

            var existingSpecification = new Specification(content, null)
            {
                Identifier = "existing-spec"
            };

            var existingRelation = new SpecRelation(content, null)
            {
                Identifier = "existing-rel"
            };

            var relationGroup = new RelationGroup(content, null)
            {
                Identifier = "rg",
                LastChange = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc)
            };

            var xml = $@"<RELATION-GROUP IDENTIFIER=""rg"" LAST-CHANGE=""2024-01-01T12:00:00Z"" IS-EDITABLE=""true"">
  <TYPE>
    <RELATION-GROUP-TYPE-REF>{relationGroupType.Identifier}</RELATION-GROUP-TYPE-REF>
  </TYPE>
  <SOURCE-SPECIFICATION>
    <SPECIFICATION-REF>missing-spec</SPECIFICATION-REF>
  </SOURCE-SPECIFICATION>
  <TARGET-SPECIFICATION>
    <SPECIFICATION-REF>{existingSpecification.Identifier}</SPECIFICATION-REF>
  </TARGET-SPECIFICATION>
  <SPEC-RELATIONS>
    <SPEC-RELATION-REF>{existingRelation.Identifier}</SPEC-RELATION-REF>
    <SPEC-RELATION-REF>missing-rel</SPEC-RELATION-REF>
  </SPEC-RELATIONS>
</RELATION-GROUP>";

            using var reader = XmlReader.Create(new StringReader(xml));
            reader.MoveToContent();

            relationGroup.ReadXml(reader);

            Assert.Multiple(() =>
            {
                Assert.That(relationGroup.IsEditable, Is.True, "IS-EDITABLE attribute should enable editing");
                Assert.That(relationGroup.Type, Is.SameAs(relationGroupType));
                Assert.That(relationGroup.SourceSpecification, Is.Not.Null, "Fallback specification should be created");
                Assert.That(relationGroup.SourceSpecification.Identifier, Is.EqualTo("missing-spec"));
                Assert.That(relationGroup.TargetSpecification, Is.SameAs(existingSpecification));
                Assert.That(relationGroup.SpecRelations.Count, Is.EqualTo(2));
                Assert.That(relationGroup.SpecRelations.SingleOrDefault(x => ReferenceEquals(x, existingRelation)), Is.Not.Null);
                Assert.That(relationGroup.SpecRelations.Any(x => x.Identifier == "missing-rel" && x.Description != null), Is.True);
            });
        }

        [Test]
        public async Task RelationGroup_ReadXmlAsync_CreatesFallbacksForMissingReferences()
        {
            var content = new ReqIFContent();
            _ = new RelationGroupType(content, null) { Identifier = "rgt" };
            _ = new Specification(content, null) { Identifier = "existing-spec" };
            _ = new SpecRelation(content, null) { Identifier = "existing-rel" };

            var relationGroup = new RelationGroup(content, null) { Identifier = "rg" };

            var xml = "<RELATION-GROUP IDENTIFIER=\"rg\"><TYPE><RELATION-GROUP-TYPE-REF>rgt</RELATION-GROUP-TYPE-REF></TYPE>"
                      + "<SOURCE-SPECIFICATION><SPECIFICATION-REF>missing-spec</SPECIFICATION-REF></SOURCE-SPECIFICATION>"
                      + "<TARGET-SPECIFICATION><SPECIFICATION-REF>existing-spec</SPECIFICATION-REF></TARGET-SPECIFICATION>"
                      + "<SPEC-RELATIONS><SPEC-RELATION-REF>existing-rel</SPEC-RELATION-REF>"
                      + "<SPEC-RELATION-REF>missing-rel</SPEC-RELATION-REF></SPEC-RELATIONS></RELATION-GROUP>";

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            await relationGroup.ReadXmlAsync(reader, CancellationToken.None);

            Assert.That(relationGroup.SpecRelations.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task SpecHierarchy_ReadXmlAsync_InitialisesContainerAndChildren()
        {
            var content = new ReqIFContent();
            var rootSpecification = new Specification(content, null) { Identifier = "root" };
            var existingObject = new SpecObject(content, null) { Identifier = "existing-object" };

            var hierarchy = new SpecHierarchy(rootSpecification, content, null)
            {
                Identifier = "hierarchy"
            };

            var xml = $@"<SPEC-HIERARCHY IDENTIFIER=""hierarchy"" IS-TABLE-INTERNAL=""true"">
  <OBJECT>
    <SPEC-OBJECT-REF>missing-object</SPEC-OBJECT-REF>
  </OBJECT>
  <CHILDREN>
    <SPEC-HIERARCHY IDENTIFIER=""child"">
      <OBJECT>
        <SPEC-OBJECT-REF>{existingObject.Identifier}</SPEC-OBJECT-REF>
      </OBJECT>
    </SPEC-HIERARCHY>
  </CHILDREN>
</SPEC-HIERARCHY>";

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            await hierarchy.ReadXmlAsync(reader, CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(hierarchy.IsTableInternal, Is.True);
                Assert.That(hierarchy.Object, Is.Null, "Missing objects should yield null reference");
                Assert.That(hierarchy.Children, Has.Count.EqualTo(1));
                Assert.That(hierarchy.Children[0].Object, Is.SameAs(existingObject));
                Assert.That(hierarchy.Children[0].Container, Is.SameAs(hierarchy));
            });
        }

        [Test]
        public void ReqIF_WriteXml_AddsXhtmlNamespaceWhenDatatypePresent()
        {
            var reqIf = CreateReqIfDocument();

            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                writer.WriteStartElement("REQ-IF");
                reqIf.WriteXml(writer);
                writer.WriteEndElement();
            }

            var xml = builder.ToString();

            StringAssert.Contains("xmlns:xhtml=\"http://www.w3.org/1999/xhtml\"", xml);
            StringAssert.Contains("REQ-IF-HEADER", xml);
            StringAssert.Contains("TOOL-EXTENSIONS", xml);
        }

        [Test]
        public async Task ReqIF_ReadXmlAsync_RestoresHeaderAndToolExtensions()
        {
            var reqIf = CreateReqIfDocument();

            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                writer.WriteStartElement("REQ-IF");
                reqIf.WriteXml(writer);
                writer.WriteEndElement();
            }

            using var reader = XmlReader.Create(new StringReader(builder.ToString()), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            var roundTrip = new ReqIF();
            await roundTrip.ReadXmlAsync(reader, CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(roundTrip.Lang, Is.EqualTo(reqIf.Lang));
                Assert.That(roundTrip.TheHeader.Title, Is.EqualTo(reqIf.TheHeader.Title));
                Assert.That(roundTrip.ToolExtension, Has.Count.EqualTo(1));
                Assert.That(roundTrip.CoreContent.DataTypes.Count, Is.EqualTo(2));
            });
        }

        [Test]
        public void AttributeValueXhtml_ExtractsPlainTextAndDetectsExternalObjects()
        {
            var attributeValue = new AttributeValueXHTML();
            attributeValue.TheValue = "<div>plain <b>text</b><object data=\"file%20name.bin\" type=\"application/octet-stream\"></object></div>";

            var plain = attributeValue.ExtractUnformattedTextFromValue();

            Assert.That(plain, Is.EqualTo("plain text"));

            var objects = attributeValue.ExternalObjects;
            Assert.That(objects, Has.Count.EqualTo(1));
            Assert.That(objects[0].Uri, Is.EqualTo("file name.bin"));
        }

        private static ReqIF CreateReqIfDocument()
        {
            var reqIf = new ReqIF
            {
                Lang = "en",
                TheHeader = new ReqIFHeader
                {
                    Identifier = "header",
                    Comment = "comment",
                    CreationTime = new DateTime(2024, 01, 01, 12, 00, 00, DateTimeKind.Utc),
                    RepositoryId = "repo",
                    ReqIFToolId = "tool",
                    ReqIFVersion = "1.0",
                    SourceToolId = "source",
                    Title = "title"
                },
                CoreContent = new ReqIFContent()
            };

            var enumeration = new DatatypeDefinitionEnumeration(reqIf.CoreContent, null)
            {
                Identifier = "enum"
            };

            var enumValue = new EnumValue(enumeration, null)
            {
                Identifier = "enum-value"
            };

            _ = new EmbeddedValue(enumValue, null)
            {
                Key = 5,
                OtherContent = "red"
            };

            var xhtmlDatatype = new DatatypeDefinitionXHTML(reqIf.CoreContent, null)
            {
                Identifier = "xhtml"
            };

            reqIf.ToolExtension.Add(new ReqIFToolExtension { InnerXml = "<extension />" });

            return reqIf;
        }
    }
}
