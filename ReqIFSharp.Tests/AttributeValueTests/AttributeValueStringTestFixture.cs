// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueStringTestFixture.cs" company="RHEA System S.A.">
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

namespace ReqIFLib.Tests
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeValueString"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueStringTestFixture
    {
        [Test]
        public void VerifyThatTheAttributeDefinitionCanBeSetOrGet()
        {
            var attributeDefinitionString = new AttributeDefinitionString();

            var attributeValueString = new AttributeValueString();
            attributeValueString.Definition = attributeDefinitionString;

            var attributeValue = (AttributeValue)attributeValueString;

            Assert.AreEqual(attributeDefinitionString, attributeValue.AttributeDefinition);

            attributeValue.AttributeDefinition = attributeDefinitionString;

            Assert.AreEqual(attributeDefinitionString, attributeValue.AttributeDefinition);
        }

        [Test]
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var attributeDefinitionString = new AttributeDefinitionReal();
            var attributeValueString = new AttributeValueString();
            var attributeValue = (AttributeValue)attributeValueString;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void VerifyThatWriteXmlWithoutDefinitionSetThrowsSerializationException()
        {
            using (var fs = new FileStream("test.xml", FileMode.Create))
            {
                using (var writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true }))
                {
                    var attributeValueString = new AttributeValueString();
                    Assert.Throws<SerializationException>(() => attributeValueString.WriteXml(writer));
                }
            }
        }

        [Test]
        public void VerifyConvenienceValueProperty()
        {
            var attributeValue = new AttributeValueString();

            var val = "test";
            attributeValue.ObjectValue = val;

            Assert.AreEqual(attributeValue.TheValue, val);
        }
    }
}
