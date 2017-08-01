// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionDateTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="AttributeDefinitionDate"/>
    /// </summary>
    [TestFixture]
    public class AttributeDefinitionDateTestFixture
    {
        [Test]
        public void VerifyThatTheAttributeDefinitionCanBeSetOrGet()
        {
            var datatypeDefinitionDate = new DatatypeDefinitionDate();

            var attributeDefinitionDate = new AttributeDefinitionDate();
            attributeDefinitionDate.Type = datatypeDefinitionDate;

            var attributeDefinition = (AttributeDefinition)attributeDefinitionDate;

            Assert.AreEqual(datatypeDefinitionDate, attributeDefinition.DatatypeDefinition);

            attributeDefinition.DatatypeDefinition = datatypeDefinitionDate;

            Assert.AreEqual(datatypeDefinitionDate, attributeDefinition.DatatypeDefinition);
        }

        [Test]
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var datatypeDefinitionString = new DatatypeDefinitionString();
            var attributeDefinitionDate = new AttributeDefinitionDate();
            var attributeDefinition = (AttributeDefinition)attributeDefinitionDate;

            Assert.Throws<ArgumentException>(() => attributeDefinition.DatatypeDefinition = datatypeDefinitionString);
        }

        [Test]
        public void VerifyThatWriteXmlThrowsExceptionWhenTypeIsNull()
        {       
            using (var fs = new FileStream("test.xml", FileMode.Create))
            {
                using (var writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true }))
                {
                    var attributeDefinitionDate = new AttributeDefinitionDate();
                    Assert.Throws<SerializationException>(() => attributeDefinitionDate.WriteXml(writer));
                }
            }
        }
    }
}