// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionDateTestFixture.cs" company="Starion Group S.A.">
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

namespace ReqIFSharp.Tests.Datatype
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    [TestFixture]
    public class DatatypeDefinitionDateTestFixture
    {
        private string resultFileUri;

        private ILoggerFactory loggerFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
        }

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
        public void Verify_that_constructor_does_not_throw_exception()
        {
            Assert.That(() => new DatatypeDefinitionDate(null), Throws.Nothing);
            Assert.That(() => new DatatypeDefinitionDate(this.loggerFactory), Throws.Nothing);
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

            var documents = new List<ReqIF> { document };

            this.resultFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, "test-result.reqif");

            var serializer = new ReqIFSerializer();
            Assert.That(() => serializer.Serialize(documents, this.resultFileUri), Throws.Nothing);

            var deserializer = new ReqIFDeserializer();

            var reqIf = deserializer.Deserialize(this.resultFileUri).First();

            var datatypeDefinition = reqIf.CoreContent.DataTypes.Single(x => x.Identifier == "dateDefinition");

            Assert.That(datatypeDefinition, Is.TypeOf<DatatypeDefinitionDate>());
            Assert.That(datatypeDefinition.LongName, Is.EqualTo("Date"));
            Assert.That(datatypeDefinition.LastChange, Is.EqualTo(lastChange));
        }
    }
}
