// -------------------------------------------------------------------------------------------------
// <copyright file="NamespaceRetentionTestFixture.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests that verify the namespace declarations and root attributes of a <see cref="ReqIF"/>
    /// document are retained when it is deserialized and serialized to a new destination (issue #44).
    /// </summary>
    [TestFixture]
    public class NamespaceRetentionTestFixture
    {
        private const string ReqIFNamespace = "http://www.omg.org/spec/ReqIF/20110401/reqif.xsd";
        private const string XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

        private string datatypeDemoPath;

        [SetUp]
        public void SetUp()
        {
            this.datatypeDemoPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Datatype-Demo.reqif");
        }

        [Test]
        public void Verify_that_namespace_declarations_are_retained_on_roundtrip()
        {
            var deserializer = new ReqIFDeserializer();
            var serializer = new ReqIFSerializer();

            var reqif = deserializer.Deserialize(this.datatypeDemoPath).First();

            using var output = new MemoryStream();
            serializer.Serialize(new[] { reqif }, output, SupportedFileExtensionKind.Reqif);

            var attributes = ExtractRootAttributes(output);

            Assert.Multiple(() =>
            {
                Assert.That(attributes.ContainsKey("xmlns"), Is.True, "the default ReqIF namespace declaration is missing");
                Assert.That(attributes["xmlns"], Is.EqualTo(ReqIFNamespace));
                Assert.That(attributes["xmlns:configuration"], Is.EqualTo("http://eclipse.org/rmf/pror/toolextensions/1.0"));
                Assert.That(attributes["xmlns:id"], Is.EqualTo("http://pror.org/presentation/id"));
                Assert.That(attributes["xmlns:xhtml"], Is.EqualTo("http://www.w3.org/1999/xhtml"));
            });
        }

        [Test]
        public async System.Threading.Tasks.Task Verify_that_namespace_declarations_are_retained_on_roundtrip_async()
        {
            var deserializer = new ReqIFDeserializer();
            var serializer = new ReqIFSerializer();

            var reqif = (await deserializer.DeserializeAsync(this.datatypeDemoPath, CancellationToken.None)).First();

            using var output = new MemoryStream();
            await serializer.SerializeAsync(new[] { reqif }, output, SupportedFileExtensionKind.Reqif, CancellationToken.None);

            var attributes = ExtractRootAttributes(output);

            Assert.Multiple(() =>
            {
                Assert.That(attributes["xmlns"], Is.EqualTo(ReqIFNamespace));
                Assert.That(attributes["xmlns:configuration"], Is.EqualTo("http://eclipse.org/rmf/pror/toolextensions/1.0"));
                Assert.That(attributes["xmlns:id"], Is.EqualTo("http://pror.org/presentation/id"));
                Assert.That(attributes["xmlns:xhtml"], Is.EqualTo("http://www.w3.org/1999/xhtml"));
            });
        }

        [Test]
        public void Verify_that_prefixed_root_attribute_is_retained_on_roundtrip()
        {
            const string schemaLocation = "http://www.omg.org/spec/ReqIF/20110401/reqif.xsd reqif.xsd";

            // start from a known-valid document and add an xsi namespace declaration plus an
            // xsi:schemaLocation prefixed attribute to the REQ-IF root element
            var xml = File.ReadAllText(this.datatypeDemoPath)
                .Replace(
                    $"xmlns=\"{ReqIFNamespace}\"",
                    $"xmlns=\"{ReqIFNamespace}\" xmlns:xsi=\"{XsiNamespace}\" xsi:schemaLocation=\"{schemaLocation}\"");

            var deserializer = new ReqIFDeserializer();
            var serializer = new ReqIFSerializer();

            ReqIF reqif;
            using (var input = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                reqif = deserializer.Deserialize(input, SupportedFileExtensionKind.Reqif).First();
            }

            using var output = new MemoryStream();
            serializer.Serialize(new[] { reqif }, output, SupportedFileExtensionKind.Reqif);

            var attributes = ExtractRootAttributes(output);

            Assert.Multiple(() =>
            {
                Assert.That(attributes["xmlns:xsi"], Is.EqualTo(XsiNamespace), "the xsi namespace declaration was not retained");
                Assert.That(attributes["xsi:schemaLocation"], Is.EqualTo(schemaLocation), "the xsi:schemaLocation attribute was not retained");
            });
        }

        [Test]
        public void Verify_that_repeated_serialization_does_not_duplicate_namespace_declarations()
        {
            var deserializer = new ReqIFDeserializer();
            var serializer = new ReqIFSerializer();

            var reqif = deserializer.Deserialize(this.datatypeDemoPath).First();

            // serialize twice from the same object; the second pass must not accumulate extra declarations
            using (var first = new MemoryStream())
            {
                serializer.Serialize(new[] { reqif }, first, SupportedFileExtensionKind.Reqif);
            }

            using var second = new MemoryStream();
            serializer.Serialize(new[] { reqif }, second, SupportedFileExtensionKind.Reqif);

            var attributes = ExtractRootAttributes(second);

            Assert.That(attributes["xmlns:xhtml"], Is.EqualTo("http://www.w3.org/1999/xhtml"));
        }

        /// <summary>
        /// Reads the attributes declared on the <c>REQ-IF</c> root element of a serialized document, keyed by
        /// their qualified name (e.g. <c>xmlns</c>, <c>xmlns:configuration</c>, <c>xsi:schemaLocation</c>).
        /// </summary>
        private static IDictionary<string, string> ExtractRootAttributes(Stream stream)
        {
            stream.Position = 0;

            var attributes = new Dictionary<string, string>();

            using var reader = XmlReader.Create(stream);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "REQ-IF")
                {
                    if (reader.MoveToFirstAttribute())
                    {
                        do
                        {
                            attributes[reader.Name] = reader.Value;
                        }
                        while (reader.MoveToNextAttribute());
                    }

                    break;
                }
            }

            return attributes;
        }
    }
}
