// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueXHTMLReadTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Verifies that <see cref="AttributeValueXHTML"/> reads THE-VALUE, THE-ORIGINAL-VALUE and DEFINITION
    /// in any order, in indented and minified documents, see GH-95.
    /// </summary>
    [TestFixture]
    public class AttributeValueXHTMLReadTestFixture
    {
        private const string FileName = "reqifsharpgenerated.reqif";

        private static readonly XNamespace Xhtml = "http://www.w3.org/1999/xhtml";

        private static readonly object[] ChildOrders =
        {
            new object[] { new[] { "DEFINITION", "THE-VALUE", "THE-ORIGINAL-VALUE" } },
            new object[] { new[] { "DEFINITION", "THE-ORIGINAL-VALUE", "THE-VALUE" } },
            new object[] { new[] { "THE-VALUE", "THE-ORIGINAL-VALUE", "DEFINITION" } },
            new object[] { new[] { "THE-ORIGINAL-VALUE", "THE-VALUE", "DEFINITION" } }
        };

        /// <summary>
        /// Orders with an element that is not part of the schema. It contains a THE-VALUE and a DEFINITION with an
        /// unknown reference: reading either would change the value or throw.
        /// </summary>
        private static readonly object[] ChildOrdersWithUnknownElement =
        {
            new object[] { new[] { "UNKNOWN", "DEFINITION", "THE-VALUE", "THE-ORIGINAL-VALUE" } },
            new object[] { new[] { "DEFINITION", "THE-VALUE", "UNKNOWN", "THE-ORIGINAL-VALUE" } }
        };

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_the_original_value_is_read(bool async)
        {
            var original = TestDocuments.Read(FileName);
            var expectedCount = XDocument.Parse(original).Descendants(TestDocuments.ReqIFNamespace + "THE-ORIGINAL-VALUE").Count();

            Assert.That(expectedCount, Is.GreaterThan(0), "the test document must contain THE-ORIGINAL-VALUE elements");

            var values = XhtmlValues(await Deserialize(original, SupportedFileExtensionKind.Reqif, async));
            var withOriginal = values.Where(x => x.TheOriginalValue != null).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(withOriginal, Has.Count.EqualTo(expectedCount));
                Assert.That(withOriginal.Select(x => x.TheOriginalValue), Has.All.StartWith("<xhtml:p").And.Contain("text after del"));
                Assert.That(withOriginal.Select(x => x.IsSimplified), Has.All.True);
            });
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task Verify_that_the_original_value_is_kept_when_a_document_is_read_written_and_read_again(bool async)
        {
            var first = await Deserialize(TestDocuments.Read(FileName), SupportedFileExtensionKind.Reqif, async);

            string written;

            using (var stream = new MemoryStream())
            {
                var serializer = new ReqIFSerializer();

                if (async)
                {
                    await serializer.SerializeAsync(first, stream, SupportedFileExtensionKind.Reqif, CancellationToken.None);
                }
                else
                {
                    serializer.Serialize(first, stream, SupportedFileExtensionKind.Reqif);
                }

                stream.Position = 0;
                written = new StreamReader(stream).ReadToEnd();
            }

            var second = await Deserialize(written, SupportedFileExtensionKind.Reqif, async);

            var expected = XhtmlValues(first).Select(x => (x.IsSimplified, x.TheOriginalValue)).ToList();
            var actual = XhtmlValues(second).Select(x => (x.IsSimplified, x.TheOriginalValue)).ToList();

            Assert.That(expected.Count(x => x.TheOriginalValue != null), Is.GreaterThan(0));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(ChildOrders))]
        [TestCaseSource(nameof(ChildOrdersWithUnknownElement))]
        public async Task Verify_that_the_children_are_read_in_any_order(string[] order)
        {
            var (xml, expected) = CreateDocumentWithChildOrder(order);

            foreach (var document in new[] { xml, TestDocuments.Minify(xml) })
            {
                foreach (var kind in TestDocuments.Kinds)
                {
                    foreach (var async in new[] { false, true })
                    {
                        var values = XhtmlValues(await Deserialize(document, kind, async));

                        var actual = values
                            .Select(x => (Definition: x.Definition?.Identifier, Value: Marker(x.TheValue, "value"), Original: Marker(x.TheOriginalValue, "original")))
                            .ToList();

                        Assert.That(actual, Is.EquivalentTo(expected), $"order {string.Join(", ", order)}, {kind}, async: {async}, minified: {document != xml}");
                    }
                }
            }
        }

        [Test]
        public void Verify_that_an_unknown_definition_reference_throws()
        {
            var ns = TestDocuments.ReqIFNamespace;
            var document = XDocument.Parse(TestDocuments.Read(FileName));

            document.Descendants(ns + "ATTRIBUTE-VALUE-XHTML").First()
                .Element(ns + "DEFINITION")!
                .Element(ns + "ATTRIBUTE-DEFINITION-XHTML-REF")!.Value = "unknown-xhtml-definition";

            var xml = document.ToString();

            foreach (var copy in new[] { xml, TestDocuments.Minify(xml) })
            {
                foreach (var kind in TestDocuments.Kinds)
                {
                    foreach (var async in new[] { false, true })
                    {
                        Assert.That(async () => await Deserialize(copy, kind, async),
                            Throws.InstanceOf<InvalidOperationException>().With.Message.Contains("The attribute-definition XHTML unknown-xhtml-definition could not be found"),
                            $"{kind}, async: {async}, minified: {copy != xml}");
                    }
                }
            }
        }

        /// <summary>
        /// Returns a copy of the test document in which every XHTML value has a THE-VALUE "value i" and a
        /// THE-ORIGINAL-VALUE "original i", with its children in the given order, and the values expected after reading it.
        /// </summary>
        private static (string Xml, List<(string Definition, string Value, string Original)> Expected) CreateDocumentWithChildOrder(string[] order)
        {
            var ns = TestDocuments.ReqIFNamespace;
            var document = XDocument.Parse(TestDocuments.Read(FileName));
            var expected = new List<(string Definition, string Value, string Original)>();

            var index = 0;

            foreach (var attributeValue in document.Descendants(ns + "ATTRIBUTE-VALUE-XHTML").ToList())
            {
                var definition = attributeValue.Element(ns + "DEFINITION");
                var definitionReference = definition!.Element(ns + "ATTRIBUTE-DEFINITION-XHTML-REF")!.Value;

                var children = new Dictionary<string, XElement>
                {
                    ["DEFINITION"] = definition,
                    ["THE-VALUE"] = new XElement(ns + "THE-VALUE", new XElement(Xhtml + "p", $"value {index}")),
                    ["THE-ORIGINAL-VALUE"] = new XElement(ns + "THE-ORIGINAL-VALUE", new XElement(Xhtml + "p", $"original {index}")),
                    ["UNKNOWN"] = new XElement(ns + "UNKNOWN",
                        new XElement(ns + "THE-VALUE", new XElement(Xhtml + "p", "value of the unknown element")),
                        new XElement(ns + "DEFINITION", new XElement(ns + "ATTRIBUTE-DEFINITION-XHTML-REF", "unknown-xhtml-definition")))
                };

                attributeValue.RemoveNodes();
                attributeValue.Add(order.Select(name => children[name]));

                expected.Add((definitionReference, $"value {index}", $"original {index}"));
                index++;
            }

            Assert.That(expected, Is.Not.Empty, "the test document must contain XHTML values");

            return (document.ToString(), expected);
        }

        private static string Marker(string xhtml, string word)
        {
            return xhtml == null ? null : Regex.Match(xhtml, $@"{word} \d+").Value;
        }

        private static List<AttributeValueXHTML> XhtmlValues(IEnumerable<ReqIF> reqifs)
        {
            var content = reqifs.Single().CoreContent;

            return content.SpecObjects.SelectMany(x => x.Values)
                .Concat(content.SpecRelations.SelectMany(x => x.Values))
                .Concat(content.Specifications.SelectMany(x => x.Values))
                .Concat(content.SpecRelationGroups.SelectMany(x => x.Values))
                .OfType<AttributeValueXHTML>()
                .ToList();
        }

        private static async Task<List<ReqIF>> Deserialize(string xml, SupportedFileExtensionKind kind, bool async)
        {
            var deserializer = new ReqIFDeserializer();
            using var stream = TestDocuments.ToStream(xml, kind);

            return async
                ? (await deserializer.DeserializeAsync(stream, kind, CancellationToken.None)).ToList()
                : deserializer.Deserialize(stream, kind).ToList();
        }
    }
}
