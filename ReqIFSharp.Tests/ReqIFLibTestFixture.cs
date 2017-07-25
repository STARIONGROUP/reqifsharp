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
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
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
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "testreqif.xml");

            using (var xmlreader = XmlReader.Create(path))
            {
                var reqif = (ReqIF)this.serializer.Deserialize(xmlreader);
                Assert.IsNotNull(reqif);

                var reqIfHeader = reqif.TheHeader.Single();
                Assert.IsNotNull(reqIfHeader);

                Assert.IsNotNull(reqIfHeader.Comment);
                Assert.IsNotEmpty(reqIfHeader.Comment);
                
                Assert.IsNotNull(reqIfHeader.Identifier);
                Assert.IsNotEmpty(reqIfHeader.Identifier);
                
                Assert.IsNotNull(reqIfHeader.RepositoryId);
                Assert.IsNotEmpty(reqIfHeader.RepositoryId);
                
                Assert.IsNotNull(reqIfHeader.ReqIFToolId);
                Assert.IsNotEmpty(reqIfHeader.ReqIFToolId);
                
                Assert.IsNotNull(reqIfHeader.ReqIFVersion);
                Assert.IsNotEmpty(reqIfHeader.ReqIFVersion);
                
                Assert.IsNotNull(reqIfHeader.SourceToolId);
                Assert.IsNotEmpty(reqIfHeader.SourceToolId);
                
                Assert.IsNotNull(reqIfHeader.Title);
                Assert.IsNotEmpty(reqIfHeader.Title);

                Assert.IsNotNull(reqIfHeader.CreationTime);

                var coreContent = reqif.CoreContent.Single();
                Assert.IsNotNull(coreContent);

                Assert.IsNotEmpty(coreContent.DataTypes);
                Assert.IsNotEmpty(coreContent.SpecObjects);
                Assert.IsEmpty(coreContent.SpecRelationGroups);
                Assert.IsNotEmpty(coreContent.SpecRelations);
                Assert.IsNotEmpty(coreContent.SpecTypes);
                Assert.IsNotEmpty(coreContent.Specifications);
            }
        }
    }
}