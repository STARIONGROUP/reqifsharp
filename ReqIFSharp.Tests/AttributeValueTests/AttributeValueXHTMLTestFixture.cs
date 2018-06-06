// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueXHTMLTestFixture.cs" company="RHEA System S.A.">
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
    using System.Text;
    using System.Xml;

    using NUnit.Framework;
    using ReqIFSharp;

    /// <summary>
    /// Suite of tests for the <see cref="AttributeValueXHTML"/>
    /// </summary>
    [TestFixture]
    public class AttributeValueXHTMLTestFixture
    {
        [Test]
        public void VerifyThatTheAttributeDefinitionCanBeSetOrGet()
        {
            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();

            var attributeValueXhtml = new AttributeValueXHTML();
            attributeValueXhtml.Definition = attributeDefinitionXhtml;

            var attributeValue = (AttributeValue)attributeValueXhtml;

            Assert.AreEqual(attributeDefinitionXhtml, attributeValue.AttributeDefinition);

            attributeValue.AttributeDefinition = attributeDefinitionXhtml;

            Assert.AreEqual(attributeDefinitionXhtml, attributeValue.AttributeDefinition);
        }

        [Test]
        public void VerifytThatExceptionIsRaisedWhenInvalidAttributeDefinitionIsSet()
        {
            var attributeDefinitionString = new AttributeDefinitionReal();
            var attributeValueXhtml = new AttributeValueXHTML();
            var attributeValue = (AttributeValue)attributeValueXhtml;

            Assert.Throws<ArgumentException>(() => attributeValue.AttributeDefinition = attributeDefinitionString);
        }

        [Test]
        public void VerifyConvenienceValueProperty()
        {
            var attributeValue = new AttributeValueXHTML();

            var val = "testetestes";
            attributeValue.ObjectValue = val;

            Assert.AreEqual(attributeValue.TheValue, val);
        }
    }
}