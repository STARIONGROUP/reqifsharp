// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueSimple.cs" company="Starion Group S.A.">
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
    /// The <see cref="AttributeValueSimple"/> is the base class for simple type attribute values.
    /// </summary>
    [ReqIfClass(name: "ATTRIBUTE-VALUE-SIMPLE", isAbstract: true)]
    public abstract class AttributeValueSimple : AttributeValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueSimple"/> class.
        /// </summary>
        protected AttributeValueSimple()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueSimple"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected AttributeValueSimple(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        /// <summary>
        /// Instantiated a new instance of the <see cref="AttributeValueSimple"/> class
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinitionSimple"/> for which this is the default value</param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinitionSimple"/>
        /// </remarks>
        protected AttributeValueSimple(AttributeDefinitionSimple attributeDefinition, ILoggerFactory loggerFactory)
            : base(attributeDefinition, loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueSimple"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected AttributeValueSimple(SpecElementWithAttributes specElAt, ILoggerFactory loggerFactory)
            : base(specElAt, loggerFactory)
        {
        }
    }
}
