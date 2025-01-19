﻿//  -------------------------------------------------------------------------------------------------
//  <copyright file="SupportedFileExtensionKindExtensions.cs" company="Starion Group S.A.">
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

namespace ReqIFSharp
{
    using System;
    using System.IO;

    /// <summary>
    /// Extension class used to convert between a file URI (path) and a <see cref="SupportedFileExtensionKind"/>
    /// </summary>
    public static class SupportedFileExtensionKindExtensions
    {
        /// <summary>
        /// extension method that converts the provided string-based file URI (path) to a <see cref="SupportedFileExtensionKind"/>
        /// </summary>
        /// <param name="fileUri"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static SupportedFileExtensionKind ConvertPathToSupportedFileExtensionKind(this string fileUri)
        {
            var extension = Path.GetExtension(fileUri);

            switch (extension)
            {
                case ".reqif":
                    return SupportedFileExtensionKind.Reqif;
                case ".reqifz":
                    return SupportedFileExtensionKind.Reqifz;
                case ".zip":
                    return SupportedFileExtensionKind.Reqifz;
                default:
                    throw new ArgumentException("only .reqif, .reqifz and .zip are supported file extensions.", nameof(fileUri));
            }
        }
    }
}
