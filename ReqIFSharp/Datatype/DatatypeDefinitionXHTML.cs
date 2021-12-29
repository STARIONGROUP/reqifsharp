// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionXHTML.cs" company="RHEA System S.A.">
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
// -------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The purpose of the <see cref="DatatypeDefinitionXHTML"/> class is to define XHTML formatted data.
    /// </summary>
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
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
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
        internal override async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            base.ReadXml(reader);

            await this.ReadAlternativeIdAsync(reader, token);
        }
    }
}
