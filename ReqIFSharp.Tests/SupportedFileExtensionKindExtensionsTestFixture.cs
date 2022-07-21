// -------------------------------------------------------------------------------------------------
// <copyright file="SupportedFileExtensionKindExtensionsTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2022 RHEA System S.A.
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
    using System.IO;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="SupportedFileExtensionKindExtensions"/>
    /// </summary>
    [TestFixture]
    public class SupportedFileExtensionKindExtensionsTestFixture
    {
        [Test]
        public void Verify_that_ConvertPathToSupportedFileExtensionKind_returns_the_expected_result()
        {
            var reqif = Path.Combine(TestContext.CurrentContext.TestDirectory, "result.reqif");
            var reqifz = Path.Combine(TestContext.CurrentContext.TestDirectory, "result.reqifz");

            Assert.Multiple(() =>
            {
                Assert.That(reqif.ConvertPathToSupportedFileExtensionKind(),
                    Is.EqualTo(SupportedFileExtensionKind.Reqif));
                Assert.That(reqifz.ConvertPathToSupportedFileExtensionKind(),
                    Is.EqualTo(SupportedFileExtensionKind.Reqifz));
            });
        }
    }
}
