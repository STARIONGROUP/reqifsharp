// -------------------------------------------------------------------------------------------------
// <copyright file="ExternalObjectDeseralizationTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
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
