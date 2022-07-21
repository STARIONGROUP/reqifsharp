// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFDeSerializerTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2022 RHEA System S.A.
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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Schema;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIFDeSerializer"/>
    /// </summary>
    [TestFixture]
    public class ReqIFDeSerializerTestFixture
    {
        private ILoggerFactory loggerFactory;

        [SetUp]
        public void SetUp()
        {
            this.loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        }
        
        [Test]
        public void Verify_that_ArgumentException_Are_thrown_on_read_from_file()
        {
            var deserializer = new ReqIFDeserializer();

            Assert.That(() => deserializer.Deserialize(null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                    .With.Message.Contains("The path of the ReqIF file cannot be null."));

            Assert.That(() => deserializer.Deserialize(""), 
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains("The path of the ReqIF file cannot be empty."));
        }

        [Test]
        public void Verify_that_ArgumentException_Are_thrown_on_read_from_file_async()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var deserializer = new ReqIFDeserializer();

            Assert.That(async () => await deserializer.DeserializeAsync(null, cancellationTokenSource.Token),
                Throws.Exception.TypeOf<ArgumentNullException>()
                    .With.Message.Contains("The path of the ReqIF file cannot be null."));

            Assert.That(async () => await deserializer.DeserializeAsync("", cancellationTokenSource.Token),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains("The path of the ReqIF file cannot be empty."));
        }

        [Test]
        public void Verify_that_ArgumentException_Are_thrown_on_read_from_stream()
        {
            var deserializer = new ReqIFDeserializer();

            MemoryStream memoryStream = null;

            Assert.That(() => deserializer.Deserialize(memoryStream, SupportedFileExtensionKind.Reqif),
                Throws.Exception.TypeOf<ArgumentNullException>());

            var validationEventHandler = new ValidationEventHandler(ValidationEventHandler);

            Assert.That(() => deserializer.Deserialize(memoryStream, SupportedFileExtensionKind.Reqif, false, validationEventHandler),
                Throws.Exception.TypeOf<ArgumentException>()
                    .With.Message.Contains("validationEventHandler must be null when validate is false"));

            memoryStream = new MemoryStream();

            Assert.That(() => deserializer.Deserialize(memoryStream, SupportedFileExtensionKind.Reqif),
                Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Verify_that_ArgumentException_Are_thrown_on_read_from_stream_async()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var deserializer = new ReqIFDeserializer();

            MemoryStream memoryStream = null;

            Assert.That(async () => await deserializer.DeserializeAsync(memoryStream, SupportedFileExtensionKind.Reqif, cancellationTokenSource.Token),
                Throws.Exception.TypeOf<ArgumentNullException>());

            var validationEventHandler = new ValidationEventHandler(ValidationEventHandler);

            Assert.That(async () => await deserializer.DeserializeAsync(memoryStream, SupportedFileExtensionKind.Reqif, cancellationTokenSource.Token, false, validationEventHandler),
                Throws.Exception.TypeOf<ArgumentException>()
                    .With.Message.Contains("validationEventHandler must be null when validate is false"));

            memoryStream = new MemoryStream();

            Assert.That(async () => await deserializer.DeserializeAsync(memoryStream, SupportedFileExtensionKind.Reqif, cancellationTokenSource.Token),
                Throws.Exception.TypeOf<ArgumentException>());
        }
        
        [Test]
        public void Verify_That_A_ReqIF_Document_Can_Be_Deserialized_Without_Validation()
        {
            var deserializer = new ReqIFDeserializer(this.loggerFactory);
            var reqIf = deserializer.Deserialize(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "output.reqif")).First();

            Assert.AreEqual("en", reqIf.Lang);

            var firstobject = reqIf.CoreContent.SpecObjects.First();
            var xhtmlAttribute = firstobject.Values.OfType<AttributeValueXHTML>().SingleOrDefault();
            Assert.IsNotNull(xhtmlAttribute);
            Assert.IsNotEmpty(xhtmlAttribute.TheValue);
            Assert.IsNotNull(xhtmlAttribute.AttributeDefinition);

            Assert.AreEqual(7, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(4, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(4, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(2, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(3, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(1, reqIf.CoreContent.Specifications[0].Children.Count);
            Assert.AreEqual(2, reqIf.CoreContent.Specifications[0].Children[0].Children.Count);
            Assert.AreEqual(2, reqIf.CoreContent.SpecRelationGroups.Count);

            Assert.That(reqIf.TheHeader.DocumentRoot, Is.EqualTo(reqIf));

            var unknownSpecRel = reqIf.CoreContent.SpecRelations.First(x => x.Identifier == "specobject_1-unknown");
            Assert.IsNotNull(unknownSpecRel.Target);
            Assert.AreEqual("unknown-specobject", unknownSpecRel.Target.Identifier);

            var unknownrelGroup = reqIf.CoreContent.SpecRelationGroups.First(x => x.Identifier == "relationgroup-no-target");
            Assert.AreEqual("unknown", unknownrelGroup.TargetSpecification.Identifier);
        }

        [Test]
        public async Task Verify_That_A_ReqIF_Document_Can_Be_DeserializedAsync_Without_Validation()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var deserializer = new ReqIFDeserializer();
            var reqIf = (await deserializer.DeserializeAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "output.reqif"), cancellationTokenSource.Token)).First();

            Assert.AreEqual("en", reqIf.Lang);

            var firstobject = reqIf.CoreContent.SpecObjects.First();
            var xhtmlAttribute = firstobject.Values.OfType<AttributeValueXHTML>().SingleOrDefault();
            Assert.IsNotNull(xhtmlAttribute);
            Assert.IsNotEmpty(xhtmlAttribute.TheValue);
            Assert.IsNotNull(xhtmlAttribute.AttributeDefinition);

            Assert.AreEqual(7, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(4, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(4, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(2, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(3, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(1, reqIf.CoreContent.Specifications[0].Children.Count);
            Assert.AreEqual(2, reqIf.CoreContent.Specifications[0].Children[0].Children.Count);
            Assert.AreEqual(2, reqIf.CoreContent.SpecRelationGroups.Count);

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
            var sw = Stopwatch.StartNew();

            var cancellationTokenSource = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");
            var deserializer = new ReqIFDeserializer();

            var reqIf = (await deserializer.DeserializeAsync(reqifPath, cancellationTokenSource.Token)).First();

            Console.WriteLine(sw.ElapsedMilliseconds);

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

            var cancellationTokenSource = new CancellationTokenSource();

            var reqIf = (await deserializer.DeserializeAsync(reqifPath, cancellationTokenSource.Token)).First();

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

            Assert.AreEqual(7, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(4, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(4, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(2, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(3, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(2, reqIf.CoreContent.SpecRelationGroups.Count);
        }

        [Test]
        public async Task Verify_That_A_ReqIF_Document_Can_Be_DeserializedAsync_With_Validation()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var deserializer = new ReqIFDeserializer();
            var reqIf = (await deserializer.DeserializeAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "output.reqif"), cancellationTokenSource.Token,true, this.ValidationEventHandler)).First();

            Assert.AreEqual("en", reqIf.Lang);

            Assert.AreEqual(7, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(4, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(4, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(2, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(3, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(2, reqIf.CoreContent.SpecRelationGroups.Count);
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
            var cancellationTokenSource = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");

            var deserializer = new ReqIFDeserializer();

            Assert.That(async () => await deserializer.DeserializeAsync(reqifPath, cancellationTokenSource.Token, true, this.ValidationEventHandler), Throws.Exception.TypeOf<XmlSchemaValidationException>());
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
            var cancellationTokenSource = new CancellationTokenSource();

            var deserializer = new ReqIFDeserializer();
            var reqIf = (await deserializer.DeserializeAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "testreqif.reqif"), cancellationTokenSource.Token)).First();

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

            Assert.AreEqual(7, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(4, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(3, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(1, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(2, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(1, reqIf.CoreContent.Specifications[0].Children.Count);
            Assert.AreEqual(2, reqIf.CoreContent.Specifications[0].Children[0].Children.Count);
            Assert.AreEqual(1, reqIf.CoreContent.SpecRelationGroups.Count);
        }

        [Test]
        public async Task Verify_That_A_ReqIF_Archive_Can_Be_DeserializedAsync_Without_Validation()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var deserializer = new ReqIFDeserializer();
            var reqIf = (await deserializer.DeserializeAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz"), cancellationTokenSource.Token)).First();

            Assert.AreEqual("en", reqIf.Lang);

            var firstobject = reqIf.CoreContent.SpecObjects.First();
            var xhtmlAttribute = firstobject.Values.OfType<AttributeValueXHTML>().SingleOrDefault();
            Assert.IsNotNull(xhtmlAttribute);
            Assert.IsNotEmpty(xhtmlAttribute.TheValue);
            Assert.IsNotNull(xhtmlAttribute.AttributeDefinition);

            Assert.AreEqual(7, reqIf.CoreContent.DataTypes.Count);
            Assert.AreEqual(4, reqIf.CoreContent.SpecTypes.Count);
            Assert.AreEqual(3, reqIf.CoreContent.SpecObjects.Count);
            Assert.AreEqual(1, reqIf.CoreContent.SpecRelations.Count);
            Assert.AreEqual(2, reqIf.CoreContent.Specifications.Count);
            Assert.AreEqual(1, reqIf.CoreContent.Specifications[0].Children.Count);
            Assert.AreEqual(2, reqIf.CoreContent.Specifications[0].Children[0].Children.Count);
            Assert.AreEqual(1, reqIf.CoreContent.SpecRelationGroups.Count);
        }

        [Test]
        public void Verify_That_A_ReqIF_Archive_Can_Be_Deserialized_With_Validation()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            var deserializer = new ReqIFDeserializer();
            Assert.That(() => deserializer.Deserialize(reqifPath, true, this.ValidationEventHandler), Throws.Nothing);

            using (var sourceStream = new FileStream(reqifPath, FileMode.Open))
            {
                Assert.That(() => deserializer.Deserialize(sourceStream, supportedFileExtensionKind), Throws.Nothing);
            }
        }

        [Test]
        public void Verify_That_A_ReqIF_Archive_Can_Be_DeserializedAsync_With_Validation()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            var deserializer = new ReqIFDeserializer();
            Assert.That(async () => await deserializer.DeserializeAsync(reqifPath, cancellationTokenSource.Token, true, this.ValidationEventHandler), Throws.Nothing);

            using (var sourceStream = new FileStream(reqifPath, FileMode.Open))
            {
                Assert.That(async () => await deserializer.DeserializeAsync(sourceStream, supportedFileExtensionKind, cancellationTokenSource.Token), Throws.Nothing);
            }
        }

        [Test]
        public void Verify_that_when_path_is_null_exception_is_thrown()
        {
            var deserializer = new ReqIFDeserializer();
            
            Assert.That(
                () => deserializer.Deserialize(""),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains("The path of the ReqIF file cannot be empty."));

            Assert.That(
                () => deserializer.Deserialize(null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                    .With.Message.Contains("The path of the ReqIF file cannot be null."));
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
            var cancellationTokenSource = new CancellationTokenSource();

            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz");

            var deserializer = new ReqIFDeserializer();

            Assert.That(
                async () => await deserializer.DeserializeAsync(path, cancellationTokenSource.Token, false, this.ValidationEventHandler),
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

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            using (var fileStream = new FileStream(reqifPath, FileMode.Open))
            {
                var sw = Stopwatch.StartNew();

                var reqif = deserializer.Deserialize(fileStream, supportedFileExtensionKind, false, null).First();

                Console.WriteLine($"deserialization done in: {sw.ElapsedMilliseconds} [ms]");

                Assert.That(reqif, Is.Not.Null);
            }
        }

        [Test]
        public async Task Verify_that_ReqIF_can_be_DeserializedAsync_from_stream()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var deserializer = new ReqIFDeserializer();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            using (var fileStream = new FileStream(reqifPath, FileMode.Open))
            {
                var sw = Stopwatch.StartNew();

                var reqif = (await deserializer.DeserializeAsync(fileStream, supportedFileExtensionKind, cancellationTokenSource.Token, false, null)).First();

                Console.WriteLine($"async deserialization done in: {sw.ElapsedMilliseconds} [ms]");

                Assert.That(reqif, Is.Not.Null);
            }
        }

        [Test]
        public void Verify_that_AlternativeId_can_be_deserialized()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Datatype-Demo.reqif");
            var deserializer = new ReqIFDeserializer();

            var reqif = deserializer.Deserialize(reqifPath, false).Single();

            var specObjectType = reqif.CoreContent.SpecTypes.OfType<SpecObjectType>().Single(x => x.Identifier == "_jgCytAfNEeeAO8RifBaE-g");

            foreach (var attributeDefinition in specObjectType.SpecAttributes)
            {
                Assert.That(attributeDefinition.AlternativeId.Identifier, Is.EqualTo(attributeDefinition.Identifier));
            }

            foreach (var datatypeDefinition in reqif.CoreContent.DataTypes)
            {
                Assert.That(datatypeDefinition.AlternativeId.Identifier, Is.EqualTo(datatypeDefinition.Identifier));
            }

            foreach (var specType in reqif.CoreContent.SpecTypes)
            {
                Assert.That(specType.AlternativeId.Identifier, Is.EqualTo(specType.Identifier));
            }

            foreach (var specObject in reqif.CoreContent.SpecObjects)
            {
                Assert.That(specObject.AlternativeId.Identifier, Is.EqualTo(specObject.Identifier));
            }

            foreach (var specification in reqif.CoreContent.Specifications)
            {
                Assert.That(specification.AlternativeId.Identifier, Is.EqualTo(specification.Identifier));

                foreach (var specHierarchy in specification.Children)
                {
                    Assert.That(specHierarchy.AlternativeId.Identifier, Is.EqualTo(specHierarchy.Identifier));
                }
            }
        }

        [Test]
        public async Task Verify_that_AlternativeId_can_be_deserialized_async()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Datatype-Demo.reqif");
            var deserializer = new ReqIFDeserializer();

            var reqif = (await deserializer.DeserializeAsync(reqifPath, cancellationTokenSource.Token, false)).Single();

            var specObjectType = reqif.CoreContent.SpecTypes.OfType<SpecObjectType>().Single(x => x.Identifier == "_jgCytAfNEeeAO8RifBaE-g");

            foreach (var attributeDefinition in specObjectType.SpecAttributes)
            {
                Assert.That(attributeDefinition.AlternativeId.Identifier, Is.EqualTo(attributeDefinition.Identifier));
            }

            foreach (var datatypeDefinition in reqif.CoreContent.DataTypes)
            {
                Assert.That(datatypeDefinition.AlternativeId.Identifier, Is.EqualTo(datatypeDefinition.Identifier));
            }

            foreach (var specType in reqif.CoreContent.SpecTypes)
            {
                Assert.That(specType.AlternativeId.Identifier, Is.EqualTo(specType.Identifier));
            }

            foreach (var specObject in reqif.CoreContent.SpecObjects)
            {
                Assert.That(specObject.AlternativeId.Identifier, Is.EqualTo(specObject.Identifier));
            }

            foreach (var specification in reqif.CoreContent.Specifications)
            {
                Assert.That(specification.AlternativeId.Identifier, Is.EqualTo(specification.Identifier));

                foreach (var specHierarchy in specification.Children)
                {
                    Assert.That(specHierarchy.AlternativeId.Identifier, Is.EqualTo(specHierarchy.Identifier));
                }
            }
        }

        [Test]
        public void Verify_that_sampleGH43_can_be_deserialized()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "sampleGH43.zip");

            var deserializer = new ReqIFDeserializer();
            Assert.That(() => deserializer.Deserialize(reqifPath), Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_sampleGH43_can_be_deserialized_async()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "sampleGH43.zip");

            var deserializer = new ReqIFDeserializer();
            
            Assert.That(async () => await deserializer.DeserializeAsync(reqifPath, cancellationTokenSource.Token), Throws.Nothing);
        }
    }
}
