// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFLibTestFixture.cs" company="RHEA System S.A.">
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
    using System.Xml;
    using System.Xml.Serialization;

    using NUnit.Framework;

    using ReqIFSharp;

    [TestFixture]
    public class ReqIFLibTestFixture
    {
        private const string ReqIFNamespace = @"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd";
        private XmlSerializer serializer;

        [SetUp]
        public void Setup()
        {
            this.serializer = new XmlSerializer(typeof(ReqIF), ReqIFNamespace);
        }

        [Test]
        public void VerifyThatReqIfObjectIsCreatedCorrectly()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "testreqif.reqif");

            using (var xmlreader = XmlReader.Create(path))
            {
                var reqif = (ReqIF)this.serializer.Deserialize(xmlreader);
                Assert.That(reqif, Is.Not.Null);
                Assert.That(reqif.GetSchema(), Is.Null);

                Assert.That(reqif.TheHeader, Is.Not.Null);
                Assert.That(reqif.TheHeader.GetSchema(), Is.Null);

                Assert.That(reqif.TheHeader.Comment, Is.Not.Null.Or.Empty);
                Assert.That(reqif.TheHeader.Identifier, Is.Not.Null.Or.Empty);
                Assert.That(reqif.TheHeader.RepositoryId, Is.Not.Null.Or.Empty);
                Assert.That(reqif.TheHeader.ReqIFToolId, Is.Not.Null.Or.Empty);
                Assert.That(reqif.TheHeader.ReqIFVersion, Is.Not.Null.Or.Empty);
                Assert.That(reqif.TheHeader.SourceToolId, Is.Not.Null.Or.Empty);
                Assert.That(reqif.TheHeader.Title, Is.Not.Null.Or.Empty);
                Assert.That(reqif.TheHeader.CreationTime, Is.Not.Null);
                
                Assert.That(reqif.CoreContent, Is.Not.Null);
                Assert.That(reqif.CoreContent.GetSchema(), Is.Null);
                Assert.That(reqif.CoreContent.DataTypes, Is.Not.Empty);
                Assert.That(reqif.CoreContent.SpecObjects, Is.Not.Empty);
                Assert.That(reqif.CoreContent.SpecRelationGroups, Is.Empty);
                Assert.That(reqif.CoreContent.SpecRelations, Is.Not.Empty);
                Assert.That(reqif.CoreContent.SpecTypes, Is.Not.Empty);
                Assert.That(reqif.CoreContent.Specifications, Is.Not.Empty);
            }
        }
    }
}
