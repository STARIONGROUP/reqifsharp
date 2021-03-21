// -------------------------------------------------------------------------------------------------
// <copyright file="DeSerializeAndSerializeTestFixture.cs" company="RHEA System S.A.">
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
    using System.IO;
    using System.Linq;
    
    using NUnit.Framework;

    using ReqIFSharp;

    [TestFixture]
    public class DeSerializeAndSerializeTestFixture
    {
        [Test]
        public void Verify_That_a_reqif_file_can_deserialized_and_serialized_with_equivalent_output()
        {
            var reqifPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "ProR_Traceability-Template-v1.0.reqif");
            var deserializer = new ReqIFDeserializer();

            var reqIf = deserializer.Deserialize(reqifPath).First();

            var resultFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, "ProR_Traceability-Template-v1.0-reserialize.reqif");

#if NETFULL
            var serializer = new ReqIFSerializer(false);
            serializer.Serialize(reqIf, resultFileUri, null);
#else
            var serializer = new ReqIFSerializer();
            serializer.Serialize(reqIf, resultFileUri);
#endif
        }
    }
}
