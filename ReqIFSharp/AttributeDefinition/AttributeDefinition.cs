// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinition.cs" company="Starion Group S.A.">
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
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The <see cref="AttributeDefinition"/> is the base class for attribute definitions.
    /// </summary>
    /// <remarks>
    /// Each concrete attribute value that is created in a requirements authoring tool needs to be valid against its related data
    /// type. In <see cref="ReqIF"/>, each attribute value (<see cref="AttributeValue"/> element) is related to its data type (<see cref="DatatypeDefinition"/>  element) via
    /// an attribute definition (<see cref="AttributeDefinition"/> element).
    /// </remarks>
    public abstract class AttributeDefinition : AccessControlledElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinition"/> class.
        /// </summary>
        protected AttributeDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinition"/> class.
        /// </summary>
        /// <param name="specType">
        /// The owning <see cref="SpecType"/>.
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        protected AttributeDefinition(SpecType specType, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            if (specType == null)
            {
                throw new ArgumentNullException(nameof(specType));
            }

            this.SpecType = specType;
            specType.SpecAttributes.Add(this);
        }

        /// <summary>
        /// Gets or sets the owning <see cref="SpecType"/>
        /// </summary>
        public SpecType SpecType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DatatypeDefinition"/>
        /// </summary>
        public DatatypeDefinition DatatypeDefinition 
        {
            get => this.GetDatatypeDefinition();

            set => this.SetDatatypeDefinition(value);
        }

        /// <summary>
        /// Gets the <see cref="DatatypeDefinition"/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="DatatypeDefinition"/>
        /// </returns>
        protected abstract DatatypeDefinition GetDatatypeDefinition();

        /// <summary>
        /// Sets the <see cref="DatatypeDefinition"/>
        /// </summary>
        /// <param name="datatypeDefinition">
        /// The instance of <see cref="DatatypeDefinition"/> that is to be set.
        /// </param>
        protected abstract void SetDatatypeDefinition(DatatypeDefinition datatypeDefinition);

        /// <summary>
        /// Asynchronously generates a <see cref="ReqIFSharp.AttributeDefinition"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal abstract Task ReadXmlAsync(XmlReader reader, CancellationToken token);

        /// <summary>
        /// Converts a <see cref="AttributeDefinitionBoolean"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Type"/> may not be null
        /// </exception>
        internal override void WriteXml(XmlWriter writer)
        {
            if (this.GetDatatypeDefinition() == null)
            {
                throw new SerializationException($"The Type property of {this.GetType().Name} {this.Identifier}:{this.LongName} may not be null");
            }

            base.WriteXml(writer);
        }


        /// <summary>
        /// Asynchronously converts a <see cref="AttributeDefinitionXHTML"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Type"/> may not be null
        /// </exception>
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            if (this.GetDatatypeDefinition() == null)
            {
                throw new SerializationException($"The Type property of {this.GetType().Name} {this.Identifier}:{this.LongName} may not be null");
            }

            await base.WriteXmlAsync(writer, token);
        }
    }
}
