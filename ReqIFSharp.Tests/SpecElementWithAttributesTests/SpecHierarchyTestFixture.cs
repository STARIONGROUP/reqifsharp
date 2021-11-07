// -------------------------------------------------------------------------------------------------
// <copyright file="SpecHierarchyTestFixture.cs" company="RHEA System S.A.">
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

namespace ReqIFSharp.Tests.SpecElementWithAttributesTests
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="SpecHierarchy"/> class
    /// </summary>
    [TestFixture]
    public class SpecHierarchyTestFixture
    {
        private XmlWriterSettings settings;

        [SetUp]
        public void SetUp()
        {
            this.settings = new XmlWriterSettings();
        }

        [Test]
        public void Verify_that_When_Object_is_null_WriteXml_throws_exception()
        {
            var stream = new MemoryStream();
            var writer = XmlWriter.Create(stream, this.settings);

            var specHierarchy = new SpecHierarchy();

            Assert.Throws<SerializationException>(() => specHierarchy.WriteXml(writer));
        }
    }
}
