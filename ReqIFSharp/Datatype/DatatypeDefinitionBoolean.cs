// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionBoolean.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
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
    /// The purpose of the <see cref="DatatypeDefinitionBoolean"/> class is to define the primitive <see cref="bool"/> data type
    /// </summary>
    /// <remarks>
    /// This element defines a data type for the representation of Boolean data values in the Exchange Document.
    /// </remarks>
    public class DatatypeDefinitionBoolean : DatatypeDefinitionSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionBoolean"/> class.
        /// </summary>
        public DatatypeDefinitionBoolean()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionBoolean"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public DatatypeDefinitionBoolean(ILoggerFactory loggerFactory) 
            : base(loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionBoolean"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal DatatypeDefinitionBoolean(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
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
