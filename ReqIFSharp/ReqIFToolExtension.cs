// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFToolExtension.cs" company="Starion Group S.A.">
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
    /// The <see cref="ReqIFToolExtension"/> class allows the optional inclusion of tool-specific information into a ReqIF Exchange Document.
    /// </summary>
    [ReqIfClass(name: "REQ-IF-TOOL-EXTENSION", isAbstract: false)]
    public class ReqIFToolExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIFToolExtension"/> class
        /// </summary>
        public ReqIFToolExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIFToolExtension"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal ReqIFToolExtension(ILoggerFactory loggerFactory)
        {
        }

        /// <summary>
        /// Gets or sets the InnerXml of the <see cref="ReqIFToolExtension"/>
        /// </summary>
        [ReqIfProperty(aggregation: AggregationKind.None, lowerValue: 0, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public string InnerXml { get; set; }

        /// <summary>
        /// Generates a <see cref="ReqIFContent"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element)
                {
                    this.InnerXml = reader.ReadInnerXml();
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="ReqIFContent"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal async Task ReadXmlAsync(XmlReader reader)
        {
            while (await reader.ReadAsync())
            {
                if (await reader.MoveToContentAsync() == XmlNodeType.Element)
                {
                    this.InnerXml = await reader.ReadInnerXmlAsync();
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="ReqIFToolExtension"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteRaw(this.InnerXml);
        }

        /// <summary>
        /// Asynchronously writes a <see cref="ReqIFToolExtension"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await writer.WriteRawAsync(this.InnerXml);
        }
    }
}
