// -------------------------------------------------------------------------------------------------
// <copyright file="RelationGroupTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
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
    /// Suite of tests for the <see cref="RelationGroup"/>
    /// </summary>
    [TestFixture]
    public class RelationGroupTestFixture
    {
        [Test]
        public void Verify_That_WriteXml_Throws_Exception_When_Type_Is_Null()
        {
            var relationGroup = new RelationGroup();

            // Type is not set
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(() => relationGroup.WriteXml(writer),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The Type property of RelationGroup : may not be null"));
                }
            }

            // Source specification is not set
            var relationGroupType = new RelationGroupType();
            relationGroup.Type = relationGroupType;
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(() => relationGroup.WriteXml(writer),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The SourceSpecification property of RelationGroup : may not be null"));
                }
            }

            // target specification is not set
            var sourceSpecification = new Specification();
            relationGroup.SourceSpecification = sourceSpecification;
            
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(() => relationGroup.WriteXml(writer),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The TargetSpecification property of RelationGroup : may not be null"));
                }
            }
        }

        [Test]
        public void Verify_That_WriteXmlAsync_Throws_Exception_When_Type_Is_Null()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var relationGroup = new RelationGroup();

            // Type is not set
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(async () => await relationGroup.WriteXmlAsync(writer, cancellationTokenSource.Token),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The Type property of RelationGroup : may not be null"));
                }
            }

            // Source specification is not set
            var relationGroupType = new RelationGroupType();
            relationGroup.Type = relationGroupType;
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(async () => await relationGroup.WriteXmlAsync(writer, cancellationTokenSource.Token),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The SourceSpecification property of RelationGroup : may not be null"));
                }
            }

            // target specification is not set
            var sourceSpecification = new Specification();
            relationGroup.SourceSpecification = sourceSpecification;

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true }))
                {
                    Assert.That(async () => await relationGroup.WriteXmlAsync(writer, cancellationTokenSource.Token),
                        Throws.TypeOf<SerializationException>()
                            .With.Message.EqualTo("The TargetSpecification property of RelationGroup : may not be null"));
                }
            }
        }

        [Test]
        public void Verify_That_The_SpectType_Can_Be_Set()
        {
            var relationGroupType = new RelationGroupType();
            var relationGroup = new RelationGroup();

            relationGroup.SpecType = relationGroupType;

            Assert.That(relationGroup.SpecType, Is.EqualTo(relationGroupType));
        }

        [Test]
        public void Verify_That_Setting_An_Incorrect_SpecType_Throws_Exception()
        {
            var specificationType = new SpecificationType();
            var relationGroup = new RelationGroup();

            Assert.That(() => relationGroup.SpecType = specificationType,
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("specType must of type RelationGroupType")); 
        }
    }
}
