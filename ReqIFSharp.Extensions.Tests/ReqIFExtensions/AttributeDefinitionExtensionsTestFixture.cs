//  -------------------------------------------------------------------------------------------------
//  <copyright file="AttributeDefinitionExtensionsTestFixture.cs" company="Starion Group S.A.">
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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.ReqIFExtensions;
    using ReqIFSharp.Extensions.Tests.TestData;

    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeDefinitionExtensions"/> class
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionExtensionsTestFixture
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
        public void Verify_that_QueryDatatypeName_returns_expected_results()
        {
            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            Assert.That(attributeDefinitionBoolean.QueryDatatypeName(), Is.EqualTo("Boolean"));

            attributeDefinitionBoolean = new AttributeDefinitionBoolean(this.loggerFactory);
            Assert.That(attributeDefinitionBoolean.QueryDatatypeName(), Is.EqualTo("Boolean"));

            var attributeDefinitionDate = new AttributeDefinitionDate();
            Assert.That(attributeDefinitionDate.QueryDatatypeName(), Is.EqualTo("Date"));

            attributeDefinitionDate = new AttributeDefinitionDate(this.loggerFactory);
            Assert.That(attributeDefinitionDate.QueryDatatypeName(), Is.EqualTo("Date"));

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            Assert.That(attributeDefinitionEnumeration.QueryDatatypeName(), Is.EqualTo("Enumeration"));

            attributeDefinitionEnumeration = new AttributeDefinitionEnumeration(this.loggerFactory);
            Assert.That(attributeDefinitionEnumeration.QueryDatatypeName(), Is.EqualTo("Enumeration"));

            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            Assert.That(attributeDefinitionInteger.QueryDatatypeName(), Is.EqualTo("Integer"));

            attributeDefinitionInteger = new AttributeDefinitionInteger(this.loggerFactory);
            Assert.That(attributeDefinitionInteger.QueryDatatypeName(), Is.EqualTo("Integer"));

            var attributeDefinitionReal = new AttributeDefinitionReal();
            Assert.That(attributeDefinitionReal.QueryDatatypeName(), Is.EqualTo("Real"));

            attributeDefinitionReal = new AttributeDefinitionReal(this.loggerFactory);
            Assert.That(attributeDefinitionReal.QueryDatatypeName(), Is.EqualTo("Real"));

            var attributeDefinitionString = new AttributeDefinitionString();
            Assert.That(attributeDefinitionString.QueryDatatypeName(), Is.EqualTo("String"));

            attributeDefinitionString = new AttributeDefinitionString(this.loggerFactory);
            Assert.That(attributeDefinitionString.QueryDatatypeName(), Is.EqualTo("String"));

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            Assert.That(attributeDefinitionXhtml.QueryDatatypeName(), Is.EqualTo("XHTML"));

            attributeDefinitionXhtml = new AttributeDefinitionXHTML(this.loggerFactory);
            Assert.That(attributeDefinitionXhtml.QueryDatatypeName(), Is.EqualTo("XHTML"));
        }

        [Test]
        public void Verify_that_QueryDatatypeName_throws_when_input_is_null()
        {
            Assert.That(() => AttributeDefinitionExtensions.QueryDatatypeName(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryDatatypeName_throws_when_type_is_not_supported()
        {
            var unsupportedAttributeDefinition = new UnsupportedAttributeDefinition();

            Assert.That(
                () => unsupportedAttributeDefinition.QueryDatatypeName(),
                Throws.Exception.TypeOf<InvalidOperationException>());
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

        [Test]
        public void Verify_that_QueryDefaultValueAsFormattedString_returns_formatted_default_values()
        {
            var attributeDefinitionBoolean = new AttributeDefinitionBoolean
            {
                DefaultValue = new AttributeValueBoolean { TheValue = true }
            };

            Assert.That(attributeDefinitionBoolean.QueryDefaultValueAsFormattedString(), Is.EqualTo("True"));

            var attributeDefinitionDate = new AttributeDefinitionDate
            {
                DefaultValue = new AttributeValueDate { TheValue = new DateTime(2020, 12, 25) }
            };

            Assert.That(attributeDefinitionDate.QueryDefaultValueAsFormattedString(), Is.EqualTo("December 25, 2020"));

            var enumValue = new EnumValue { Identifier = "enum" };
            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration
            {
                DefaultValue = new AttributeValueEnumeration()
            };
            attributeDefinitionEnumeration.DefaultValue.Values.Add(enumValue);

            Assert.That(
                attributeDefinitionEnumeration.QueryDefaultValueAsFormattedString(),
                Is.EqualTo(enumValue.ToString()));

            var attributeDefinitionInteger = new AttributeDefinitionInteger
            {
                DefaultValue = new AttributeValueInteger { TheValue = 42 }
            };

            Assert.That(attributeDefinitionInteger.QueryDefaultValueAsFormattedString(), Is.EqualTo("42"));

            var attributeDefinitionReal = new AttributeDefinitionReal
            {
                DefaultValue = new AttributeValueReal { TheValue = 4.2 }
            };

            Assert.That(attributeDefinitionReal.QueryDefaultValueAsFormattedString(), Is.EqualTo("4.2"));

            var attributeDefinitionString = new AttributeDefinitionString
            {
                DefaultValue = new AttributeValueString { TheValue = "value" }
            };

            Assert.That(attributeDefinitionString.QueryDefaultValueAsFormattedString(), Is.EqualTo("value"));

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML
            {
                DefaultValue = new AttributeValueXHTML { TheValue = "<xhtml:p>content</xhtml:p>" }
            };

            Assert.That(attributeDefinitionXhtml.QueryDefaultValueAsFormattedString(), Is.EqualTo("<xhtml:p>content</xhtml:p>"));
        }

        [Test]
        public void Verify_that_QueryDefaultValueAsFormattedString_throws_when_input_is_null()
        {
            Assert.That(() => AttributeDefinitionExtensions.QueryDefaultValueAsFormattedString(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryDefaultValueAsFormattedString_throws_when_type_is_not_supported()
        {
            var unsupportedAttributeDefinition = new UnsupportedAttributeDefinition();

            Assert.That(
                () => unsupportedAttributeDefinition.QueryDefaultValueAsFormattedString(),
                Throws.Exception.TypeOf<InvalidOperationException>());
        }

        private class UnsupportedAttributeDefinition : AttributeDefinition
        {
            protected override DatatypeDefinition GetDatatypeDefinition()
            {
                return null;
            }

            protected override void SetDatatypeDefinition(DatatypeDefinition datatypeDefinition)
            {
            }

            internal override Task ReadXmlAsync(System.Xml.XmlReader reader, CancellationToken token)
            {
                return Task.CompletedTask;
            }
        }
    }
}
