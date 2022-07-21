//  -------------------------------------------------------------------------------------------------
//  <copyright file="AttributeDefinitionExtensionsTestFixture.cs" company="RHEA System S.A.">
// 
//    Copyright 2017-2022 RHEA System S.A.
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
    /// Suite of tests for the <see cref="AttributeDefinitionExtensions"/> class
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionExtensionsTestFixture
    {
        [Test]
        public void Verify_that_QueryDatatypeName_returns_expected_results()
        {
            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            Assert.That(attributeDefinitionBoolean.QueryDatatypeName(), Is.EqualTo("Boolean"));

            var attributeDefinitionDate = new AttributeDefinitionDate();
            Assert.That(attributeDefinitionDate.QueryDatatypeName(), Is.EqualTo("Date"));

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            Assert.That(attributeDefinitionEnumeration.QueryDatatypeName(), Is.EqualTo("Enumeration"));

            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            Assert.That(attributeDefinitionInteger.QueryDatatypeName(), Is.EqualTo("Integer"));

            var attributeDefinitionReal = new AttributeDefinitionReal();
            Assert.That(attributeDefinitionReal.QueryDatatypeName(), Is.EqualTo("Real"));

            var attributeDefinitionString = new AttributeDefinitionString();
            Assert.That(attributeDefinitionString.QueryDatatypeName(), Is.EqualTo("String"));

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            Assert.That(attributeDefinitionXhtml.QueryDatatypeName(), Is.EqualTo("XHTML"));
        }

        [Test]
        public void Verify_that_QueryDefaultValueAsFormattedString_returns_expected_results()
        {
            var testDataCreator = new ReqIFTestDataCreator();
            var reqif = testDataCreator.Create();

            var specType = reqif.CoreContent.SpecTypes.Single(x => x.Identifier == "requirement");

            var attributeDefinitionBoolean = (AttributeDefinitionBoolean)specType.SpecAttributes.Single(x => x.Identifier == "requirement-boolean-attribute");
            Assert.That(attributeDefinitionBoolean.QueryDefaultValueAsFormattedString(), Is.EqualTo("NOT SET"));

            var attributeDefinitionDate = (AttributeDefinitionDate)specType.SpecAttributes.Single(x => x.Identifier == "requirement-date-attribute");
            Assert.That(attributeDefinitionDate.QueryDefaultValueAsFormattedString(), Is.EqualTo("NOT SET"));

            var attributeDefinitionEnumeration = (AttributeDefinitionEnumeration)specType.SpecAttributes.Single(x => x.Identifier == "requirement-enumeration-attribute");
            Assert.That(attributeDefinitionEnumeration.QueryDefaultValueAsFormattedString(), Is.EqualTo("NOT SET"));

            var attributeDefinitionInteger = (AttributeDefinitionInteger)specType.SpecAttributes.Single(x => x.Identifier == "requirement-integer-attribute");
            Assert.That(attributeDefinitionInteger.QueryDefaultValueAsFormattedString(), Is.EqualTo("NOT SET"));

            var attributeDefinitionReal = (AttributeDefinitionReal)specType.SpecAttributes.Single(x => x.Identifier == "requirement-real-attribute");
            Assert.That(attributeDefinitionReal.QueryDefaultValueAsFormattedString(), Is.EqualTo("NOT SET"));

            var attributeDefinitionString = (AttributeDefinitionString)specType.SpecAttributes.Single(x => x.Identifier == "requirement-string-attribute");
            Assert.That(attributeDefinitionString.QueryDefaultValueAsFormattedString(), Is.EqualTo("NOT SET"));

            var attributeDefinitionXHTML = (AttributeDefinitionXHTML)specType.SpecAttributes.Single(x => x.Identifier == "requirement-xhtml-attribute");
            Assert.That(attributeDefinitionXHTML.QueryDefaultValueAsFormattedString(), Is.EqualTo("NOT SET"));
        }
    }
}
