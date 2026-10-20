// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIfFactoryTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="ReqIfFactory"/> class.
    /// </summary>
    [TestFixture]
    public class ReqIfFactoryTestFixture
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
        public void Verify_That_XmlElementName_Returns_AttributeDefinition()
        {
            var spectType = new SpecObjectType();
            Assert.That(() => ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-BOOLEAN", spectType, this.loggerFactory), Is.InstanceOf<AttributeDefinitionBoolean>());
            Assert.That(() => ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-DATE", spectType, this.loggerFactory), Is.InstanceOf<AttributeDefinitionDate>());
            Assert.That(() => ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-ENUMERATION", spectType, this.loggerFactory), Is.InstanceOf<AttributeDefinitionEnumeration>());
            Assert.That(() => ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-INTEGER", spectType, this.loggerFactory), Is.InstanceOf<AttributeDefinitionInteger>());
            Assert.That(() => ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-REAL", spectType, this.loggerFactory), Is.InstanceOf<AttributeDefinitionReal>());
            Assert.That(() => ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-STRING", spectType, this.loggerFactory), Is.InstanceOf<AttributeDefinitionString>());
            Assert.That(() => ReqIfFactory.AttributeDefinitionConstruct("ATTRIBUTE-DEFINITION-XHTML", spectType, this.loggerFactory), Is.InstanceOf<AttributeDefinitionXHTML>());
        }

        [Test]
        public void Verify_That_Unkown_Element_AttributeDefinition_Throws_ArgumentException()
        {
            var spectType = new SpecObjectType(this.loggerFactory);

            string unknownName = "Starion";
            Assert.That(ReqIfFactory.AttributeDefinitionConstruct(unknownName, spectType, this.loggerFactory), Is.Null);
        }

        [Test]
        public void Verify_That_XmlElementName_Returns_DataTypeDefinition()
        {
            var reqIfContent = new ReqIFContent(this.loggerFactory);

            Assert.That(() => ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-BOOLEAN", reqIfContent, this.loggerFactory), Is.InstanceOf<DatatypeDefinitionBoolean>());
            Assert.That(() => ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-DATE", reqIfContent, this.loggerFactory), Is.InstanceOf<DatatypeDefinitionDate>());
            Assert.That(() => ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-ENUMERATION", reqIfContent, this.loggerFactory), Is.InstanceOf<DatatypeDefinitionEnumeration>());
            Assert.That(() => ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-INTEGER", reqIfContent, this.loggerFactory), Is.InstanceOf<DatatypeDefinitionInteger>()); Assert.That(() => ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-REAL", reqIfContent, this.loggerFactory), Is.InstanceOf<DatatypeDefinitionReal>());
            Assert.That(() => ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-STRING", reqIfContent, this.loggerFactory), Is.InstanceOf<DatatypeDefinitionString>());
            Assert.That(() => ReqIfFactory.DatatypeDefinitionConstruct("DATATYPE-DEFINITION-XHTML", reqIfContent, this.loggerFactory), Is.InstanceOf<DatatypeDefinitionXHTML>());
        }

        [Test]
        public void Verify_That_Unknown_Element_DataTypeDefinition_Throws_ArgumentException()
        {
            var reqIfContent = new ReqIFContent(this.loggerFactory);

            string unknownName = "Starion";
            Assert.That(() => ReqIfFactory.DatatypeDefinitionConstruct(unknownName, reqIfContent, this.loggerFactory), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Verify_that_unknown_SpecTypeName_throws_exception()
        {
            var reqIfContent = new ReqIFContent(this.loggerFactory);

            string unknownName = "Starion";
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
