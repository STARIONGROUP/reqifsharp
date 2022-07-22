// -------------------------------------------------------------------------------------------------
// <copyright file="IReqIFSerializer.cs" company="RHEA System S.A.">
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

namespace ReqIFSharp
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The interface for the a <see cref="ReqIF"/> serializer
    /// </summary>
    public interface IReqIFSerializer
    {
        /// <summary>
        /// Serialize a <see cref="IEnumerable{ReqIF}"/> object and write its content in an XML-file in the corresponding path
        /// </summary>
        /// <param name="reqIfs">The <see cref="ReqIF"/> object to serialize</param>
        /// <param name="fileUri">The path of the output file</param>
        void Serialize(IEnumerable<ReqIF> reqIfs, string fileUri);

        /// <summary>
        /// Serialize a <see cref="IEnumerable{ReqIF}"/> object and write its content to the provided <see cref="Stream"/>
        /// </summary>
        /// <param name="reqIfs">
        /// The <see cref="ReqIF"/> object to serialize
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to serialize to
        /// </param>
        void Serialize(IEnumerable<ReqIF> reqifs, Stream stream, SupportedFileExtensionKind fileExtensionKind);

        /// <summary>
        /// Async Serialize a <see cref="IEnumerable{ReqIF}"/> object and write its content in an XML-file in the corresponding path
        /// </summary>
        /// <param name="reqIfs">The <see cref="ReqIF"/> object to serialize</param>
        /// <param name="fileUri">The path of the output file</param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        Task SerializeAsync(IEnumerable<ReqIF> reqIfs, string fileUri, CancellationToken token);

        /// <summary>
        /// Async Serialize a <see cref="IEnumerable{ReqIF}"/> object and write its content to the provided <see cref="Stream"/>
        /// </summary>
        /// <param name="reqIfs">
        /// The <see cref="ReqIF"/> object to serialize
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to serialize to
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        Task SerializeAsync(IEnumerable<ReqIF> reqIfs, Stream stream, SupportedFileExtensionKind fileExtensionKind, CancellationToken token);
    }
}