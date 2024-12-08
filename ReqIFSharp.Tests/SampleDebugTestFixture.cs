// -------------------------------------------------------------------------------------------------
// <copyright file="SampleDebugTestFixture.cs" company="Starion Group S.A.">
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
