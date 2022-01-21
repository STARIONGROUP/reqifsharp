// -------------------------------------------------------------------------------------------------
// <copyright file="ExternalObjectTestFixture.cs" company="RHEA System S.A.">
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
