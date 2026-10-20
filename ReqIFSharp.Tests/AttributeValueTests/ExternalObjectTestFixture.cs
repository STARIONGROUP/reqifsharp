// -------------------------------------------------------------------------------------------------
// <copyright file="ExternalObjectTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests.AttributeValueTests
{
    using System;
    using System.IO;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ExternalObject"/> class.
    /// </summary>
    [TestFixture]
    public class ExternalObjectTestFixture
    {
        private ExternalObject externalObject;

        [SetUp]
        public void SetUp()
        {
            this.externalObject = new ExternalObject(null);
        }

        [Test]
        public void Verify_that_expected_exceptions_are_raised_when_QueryLocalData_is_called()
        {
            string reqifPath = null;
            Assert.Throws<ArgumentException>(() => this.externalObject.QueryLocalData(reqifPath, null));

            Stream sourceStream = null;
            Assert.Throws<ArgumentNullException>(() => this.externalObject.QueryLocalData(sourceStream, null));

            reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "requirements-and-objects.reqifz");
            Assert.Throws<ArgumentNullException>(() => this.externalObject.QueryLocalData(reqifPath, null));

            using (sourceStream = new FileStream(reqifPath, FileMode.Open))
            {
                Assert.Throws<ArgumentNullException>(() => this.externalObject.QueryLocalData(sourceStream, null));
            }

            var targetStream = new MemoryStream();
            
            using (sourceStream = new MemoryStream())
            {
                Assert.Throws<ArgumentException>(() => this.externalObject.QueryLocalData(sourceStream, null));
            }

            this.externalObject.Uri = "http";
            Assert.Throws<InvalidOperationException>(() => this.externalObject.QueryLocalData(reqifPath, targetStream));

            using (sourceStream = new FileStream(reqifPath, FileMode.Open))
            {
                Assert.Throws<InvalidOperationException>(() => this.externalObject.QueryLocalData(sourceStream, targetStream));
            }
        }
    }
}
