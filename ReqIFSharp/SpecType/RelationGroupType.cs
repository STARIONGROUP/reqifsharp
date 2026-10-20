// -------------------------------------------------------------------------------------------------
// <copyright file="RelationGroupType.cs" company="Starion Group S.A.">
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
    /// The purpose of the <see cref="RelationGroupType"/> is to contain a set of attribute definitions for a <see cref="RelationGroup"/> element.    
    /// </summary>
    /// <remarks>
    /// Inherits a set of attribute definitions from <see cref="SpecType"/>. By using <see cref="RelationGroupType"/> elements, <see cref="RelationGroup"/> elements can
    /// be associated with attribute names, default values, data types, etc.
    /// </remarks>
    [ReqIfClass(name: "RELATION-GROUP-TYPE", isAbstract: false)]
    public class RelationGroupType : SpecType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationGroupType"/> class.
        /// </summary>
        public RelationGroupType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationGroupType"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public RelationGroupType(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationGroupType"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal RelationGroupType(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
        }
    }
}
