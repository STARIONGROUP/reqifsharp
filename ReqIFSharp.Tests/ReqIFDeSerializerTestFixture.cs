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
        private string xmlfilepath;

        private const int AmountOfDataTypes = 7;
        private const int AmountOfSpecTypes = 4;
        private const int AmountOfSpecObjects = 3;
        private const int AmountOfSpecRelations = 1;
        private const int AmountOfSpecifications = 2;
        private const int AmountOfSpecificationChildren = 1;
        private const int AmountOfSpecificationSubChildren = 2;
        private const int AmountOfSpecRelationGroups = 1;

        [SetUp]
        public void SetUp()
        {
            this.xmlfilepath = Path.Combine(TestContext.CurrentContext.TestDirectory, "_Basic_ReqIF_Exchange.reqif");
        }

        [Test]
        public void VerifyThatAReqIFXMLDocumentCanBeDeserializedWitouthValidation()
        {
            var deserializer = new ReqIFDeserializer();
            var reqIf = deserializer.Deserialize(this.xmlfilepath);

            Assert.AreEqual("en", reqIf.Lang);

            var reqIfContent = reqIf.CoreContent.FirstOrDefault();
            var firstobject = reqIfContent.SpecObjects.First();
            var xhtmlAttribute = firstobject.Values.OfType<AttributeValueXHTML>().SingleOrDefault();
            Assert.IsNotNull(xhtmlAttribute);
            Assert.IsNotNullOrEmpty(xhtmlAttribute.TheValue);
            Assert.IsNotNull(xhtmlAttribute.AttributeDefinition);

            Assert.AreEqual(AmountOfDataTypes, reqIfContent.DataTypes.Count);
            Assert.AreEqual(AmountOfSpecTypes, reqIfContent.SpecTypes.Count);
            Assert.AreEqual(AmountOfSpecObjects, reqIfContent.SpecObjects.Count);
            Assert.AreEqual(AmountOfSpecRelations, reqIfContent.SpecRelations.Count);
            Assert.AreEqual(AmountOfSpecifications, reqIfContent.Specifications.Count);
            Assert.AreEqual(AmountOfSpecificationChildren, reqIfContent.Specifications[0].Children.Count);
            Assert.AreEqual(AmountOfSpecificationSubChildren, reqIfContent.Specifications[0].Children[0].Children.Count);
            Assert.AreEqual(AmountOfSpecRelationGroups, reqIfContent.SpecRelationGroups.Count);
        }

        [Test]
        public void VerifyThatAReqIFXMLDocumentCanBeDeserializedWithValidation()
        {
            var deserializer = new ReqIFDeserializer();
            var reqIf = deserializer.Deserialize(this.xmlfilepath, true, this.ValidationEventHandler);

            Assert.AreEqual("en", reqIf.Lang);

            var reqIfContent = reqIf.CoreContent.FirstOrDefault();

            Assert.AreEqual(AmountOfDataTypes, reqIfContent.DataTypes.Count);
            Assert.AreEqual(AmountOfSpecTypes, reqIfContent.SpecTypes.Count);
            Assert.AreEqual(AmountOfSpecObjects, reqIfContent.SpecObjects.Count);
            Assert.AreEqual(AmountOfSpecRelations, reqIfContent.SpecRelations.Count);
            Assert.AreEqual(AmountOfSpecifications, reqIfContent.Specifications.Count);
            Assert.AreEqual(AmountOfSpecRelationGroups, reqIfContent.SpecRelationGroups.Count);
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
