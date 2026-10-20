// -------------------------------------------------------------------------------------------------
// <copyright file="IReqIFSerializer.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
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
        /// <param name="reqifs">The <see cref="ReqIF"/> object to serialize</param>
        /// <param name="fileUri">The path of the output file</param>
        void Serialize(IEnumerable<ReqIF> reqifs, string fileUri);

        /// <summary>
        /// Serialize a <see cref="IEnumerable{ReqIF}"/> object and write its content to the provided <see cref="Stream"/>
        /// </summary>
        /// <param name="reqifs">
        /// The <see cref="ReqIF"/> object to serialize
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to serialize to
        /// </param>
        /// <param name="fileExtensionKind">
        /// The <see cref="SupportedFileExtensionKind"/> that specifies whether the resulting <see cref="Stream"/>
        /// contains the reqif file or a zip-archive of reqif files
        /// </param>
        void Serialize(IEnumerable<ReqIF> reqifs, Stream stream, SupportedFileExtensionKind fileExtensionKind);

        /// <summary>
        /// Async Serialize a <see cref="IEnumerable{ReqIF}"/> object and write its content in an XML-file in the corresponding path
        /// </summary>
        /// <param name="reqifs">The <see cref="ReqIF"/> object to serialize</param>
        /// <param name="fileUri">The path of the output file</param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        Task SerializeAsync(IEnumerable<ReqIF> reqifs, string fileUri, CancellationToken token);

        /// <summary>
        /// Async Serialize a <see cref="IEnumerable{ReqIF}"/> object and write its content to the provided <see cref="Stream"/>
        /// </summary>
        /// <param name="reqifs">
        /// The <see cref="ReqIF"/> object to serialize
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to serialize to
        /// </param>
        /// <param name="fileExtensionKind">
        /// The <see cref="SupportedFileExtensionKind"/> that specifies whether the resulting <see cref="Stream"/>
        /// contains the reqif file or a zip-archive of reqif files
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        Task SerializeAsync(IEnumerable<ReqIF> reqifs, Stream stream, SupportedFileExtensionKind fileExtensionKind, CancellationToken token);
    }
}
