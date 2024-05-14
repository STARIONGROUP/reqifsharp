// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinition.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
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
    /// The <see cref="DatatypeDefinition"/> is the base class for all data types available to the Exchange Document.
    /// </summary>
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
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        protected DatatypeDefinition(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
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
