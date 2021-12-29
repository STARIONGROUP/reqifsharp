// -------------------------------------------------------------------------------------------------
// <copyright file="AlternativeIdTestFixture.cs" company="RHEA System S.A.">
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
    using System.Threading;
    using System.Xml;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="AlternativeId"/> class
    /// </summary>
    [TestFixture]
    public class AlternativeIdTestFixture
    {
        [Test]
        public void Verify_that_constructor_works_as_expected()
        {
            var specObject = new SpecObject();

            var alternativeId = new AlternativeId(specObject);

            Assert.That(specObject.AlternativeId, Is.EqualTo(alternativeId));

            Assert.That(alternativeId.Ident, Is.EqualTo(specObject));
        }

        [Test]
        public void Verify_That_WriteXmlAsync_throws_exception_when_cancelled()
        {
            using var memoryStream = new MemoryStream();
            using var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true });

            var alternativeId = new AlternativeId();

            var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.That(
                async () => await alternativeId.WriteXmlAsync(writer, cts.Token),
                Throws.Exception.TypeOf<OperationCanceledException>());
        }
    }
}
