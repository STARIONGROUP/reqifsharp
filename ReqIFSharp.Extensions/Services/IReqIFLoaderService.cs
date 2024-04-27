//  -------------------------------------------------------------------------------------------------
//  <copyright file="IReqIFLoaderService.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2024 Starion Group S.A.
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

namespace ReqIFSharp.Extensions.Services
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using ReqIFSharp;

    /// <summary>
    /// The purpose of the <see cref="IReqIFLoaderService"/> is to load a ReqIF from a provided <see cref="Stream"/>
    /// and make the loaded <see cref="ReqIF"/> objects available
    /// </summary>
    public interface IReqIFLoaderService
    {
        /// <summary>
        /// Gets the instances of <see cref="ReqIF"/>
        /// </summary>
        IEnumerable<ReqIF> ReqIFData { get; }

        /// <summary>
        /// Gets a copy of the <see cref="Stream"/> from which the ReqIF is loaded
        /// </summary>
        Task<Stream> GetSourceStream(CancellationToken token);

        /// <summary>
        /// Loads the <see cref="ReqIF"/> objects from the provided <see cref="Stream"/>
        /// and stores the result in the <see cref="ReqIFData"/> property.
        /// </summary>
        /// <param name="reqifStream">
        /// a <see cref="Stream"/> that contains <see cref="ReqIF"/> content
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        Task Load(Stream reqifStream, SupportedFileExtensionKind fileExtensionKind, CancellationToken token);

        /// <summary>
        /// Query the data object from associated to the <see cref="ExternalObject"/>
        /// </summary>
        /// <param name="externalObject">
        /// The <see cref="ExternalObject"/> that holds a reference to the data
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// a Base64 encoded string that can be used in an HTML image element
        /// </returns>
        /// <remarks>
        /// The <see cref="IReqIFLoaderService"/> caches the data for fast
        /// </remarks>
        Task<string> QueryData(ExternalObject externalObject, CancellationToken token);

        /// <summary>
        /// Resets the <see cref="IReqIFLoaderService"/> by clearing <see cref="ReqIFData"/> and
        /// <see cref="SourceStream"/>
        /// </summary>
        void Reset();

        /// <summary>
        /// Event Handler that is invoked when the <see cref="ReqIFLoaderService"/> has either loaded data or has been reset
        /// </summary>
        event EventHandler<IEnumerable<ReqIF>> ReqIfChanged;
    }
}