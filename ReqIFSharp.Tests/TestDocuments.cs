// -------------------------------------------------------------------------------------------------
// <copyright file="TestDocuments.cs" company="Starion Group S.A.">
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
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Helpers for tests that read a modified copy of a test document and compare it with the original.
    /// </summary>
    internal static class TestDocuments
    {
        /// <summary>
        /// The ReqIF namespace
        /// </summary>
        public static readonly XNamespace ReqIFNamespace = DefaultXmlAttributeFactory.ReqIFSchemaUri;

        /// <summary>
        /// Test documents whose object graphs can be serialized again, so they can be compared.
        /// output.reqif and testreqif.reqif are left out: they contain unresolved type references.
        /// </summary>
        public static readonly string[] ComparableFiles =
        {
            "Datatype-Demo.reqif",
            "DefaultValueDemo.reqif",
            "ProR_Traceability-Template-v1.0.reqif",
            "reqifsharpgenerated.reqif"
        };

        /// <summary>
        /// Both file kinds the deserializer reads
        /// </summary>
        public static readonly SupportedFileExtensionKind[] Kinds = { SupportedFileExtensionKind.Reqif, SupportedFileExtensionKind.Reqifz };

        /// <summary>
        /// Reads a file from the TestData folder
        /// </summary>
        public static string Read(string fileName)
        {
            return File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", fileName));
        }

        /// <summary>
        /// Returns the XML as a stream, either as is or as the only entry of a reqifz archive
        /// </summary>
        public static MemoryStream ToStream(string xml, SupportedFileExtensionKind kind)
        {
            var bytes = Encoding.UTF8.GetBytes(xml);

            if (kind == SupportedFileExtensionKind.Reqif)
            {
                return new MemoryStream(bytes);
            }

            var zipStream = new MemoryStream();

            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                using var entryStream = archive.CreateEntry("document.reqif").Open();
                entryStream.Write(bytes, 0, bytes.Length);
            }

            zipStream.Position = 0;
            return zipStream;
        }

        /// <summary>
        /// Returns a copy of the document without the whitespace between its ReqIF elements. Whitespace inside
        /// XHTML values (THE-VALUE, THE-ORIGINAL-VALUE) and tool extensions is kept: it is content, read as raw XML.
        /// </summary>
        public static string Minify(string original)
        {
            var document = XDocument.Parse(original, LoadOptions.PreserveWhitespace);

            var whitespace = document.DescendantNodes()
                .OfType<XText>()
                .Where(t => string.IsNullOrWhiteSpace(t.Value) && !IsRawXmlContent(t))
                .ToList();

            Assert.That(whitespace, Is.Not.Empty, "the original must contain whitespace between elements");

            whitespace.ForEach(t => t.Remove());

            return document.ToString(SaveOptions.DisableFormatting);
        }

        private static bool IsRawXmlContent(XText text)
        {
            return text.Ancestors().Any(a =>
                a.Name.Namespace != ReqIFNamespace
                || a.Name.LocalName == "THE-VALUE"
                || a.Name.LocalName == "THE-ORIGINAL-VALUE"
                || a.Name.LocalName == "REQ-IF-TOOL-EXTENSION");
        }

        /// <summary>
        /// Returns a copy of the document in which every ReqIF element is written as reqif:...
        /// </summary>
        public static string Prefix(string original)
        {
            // keep the original whitespace, so that the copy differs from the original only in its prefixes
            var document = XDocument.Parse(original, LoadOptions.PreserveWhitespace);

            RemoveReqIFNamespaceDeclarations(document);

            document.Root!.SetAttributeValue(XNamespace.Xmlns + "reqif", ReqIFNamespace.NamespaceName);

            var prefixed = document.ToString(SaveOptions.DisableFormatting);

            Assert.That(prefixed, Does.Contain("<reqif:REQ-IF "), "the copy must use the reqif prefix");
            Assert.That(prefixed, Does.Not.Contain("<REQ-IF "), "the copy must not use the default namespace for ReqIF elements");

            return prefixed;
        }

        /// <summary>
        /// Returns a copy of the document in which the ReqIF elements are in no namespace
        /// </summary>
        public static string RemoveNamespace(string original)
        {
            var document = XDocument.Parse(original, LoadOptions.PreserveWhitespace);

            RemoveReqIFNamespaceDeclarations(document);

            foreach (var element in document.Descendants().Where(e => e.Name.Namespace == ReqIFNamespace))
            {
                element.Name = XNamespace.None + element.Name.LocalName;
            }

            var withoutNamespace = document.ToString(SaveOptions.DisableFormatting);

            Assert.That(withoutNamespace, Does.Contain("<REQ-IF "), "the copy must contain an unprefixed root");
            Assert.That(XDocument.Parse(withoutNamespace).Descendants().Any(e => e.Name.Namespace == ReqIFNamespace), Is.False, "the copy must not contain elements in the ReqIF namespace");

            return withoutNamespace;
        }

        private static void RemoveReqIFNamespaceDeclarations(XDocument document)
        {
            foreach (var element in document.Descendants())
            {
                element.Attributes()
                    .Where(a => a.IsNamespaceDeclaration && a.Value == ReqIFNamespace.NamespaceName)
                    .Remove();
            }
        }

        /// <summary>
        /// Compares two object graphs by serializing both and comparing the XML, ignoring namespace declarations
        /// (which differ by design between an original and some of its copies).
        /// </summary>
        public static void AssertSameGraph(IReadOnlyList<ReqIF> expected, IReadOnlyList<ReqIF> actual)
        {
            Assert.That(expected, Has.Count.EqualTo(1));
            Assert.That(actual, Has.Count.EqualTo(expected.Count), "the copy must be read as the same number of documents");
            Assert.That(actual[0].CoreContent.SpecObjects, Has.Count.EqualTo(expected[0].CoreContent.SpecObjects.Count));

            var expectedXml = Normalize(expected);
            var actualXml = Normalize(actual);

            Assert.That(actualXml.ToString(), Is.EqualTo(expectedXml.ToString()));
        }

        private static XDocument Normalize(IEnumerable<ReqIF> reqifs)
        {
            using var stream = new MemoryStream();

            new ReqIFSerializer().Serialize(reqifs, stream, SupportedFileExtensionKind.Reqif);

            stream.Position = 0;

            var document = XDocument.Load(stream);

            foreach (var element in document.Descendants())
            {
                element.Attributes().Where(a => a.IsNamespaceDeclaration).Remove();
            }

            return document;
        }
    }
}
