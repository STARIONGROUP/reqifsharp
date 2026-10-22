// -------------------------------------------------------------------------------------------------
// <copyright file="IsEditableTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Verifies that the IS-EDITABLE attribute of an <see cref="AccessControlledElement"/> (a spec hierarchy
    /// or an attribute definition) is written as it was read, including an explicit IS-EDITABLE="false".
    /// </summary>
    [TestFixture]
    public class IsEditableTestFixture
    {
        private const string FileName = "reqifsharpgenerated.reqif";

        /// <summary>
        /// Returns the test document with IS-EDITABLE="false" on its first spec hierarchy and on its first
        /// attribute definition, IS-EDITABLE="true" on its second spec hierarchy and no IS-EDITABLE on its third
        /// </summary>
        private static string CreateDocument(out string[] identifiers)
        {
            var ns = TestDocuments.ReqIFNamespace;
            var document = XDocument.Parse(TestDocuments.Read(FileName));

            var specHierarchies = document.Descendants(ns + "SPEC-HIERARCHY").ToList();
            Assert.That(specHierarchies, Has.Count.GreaterThanOrEqualTo(3));

            var attributeDefinition = document.Descendants(ns + "SPEC-ATTRIBUTES").Elements().First();

            specHierarchies[0].SetAttributeValue("IS-EDITABLE", "false");
            specHierarchies[1].SetAttributeValue("IS-EDITABLE", "true");
            specHierarchies[2].SetAttributeValue("IS-EDITABLE", null);
            attributeDefinition.SetAttributeValue("IS-EDITABLE", "false");

            identifiers = new[]
            {
                specHierarchies[0].Attribute("IDENTIFIER")!.Value,
                specHierarchies[1].Attribute("IDENTIFIER")!.Value,
                specHierarchies[2].Attribute("IDENTIFIER")!.Value,
                attributeDefinition.Attribute("IDENTIFIER")!.Value
            };

            return document.ToString();
        }

        /// <summary>
        /// Returns the spec hierarchy with the identifier
        /// </summary>
        private static SpecHierarchy SpecHierarchyOf(ReqIF reqif, string identifier)
        {
            IEnumerable<SpecHierarchy> All(IEnumerable<SpecHierarchy> hierarchies) =>
                hierarchies.SelectMany(x => new[] { x }.Concat(All(x.Children)));

            return All(reqif.CoreContent.Specifications.SelectMany(x => x.Children)).Single(x => x.Identifier == identifier);
        }

        /// <summary>
        /// Returns the access controlled element (spec hierarchy or attribute definition) with the identifier
        /// </summary>
        private static AccessControlledElement AccessControlledElementOf(ReqIF reqif, string identifier)
        {
            return reqif.CoreContent.SpecTypes.SelectMany(x => x.SpecAttributes).SingleOrDefault(x => x.Identifier == identifier)
                   ?? (AccessControlledElement)SpecHierarchyOf(reqif, identifier);
        }

        /// <summary>
        /// Returns the IS-EDITABLE attribute of the element with the identifier, or null when it has none
        /// </summary>
        private static string IsEditableOf(XDocument document, string identifier)
        {
            return document.Descendants()
                .Single(x => (string)x.Attribute("IDENTIFIER") == identifier)
                .Attribute("IS-EDITABLE")?.Value;
        }

        private static void AssertRoundTrip(Stream output, string[] identifiers)
        {
            output.Position = 0;
            var document = XDocument.Load(output);

            Assert.Multiple(() =>
            {
                Assert.That(IsEditableOf(document, identifiers[0]), Is.EqualTo("false"));
                Assert.That(IsEditableOf(document, identifiers[1]), Is.EqualTo("true"));
                Assert.That(IsEditableOf(document, identifiers[2]), Is.Null);
                Assert.That(IsEditableOf(document, identifiers[3]), Is.EqualTo("false"));
            });
        }

        [TestCaseSource(typeof(TestDocuments), nameof(TestDocuments.Kinds))]
        public void Verify_that_IS_EDITABLE_is_written_as_it_was_read(SupportedFileExtensionKind kind)
        {
            var xml = CreateDocument(out var identifiers);

            var reqifs = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(xml, kind), kind).ToList();

            var reqif = reqifs.Single();

            Assert.Multiple(() =>
            {
                Assert.That(SpecHierarchyOf(reqif, identifiers[0]).IsEditableSpecified, Is.True);
                Assert.That(SpecHierarchyOf(reqif, identifiers[0]).IsEditable, Is.False);
                Assert.That(SpecHierarchyOf(reqif, identifiers[1]).IsEditableSpecified, Is.True);
                Assert.That(SpecHierarchyOf(reqif, identifiers[1]).IsEditable, Is.True);
                Assert.That(SpecHierarchyOf(reqif, identifiers[2]).IsEditableSpecified, Is.False);
                Assert.That(SpecHierarchyOf(reqif, identifiers[2]).IsEditable, Is.False);
                Assert.That(AccessControlledElementOf(reqif, identifiers[3]).IsEditableSpecified, Is.True);
                Assert.That(AccessControlledElementOf(reqif, identifiers[3]).IsEditable, Is.False);
            });

            using var output = new MemoryStream();
            new ReqIFSerializer().Serialize(reqifs, output, SupportedFileExtensionKind.Reqif);

            AssertRoundTrip(output, identifiers);
        }

        [TestCaseSource(typeof(TestDocuments), nameof(TestDocuments.Kinds))]
        public async Task Verify_that_IS_EDITABLE_is_written_as_it_was_read_async(SupportedFileExtensionKind kind)
        {
            var xml = CreateDocument(out var identifiers);

            var reqifs = (await new ReqIFDeserializer().DeserializeAsync(TestDocuments.ToStream(xml, kind), kind, CancellationToken.None)).ToList();

            using var output = new MemoryStream();
            await new ReqIFSerializer().SerializeAsync(reqifs, output, SupportedFileExtensionKind.Reqif, CancellationToken.None);

            AssertRoundTrip(output, identifiers);
        }

        [Test]
        public void Verify_that_IsEditableSpecified_is_false_for_a_new_element_and_set_by_IsEditable()
        {
            var specHierarchy = new SpecHierarchy();

            Assert.That(specHierarchy.IsEditableSpecified, Is.False);

            specHierarchy.IsEditable = false;

            Assert.Multiple(() =>
            {
                Assert.That(specHierarchy.IsEditableSpecified, Is.True);
                Assert.That(specHierarchy.IsEditable, Is.False);
            });
        }

        [TestCase(0, Description = "spec hierarchy")]
        [TestCase(3, Description = "attribute definition")]
        public async Task Verify_that_a_false_IS_EDITABLE_that_was_read_is_left_out_when_IsEditableSpecified_is_set_to_false(int index)
        {
            var xml = CreateDocument(out var identifiers);
            var reqifs = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(xml, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif).ToList();

            var element = AccessControlledElementOf(reqifs.Single(), identifiers[index]);
            Assert.That(element.IsEditableSpecified, Is.True, "the element should have been read with IS-EDITABLE=\"false\"");

            element.IsEditableSpecified = false;

            foreach (var document in await SerializeBothWays(reqifs))
            {
                Assert.That(IsEditableOf(document, identifiers[index]), Is.Null);
            }
        }

        [Test]
        public async Task Verify_that_a_false_IS_EDITABLE_is_written_when_IsEditableSpecified_is_set_to_true()
        {
            var xml = CreateDocument(out var identifiers);
            var reqifs = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(xml, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif).ToList();

            SpecHierarchyOf(reqifs.Single(), identifiers[2]).IsEditableSpecified = true;

            foreach (var document in await SerializeBothWays(reqifs))
            {
                Assert.That(IsEditableOf(document, identifiers[2]), Is.EqualTo("false"));
            }
        }

        [TestCase(false, "false")]
        [TestCase(true, "true")]
        public async Task Verify_that_IsEditable_set_in_code_is_written(bool isEditable, string expected)
        {
            var xml = CreateDocument(out var identifiers);
            var reqifs = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(xml, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif).ToList();

            // identifiers[2] was read without IS-EDITABLE; set it in code and leave the others as read
            SpecHierarchyOf(reqifs.Single(), identifiers[2]).IsEditable = isEditable;

            foreach (var document in await SerializeBothWays(reqifs))
            {
                Assert.Multiple(() =>
                {
                    Assert.That(IsEditableOf(document, identifiers[2]), Is.EqualTo(expected));
                    Assert.That(IsEditableOf(document, identifiers[0]), Is.EqualTo("false"));
                });
            }
        }

        [TestCase(false, "false")]
        [TestCase(true, "true")]
        public async Task Verify_that_IsEditable_set_in_code_on_an_attribute_definition_is_written(bool isEditable, string expected)
        {
            var xml = CreateDocument(out var identifiers);
            var reqifs = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(xml, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif).ToList();

            // an attribute definition that was read without IS-EDITABLE
            var attributeDefinition = reqifs.Single().CoreContent.SpecTypes
                .SelectMany(x => x.SpecAttributes)
                .First(x => x.Identifier != identifiers[3]);

            Assert.That(attributeDefinition.IsEditableSpecified, Is.False);

            attributeDefinition.IsEditable = isEditable;

            foreach (var document in await SerializeBothWays(reqifs))
            {
                Assert.That(IsEditableOf(document, attributeDefinition.Identifier), Is.EqualTo(expected));
            }
        }

        [Test]
        public async Task Verify_that_a_true_IS_EDITABLE_is_written_when_IsEditableSpecified_is_false()
        {
            var xml = CreateDocument(out var identifiers);
            var reqifs = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(xml, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif).ToList();

            SpecHierarchyOf(reqifs.Single(), identifiers[1]).IsEditableSpecified = false;

            foreach (var document in await SerializeBothWays(reqifs))
            {
                Assert.That(IsEditableOf(document, identifiers[1]), Is.EqualTo("true"));
            }
        }

        [TestCase("true", null)]
        [TestCase("false", "true")]
        public async Task Verify_that_the_attributes_of_a_spec_hierarchy_with_an_alternative_id_are_written(string isEditable, string isTableInternal)
        {
            var ns = TestDocuments.ReqIFNamespace;
            var source = XDocument.Parse(TestDocuments.Read(FileName));

            var specHierarchy = source.Descendants(ns + "SPEC-HIERARCHY").First();
            var identifier = specHierarchy.Attribute("IDENTIFIER")!.Value;
            specHierarchy.SetAttributeValue("IS-EDITABLE", isEditable);
            specHierarchy.SetAttributeValue("IS-TABLE-INTERNAL", isTableInternal);
            specHierarchy.Elements(ns + "ALTERNATIVE-ID").Remove();
            specHierarchy.AddFirst(new XElement(ns + "ALTERNATIVE-ID", new XElement(ns + "ALTERNATIVE-ID", new XAttribute("IDENTIFIER", "hierarchy_alternative"))));

            var reqifs = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(source.ToString(), SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif).ToList();

            foreach (var document in await SerializeBothWays(reqifs))
            {
                var element = document.Descendants(ns + "SPEC-HIERARCHY").Single(x => (string)x.Attribute("IDENTIFIER") == identifier);

                Assert.Multiple(() =>
                {
                    Assert.That((string)element.Attribute("IS-EDITABLE"), Is.EqualTo(isEditable));
                    Assert.That((string)element.Attribute("IS-TABLE-INTERNAL"), Is.EqualTo(isTableInternal));
                    Assert.That((string)element.Element(ns + "ALTERNATIVE-ID")?.Element(ns + "ALTERNATIVE-ID")?.Attribute("IDENTIFIER"), Is.EqualTo("hierarchy_alternative"));
                });
            }
        }

        [TestCase("true")]
        [TestCase("false")]
        public async Task Verify_that_the_attributes_of_an_attribute_definition_with_an_alternative_id_are_written(string isEditable)
        {
            var ns = TestDocuments.ReqIFNamespace;
            var source = XDocument.Parse(TestDocuments.Read(FileName));

            var attributeDefinition = source.Descendants(ns + "ATTRIBUTE-DEFINITION-ENUMERATION").First();
            var identifier = attributeDefinition.Attribute("IDENTIFIER")!.Value;
            attributeDefinition.SetAttributeValue("IS-EDITABLE", isEditable);
            attributeDefinition.SetAttributeValue("MULTI-VALUED", "true");
            attributeDefinition.Elements(ns + "ALTERNATIVE-ID").Remove();
            attributeDefinition.AddFirst(new XElement(ns + "ALTERNATIVE-ID", new XElement(ns + "ALTERNATIVE-ID", new XAttribute("IDENTIFIER", "definition_alternative"))));

            var reqifs = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(source.ToString(), SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif).ToList();

            foreach (var document in await SerializeBothWays(reqifs))
            {
                var element = document.Descendants(ns + "ATTRIBUTE-DEFINITION-ENUMERATION").Single(x => (string)x.Attribute("IDENTIFIER") == identifier);

                Assert.Multiple(() =>
                {
                    Assert.That((string)element.Attribute("IS-EDITABLE"), Is.EqualTo(isEditable));
                    Assert.That((string)element.Attribute("MULTI-VALUED"), Is.EqualTo("true"));
                    Assert.That((string)element.Element(ns + "ALTERNATIVE-ID")?.Element(ns + "ALTERNATIVE-ID")?.Attribute("IDENTIFIER"), Is.EqualTo("definition_alternative"));
                });
            }
        }

        [Test]
        public async Task Verify_that_the_attributes_of_datatypes_and_enumeration_definitions_are_written()
        {
            var ns = TestDocuments.ReqIFNamespace;
            var numericAttributes = new[] { "MIN", "MAX", "ACCURACY", "MAX-LENGTH" };

            var xml = TestDocuments.Read("Datatype-Demo.reqif");
            var source = XDocument.Parse(xml);

            var elements = source.Descendants()
                .Where(x => numericAttributes.Any(a => x.Attribute(a) != null) || x.Attribute("MULTI-VALUED") != null)
                .ToList();

            var localNames = elements.Select(x => x.Name.LocalName).Distinct().ToList();
            Assert.That(localNames, Is.SupersetOf(new[] { "DATATYPE-DEFINITION-INTEGER", "DATATYPE-DEFINITION-REAL", "DATATYPE-DEFINITION-STRING", "ATTRIBUTE-DEFINITION-ENUMERATION" }));
            Assert.That(elements.Where(x => x.Element(ns + "ALTERNATIVE-ID") != null), Is.Not.Empty, "the test document should have a datatype with an alternative id");

            var reqifs = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(xml, SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif).ToList();

            foreach (var document in await SerializeBothWays(reqifs))
            {
                foreach (var expected in elements)
                {
                    var identifier = expected.Attribute("IDENTIFIER")!.Value;
                    var actual = document.Descendants(expected.Name).Single(x => (string)x.Attribute("IDENTIFIER") == identifier);

                    Assert.Multiple(() =>
                    {
                        foreach (var name in numericAttributes.Where(a => expected.Attribute(a) != null))
                        {
                            Assert.That(actual.Attribute(name), Is.Not.Null, $"{name} of {identifier} is not written");
                            Assert.That(XmlConvert.ToDouble(actual.Attribute(name)!.Value), Is.EqualTo(XmlConvert.ToDouble(expected.Attribute(name)!.Value)), $"{name} of {identifier}");
                        }

                        if (expected.Attribute("MULTI-VALUED") != null)
                        {
                            Assert.That(actual.Attribute("MULTI-VALUED"), Is.Not.Null, $"MULTI-VALUED of {identifier} is not written");
                            Assert.That(XmlConvert.ToBoolean(actual.Attribute("MULTI-VALUED")!.Value), Is.EqualTo(XmlConvert.ToBoolean(expected.Attribute("MULTI-VALUED")!.Value)), $"MULTI-VALUED of {identifier}");
                        }

                        Assert.That(actual.Element(ns + "ALTERNATIVE-ID") != null, Is.EqualTo(expected.Element(ns + "ALTERNATIVE-ID") != null), $"ALTERNATIVE-ID of {identifier}");
                    });
                }
            }
        }

        [TestCase("")]
        [TestCase("  ")]
        public async Task Verify_that_an_empty_IS_EDITABLE_is_read_as_not_specified_and_left_out(string isEditable)
        {
            var ns = TestDocuments.ReqIFNamespace;
            var source = XDocument.Parse(TestDocuments.Read(FileName));

            var specHierarchy = source.Descendants(ns + "SPEC-HIERARCHY").First();
            var identifier = specHierarchy.Attribute("IDENTIFIER")!.Value;
            specHierarchy.SetAttributeValue("IS-EDITABLE", isEditable);

            var reqifs = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(source.ToString(), SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif).ToList();

            var element = SpecHierarchyOf(reqifs.Single(), identifier);

            Assert.Multiple(() =>
            {
                Assert.That(element.IsEditableSpecified, Is.False);
                Assert.That(element.IsEditable, Is.False);
            });

            foreach (var document in await SerializeBothWays(reqifs))
            {
                Assert.That(IsEditableOf(document, identifier), Is.Null);
            }
        }

        [TestCase("0", false, "false")]
        [TestCase("1", true, "true")]
        public async Task Verify_that_a_numeric_IS_EDITABLE_is_read_and_written_as_a_boolean(string isEditable, bool expectedValue, string expectedText)
        {
            var ns = TestDocuments.ReqIFNamespace;
            var source = XDocument.Parse(TestDocuments.Read(FileName));

            var specHierarchy = source.Descendants(ns + "SPEC-HIERARCHY").First();
            var identifier = specHierarchy.Attribute("IDENTIFIER")!.Value;
            specHierarchy.SetAttributeValue("IS-EDITABLE", isEditable);

            var reqifs = new ReqIFDeserializer().Deserialize(TestDocuments.ToStream(source.ToString(), SupportedFileExtensionKind.Reqif), SupportedFileExtensionKind.Reqif).ToList();

            var element = SpecHierarchyOf(reqifs.Single(), identifier);

            Assert.Multiple(() =>
            {
                Assert.That(element.IsEditableSpecified, Is.True);
                Assert.That(element.IsEditable, Is.EqualTo(expectedValue));
            });

            foreach (var document in await SerializeBothWays(reqifs))
            {
                Assert.That(IsEditableOf(document, identifier), Is.EqualTo(expectedText));
            }
        }

        /// <summary>
        /// Writes the documents with the sync and with the async serializer
        /// </summary>
        private static async Task<IEnumerable<XDocument>> SerializeBothWays(IEnumerable<ReqIF> reqifs)
        {
            var serializer = new ReqIFSerializer();

            using var syncOutput = new MemoryStream();
            serializer.Serialize(reqifs, syncOutput, SupportedFileExtensionKind.Reqif);
            syncOutput.Position = 0;

            using var asyncOutput = new MemoryStream();
            await serializer.SerializeAsync(reqifs, asyncOutput, SupportedFileExtensionKind.Reqif, CancellationToken.None);
            asyncOutput.Position = 0;

            return new[] { XDocument.Load(syncOutput), XDocument.Load(asyncOutput) };
        }
    }
}
