// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionIntegerTestFixture.cs" company="RHEA System S.A.">
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
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeDefinitionInteger"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionIntegerTestFixture
    {
        [Test]
        public void Verify_That_The_AttributeDefinition_Can_Be_Set_Or_Get()
        {
            var datatypeDefinitionInteger = new DatatypeDefinitionInteger();

            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            attributeDefinitionInteger.Type = datatypeDefinitionInteger;

            var attributeDefinition = (AttributeDefinition)attributeDefinitionInteger;

            Assert.AreEqual(datatypeDefinitionInteger, attributeDefinition.DatatypeDefinition);

            attributeDefinition.DatatypeDefinition = datatypeDefinitionInteger;

            Assert.AreEqual(datatypeDefinitionInteger, attributeDefinition.DatatypeDefinition);
        }

        [Test]
        public void Verify_That_Exception_Is_Raised_When_Invalid_AttributeDefinition_Is_Set()
        {
            var datatypeDefinitionString = new DatatypeDefinitionString();
            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionInteger;

            Assert.Throws<ArgumentException>(() => attributeDefinition.DatatypeDefinition = datatypeDefinitionString);
        }

        [Test]
        public void Verify_that_GetSchema_returns_null()
        {
            var attribute = new AttributeDefinitionInteger();
            Assert.That(attribute.GetSchema(), Is.Null);
        }

        [Test]
        public void Verify_That_WriteXml_Throws_Exception_When_Type_Is_Null()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeDefinitionInteger = new AttributeDefinitionInteger();

            Assert.That(() => attributeDefinitionInteger.WriteXml(writer), Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_When_Type_Is_Null()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });
            var attributeDefinitionInteger = new AttributeDefinitionInteger();

            Assert.That(async () => await attributeDefinitionInteger.WriteXmlAsync(writer, cancellationTokenSource.Token), Throws.Exception.TypeOf<SerializationException>());
        }
    }
}
