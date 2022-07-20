//  -------------------------------------------------------------------------------------------------
//  <copyright file="StringExtensionsTestFixture.cs" company="RHEA System S.A.">
// 
//    Copyright 2017-2022 RHEA System S.A.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  -------------------------------------------------------------------------------------------------

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
