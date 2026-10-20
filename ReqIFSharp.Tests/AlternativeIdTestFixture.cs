// -------------------------------------------------------------------------------------------------
// <copyright file="AlternativeIdTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
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
