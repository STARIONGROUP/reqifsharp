// -------------------------------------------------------------------------------------------------
// <copyright file="ExternalObjectExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.Tests.ReqIFExtensions
{
    using System;
    using System.Text;

    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.ReqIFExtensions;

    /// <summary>
    /// Suite of tests for the <see cref="ExternalObjectExtensions"/> class
    /// </summary>
    [TestFixture]
    public class ExternalObjectExtensionsTestFixture
    {
        [Test]
        public void Verify_that_CreateUrl_throws_when_externalObject_is_null()
        {
            Assert.That(() => ExternalObjectExtensions.CreateUrl(null),
                Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("externalObject"));
        }

        [Test]
        public void Verify_that_CreateUrl_returns_expected_url()
        {
            var reqif = new ReqIF();

            var header = new ReqIFHeader
            {
                Identifier = "reqif-identifier",
                DocumentRoot = reqif
            };

            reqif.TheHeader = header;

            var coreContent = new ReqIFContent
            {
                DocumentRoot = reqif
            };

            reqif.CoreContent = coreContent;

            var specObject = new SpecObject
            {
                ReqIFContent = coreContent
            };

            coreContent.SpecObjects.Add(specObject);

            var attributeValueXhtml = new AttributeValueXHTML
            {
                SpecElAt = specObject
            };

            var externalObject = new ExternalObject(attributeValueXhtml)
            {
                Uri = "graphics/diagram.png"
            };

            var expectedBase64Uri = Convert.ToBase64String(Encoding.UTF8.GetBytes(externalObject.Uri));
            var expectedUrl = $"/reqif/{header.Identifier}/externalobject/{expectedBase64Uri}";

            Assert.That(externalObject.CreateUrl(), Is.EqualTo(expectedUrl));
        }
    }
}
