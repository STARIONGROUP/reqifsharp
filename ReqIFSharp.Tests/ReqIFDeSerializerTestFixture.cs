// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFDeSerializerTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
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

using System.Threading.Tasks;

namespace ReqIFSharp.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Schema;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIFDeSerializer"/>
    /// </summary>
    [TestFixture]
    public class ReqIFDeSerializerTestFixture
    {
        private const int AmountOfDataTypes = 7;
        private const int AmountOfSpecTypes = 4;
        private const int AmountOfSpecObjects = 3;
        private const int AmountOfSpecRelations = 2;
        private const int AmountOfSpecifications = 2;
        private const int AmountOfSpecificationChildren = 1;
        private const int AmountOfSpecificationSubChildren = 2;
        private const int AmountOfSpecRelationGroups = 2;

        [Test]
        public void Verify_That_A_ReqIF_XML_Document_Can_Be_Deserialized_Without_Validation()
        {
            var deserializer = new ReqIFDeserializer();
            var reqIf = deserializer.Deserialize(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "output.reqif")).First();

            Assert.AreEqual("en", reqIf.Lang);

            var firstobject = reqIf.CoreContent.SpecObjects.First();
            var xhtmlAttribute = firstobject.Values.OfType<AttributeValueXHTML>().SingleOrDefault();
            Assert.IsNotNull(xhtmlAttribute);
            Assert.IsNotEmpty(xhtmlAttribute.TheValue);
            Assert.IsNotNull(xhtmlAttribute.AttributeDefinition);

            Assert.AreEqual(AmountOfDataTypes, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(AmountOfSpecTypes, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(AmountOfSpecObjects, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(AmountOfSpecRelations, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(AmountOfSpecifications, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(AmountOfSpecificationChildren, reqIf.CoreContent.Specifications[0].Children.Count);
            Assert.AreEqual(AmountOfSpecificationSubChildren, reqIf.CoreContent.Specifications[0].Children[0].Children.Count);
            Assert.AreEqual(AmountOfSpecRelationGroups, reqIf.CoreContent.SpecRelationGroups.Count);

            var unknownSpecRel = reqIf.CoreContent.SpecRelations.First(x => x.Identifier == "specobject_1-unknown");
            Assert.IsNotNull(unknownSpecRel.Target);
            Assert.AreEqual("unknown-specobject", unknownSpecRel.Target.Identifier);

            var unknownrelGroup = reqIf.CoreContent.SpecRelationGroups.First(x => x.Identifier == "relationgroup-no-target");
            Assert.AreEqual("unknown", unknownrelGroup.TargetSpecification.Identifier);
        }

        [Test]
        public async Task Verify_That_A_ReqIF_XML_Document_Can_Be_DeserializedAsync_Without_Validation()
        {
            var deserializer = new ReqIFDeserializer();
            var reqIf = (await deserializer.DeserializeAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "output.reqif"))).First();

            Assert.AreEqual("en", reqIf.Lang);

            var firstobject = reqIf.CoreContent.SpecObjects.First();
            var xhtmlAttribute = firstobject.Values.OfType<AttributeValueXHTML>().SingleOrDefault();
            Assert.IsNotNull(xhtmlAttribute);
            Assert.IsNotEmpty(xhtmlAttribute.TheValue);
            Assert.IsNotNull(xhtmlAttribute.AttributeDefinition);

            Assert.AreEqual(AmountOfDataTypes, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(AmountOfSpecTypes, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(AmountOfSpecObjects, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(AmountOfSpecRelations, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(AmountOfSpecifications, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(AmountOfSpecificationChildren, reqIf.CoreContent.Specifications[0].Children.Count);
            Assert.AreEqual(AmountOfSpecificationSubChildren, reqIf.CoreContent.Specifications[0].Children[0].Children.Count);
            Assert.AreEqual(AmountOfSpecRelationGroups, reqIf.CoreContent.SpecRelationGroups.Count);

            var unknownSpecRel = reqIf.CoreContent.SpecRelations.First(x => x.Identifier == "specobject_1-unknown");
            Assert.IsNotNull(unknownSpecRel.Target);
            Assert.AreEqual("unknown-specobject", unknownSpecRel.Target.Identifier);

            var unknownrelGroup = reqIf.CoreContent.SpecRelationGroups.First(x => x.Identifier == "relationgroup-no-target");
            Assert.AreEqual("unknown", unknownrelGroup.TargetSpecification.Identifier);
        }

        [Test]
        public void Verify_that_the_Tool_Extensions_are_Deserialized_from_ProR_Traceability_template()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");
            var deserializer = new ReqIFDeserializer();

            var reqIf = deserializer.Deserialize(reqifPath).First() ;

            Assert.That(reqIf.TheHeader.Identifier, Is.EqualTo("_o7scMadbEeafNduaIhMwQg"));
            Assert.That(reqIf.TheHeader.Comment, Is.EqualTo("Download this template and others at https://reqif.academy"));
            Assert.That(reqIf.TheHeader.ReqIFToolId, Is.EqualTo("fmStudio (http://formalmind.com/studio)"));
            Assert.That(reqIf.TheHeader.ReqIFVersion, Is.EqualTo("1.0"));
            Assert.That(reqIf.TheHeader.SourceToolId, Is.EqualTo("fmStudio (http://formalmind.com/studio)"));
            Assert.That(reqIf.TheHeader.Title, Is.EqualTo("Traceability Template"));

            Assert.That(reqIf.CoreContent.DocumentRoot, Is.EqualTo(reqIf));

            Assert.That(reqIf.CoreContent.DataTypes.Count, Is.EqualTo(8));
            Assert.That(reqIf.CoreContent.DataTypes.First().ReqIFContent, Is.EqualTo(reqIf.CoreContent));

            Assert.That(reqIf.CoreContent.SpecTypes.Count, Is.EqualTo(3));
            Assert.That(reqIf.CoreContent.SpecTypes.First().ReqIFContent, Is.EqualTo(reqIf.CoreContent));

            Assert.That(reqIf.CoreContent.SpecObjects.Count, Is.EqualTo(21));
            Assert.That(reqIf.CoreContent.SpecObjects.First().ReqIFContent, Is.EqualTo(reqIf.CoreContent));

            Assert.That(reqIf.CoreContent.SpecRelations.Count, Is.EqualTo(9));
            Assert.That(reqIf.CoreContent.SpecRelations.First().ReqIFContent, Is.EqualTo(reqIf.CoreContent));

            Assert.That(reqIf.CoreContent.Specifications.Count, Is.EqualTo(2));
            Assert.That(reqIf.CoreContent.Specifications.First().ReqIFContent, Is.EqualTo(reqIf.CoreContent));

            Assert.That(reqIf.ToolExtension.Count, Is.EqualTo(1));

            var toolExtension = reqIf.ToolExtension.First();
            Assert.That(toolExtension.InnerXml, Is.Not.Empty);
        }

        [Test]
        public async Task Verify_that_the_Tool_Extensions_are_DeserializedAsync_from_ProR_Traceability_template()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");
            var deserializer = new ReqIFDeserializer();

            var reqIf = (await deserializer.DeserializeAsync(reqifPath)).First();

            Assert.That(reqIf.TheHeader.Identifier, Is.EqualTo("_o7scMadbEeafNduaIhMwQg"));
            Assert.That(reqIf.TheHeader.Comment, Is.EqualTo("Download this template and others at https://reqif.academy"));
            Assert.That(reqIf.TheHeader.ReqIFToolId, Is.EqualTo("fmStudio (http://formalmind.com/studio)"));
            Assert.That(reqIf.TheHeader.ReqIFVersion, Is.EqualTo("1.0"));
            Assert.That(reqIf.TheHeader.SourceToolId, Is.EqualTo("fmStudio (http://formalmind.com/studio)"));
            Assert.That(reqIf.TheHeader.Title, Is.EqualTo("Traceability Template"));

            Assert.That(reqIf.CoreContent.DocumentRoot, Is.EqualTo(reqIf));

            Assert.That(reqIf.CoreContent.DataTypes.Count, Is.EqualTo(8));
            Assert.That(reqIf.CoreContent.DataTypes.First().ReqIFContent, Is.EqualTo(reqIf.CoreContent));

            Assert.That(reqIf.CoreContent.SpecTypes.Count, Is.EqualTo(3));
            Assert.That(reqIf.CoreContent.SpecTypes.First().ReqIFContent, Is.EqualTo(reqIf.CoreContent));

            Assert.That(reqIf.CoreContent.SpecObjects.Count, Is.EqualTo(21));
            Assert.That(reqIf.CoreContent.SpecObjects.First().ReqIFContent, Is.EqualTo(reqIf.CoreContent));

            Assert.That(reqIf.CoreContent.SpecRelations.Count, Is.EqualTo(9));
            Assert.That(reqIf.CoreContent.SpecRelations.First().ReqIFContent, Is.EqualTo(reqIf.CoreContent));

            Assert.That(reqIf.CoreContent.Specifications.Count, Is.EqualTo(2));
            Assert.That(reqIf.CoreContent.Specifications.First().ReqIFContent, Is.EqualTo(reqIf.CoreContent));

            Assert.That(reqIf.ToolExtension.Count, Is.EqualTo(1));

            var toolExtension = reqIf.ToolExtension.First();
            Assert.That(toolExtension.InnerXml, Is.Not.Empty);
        }

        [Test]
        public void Verify_that_ExternalObjects_are_created_and_that_local_data_can_be_queried()
        {
            var sw = Stopwatch.StartNew();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "requirements-and-objects.reqifz");
            var deserializer = new ReqIFDeserializer();

            var reqIf = deserializer.Deserialize(reqifPath).First();

            foreach (var specObject in reqIf.CoreContent.SpecObjects)
            {
                foreach (var attributeValueXhtml in specObject.Values.OfType<AttributeValueXHTML>())
                {
                    foreach (var externalObject in attributeValueXhtml.ExternalObjects)
                    {
                        var targetStream = new MemoryStream();
                        externalObject.QueryLocalData(reqifPath, targetStream);
                        Assert.That(targetStream.Length, Is.GreaterThan(0));
                        targetStream.Dispose();
                    }
                }
            }

            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [Test]
        public async Task Verify_that_ExternalObjects_are_created_and_that_local_data_can_be_queried_async()
        {
            var sw = Stopwatch.StartNew();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "requirements-and-objects.reqifz");
            var deserializer = new ReqIFDeserializer();

            var reqIf = (await deserializer.DeserializeAsync(reqifPath)).First();

            foreach (var specObject in reqIf.CoreContent.SpecObjects)
            {
                foreach (var attributeValueXhtml in specObject.Values.OfType<AttributeValueXHTML>())
                {
                    foreach (var externalObject in attributeValueXhtml.ExternalObjects)
                    {
                        var targetStream = new MemoryStream();
                        externalObject.QueryLocalData(reqifPath, targetStream);
                        Assert.That(targetStream.Length, Is.GreaterThan(0));
                        await targetStream.DisposeAsync();
                    }
                }
            }

            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        [Test]
        public void Verify_That_A_ReqIF_XML_Document_Can_Be_Deserialized_With_Validation()
        {
            var deserializer = new ReqIFDeserializer();
            var reqIf = deserializer.Deserialize(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "output.reqif"), true, this.ValidationEventHandler).First();

            Assert.AreEqual("en", reqIf.Lang);

            Assert.AreEqual(AmountOfDataTypes, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(AmountOfSpecTypes, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(AmountOfSpecObjects, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(AmountOfSpecRelations, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(AmountOfSpecifications, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(AmountOfSpecRelationGroups, reqIf.CoreContent.SpecRelationGroups.Count);
        }

        [Test]
        public async Task Verify_That_A_ReqIF_XML_Document_Can_Be_DeserializedAsync_With_Validation()
        {
            var deserializer = new ReqIFDeserializer();
            var reqIf = (await deserializer.DeserializeAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "output.reqif"), true, this.ValidationEventHandler)).First();

            Assert.AreEqual("en", reqIf.Lang);

            Assert.AreEqual(AmountOfDataTypes, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(AmountOfSpecTypes, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(AmountOfSpecObjects, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(AmountOfSpecRelations, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(AmountOfSpecifications, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(AmountOfSpecRelationGroups, reqIf.CoreContent.SpecRelationGroups.Count);
        }

        [Test]
        public void Verify_That_when_schema_is_not_available_A_ReqIF_file_Can_Be_Deserialized_With_Validation_and_that_schemavalidationexception_is_thrown()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            var deserializer = new ReqIFDeserializer();
            
            Assert.That(() => deserializer.Deserialize(reqifPath, true, this.ValidationEventHandler), Throws.Exception.TypeOf<XmlSchemaValidationException>());
        }

        [Test]
        public void Verify_That_when_schema_is_not_available_A_ReqIF_file_Can_Be_DeserializedAsync_With_Validation_and_that_schemavalidationexception_is_thrown()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            var deserializer = new ReqIFDeserializer();

            Assert.That(async () => await deserializer.DeserializeAsync(reqifPath, true, this.ValidationEventHandler), Throws.Exception.TypeOf<XmlSchemaValidationException>());
        }

        [Test]
        public void Verify_that_XHTML_attributes_can_de_Deserialized()
        {
            var deserializer = new ReqIFDeserializer();
            var reqIf = deserializer.Deserialize(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "testreqif.reqif")).First();

            Assert.AreEqual("en", reqIf.Lang);

            var specObject = reqIf.CoreContent.SpecObjects.Single(r => r.Identifier == "R001");

            var xhtmlValue = specObject.Values.Single(x => x.AttributeDefinition.Identifier == "FUNC-REQ-NOTES") as AttributeValueXHTML;

            Assert.That(xhtmlValue.TheValue, Is.Not.Null.Or.Empty);
        }

        [Test]
        public async Task Verify_that_XHTML_attributes_can_de_DeserializedAsync()
        {
            var deserializer = new ReqIFDeserializer();
            var reqIf = (await deserializer.DeserializeAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "testreqif.reqif"))).First();

            Assert.AreEqual("en", reqIf.Lang);

            var specObject = reqIf.CoreContent.SpecObjects.Single(r => r.Identifier == "R001");

            var xhtmlValue = specObject.Values.Single(x => x.AttributeDefinition.Identifier == "FUNC-REQ-NOTES") as AttributeValueXHTML;

            Assert.That(xhtmlValue.TheValue, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void Verify_That_A_ReqIF_Archive_Can_Be_Deserialized_Without_Validation()
        {
            var deserializer = new ReqIFDeserializer();
            var reqIf = deserializer.Deserialize(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz")).First();

            Assert.AreEqual("en", reqIf.Lang);

            var firstobject = reqIf.CoreContent.SpecObjects.First();
            var xhtmlAttribute = firstobject.Values.OfType<AttributeValueXHTML>().SingleOrDefault();
            Assert.IsNotNull(xhtmlAttribute);
            Assert.IsNotEmpty(xhtmlAttribute.TheValue);
            Assert.IsNotNull(xhtmlAttribute.AttributeDefinition);

            Assert.AreEqual(AmountOfDataTypes, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(AmountOfSpecTypes, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(AmountOfSpecObjects, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(1, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(AmountOfSpecifications, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(AmountOfSpecificationChildren, reqIf.CoreContent.Specifications[0].Children.Count);
            Assert.AreEqual(AmountOfSpecificationSubChildren, reqIf.CoreContent.Specifications[0].Children[0].Children.Count);
            Assert.AreEqual(1, reqIf.CoreContent.SpecRelationGroups.Count);
        }

        [Test]
        public async Task Verify_That_A_ReqIF_Archive_Can_Be_DeserializedAsync_Without_Validation()
        {
            var deserializer = new ReqIFDeserializer();
            var reqIf = (await deserializer.DeserializeAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz"))).First();

            Assert.AreEqual("en", reqIf.Lang);

            var firstobject = reqIf.CoreContent.SpecObjects.First();
            var xhtmlAttribute = firstobject.Values.OfType<AttributeValueXHTML>().SingleOrDefault();
            Assert.IsNotNull(xhtmlAttribute);
            Assert.IsNotEmpty(xhtmlAttribute.TheValue);
            Assert.IsNotNull(xhtmlAttribute.AttributeDefinition);

            Assert.AreEqual(AmountOfDataTypes, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(AmountOfSpecTypes, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(AmountOfSpecObjects, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(1, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(AmountOfSpecifications, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(AmountOfSpecificationChildren, reqIf.CoreContent.Specifications[0].Children.Count);
            Assert.AreEqual(AmountOfSpecificationSubChildren, reqIf.CoreContent.Specifications[0].Children[0].Children.Count);
            Assert.AreEqual(1, reqIf.CoreContent.SpecRelationGroups.Count);
        }

        [Test]
        public void Verify_That_A_ReqIF_Archive_Can_Be_Deserialized_With_Validation()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz");

            var deserializer = new ReqIFDeserializer();
            Assert.That(() => deserializer.Deserialize(reqifPath, true, this.ValidationEventHandler), Throws.Nothing);

            using (var sourceStream = new FileStream(reqifPath, FileMode.Open))
            {
                Assert.That(() => deserializer.Deserialize(sourceStream), Throws.Nothing);
            }
        }

        [Test]
        public void Verify_That_A_ReqIF_Archive_Can_Be_DeserializedAsync_With_Validation()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz");

            var deserializer = new ReqIFDeserializer();
            Assert.That(async () => await deserializer.DeserializeAsync(reqifPath, true, this.ValidationEventHandler), Throws.Nothing);

            using (var sourceStream = new FileStream(reqifPath, FileMode.Open))
            {
                Assert.That(async () => await deserializer.DeserializeAsync(sourceStream), Throws.Nothing);
            }
        }

        [Test]
        public void Verify_that_when_path_is_null_exception_is_thrown()
        {
            var deserializer = new ReqIFDeserializer();

            string xmlPath = null;

            Assert.That(
                () => deserializer.Deserialize(xmlPath),
                Throws.Exception.TypeOf<ArgumentException>()
                    .With.Message.Contains("The xml file path may not be null or empty"));
        }

        [Test]
        public void Verify_that_when_ValidationEventHandler_is_not_null_exception_is_thrown()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz");

            var deserializer = new ReqIFDeserializer();

            Assert.That(
                () => deserializer.Deserialize(path, false, this.ValidationEventHandler),
                Throws.Exception.TypeOf<ArgumentException>()
                    .With.Message.Contains("validationEventHandler must be null when validate is false"));
        }

        [Test]
        public void Verify_that_when_ValidationEventHandler_is_not_null_exception_is_thrown_async()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz");

            var deserializer = new ReqIFDeserializer();

            Assert.That(
                async () => await deserializer.DeserializeAsync(path, false, this.ValidationEventHandler),
                Throws.Exception.TypeOf<ArgumentException>()
                    .With.Message.Contains("validationEventHandler must be null when validate is false"));
        }

        /// <summary>
        /// Validation Event Handler
        /// </summary>
        /// <param name="sender">
        /// The sender of the event
        /// </param>
        /// <param name="validationEventArgs">
        /// The event handler arguments
        /// </param>
        private void ValidationEventHandler(object sender, ValidationEventArgs validationEventArgs)
        {
            throw validationEventArgs.Exception;
        }

        [Test]
        public void Verify_that_ReqIF_can_be_Deserializaed_from_stream()
        {
            var deserializer = new ReqIFDeserializer();

            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz");

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                var sw = Stopwatch.StartNew();

                var reqif = deserializer.Deserialize(fileStream, false, null).First();

                Console.WriteLine($"deserialization done in: {sw.ElapsedMilliseconds} [ms]");

                Assert.That(reqif, Is.Not.Null);
            }
        }

        [Test]
        public async Task Verify_that_ReqIF_can_be_DeserializaedAsync_from_stream()
        {
            var deserializer = new ReqIFDeserializer();

            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz");

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                var sw = Stopwatch.StartNew();

                var reqif = (await deserializer.DeserializeAsync(fileStream, false, null)).First();

                Console.WriteLine($"async deserialization done in: {sw.ElapsedMilliseconds} [ms]");

                Assert.That(reqif, Is.Not.Null);
            }
        }
    }
}
