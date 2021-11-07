// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueIntegerTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2019 RHEA System S.A.
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
    /// Suite of tests for the <see cref="AttributeValueInteger"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueIntegerTestFixture
    {
        [Test]
        public void VerifyThatTheAttributeDefinitionCanBeSetOrGet()
        {
            var attributeDefinitionInteger = new AttributeDefinitionInteger();

            var attributeValueInteger = new AttributeValueInteger();
            attributeValueInteger.Definition = attributeDefinitionInteger;

            var attributeValue = (AttributeValue)attributeValueInteger;

            Assert.AreEqual(attributeDefinitionInteger, attributeValue.AttributeDefinition);

            attributeValue.AttributeDefinition = attributeDefinitionInteger;

            Assert.AreEqual(attributeDefinitionInteger, attributeValue.AttributeDefinition);
        }

        [Test]
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var attributeDefinitionString = new AttributeDefinitionString();
            var attributeValueInteger = new AttributeValueInteger();
            var attributeValue = (AttributeValue)attributeValueInteger;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void Verify_that_GetSchema_returns_null()
        {
            var attributeValue = new AttributeValueInteger();
            Assert.That(attributeValue.GetSchema(), Is.Null);
        }

        [Test]
        public void VerifyThatWriteXmlWithoutDefinitionSetThrowsSerializationException()
        {
            using (var fs = new FileStream("test.xml", FileMode.Create))
            {
                using (var writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true }))
                {
                    var attributeValueInteger = new AttributeValueInteger();
                    Assert.Throws<SerializationException>(() => attributeValueInteger.WriteXml(writer));
                }
            }
        }

        [Test]
        public void VerifyConvenienceValueProperty()
        {
            var attributeValue = new AttributeValueInteger();

            long val = 3;
            attributeValue.ObjectValue = val;

            Assert.That(attributeValue.TheValue, Is.EqualTo(val));
        }

        [Test]
        public void Verify_that_when_ObjectValue_is_not_integer_an_exception_is_thrown()
        {
            var attributeValue = new AttributeValueInteger();

            Assert.That(
                () => attributeValue.ObjectValue = "true",
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("Cannot use true as value for this AttributeValueInteger."));
        }
    }
}