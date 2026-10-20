// -------------------------------------------------------------------------------------------------
// <copyright file="SupportedFileExtensionKind.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    /// <summary>
    /// Enumeration data type that defines the supported file extensions to serialize to and from
    /// </summary>
    public enum SupportedFileExtensionKind
    {
        /// <summary>
        /// Single ReqIF file
        /// </summary>
        Reqif,

        /// <summary>
        /// ReqIF (zip) archive that may contain multiple ReqIF documents and associated attachments
        /// </summary>
        Reqifz
    }
}
