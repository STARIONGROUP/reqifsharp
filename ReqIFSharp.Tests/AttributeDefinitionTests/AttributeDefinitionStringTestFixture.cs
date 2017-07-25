// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionStringTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="AttributeDefinitionString"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionStringTestFixture
    {
        [Test]
        public void VerifyThatTheAttributeDefinitionCanBeSetOrGet()
        {
            var datatypeDefinitionString = new DatatypeDefinitionString();

            var attributeDefinitionString = new AttributeDefinitionString();
            attributeDefinitionString.Type = datatypeDefinitionString;

            var attributeDefinition = (AttributeDefinition)attributeDefinitionString;

            Assert.AreEqual(datatypeDefinitionString, attributeDefinition.DatatypeDefinition);

            attributeDefinition.DatatypeDefinition = datatypeDefinitionString;

            Assert.AreEqual(datatypeDefinitionString, attributeDefinition.DatatypeDefinition);
        }

        [Test]
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var datatypeDefinitionDate = new DatatypeDefinitionDate();
            var attributeDefinitionString = new AttributeDefinitionString();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionString;

            Assert.Throws<ArgumentException>(() => attributeDefinition.DatatypeDefinition = datatypeDefinitionDate);
        }

        [Test]
        public void VerifyThatWriteXmlThrowsExceptionWhenTypeIsNull()
        {
            using(var writer = XmlWriter.Create("test.xml"))
            {
                var attributeDefinitionString = new AttributeDefinitionString();
                Assert.Throws<SerializationException>(() => attributeDefinitionString.WriteXml(writer));
            }
        }
    }
}
