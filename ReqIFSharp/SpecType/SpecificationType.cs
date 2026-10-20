// -------------------------------------------------------------------------------------------------
// <copyright file="SpecificationType.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using Microsoft.Extensions.Logging;
    
    /// <summary>
    /// Contains a set of attribute definitions for a Specification element.
    /// Inherits a set of attribute definitions from SpecType. By using SpecificationType elements, multiple specifications can be
    /// associated with the same set of attribute definitions (attribute names, default values, data types, etc.).
    /// </summary>
    [ReqIfClass(name: "SPECIFICATION-TYPE", isAbstract: false)]
    public class SpecificationType : SpecType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificationType"/> class.
        /// </summary>
        public SpecificationType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificationType"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public SpecificationType(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificationType"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal SpecificationType(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
        }
    }
}
