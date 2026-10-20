// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionXHTML.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The purpose of the <see cref="DatatypeDefinitionXHTML"/> class is to define XHTML formatted data.
    /// </summary>
    [ReqIfClass(name: "DATATYPE-DEFINITION-XHTML", isAbstract: false)]
    public class DatatypeDefinitionXHTML : DatatypeDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionXHTML"/> class.
        /// </summary>
        public DatatypeDefinitionXHTML()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionXHTML"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public DatatypeDefinitionXHTML(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionXHTML"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal DatatypeDefinitionXHTML(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
        }

        /// <summary>
        /// Generates a <see cref="AttributeDefinition"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            this.ReadAlternativeId(reader);
        }

        /// <summary>
        /// Generates a <see cref="AttributeDefinition"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            base.ReadXml(reader);

            await this.ReadAlternativeIdAsync(reader, token);
        }
    }
}
