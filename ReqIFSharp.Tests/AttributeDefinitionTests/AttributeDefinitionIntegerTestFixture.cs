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
    using System.Text;
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
        public void VerifyThatTheAttributeDefinitionCanBeSetOrGet()
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
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var datatypeDefinitionString = new DatatypeDefinitionString();
            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionInteger;

            Assert.Throws<ArgumentException>(() => attributeDefinition.DatatypeDefinition = datatypeDefinitionString);
        }

        [Test]
        public void VerifyThatWriteXmlThrowsExceptionWhenTypeIsNull()
        {
            using (var fs = new FileStream("test.xml", FileMode.Create))
            {
                using (var writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true }))
                {
                    var attributeDefinitionInteger = new AttributeDefinitionInteger();
                    Assert.Throws<SerializationException>(() => attributeDefinitionInteger.WriteXml(writer));
                }
            }
        }
    }
}
