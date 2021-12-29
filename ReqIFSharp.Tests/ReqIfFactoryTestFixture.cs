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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIfFactory"/> class.
    /// </summary>
    [TestFixture]
    public class ReqIfFactoryTestFixture
    {
        private ILoggerFactory loggerFactory;

        [SetUp]
        public void SetUp()
        {
            this.loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        }

        [Test]
        public void Verify_That_XmlElementName_Returns_AttributeDefinition()
        {
            var spectType = new SpecObjectType();

            Assert.IsInstanceOf<AttributeDefinitionBoolean>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-BOOLEAN", spectType, this.loggerFactory));
            Assert.IsInstanceOf<AttributeDefinitionDate>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-DATE", spectType, this.loggerFactory));
            Assert.IsInstanceOf<AttributeDefinitionEnumeration>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-ENUMERATION", spectType, this.loggerFactory));
            Assert.IsInstanceOf<AttributeDefinitionInteger>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-INTEGER", spectType, this.loggerFactory));
            Assert.IsInstanceOf<AttributeDefinitionReal>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-REAL", spectType, this.loggerFactory));
            Assert.IsInstanceOf<AttributeDefinitionString>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-STRING", spectType, this.loggerFactory));
            Assert.IsInstanceOf<AttributeDefinitionXHTML>(ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-XHTML", spectType, this.loggerFactory));
        }

        [Test]
        public void Verify_That_Unkown_Element_AttributeDefinition_Throws_ArgumentException()
        {
            var spectType = new SpecObjectType();

            string unknownName = "RHEA";
            Assert.IsNull(ReqIfFactory.AttributeDefinitionConstruct(unknownName, spectType, this.loggerFactory));
        }

        [Test]
        public void Verify_That_XmlElementName_Returns_DataTypeDefinition()
        {
            var reqIfContent = new ReqIFContent();

            Assert.IsInstanceOf<DatatypeDefinitionBoolean>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-BOOLEAN", reqIfContent, this.loggerFactory));
            Assert.IsInstanceOf<DatatypeDefinitionDate>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-DATE", reqIfContent, this.loggerFactory));
            Assert.IsInstanceOf<DatatypeDefinitionEnumeration>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-ENUMERATION", reqIfContent, this.loggerFactory));
            Assert.IsInstanceOf<DatatypeDefinitionInteger>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-INTEGER", reqIfContent, this.loggerFactory));
            Assert.IsInstanceOf<DatatypeDefinitionReal>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-REAL", reqIfContent, this.loggerFactory));
            Assert.IsInstanceOf<DatatypeDefinitionString>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-STRING", reqIfContent, this.loggerFactory));
            Assert.IsInstanceOf<DatatypeDefinitionXHTML>(ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-XHTML", reqIfContent, this.loggerFactory));
        }

        [Test]
        public void Verify_That_Unkown_Element_DataTypeDefinition_Throws_ArgumentException()
        {
            var reqIfContent = new ReqIFContent();

            string unknownName = "RHEA";
            Assert.That(() => ReqIfFactory.DatatypeDefinitionConstruct(unknownName, reqIfContent, this.loggerFactory), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Verify_that_unknown_SpecTypeName_throws_exception()
        {
            var reqIfContent = new ReqIFContent();

            string unknownName = "RHEA";
            Assert.That(() => ReqIfFactory.SpecTypeConstruct(unknownName, reqIfContent, this.loggerFactory), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Verify_that_AttributeDefinition_XmlName_throws_exception_for_unsupported_type()
        {
            var testAttributeDefinition = new TestAttributeDefinition();

            Assert.That(() => ReqIfFactory.XmlName(testAttributeDefinition), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Verify_that_DatatypeDefinition_XmlName_throws_exception_for_unsupported_type()
        {
            var testDatatypeDefinition = new TestDatatypeDefinition();

            Assert.That(() => ReqIfFactory.XmlName(testDatatypeDefinition), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Verify_that_SpecType_XmlName_throws_exception_for_unsupported_type()
        {
            var specType = new TestSpecType();

            Assert.That(() => ReqIfFactory.XmlName(specType), Throws.Exception.TypeOf<ArgumentException>());
        }

        private class TestAttributeDefinition : AttributeDefinition
        {
            protected override DatatypeDefinition GetDatatypeDefinition()
            {
                throw new NotImplementedException();
            }

            protected override void SetDatatypeDefinition(DatatypeDefinition datatypeDefinition)
            {
                throw new NotImplementedException();
            }

            internal override Task ReadXmlAsync(XmlReader reader, CancellationToken token)
            {
                throw new NotImplementedException();
            }
        }

        private class TestDatatypeDefinition : DatatypeDefinition
        {
            internal override Task ReadXmlAsync(XmlReader reader, CancellationToken token)
            {
                throw new NotImplementedException();
            }
        }

        private class TestSpecType : SpecType
        {
        }
    }
}
