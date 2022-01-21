// -------------------------------------------------------------------------------------------------
// <copyright file="SpecObject.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2022 RHEA System S.A.
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
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// Constitutes an identifiable requirements object that can be associated with various attributes. 
    /// This is the smallest granularity by which requirements are referenced.
    /// </summary>
    /// <remarks>
    /// The <see cref="SpecObject"/> instance itself does not carry the requirements text or any other user defined content.
    /// This data is stored in <see cref="AttributeValue"/> instances that are associated to the <see cref="SpecObject"/> instance.
    /// </remarks>
    public class SpecObject : SpecElementWithAttributes
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<SpecObject> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecObject"/> class.
        /// </summary>
        public SpecObject()
        {
            this.logger = NullLogger<SpecObject>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecObject"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="reqIfContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal SpecObject(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<SpecObject>.Instance : loggerFactory.CreateLogger<SpecObject>();

            this.ReqIFContent.SpecObjects.Add(this);
        }
        
        /// <summary>
        /// Gets or sets the <see cref="SpecObject"/> reference.
        /// </summary>
        public SpecObjectType Type { get; set; }

        /// <summary>
        /// Gets the <see cref="SpecType"/>
        /// </summary>
        /// <returns>
        /// an instance of <see cref="SpecType"/>
        /// </returns>
        protected override SpecType GetSpecType()
        {
            return this.Type;
        }

        /// <summary>
        /// Sets the <see cref="SpecType"/> 
        /// </summary>
        /// <param name="specType">
        /// The <see cref="SpecType"/> to set.
        /// </param>
        protected override void SetSpecType(SpecType specType)
        {
            if (specType.GetType() != typeof(SpecObjectType))
            {
                throw new ArgumentException("specType must of type SpecObjectType");
            }

            this.Type = (SpecObjectType)specType;
        }

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="SpecObject"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected override void ReadSpecType(XmlReader reader)
        {
            if (reader.ReadToDescendant("SPEC-OBJECT-TYPE-REF"))
            {
                var reference = reader.ReadElementContentAsString();
                var specType = this.ReqIFContent.SpecTypes.SingleOrDefault(x => x.Identifier == reference);
                this.Type = (SpecObjectType)specType;

                if (specType == null)
                {
                    this.logger.LogTrace("The SpecObjectType:{reference} could not be found and has been set to null on SpecObject:{Identifier}", reference, Identifier);
                }
            }
        }

        /// <summary>
        /// Asynchronously reads the <see cref="SpecType"/> which is specific to the <see cref="SpecObject"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        protected override async Task ReadSpecTypeAsync(XmlReader reader, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (reader.ReadToDescendant("SPEC-OBJECT-TYPE-REF"))
            {
                var reference = await reader.ReadElementContentAsStringAsync();
                var specType = this.ReqIFContent.SpecTypes.SingleOrDefault(x => x.Identifier == reference);
                this.Type = (SpecObjectType)specType;

                if (specType == null)
                {
                    this.logger.LogTrace("The SpecObjectType:{reference} could not be found and has been set to null on SpecObject:{Identifier}", reference, Identifier);
                }
            }
        }

        /// <summary>
        /// Reads the <see cref="SpecHierarchy"/> which is specific to the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected override void ReadHierarchy(XmlReader reader)
        {
            throw new InvalidOperationException("SpecRelation does not have a hierarchy");
        }

        /// <summary>
        /// Asynchronously reads the <see cref="SpecHierarchy"/> which is specific to the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        protected override Task ReadHierarchyAsync(XmlReader reader, CancellationToken token)
        {
            throw new InvalidOperationException("SpecRelation does not have a hierarchy");
        }

        /// <summary>
        /// Converts a <see cref="SpecObject"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Type"/> property may not be null.
        /// </exception>
        internal override void WriteXml(XmlWriter writer)
        {
            if (this.Type == null)
            {
                throw new SerializationException($"The Type property of SpecObject {this.Identifier}:{this.LongName} may not be null");
            }

            base.WriteXml(writer);

            writer.WriteStartElement("TYPE");
            writer.WriteElementString("SPEC-OBJECT-TYPE-REF", this.Type.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously converts a <see cref="SpecObject"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Type"/> property may not be null.
        /// </exception>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            if (this.Type == null)
            {
                throw new SerializationException($"The Type property of SpecObject {this.Identifier}:{this.LongName} may not be null");
            }

            await base.WriteXmlAsync(writer, token);

            await writer.WriteStartElementAsync(null, "TYPE", null);
            await writer.WriteElementStringAsync(null, "SPEC-OBJECT-TYPE-REF", null, this.Type.Identifier);
            await writer.WriteEndElementAsync();
        }
    }
}
