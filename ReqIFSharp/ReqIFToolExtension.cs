// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFToolExtension.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
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
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal ReqIFToolExtension(ILoggerFactory loggerFactory)
        {
        }

        /// <summary>
        /// Gets or sets the InnerXml of the <see cref="ReqIFToolExtension"/>
        /// </summary>
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
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            await writer.WriteRawAsync(this.InnerXml);
        }
    }
}
