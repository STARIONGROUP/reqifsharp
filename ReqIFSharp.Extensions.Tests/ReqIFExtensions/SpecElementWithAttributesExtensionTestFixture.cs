//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecElementWithAttributesExtensionTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2025 Starion Group S.A.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  -------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.Tests.ReqIFExtensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    
    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.ReqIFExtensions;
    using ReqIFSharp.Extensions.Services;

    /// <summary>
    /// Suite of tests for the <see cref="SpecElementWithAttributesExtensions"/>
    /// </summary>
    [TestFixture]
    public class SpecElementWithAttributesExtensionTestFixture
    {
        private ReqIF reqIf;

        private ReqIFLoaderService reqIFLoaderService;

        [SetUp]
        public async Task SetUp()
        {
            var reqIfDeserializer = new ReqIFDeserializer();

            var cts = new CancellationTokenSource();

            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "requirements-and-objects.reqifz");

            var supportedFileExtensionKind = reqifPath.ConvertPathToSupportedFileExtensionKind();

            await using var fileStream = new FileStream(reqifPath, FileMode.Open);
            this.reqIFLoaderService = new ReqIFLoaderService(reqIfDeserializer);
            await this.reqIFLoaderService.LoadAsync(fileStream, supportedFileExtensionKind, cts.Token);

            this.reqIf = this.reqIFLoaderService.ReqIFData.Single();
        }

        [Test]
        public void Verify_that_QueryExternalObjects_returns_expected_results()
        {
            var specObject = this.reqIf.CoreContent.SpecObjects.Single(x => x.Identifier == "_3.4.2.2.2_BrLeft_2_BrRight_._BrLeft_f_BrRight_1");

            var externalObjects = specObject.QueryExternalObjects();

            Assert.That(externalObjects.Count, Is.EqualTo(1));
        }

        [Test]
        public void Verify_that_QueryExternalObjects_returns_empty_when_no_external_objects_are_present()
        {
            var specObject = new SpecObject
            {
                Values =
                {
                    new AttributeValueBoolean(),
                    new AttributeValueString()
                }
            };

            Assert.That(specObject.QueryExternalObjects(), Is.Empty);
        }

        [Test]
        public void Verify_that_QueryExternalObjects_throws_when_specElement_is_null()
        {
            Assert.That(() => SpecElementWithAttributesExtensions.QueryExternalObjects(null), Throws.ArgumentNullException);
        }

        [Test]
        public async Task Verify_that_QueryBase64Payloads_returns_expected_results()
        {
            var specObject = this.reqIf.CoreContent.SpecObjects.Single(x => x.Identifier == "_3.4.2.2.2_BrLeft_2_BrRight_._BrLeft_f_BrRight_1");

            var base64Payloads = await specObject.QueryBase64PayloadsAsync(this.reqIFLoaderService);

            var base64Payload = base64Payloads.Single();

            Assert.That(base64Payload.Item2, Does.StartWith("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAfgAAACfCAIAAACazFx+AAAAAXNSR0IArs4c6QAA"));
        }

        [Test]
        public void Verify_that_QueryBase64PayloadsAsync_throws_when_specElement_is_null()
        {
            Assert.That(
                () => SpecElementWithAttributesExtensions.QueryBase64PayloadsAsync(null, this.reqIFLoaderService),
                Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryBase64PayloadsAsync_throws_when_loader_is_null()
        {
            var specObject = this.reqIf.CoreContent.SpecObjects.First();

            Assert.That(() => specObject.QueryBase64PayloadsAsync(null), Throws.ArgumentNullException);
        }

        [Test]
        public async Task Verify_that_QueryBase64PayloadsAsync_returns_payloads_for_all_external_objects()
        {
            var specObject = new SpecObject
            {
                Values =
                {
                    new AttributeValueXHTML
                    {
                        ExternalObjects =
                        {
                            new ExternalObject { Identifier = "object-1" },
                            new ExternalObject { Identifier = "object-2" }
                        }
                    },
                    new AttributeValueXHTML
                    {
                        ExternalObjects =
                        {
                            new ExternalObject { Identifier = "object-3" }
                        }
                    }
                }
            };

            var loader = new StubReqIfLoaderService();

            var payloads = await specObject.QueryBase64PayloadsAsync(loader);

            Assert.That(payloads.Select(x => x.Item1.Identifier), Is.EquivalentTo(new[] { "object-1", "object-2", "object-3" }));
            Assert.That(payloads.Select(x => x.Item2), Is.EquivalentTo(new[] { "payload-object-1", "payload-object-2", "payload-object-3" }));
        }

        [Test]
        public void Verify_that_ReqIFContent_QueryExternalObjects_returns_expected_results()
        {
            var reqIfContent = new ReqIFContent();

            var specObject = new SpecObject
            {
                ReqIFContent = reqIfContent,
                Values =
                {
                    new AttributeValueXHTML
                    {
                        ExternalObjects =
                        {
                            new ExternalObject { Identifier = "object-1" }
                        }
                    }
                }
            };

            var specRelation = new SpecRelation
            {
                ReqIFContent = reqIfContent,
                Values =
                {
                    new AttributeValueXHTML
                    {
                        ExternalObjects =
                        {
                            new ExternalObject { Identifier = "object-2" }
                        }
                    }
                }
            };

            reqIfContent.SpecObjects.Add(specObject);
            reqIfContent.SpecRelations.Add(specRelation);

            var externalObjects = reqIfContent.QueryExternalObjects().ToList();

            Assert.That(externalObjects.Select(x => x.Identifier), Is.EquivalentTo(new[] { "object-1", "object-2" }));
        }

        [Test]
        public void Verify_that_ReqIFContent_QueryExternalObjects_throws_when_reqIfContent_is_null()
        {
            Assert.That(() => ReqIFContentExtensions.QueryExternalObjects(null), Throws.ArgumentNullException);
        }

        private class StubReqIfLoaderService : IReqIFLoaderService
        {
            public IEnumerable<ReqIF> ReqIFData => throw new NotImplementedException();

            public event EventHandler<IEnumerable<ReqIF>> ReqIfChanged
            {
                add => throw new NotImplementedException();
                remove => throw new NotImplementedException();
            }

            public Task<Stream> GetSourceStreamAsync(CancellationToken token)
            {
                throw new NotImplementedException();
            }

            public Task LoadAsync(Stream reqifStream, SupportedFileExtensionKind fileExtensionKind, CancellationToken token)
            {
                throw new NotImplementedException();
            }

            public Task<string> QueryDataAsync(ExternalObject externalObject, CancellationToken token)
            {
                return Task.FromResult($"payload-{externalObject.Identifier}");
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
