// -------------------------------------------------------------------------------------------------
// <copyright file="XmlAttribute.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    /// <summary>
    /// Encapsulates an xml attribute extracted from a ReqIF document
    /// </summary>
    internal class XmlAttribute
    {
        /// <summary>
        /// Gets or sets the namespace prefix of the attribute
        /// </summary>
        internal string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the local name of the attribute
        /// </summary>
        internal string LocalName { get; set; }

        /// <summary>
        /// Gets or sets the namespace URI of the attribute.
        /// </summary>
        /// <remarks>
        /// For namespace declarations (<c>xmlns</c> / <c>xmlns:prefix</c>) this is the reserved
        /// <c>http://www.w3.org/2000/xmlns/</c> URI; for ordinary prefixed attributes (e.g.
        /// <c>xsi:schemaLocation</c>) it is the URI the prefix is bound to. Captured on read so the
        /// attribute can be re-emitted faithfully.
        /// </remarks>
        internal string NamespaceUri { get; set; }

        /// <summary>
        /// Gets or sets the value of the attribute.
        /// </summary>
        internal string Value { get; set; }
    }
}
