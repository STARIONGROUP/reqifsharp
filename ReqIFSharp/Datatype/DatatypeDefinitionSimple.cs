// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionSimple.cs" company="Starion Group S.A.">
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
    /// The <see cref="DatatypeDefinitionSimple"/> is the base class from which all primitive data types, except enumeration, are derived.
    /// </summary>
    [ReqIfClass(name: "DATATYPE-DEFINITION-SIMPLE", isAbstract: true)]
    public abstract class DatatypeDefinitionSimple : DatatypeDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionSimple"/> class.
        /// </summary>
        protected DatatypeDefinitionSimple()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionSimple"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected DatatypeDefinitionSimple(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionSimple"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected DatatypeDefinitionSimple(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
        }
    }
}
