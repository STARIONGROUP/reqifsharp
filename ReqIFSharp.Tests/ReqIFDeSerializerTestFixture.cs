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

namespace ReqIFSharp.Tests
{
    using System;
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

        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void VerifyThatAReqIFXMLDocumentCanBeDeserializedWitouthValidation()
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
        public void VerifyThatAReqIFArchiveCanBeDeserializedWitouthValidation()
        {
            var deserializer = new ReqIFDeserializer();
            
            Assert.DoesNotThrow(() => deserializer.Deserialize(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "test-multiple-reqif.reqifz")));
        }

        [Test]
        public void Verify_that_the_Tool_Extensions_are_deserialized_from_ProR_Traceability_template()
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
        public void Verify_that_ExternalObjects_are_created_and_that_local_data_can_be_queried()
        {
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
                        externalObject.QueryLocalData(targetStream);
                        Assert.That(targetStream.Length, Is.GreaterThan(0));
                        targetStream.Dispose();
                    }
                }
            }
        }

        [Test]
        public void VerifyThatAReqIFXMLDocumentCanBeDeserializedWithValidation()
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
        public void Verify_that_XHTML_attributes_can_de_deserialized()
        {
            var deserializer = new ReqIFDeserializer();
            var reqIf = deserializer.Deserialize(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "testreqif.reqif")).First();

            Assert.AreEqual("en", reqIf.Lang);

            var specObject = reqIf.CoreContent.SpecObjects.Single(r => r.Identifier == "R001");

            var xhtmlValue = specObject.Values.Single(x => x.AttributeDefinition.Identifier == "FUNC-REQ-NOTES") as AttributeValueXHTML;

            Assert.That(xhtmlValue.TheValue, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void VerifyThatAReqIFArchiveCanBeDeserializedWitouthValidationNET()
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
    }
}
