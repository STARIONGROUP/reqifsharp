// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIfFactoryTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
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
// -------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIfFactory"/> class.
    /// </summary>
    [TestFixture]
    public class ReqIfFactoryTestFixture
    {
        [Test]
        public void VerifyThatXmlElementNameReturnsAttributeDefinition()
        {
            var spectType = new SpecObjectType();

            Assert.IsInstanceOf<AttributeDefinitionBoolean>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-BOOLEAN", spectType));
            Assert.IsInstanceOf<AttributeDefinitionDate>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-DATE", spectType));
            Assert.IsInstanceOf<AttributeDefinitionEnumeration>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-ENUMERATION", spectType));
            Assert.IsInstanceOf<AttributeDefinitionInteger>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-INTEGER", spectType));
            Assert.IsInstanceOf<AttributeDefinitionReal>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-REAL", spectType));
            Assert.IsInstanceOf<AttributeDefinitionString>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-STRING", spectType));
            Assert.IsInstanceOf<AttributeDefinitionXHTML>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-XHTML", spectType));
        }

        [Test]
        public void VerifyThatUnkownElementAttributeDefinitionThrowsArgumentException()
        {
            var spectType = new SpecObjectType();

            string unknownName = "RHEA";
            Assert.IsNull(ReqIfFactory.AttributeDefinitionConstruct(unknownName, spectType));
        }

        [Test]
        public void VerifyThatXmlElementNameReturnsDataTypeDefinition()
        {
            var reqIfContent = new ReqIFContent();

            Assert.IsInstanceOf<DatatypeDefinitionBoolean>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-BOOLEAN", reqIfContent));
            Assert.IsInstanceOf<DatatypeDefinitionDate>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-DATE", reqIfContent));
            Assert.IsInstanceOf<DatatypeDefinitionEnumeration>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-ENUMERATION", reqIfContent));
            Assert.IsInstanceOf<DatatypeDefinitionInteger>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-INTEGER", reqIfContent));
            Assert.IsInstanceOf<DatatypeDefinitionReal>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-REAL", reqIfContent));
            Assert.IsInstanceOf<DatatypeDefinitionString>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-STRING", reqIfContent));
            Assert.IsInstanceOf<DatatypeDefinitionXHTML>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-XHTML", reqIfContent));
        }

        [Test]        
        public void VerifyThatUnkownElementDataTypeDefinitionThrowsArgumentException()
        {
            var reqIfContent = new ReqIFContent();

            string unknownName = "RHEA";
            Assert.Throws<ArgumentException>(() => ReqIfFactory.DatatypeDefinitionConstruct(unknownName, reqIfContent));
        }
    }
}
