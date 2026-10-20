// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionSimple.cs" company="Starion Group S.A.">
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
    /// The <see cref="AttributeDefinitionSimple"/> is the base class for simple type attributes.
    /// </summary>
    [ReqIfClass(name: "ATTRIBUTE-DEFINITION-SIMPLE", isAbstract: true)]
    public abstract class AttributeDefinitionSimple : AttributeDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionSimple"/> class.
        /// </summary>
        protected AttributeDefinitionSimple()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionSimple"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected AttributeDefinitionSimple(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionSimple"/> class.
        /// </summary>
        /// <param name="specType">
        /// The owning <see cref="SpecType"/>.
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected AttributeDefinitionSimple(SpecType specType, ILoggerFactory loggerFactory) 
            : base(specType, loggerFactory)
        {
        }
    }
}
