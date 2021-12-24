// -------------------------------------------------------------------------------------------------
// <copyright file="AccessControlledElement.cs" company="RHEA System S.A.">
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

    /// <summary>
    /// The <see cref="AccessControlledElement"/> is the base class for classes that may restrict user access to their information.
    /// </summary>
    public abstract class AccessControlledElement : Identifiable
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AccessControlledElement"/> is editable
        /// </summary>
        /// <remarks>
        /// True means that the element’s contents may be modified by the user of a tool containing the element.
        /// False or leaving isEditable out means that the element is read-only to the user of a tool containing the element.
        /// </remarks>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Generates a <see cref="AccessControlledElement"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            var isEditable = reader.GetAttribute("IS-EDITABLE");

            if (isEditable != null)
            {
                this.IsEditable = XmlConvert.ToBoolean(isEditable);
            }
        }

        /// <summary>
        /// Asynchronously generates a <see cref="AccessControlledElement"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        public override async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            await base.ReadXmlAsync(reader, token);
            
            var isEditable = reader.GetAttribute("IS-EDITABLE");

            if (isEditable != null)
            {
                this.IsEditable = XmlConvert.ToBoolean(isEditable);
            }
        }

        /// <summary>
        /// Converts a <see cref="AttributeDefinition"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            if (this.IsEditable)
            {
                writer.WriteAttributeString("IS-EDITABLE", "true");
            }
        }

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeDefinition"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public override async Task WriteXmlAsync(XmlWriter writer)
        {
            await base.WriteXmlAsync(writer);

            if (this.IsEditable)
            {
                await writer.WriteAttributeStringAsync(null, "IS-EDITABLE", null, "true");
            }
        }
    }
}
