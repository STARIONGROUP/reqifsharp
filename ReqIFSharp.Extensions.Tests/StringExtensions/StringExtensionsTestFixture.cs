// -------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.Tests.StringExtensions
{
    using NUnit.Framework;

    using ReqIFSharp.Extensions;

    /// <summary>
    /// Suite of tests for the <see cref="StringExtensions"/> class
    /// </summary>
    [TestFixture]
    public class StringExtensionsTestFixture
    {
        private string decoded = "testing 1 2 3";

        private string encoded = "dGVzdGluZyAxIDIgMw==";

        [Test]
        public void Verify_that_Base64Encode_returns_Expected_results()
        {
            var base64Encoded = StringExtensions.Base64Encode(this.decoded);

            Assert.That(base64Encoded, Is.EqualTo(this.encoded));
        }

        [Test]
        public void Verify_that_Base64Decode_returns_Expected_results()
        {
            var base64Decoded = StringExtensions.Base64Decode(this.encoded);

            Assert.That(base64Decoded, Is.EqualTo(this.decoded));
        }
    }
}
