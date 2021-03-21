// -------------------------------------------------------------------------------------------------
// <copyright file="IReqIFSerializer.cs" company="RHEA System S.A.">
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

namespace ReqIFSharp
{
#if NETFRAMEWORK || NETSTANDARD2_0
    using System;
    using System.IO;
    using System.Security;
    using System.Xml.Schema;
#endif

    /// <summary>
    /// The interface for the a <see cref="ReqIF"/> serializer
    /// </summary>
    public interface IReqIFSerializer
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        /// <summary>
        /// Serialize a <see cref="ReqIF"/> object and write its content in an XML-file in the corresponding path
        /// </summary>
        /// <param name="reqIf">The <see cref="ReqIF"/> object to serialize</param>
        /// <param name="fileUri">The path of the output file</param>
        /// <param name="validationEventHandler">The <see cref="ValidationEventHandler"/> that processes the result of the reqif validation</param>
        /// <exception cref="XmlSchemaValidationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        void Serialize(ReqIF reqIf, string fileUri, ValidationEventHandler validationEventHandler);
#else
        /// <summary>
        /// Serialize a <see cref="ReqIF"/> object and write its content in an XML-file in the corresponding path
        /// </summary>
        /// <param name="reqIf">The <see cref="ReqIF"/> object to serialize</param>
        /// <param name="fileUri">The path of the output file</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        void Serialize(ReqIF reqIf, string fileUri);
#endif
    }
}