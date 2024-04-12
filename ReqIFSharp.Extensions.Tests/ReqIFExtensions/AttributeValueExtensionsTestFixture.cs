//  -------------------------------------------------------------------------------------------------
//  <copyright file="AttributeValueExtensionsTestFixture.cs" company="RHEA System S.A.">
// 
//    Copyright 2017-2024 RHEA System S.A.
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
    using System.Linq;

    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.ReqIFExtensions;
    using ReqIFSharp.Extensions.Tests.TestData;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeValueExtensions"/> class
    /// </summary>
    [TestFixture]
    public class AttributeValueExtensionsTestFixture
    {
        [Test]
        public void Verify_that_QueryFormattedValue_returns_expected_results()
        {
            var testDataCreator = new ReqIFTestDataCreator();
            var reqif = testDataCreator.Create();

            var specObject = reqif.CoreContent.SpecObjects.Single(x => x.Identifier == "specobject_1");

            var attributeValueBoolean =  specObject.Values.OfType<AttributeValueBoolean>().Single();
            Assert.That(attributeValueBoolean.QueryFormattedValue(), Is.EqualTo("True"));

            var attributeValueDate = specObject.Values.OfType<AttributeValueDate>().Single();
            Assert.That(attributeValueDate.QueryFormattedValue(), Is.EqualTo("2015-12-01, 00:00:00"));

            var attributeValueEnumeration = specObject.Values.OfType<AttributeValueEnumeration>().Single();
            Assert.That(attributeValueEnumeration.QueryFormattedValue(), Is.EqualTo("foo"));

            var attributeValueInteger = specObject.Values.OfType<AttributeValueInteger>().Single();
            Assert.That(attributeValueInteger.QueryFormattedValue(), Is.EqualTo("1"));

            var attributeValueReal = specObject.Values.OfType<AttributeValueReal>().Single();
            Assert.That(attributeValueReal.QueryFormattedValue(), Is.EqualTo("100"));

            var attributeValueString = specObject.Values.OfType<AttributeValueString>().Single();
            Assert.That(attributeValueString.QueryFormattedValue(), Is.EqualTo("a string value"));

            var attributeValueXhtml = specObject.Values.OfType<AttributeValueXHTML>().First();
            Assert.That(attributeValueXhtml.QueryFormattedValue(), Is.EqualTo("<xhtml:p>XhtmlPType<xhtml:a accesskey=\"a\" charset=\"UTF-8\" href=\"http://eclipse.org/rmf\" hreflang=\"en\" rel=\"LinkTypes\" rev=\"LinkTypes\" style=\"text-decoration:underline\" tabindex=\"1\" title=\"text\" type=\"text/html\"> text before br<xhtml:br/>text after br text before span<xhtml:span>XhtmlSpanType</xhtml:span>text after span text before em<xhtml:em>XhtmlEmType</xhtml:em>text after em text before strong<xhtml:strong>XhtmlStrongType</xhtml:strong>text after strong text before dfn<xhtml:dfn>XhtmlDfnType</xhtml:dfn>text after dfn text before code<xhtml:code>XhtmlCodeType</xhtml:code>text after code text before samp<xhtml:samp>XhtmlSampType</xhtml:samp>text after samp text before kbd<xhtml:kbd>XhtmlKbdType</xhtml:kbd>text after kbd text before var<xhtml:var>XhtmlVarType</xhtml:var>text after var text before cite<xhtml:cite>XhtmlCiteType</xhtml:cite>text after cite text before abbr<xhtml:abbr>XhtmlAbbrType</xhtml:abbr>text after abbr text before acronym<xhtml:acronym>XhtmlAcronymType</xhtml:acronym>text after acronym text before q<xhtml:q>XhtmlQType</xhtml:q>text after q text before tt<xhtml:tt>XhtmlInlPresType</xhtml:tt>text after tt text before i<xhtml:i>XhtmlInlPresType</xhtml:i>text after i text before b<xhtml:b>XhtmlInlPresType</xhtml:b>text after b text before big<xhtml:big>XhtmlInlPresType</xhtml:big>text after big text before small<xhtml:small>XhtmlInlPresType</xhtml:small>text after small text before sub<xhtml:sub>XhtmlInlPresType</xhtml:sub>text after sub text before sup<xhtml:sup>XhtmlInlPresType</xhtml:sup>text after sup text before ins<xhtml:ins>XhtmlEditType</xhtml:ins>text after ins text before del<xhtml:del>XhtmlEditType</xhtml:del>text after del</xhtml:a></xhtml:p>"));
        }
    }
}
