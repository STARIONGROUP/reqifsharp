// -------------------------------------------------------------------------------------------------
// <copyright file="SpecElementWithAttributesExtensionTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.Tests.ReqIFExtensions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging.Abstractions;

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
        public async Task Verify_that_the_external_objects_of_xhtml_values_created_with_a_public_constructor_can_be_queried()
        {
            // every public constructor creates the list of external objects, so the queries do not throw
            var specObject = new SpecObject();
            specObject.Values.Add(new AttributeValueXHTML { TheValue = "<xhtml:p>text</xhtml:p>" });
            specObject.Values.Add(new AttributeValueXHTML(NullLoggerFactory.Instance) { TheValue = "<xhtml:p>text</xhtml:p>" });
            specObject.Values.Add(new AttributeValueXHTML(null) { TheValue = "<xhtml:p>text</xhtml:p>" });

            Assert.That(specObject.QueryExternalObjects(), Is.Empty);
            Assert.That(await specObject.QueryBase64PayloadsAsync(this.reqIFLoaderService), Is.Empty);
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
        public void Verify_that_QueryBase64PayloadsAsync_honors_cancellation()
        {
            var specObject = this.reqIf.CoreContent.SpecObjects.Single(x => x.Identifier == "_3.4.2.2.2_BrLeft_2_BrRight_._BrLeft_f_BrRight_1");

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await specObject.QueryBase64PayloadsAsync(this.reqIFLoaderService, cts.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }
}
