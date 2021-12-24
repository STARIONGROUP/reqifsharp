// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueDateTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="AttributeValueDate"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueDateTestFixture
    {
        [Test]
        public void VerifyThatTheAttributeDefinitionCanBeSetOrGet()
        {
            var attributeDefinitionDate = new AttributeDefinitionDate();

            var attributeValueDate = new AttributeValueDate();
            attributeValueDate.Definition = attributeDefinitionDate;

            var attributeValue = (AttributeValue)attributeValueDate;

            Assert.AreEqual(attributeDefinitionDate, attributeValue.AttributeDefinition);

            attributeValue.AttributeDefinition = attributeDefinitionDate;

            Assert.AreEqual(attributeDefinitionDate, attributeValue.AttributeDefinition);
        }

        [Test]
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var attributeDefinitionString = new AttributeDefinitionString();
            var attributeValueDate = new AttributeValueDate();
            var attributeValue = (AttributeValue)attributeValueDate;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void Verify_that_GetSchema_returns_null()
        {
            var attributeValue = new AttributeValueDate();
            Assert.That(attributeValue.GetSchema(), Is.Null);
        }

        [Test]
        public void Verify_That_WriteXml_Without_Definition_Set_Throws_SerializationException()
        {
            using var fs = new FileStream("test.xml", FileMode.Create);
            using var writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true });
            var attributeValueDate = new AttributeValueDate();
            Assert.That(() => attributeValueDate.WriteXml(writer), Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Without_Definition_Set_Throws_SerializationException()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            using var fs = new FileStream("test.xml", FileMode.Create);
            using var writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true });
            var attributeValueDate = new AttributeValueDate();
            Assert.That(async () => await attributeValueDate.WriteXmlAsync(writer, cancellationTokenSource.Token), Throws.Exception.TypeOf<SerializationException>());
        }

        [Test]
        public void VerifyConvenienceValueProperty()
        {
            var attributeValue = new AttributeValueDate();

            var date = DateTime.Now;
            attributeValue.ObjectValue = date;

            Assert.That(attributeValue.TheValue, Is.EqualTo(date));

            date = DateTime.Now;
            attributeValue.TheValue = date;
            Assert.That(attributeValue.ObjectValue, Is.EqualTo(date));
        }

        [Test]
        public void Verify_that_when_ObjectValue_is_not_Date_an_exception_is_thrown()
        {
            var attributeValue = new AttributeValueDate();

            Assert.That(
                () => attributeValue.ObjectValue = "true",
                Throws.Exception.TypeOf<InvalidOperationException>()
                    .With.Message.Contains("Cannot use true as value for this AttributeValueDate."));
        }
    }
}
