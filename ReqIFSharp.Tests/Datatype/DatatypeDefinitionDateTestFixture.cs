// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionDateTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2022 RHEA System S.A.
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

namespace ReqIFSharp.Tests.Datatype
{
    using System;
    using System.IO;
    using System.Linq;
    
    using NUnit.Framework;

    using ReqIFSharp;

    [TestFixture]
    public class DatatypeDefinitionDateTestFixture
    {
        private string resultFileUri;

        [SetUp]
        public void SetUp()
        {
            this.resultFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, "result.xml");
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(this.resultFileUri);
        }

        [Test]
        public void Verify_that_DatatypeDefinitionDate_can_be_serialized_and_deserialized()
        {
            var document = new ReqIF
            {
                Lang = "en",
                TheHeader = new ReqIFHeader(),
                CoreContent = new ReqIFContent()
            };

            var lastChange = new DateTime(1831, 07, 21);
            
            var dateDefinition = new DatatypeDefinitionDate
            {
                Identifier = "dateDefinition",
                LongName = "Date",
                LastChange = lastChange
            };

            document.CoreContent.DataTypes.Add(dateDefinition);

            this.resultFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, "result.xml");

            var serializer = new ReqIFSerializer();
            Assert.That(() => serializer.Serialize(document, this.resultFileUri), Throws.Nothing);

            var deserializer = new ReqIFDeserializer();

            var reqIf = deserializer.Deserialize(this.resultFileUri).First();

            var datatypeDefinition = reqIf.CoreContent.DataTypes.Single(x => x.Identifier == "dateDefinition");

            Assert.That(datatypeDefinition, Is.TypeOf<DatatypeDefinitionDate>());
            Assert.That(datatypeDefinition.LongName, Is.EqualTo("Date"));
            Assert.That(datatypeDefinition.LastChange, Is.EqualTo(lastChange));
        }
    }
}
