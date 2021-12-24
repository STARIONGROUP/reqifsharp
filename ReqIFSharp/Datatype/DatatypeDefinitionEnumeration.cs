// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionEnumeration.cs" company="RHEA System S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The purpose of the <see cref="DatatypeDefinitionEnumeration"/> class is to define enumeration types.
    /// </summary>
    /// <remarks>
    /// Data type definition for enumeration types. The set of enumeration values referenced by specifiedValues constrains the
    /// possible choices for enumeration attribute values
    /// </remarks>
    public class DatatypeDefinitionEnumeration : DatatypeDefinition
    {
        /// <summary>
        /// Backing field for the <see cref="SpecifiedValues"/> property;
        /// </summary>
        private readonly List<EnumValue> specifiedValues = new List<EnumValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionEnumeration"/> class.
        /// </summary>
        public DatatypeDefinitionEnumeration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionEnumeration"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        internal DatatypeDefinitionEnumeration(ReqIFContent reqIfContent) 
            : base(reqIfContent)            
        {
            this.ReqIFContent = reqIfContent;
        }

        /// <summary>
        /// Gets the owned enumeration literals
        /// </summary>
        public List<EnumValue> SpecifiedValues 
        {
            get
            {
                return this.specifiedValues;
            }
        }

        /// <summary>
        /// Generates a <see cref="DatatypeDefinitionEnumeration"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            using (var subtree = reader.ReadSubtree())
            {
                while (subtree.Read())
                {
                    if (subtree.MoveToContent() == XmlNodeType.Element)
                    {
                        switch (subtree.LocalName)
                        {
                            case "ALTERNATIVE-ID":
                                var alternativeId = new AlternativeId(this);
                                alternativeId.ReadXml(subtree);
                                break;
                            case "ENUM-VALUE":
                                var enumValue = new EnumValue(this);
                                enumValue.ReadXml(subtree);
                                break;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Asynchronously generates a <see cref="DatatypeDefinitionEnumeration"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        public override async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            base.ReadXml(reader);

            using (var subtree = reader.ReadSubtree())
            {
                while (await subtree.ReadAsync())
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    if (await subtree.MoveToContentAsync() == XmlNodeType.Element)
                    {
                        switch (subtree.LocalName)
                        {
                            case "ALTERNATIVE-ID":
                                var alternativeId = new AlternativeId(this);
                                alternativeId.ReadXml(reader);
                                break;
                            case "ENUM-VALUE":
                                var enumValue = new EnumValue(this);
                                await enumValue.ReadXmlAsync(subtree, token);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="DatatypeDefinitionEnumeration"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <exception cref="SerializationException">
        /// The <see cref="Type"/> may not be null
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteStartElement("SPECIFIED-VALUES");

            foreach (var specifiedValue in this.specifiedValues)
            {
                writer.WriteStartElement("ENUM-VALUE");
                specifiedValue.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously converts a <see cref="DatatypeDefinitionEnumeration"/> object into its XML representation.
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
        public override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            await base.WriteXmlAsync(writer, token);

            await writer.WriteStartElementAsync(null, "SPECIFIED-VALUES", null);

            foreach (var specifiedValue in this.specifiedValues)
            {
                await writer.WriteStartElementAsync(null, "ENUM-VALUE", null);
                await specifiedValue.WriteXmlAsync(writer, token);
                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
        }
    }
}
