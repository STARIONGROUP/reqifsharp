// -------------------------------------------------------------------------------------------------
// <copyright file="SampleDebugTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System;
    using System.IO;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    [TestFixture]
    public class SampleDebugTestFixture
    {
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

        [Test]
        public void Verify_that_sampledebug_reqif_file_can_be_deserialized()
        {
            var deserializer = new ReqIFDeserializer(this.loggerFactory);
            var reqIf = deserializer.Deserialize(Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "sample_debug.reqif")).Single();
            var header = reqIf.TheHeader;
            var content = reqIf.CoreContent;

            Assert.That(header.Identifier, Is.EqualTo("_c1638633-9ec4-4f9f-aabd-4804f5e0110c")); 

            var specObject = content.SpecObjects.Single(x => x.Identifier == "rmf-249cb10a-69cf-48ac-ac40-bcba7fc15d47");

            foreach (var attributeValueXhtml in specObject.Values.OfType<AttributeValueXHTML>())
            {
                Assert.That(attributeValueXhtml.TheValue, Is.Not.Null.Or.Empty);
                Console.WriteLine(attributeValueXhtml.TheValue);
            }

            var xhtmlAttributes = content.SpecObjects.SelectMany(x => x.Values.OfType<AttributeValueXHTML>()).ToList();

            foreach (var attributeValueXhtml in xhtmlAttributes)
            {
                Assert.That(attributeValueXhtml.TheValue, Is.Not.Null.Or.Empty);
                Console.WriteLine(attributeValueXhtml.TheValue);
            }
        }
    }
}
