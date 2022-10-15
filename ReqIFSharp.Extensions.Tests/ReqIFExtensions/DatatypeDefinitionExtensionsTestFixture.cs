//  -------------------------------------------------------------------------------------------------
//  <copyright file="DatatypeDefinitionExtensionsTestFixture.cs" company="RHEA System S.A.">
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
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.ReqIFExtensions;
    using ReqIFSharp.Extensions.Tests.TestData;

    /// <summary>
    /// Suite of tests for the <see cref="DatatypeDefinitionExtensions"/> class
    /// </summary>
    [TestFixture]
    public class DatatypeDefinitionExtensionsTestFixture
    {
        [Test]
        public void Verify_that_QueryDatatypeName_returns_expected_results()
        {
            var datatypeDefinitionBoolean = new DatatypeDefinitionBoolean();
            Assert.That(datatypeDefinitionBoolean.QueryDatatypeName(), Is.EqualTo("Boolean"));

            var datatypeDefinitionDate = new DatatypeDefinitionDate();
            Assert.That(datatypeDefinitionDate.QueryDatatypeName(), Is.EqualTo("Date"));

            var datatypeDefinitionEnumeration = new DatatypeDefinitionEnumeration();
            Assert.That(datatypeDefinitionEnumeration.QueryDatatypeName(), Is.EqualTo("Enumeration"));

            var datatypeDefinitionInteger = new DatatypeDefinitionInteger();
            Assert.That(datatypeDefinitionInteger.QueryDatatypeName(), Is.EqualTo("Integer"));

            var datatypeDefinitionReal = new DatatypeDefinitionReal();
            Assert.That(datatypeDefinitionReal.QueryDatatypeName(), Is.EqualTo("Real"));

            var datatypeDefinitionString = new DatatypeDefinitionString();
            Assert.That(datatypeDefinitionString.QueryDatatypeName(), Is.EqualTo("String"));

            var datatypeDefinitionXhtml = new DatatypeDefinitionXHTML();
            Assert.That(datatypeDefinitionXhtml.QueryDatatypeName(), Is.EqualTo("XHTML"));
        }

        [Test]
        public void Verify_that_QueryReferencingAttributeDefinitions_returns_expected_results()
        {
            var testDataCreator = new ReqIFTestDataCreator();
            var reqif = testDataCreator.Create();

            var datatypeDefinitionBoolean = (DatatypeDefinitionBoolean)reqif.CoreContent.DataTypes.Single(x => x.Identifier == "boolean");

            var attributeDefinitions = datatypeDefinitionBoolean.QueryReferencingAttributeDefinitions();

            Assert.That(attributeDefinitions.Count(), Is.EqualTo(4));

            var attributeDefinition = attributeDefinitions.Single(x => x.Identifier == "specification-boolean-attribute");

            Assert.That(attributeDefinition.LongName, Is.EqualTo("boolean attribute"));
        }

        [Test]
        public void Verify_that_on_QueryReferencingAttributeDefinitions_NullReferenceException_is_thrown_when_owning_ReqIFContent_is_not_set()
        {
            var datatypeDefinitionBoolean = new DatatypeDefinitionBoolean();
            
            Assert.That(() => datatypeDefinitionBoolean.QueryReferencingAttributeDefinitions(),
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("The owning ReqIFContent of the DatatypeDefinition is not set."));
        }

        [Test]
        public void Verify_that_Unsupported_QueryDatatypeName_throws_exception()
        {
            var testDatatypeDefinitionSimple = new TestDatatypeDefinitionSimple();

            Assert.That(() => testDatatypeDefinitionSimple.QueryDatatypeName(),
                Throws.Exception.TypeOf<InvalidOperationException>());
        }
    }

    /// <summary>
    /// test class
    /// </summary>
    public class TestDatatypeDefinitionSimple : DatatypeDefinitionSimple
    {
        internal override Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
