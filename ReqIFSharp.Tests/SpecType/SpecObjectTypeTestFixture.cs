// -------------------------------------------------------------------------------------------------
//  <copyright file="SpecObjectTypeTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2025 Starion Group S.A.
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

namespace ReqIFSharp.Tests.SpecTypes
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using ReqIFSharp;

    using Serilog;

    [TestFixture]
    public class SpecObjectTypeTestFixture
    {
        private ILoggerFactory loggerFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
        }

        [Test]
        public void Verify_that_constructor_does_not_throw_exception()
        {
            Assert.That(() => new SpecObjectType(null), Throws.Nothing);
            Assert.That(() => new SpecObjectType(this.loggerFactory), Throws.Nothing);
        }

        [Test]
        public void Verify_that_when_argument_null_exception_is_thrown()
        {
            ReqIFContent content = null;

            Assert.That(() => new SpecObjectType(content, NullLoggerFactory.Instance), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_when_constructed_no_exception_is_thrown()
        {
            var content = new ReqIFContent();

            Assert.That(() => new SpecObjectType(content, NullLoggerFactory.Instance), Throws.Nothing);

            Assert.That(() => new SpecObjectType(NullLoggerFactory.Instance), Throws.Nothing);
        }
    }
}
