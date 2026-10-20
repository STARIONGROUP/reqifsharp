// -------------------------------------------------------------------------------------------------
// <copyright file="SupportedFileExtensionKindExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
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
            var zip = Path.Combine(TestContext.CurrentContext.TestDirectory, "result.zip");

            Assert.Multiple(() =>
            {
                Assert.That(reqif.ConvertPathToSupportedFileExtensionKind(),
                    Is.EqualTo(SupportedFileExtensionKind.Reqif));

                Assert.That(reqifz.ConvertPathToSupportedFileExtensionKind(),
                    Is.EqualTo(SupportedFileExtensionKind.Reqifz));

                Assert.That(zip.ConvertPathToSupportedFileExtensionKind(),
                    Is.EqualTo(SupportedFileExtensionKind.Reqifz));
            });
        }
    }
}
