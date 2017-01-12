// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionRealTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="AttributeDefinitionReal"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionRealTestFixture
    {
        [Test]
        public void VerifyThatTheAttributeDefinitionCanBeSetOrGet()
        {
            var datatypeDefinitionReal = new DatatypeDefinitionReal();

            var attributeDefinitionReal = new AttributeDefinitionReal();
            attributeDefinitionReal.Type = datatypeDefinitionReal;

            var attributeDefinition = (AttributeDefinition)attributeDefinitionReal;

            Assert.AreEqual(datatypeDefinitionReal, attributeDefinition.DatatypeDefinition);

            attributeDefinition.DatatypeDefinition = datatypeDefinitionReal;

            Assert.AreEqual(datatypeDefinitionReal, attributeDefinition.DatatypeDefinition);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var datatypeDefinitionString = new DatatypeDefinitionString();
            var attributeDefinitionReal = new AttributeDefinitionReal();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionReal;

            attributeDefinition.DatatypeDefinition = datatypeDefinitionString;
        }

        [Test]
        public void VerifyThatWriteXmlThrowsExceptionWhenTypeIsNull()
        {
            using (var writer = XmlWriter.Create("test.xml"))
            {
                var attributeDefinitionReal = new AttributeDefinitionReal();
                Assert.Throws<SerializationException>(() => attributeDefinitionReal.WriteXml(writer));
            }
        }
    }
}
