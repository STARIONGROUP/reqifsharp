// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionEnumerationTestFixture.cs" company="RHEA System S.A.">
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
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;

    using NUnit.Framework;
    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeDefinitionEnumeration"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionEnumerationTestFixture
    {
        [Test]
        public void VerifyThatTheAttributeDefinitionCanBeSetOrGet()
        {
            var datatypeDefinitionEnumeration = new DatatypeDefinitionEnumeration();

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            attributeDefinitionEnumeration.Type = datatypeDefinitionEnumeration;

            var attributeDefinition = (AttributeDefinition)attributeDefinitionEnumeration;

            Assert.AreEqual(datatypeDefinitionEnumeration, attributeDefinition.DatatypeDefinition);

            attributeDefinition.DatatypeDefinition = datatypeDefinitionEnumeration;

            Assert.AreEqual(datatypeDefinitionEnumeration, attributeDefinition.DatatypeDefinition);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var datatypeDefinitionString = new DatatypeDefinitionString();
            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionEnumeration;

            attributeDefinition.DatatypeDefinition = datatypeDefinitionString;
        }

        [Test]
        public void VerifyThatWriteXmlThrowsExceptionWhenTypeIsNull()
        {
            using (var writer = XmlWriter.Create("test.xml"))
            {
                var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
                Assert.Throws<SerializationException>(() => attributeDefinitionEnumeration.WriteXml(writer));
            }            
        }
    }
}
