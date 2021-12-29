// -------------------------------------------------------------------------------------------------
// <copyright file="EnumValue.cs" company="RHEA System S.A.">
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
    /// The class <see cref="EnumValue"/> represents enumeration literals.
    /// </summary>
    public class EnumValue : Identifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValue"/> class.
        /// </summary>
        public EnumValue()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValue"/> class.
        /// </summary>
        /// <param name="datatypeDefinitionEnumeration">
        /// The owning <see cref="DatatypeDefinitionEnumeration"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal EnumValue(DatatypeDefinitionEnumeration datatypeDefinitionEnumeration, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.DataTpeDefEnum = datatypeDefinitionEnumeration;
            this.DataTpeDefEnum.SpecifiedValues.Add(this);
        }

        /// <summary>
        /// Gets or sets the owned <see cref="EmbeddedValue"/>
        /// </summary>
        public EmbeddedValue Properties { get; set; }

        /// <summary>
        /// Gets or sets the owning <see cref="DatatypeDefinitionEnumeration"/> class.
        /// </summary>
        public DatatypeDefinitionEnumeration DataTpeDefEnum { get; set; }

        /// <summary>
        /// Generates a <see cref="EnumValue"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            using (var subtree = reader.ReadSubtree())
            {
                while (subtree.Read())
                {
                    if (subtree.MoveToContent() == XmlNodeType.Element && reader.LocalName == "EMBEDDED-VALUE")
                    {
                        var embeddedValue = new EmbeddedValue(this, this.loggerFactory);
                        embeddedValue.ReadXml(subtree);
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="EnumValue"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            base.ReadXml(reader);
            
            using (var subtree = reader.ReadSubtree())
            {
                while (await subtree.ReadAsync())
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    if (await subtree.MoveToContentAsync() == XmlNodeType.Element && reader.LocalName == "EMBEDDED-VALUE")
                    {
                        var embeddedValue = new EmbeddedValue(this, this.loggerFactory);
                        embeddedValue.ReadXml(subtree);
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeDefinition"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            if (this.Properties == null)
            {
                return;
            }

            writer.WriteStartElement("PROPERTIES");
            writer.WriteStartElement("EMBEDDED-VALUE");
            this.Properties.WriteXml(writer);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeDefinition"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task WriteXmlAsync(XmlWriter writer, CancellationToken token)
        {
            await base.WriteXmlAsync(writer, token);

            if (this.Properties == null)
            {
                return;
            }

            await writer.WriteStartElementAsync(null, "PROPERTIES", null);
            await writer.WriteStartElementAsync(null, "EMBEDDED-VALUE", null);
            await this.Properties.WriteXmlAsync(writer, token);
            await writer.WriteEndElementAsync();
            await writer.WriteEndElementAsync();
        }
    }
}
