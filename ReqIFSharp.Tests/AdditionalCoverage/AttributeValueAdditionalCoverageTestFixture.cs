// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueAdditionalCoverageTestFixture.cs" company="Starion Group S.A.">
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
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Additional attribute value tests that focus on asynchronous deserialisation branches.
    /// </summary>
    [TestFixture]
    public class AttributeValueAdditionalCoverageTestFixture
    {
        [Test]
        public async Task AttributeValueEnumeration_ReadXmlAsync_ResolvesDefinitions()
        {
            var content = new ReqIFContent();
            var specType = new SpecObjectType(content, null) { Identifier = "spec-type" };

            var enumDatatype = new DatatypeDefinitionEnumeration(content, null) { Identifier = "enum-type" };
            var enumValue = new EnumValue(enumDatatype, null) { Identifier = "enum-value" };
            _ = new EmbeddedValue(enumValue, null) { Key = 1, OtherContent = "green" };

            var attributeDefinition = new AttributeDefinitionEnumeration(specType, null)
            {
                Identifier = "enum-def",
                Type = enumDatatype
            };

            var specObject = new SpecObject(content, null)
            {
                Identifier = "spec-object",
                SpecType = specType
            };

            var attributeValue = new AttributeValueEnumeration(specObject, null);

            var xml = "<ATTRIBUTE-VALUE-ENUMERATION>"
                      + "<ATTRIBUTE-DEFINITION-ENUMERATION-REF>enum-def</ATTRIBUTE-DEFINITION-ENUMERATION-REF>"
                      + "<ENUM-VALUE-REF>enum-value</ENUM-VALUE-REF>"
                      + "</ATTRIBUTE-VALUE-ENUMERATION>";

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            await attributeValue.ReadXmlAsync(reader, CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(attributeValue.Definition, Is.SameAs(attributeDefinition));
                Assert.That(attributeValue.Values.Single(), Is.SameAs(enumValue));
            });
        }

        [Test]
        public async Task AttributeValueInteger_ReadXmlAsync_ResolvesDefinitionAndValue()
        {
            var content = new ReqIFContent();
            var specType = new SpecObjectType(content, null) { Identifier = "spec-type" };

            var integerDatatype = new DatatypeDefinitionInteger(content, null) { Identifier = "int-type" };
            var attributeDefinition = new AttributeDefinitionInteger(specType, null)
            {
                Identifier = "int-def",
                Type = integerDatatype
            };

            var specObject = new SpecObject(content, null)
            {
                Identifier = "spec-object",
                SpecType = specType
            };

            var attributeValue = new AttributeValueInteger(specObject, null);

            var xml = "<ATTRIBUTE-VALUE-INTEGER THE-VALUE=\"42\">"
                      + "<ATTRIBUTE-DEFINITION-INTEGER-REF>int-def</ATTRIBUTE-DEFINITION-INTEGER-REF>"
                      + "</ATTRIBUTE-VALUE-INTEGER>";

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            await attributeValue.ReadXmlAsync(reader, CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(attributeValue.Definition, Is.SameAs(attributeDefinition));
                Assert.That(attributeValue.TheValue, Is.EqualTo(42));
            });
        }

        [Test]
        public async Task AttributeValueReal_ReadXmlAsync_ResolvesDefinition()
        {
            var content = new ReqIFContent();
            var specType = new SpecObjectType(content, null) { Identifier = "spec-type" };

            var realDatatype = new DatatypeDefinitionReal(content, null) { Identifier = "real-type" };
            var attributeDefinition = new AttributeDefinitionReal(specType, null)
            {
                Identifier = "real-def",
                Type = realDatatype
            };

            var specObject = new SpecObject(content, null)
            {
                Identifier = "spec-object",
                SpecType = specType
            };

            var attributeValue = new AttributeValueReal(specObject, null);

            var xml = "<ATTRIBUTE-VALUE-REAL THE-VALUE=\"4.2\">"
                      + "<ATTRIBUTE-DEFINITION-REAL-REF>real-def</ATTRIBUTE-DEFINITION-REAL-REF>"
                      + "</ATTRIBUTE-VALUE-REAL>";

            using var reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings { Async = true });
            await reader.MoveToContentAsync();

            await attributeValue.ReadXmlAsync(reader, CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(attributeValue.Definition, Is.SameAs(attributeDefinition));
                Assert.That(attributeValue.TheValue, Is.EqualTo(4.2d));
            });
        }
    }
}
