// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinition.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The <see cref="DatatypeDefinition"/> is the base class for all data types available to the Exchange Document.
    /// </summary>
    [ReqIfClass(name: "DATATYPE-DEFINITION", isAbstract: true)]
    public abstract class DatatypeDefinition : Identifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinition"/> class.
        /// </summary>
        protected DatatypeDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinition"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected DatatypeDefinition(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinition"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected DatatypeDefinition(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            if (reqIfContent == null)
            {
                throw new ArgumentNullException(nameof(reqIfContent));
            }

            this.ReqIFContent = reqIfContent;
            reqIfContent.DataTypes.Add(this);
        }

        /// <summary>
        /// Generates a <see cref="DatatypeDefinition"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal abstract Task ReadXmlAsync(XmlReader reader, CancellationToken token);

        /// <summary>
        /// Gets or sets the owning <see cref="ReqIFContent"/>
        /// </summary>
        public ReqIFContent ReqIFContent { get; set; }
    }
}
