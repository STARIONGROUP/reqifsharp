﻿// -------------------------------------------------------------------------------------------------
// <copyright file="DeSerializeAndSerializeTestFixture.cs" company="Starion Group S.A.">
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
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    [TestFixture]
    public class DeSerializeAndSerializeTestFixture
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
        public void Verify_That_ProR_Traceability_Template_reqif_file_can_deserialized_and_serialized_with_equivalent_output()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            var reqIfs = deserializer.Deserialize(reqifPath);

            var resultFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, "ProR_Traceability-Template-v1.0-reserialize.reqif");

            var serializer = new ReqIFSerializer();
            Assert.DoesNotThrow(() => serializer.Serialize(reqIfs, resultFileUri));
        }

        [Test]
        public async Task Verify_That_ProR_Traceability_Template_reqif_file_can_deserialized_and_async_serialized_with_equivalent_output()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            var reqIfs = await deserializer.DeserializeAsync(reqifPath, cancellationTokenSource.Token); var reqIf = reqIfs.First();

            var resultFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, "ProR_Traceability-Template-v1.0-async-reserialize.reqif");

            var serializer = new ReqIFSerializer();
            Assert.DoesNotThrowAsync(async () => await serializer.SerializeAsync(reqIfs, resultFileUri, cancellationTokenSource.Token));
        }

        [Test]
        public void Verify_That_Datatype_Demo_reqif_file_is_deserialized_and_serialized_with_equivalent_output()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Datatype-Demo.reqif");
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            var reqIfS = deserializer.Deserialize(reqifPath);

            var resultFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, "Datatype-Demo-reserialize.reqif");

            var serializer = new ReqIFSerializer();
            Assert.DoesNotThrow(() => serializer.Serialize(reqIfS, resultFileUri));
        }

        [Test]
        public async Task Verify_That_Datatype_Demo_reqif_file_is_deserialized_and_async_serialized_with_equivalent_output()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "Datatype-Demo.reqif");
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            var reqIfs = await deserializer.DeserializeAsync(reqifPath, cancellationTokenSource.Token); var reqIf = reqIfs.First();

            var resultFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, "Datatype-Demo-async-reserialize.reqif");

            var serializer = new ReqIFSerializer();
            Assert.DoesNotThrowAsync(async () => await serializer.SerializeAsync(reqIfs, resultFileUri, cancellationTokenSource.Token));
        }

        [Test]
        public void Verify_that_DefaultValueDemo_file_is_deserialized_and_serialized_with_equivalent_output()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "DefaultValueDemo.reqif");
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            var reqIfs = deserializer.Deserialize(reqifPath);

            var resultFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, "DefaultValueDemo-reserialize.reqif");

            var serializer = new ReqIFSerializer();
            serializer.Serialize(reqIfs, resultFileUri);

            var reqIf2 = deserializer.Deserialize(resultFileUri).First();

            var OBJECT_TYPE = reqIf2.CoreContent.SpecTypes.First(x => x.Identifier == "_94301492-ef46-443d-9778-596b14a0a20e");
            var attributeDefinitionEnumeration = OBJECT_TYPE.SpecAttributes.OfType<AttributeDefinitionEnumeration>().First(x => x.Identifier == "_e5e971b3-62e3-4607-8696-f359c1ae85f3") ;
            Assert.That(attributeDefinitionEnumeration.DefaultValue, Is.Not.Null);
            var defaultEnumerationValue = attributeDefinitionEnumeration.DefaultValue.Values.Single();
            Assert.That(defaultEnumerationValue.Identifier, Is.EqualTo("_6feb570f-8ad8-40d9-9517-5abd6fe6f63a") );
            Assert.That(attributeDefinitionEnumeration.DefaultValue.Definition.Identifier, Is.EqualTo("_e5e971b3-62e3-4607-8696-f359c1ae85f3"));

            var DOCTYPE_GROUP_TYPE = reqIf2.CoreContent.SpecTypes.First(x => x.Identifier == "_80811ce1-0a28-4554-b49a-11bf2b7e9422");
            var attributeDefinitionString = DOCTYPE_GROUP_TYPE.SpecAttributes.OfType<AttributeDefinitionString>().First(x => x.Identifier == "_d0368b53-d0c6-43a1-a746-45243624b027");
            Assert.That(attributeDefinitionString.DefaultValue.TheValue, Is.EqualTo("LAH ---.---.---.-"));
            Assert.That(attributeDefinitionString.DefaultValue.Definition.Identifier, Is.EqualTo("_d0368b53-d0c6-43a1-a746-45243624b027"));
        }

        [Test]
        public async Task Verify_that_DefaultValueDemo_file_is_deserialized_and_async_serialized_with_equivalent_output()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "DefaultValueDemo.reqif");
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            var reqIfs = await deserializer.DeserializeAsync(reqifPath, cancellationTokenSource.Token);

            var resultFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, "DefaultValueDemo-async-reserialize.reqif");

            var serializer = new ReqIFSerializer();
            await serializer.SerializeAsync(reqIfs, resultFileUri, cancellationTokenSource.Token);

            var reqIf2 = deserializer.Deserialize(resultFileUri).First();

            var OBJECT_TYPE = reqIf2.CoreContent.SpecTypes.First(x => x.Identifier == "_94301492-ef46-443d-9778-596b14a0a20e");
            var attributeDefinitionEnumeration = OBJECT_TYPE.SpecAttributes.OfType<AttributeDefinitionEnumeration>().First(x => x.Identifier == "_e5e971b3-62e3-4607-8696-f359c1ae85f3");
            Assert.That(attributeDefinitionEnumeration.DefaultValue, Is.Not.Null);
            var defaultEnumerationValue = attributeDefinitionEnumeration.DefaultValue.Values.Single();
            Assert.That(defaultEnumerationValue.Identifier, Is.EqualTo("_6feb570f-8ad8-40d9-9517-5abd6fe6f63a"));
            Assert.That(attributeDefinitionEnumeration.DefaultValue.Definition.Identifier, Is.EqualTo("_e5e971b3-62e3-4607-8696-f359c1ae85f3"));

            var DOCTYPE_GROUP_TYPE = reqIf2.CoreContent.SpecTypes.First(x => x.Identifier == "_80811ce1-0a28-4554-b49a-11bf2b7e9422");
            var attributeDefinitionString = DOCTYPE_GROUP_TYPE.SpecAttributes.OfType<AttributeDefinitionString>().First(x => x.Identifier == "_d0368b53-d0c6-43a1-a746-45243624b027");
            Assert.That(attributeDefinitionString.DefaultValue.TheValue, Is.EqualTo("LAH ---.---.---.-"));
            Assert.That(attributeDefinitionString.DefaultValue.Definition.Identifier, Is.EqualTo("_d0368b53-d0c6-43a1-a746-45243624b027"));
        }

        [Test]
        public void Verify_That_ProRailDemo_with_validation_and_assert_alternaiveid()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");
            var deserializer = new ReqIFDeserializer(this.loggerFactory);

            var reqIfs = deserializer.Deserialize(reqifPath, true);

            var reqif = reqIfs.Single();
            var alternativeId = reqif.CoreContent.DataTypes.Single(x => x.Identifier == "_o7scM6dbEeafNduaIhMwQg_ald").AlternativeId;

            Assert.That(alternativeId.Identifier, Is.EqualTo("_o7scM6dbEeafNduaIhMwQg_ald"));
        }
    }
}
