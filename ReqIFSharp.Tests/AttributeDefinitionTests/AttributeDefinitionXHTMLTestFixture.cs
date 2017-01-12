// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionXHTMLTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="AttributeDefinitionXHTML"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionXHTMLTestFixture
    {
        [Test]
        public void VerifyThatTheAttributeDefinitionCanBeSetOrGet()
        {
            var datatypeDefinitionXhtml = new DatatypeDefinitionXHTML();

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            attributeDefinitionXhtml.Type = datatypeDefinitionXhtml;

            var attributeDefinition = (AttributeDefinition)attributeDefinitionXhtml;

            Assert.AreEqual(datatypeDefinitionXhtml, attributeDefinition.DatatypeDefinition);

            attributeDefinition.DatatypeDefinition = datatypeDefinitionXhtml;

            Assert.AreEqual(datatypeDefinitionXhtml, attributeDefinition.DatatypeDefinition);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var datatypeDefinitionDate = new DatatypeDefinitionDate();
            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionXhtml;

            attributeDefinition.DatatypeDefinition = datatypeDefinitionDate;
        }

        [Test]
        public void VerifyThatWriteXmlThrowsExceptionWhenTypeIsNull()
        {
            using (var writer = XmlWriter.Create("test.xml"))
            {
                var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
                Assert.Throws<SerializationException>(() => attributeDefinitionXhtml.WriteXml(writer));
            }
        }
    }
}
