// -------------------------------------------------------------------------------------------------
// <copyright file="SupportedFileExtensionKindExtensions.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

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
