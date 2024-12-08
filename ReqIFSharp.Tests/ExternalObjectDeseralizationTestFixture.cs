// -------------------------------------------------------------------------------------------------
// <copyright file="ExternalObjectDeseralizationTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
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
    using System.Linq;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    [TestFixture]
    public class ExternalObjectDeseralizationTestFixture
    {
        private string reqiffile;

        private ILoggerFactory loggerFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
        }

        [SetUp]
        public void SetUp()
        {
            this.reqiffile = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Spielwiese.reqifz");
        }

        [Test]
        public void Verify_that_External_objects_are_Deserialized()
        {
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            var reqIf = deserializer.Deserialize(this.reqiffile).First();

            var specObjects = reqIf.CoreContent.SpecObjects;
            Assert.That(specObjects.Count, Is.EqualTo(4));

            var specObjectWithExternalObjects = specObjects.Single(x => x.Identifier == "_5_b23d3568-8478-401c-8ee3-3246da83641d");
            
            var attributeValueXhtml = specObjectWithExternalObjects.Values.OfType<AttributeValueXHTML>().Single(x =>
                x.Definition.Identifier == "_2792cc0c-3af9-4619-9968-c1d0f53d5bcb_OBJECTTEXT");

            Assert.That(attributeValueXhtml.ExternalObjects.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Verify_that_External_objects_are_Deserialized_Async()
        {
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            var reqIf = await deserializer.DeserializeAsync(this.reqiffile, CancellationToken.None);

            var specObjects = reqIf.First().CoreContent.SpecObjects;
            Assert.That(specObjects.Count, Is.EqualTo(4));

            var specObjectWithExternalObjects = specObjects.Single(x => x.Identifier == "_5_b23d3568-8478-401c-8ee3-3246da83641d");
            
            var attributeValueXhtml = specObjectWithExternalObjects.Values.OfType<AttributeValueXHTML>().Single(x =>
                x.Definition.Identifier == "_2792cc0c-3af9-4619-9968-c1d0f53d5bcb_OBJECTTEXT");

            Assert.That(attributeValueXhtml.ExternalObjects.Count, Is.EqualTo(2));
        }
    }
}
