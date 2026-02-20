//  -------------------------------------------------------------------------------------------------
//  <copyright file="ExternalObjectExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//    Copyright 2017-2026 Starion Group S.A.
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
            var expectedUrl = $"/reqif/{header.Identifier}/externalobject/{expectedBase64Uri} ";

            Assert.That(externalObject.CreateUrl(), Is.EqualTo(expectedUrl));
        }
    }
}
