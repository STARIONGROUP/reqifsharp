// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValue.cs" company="RHEA System S.A.">
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
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="AttributeDefinition"/> is the base class for attribute values.
    /// </summary>
    public abstract class AttributeValue : IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValue"/> class.
        /// </summary>
        protected AttributeValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValue"/> class.         
        /// </summary>
        /// <param name="attributeDefinition">
        /// The <see cref="AttributeDefinition"/> for which this is the default value
        /// </param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinition"/>
        /// </remarks>
        protected AttributeValue(AttributeDefinition attributeDefinition)
        {
            this.AttributeDefinition = attributeDefinition;
            this.ReqIFContent = this.AttributeDefinition.SpecType.ReqIFContent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValue"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        protected AttributeValue(SpecElementWithAttributes specElAt)
        {
            this.SpecElAt = specElAt;
            this.SpecElAt.Values.Add(this);
            this.ReqIFContent = this.SpecElAt.ReqIFContent;
        }

        /// <summary>
        /// Gets or sets the owning <see cref="SpecElementWithAttributes"/>
        /// </summary>
        public SpecElementWithAttributes SpecElAt { get; set; }

        /// <summary>
        /// Gets the <see cref="ReqIFContent"/>
        /// </summary>
        protected ReqIFContent ReqIFContent { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="AttributeDefinition"/>
        /// </summary>
        public AttributeDefinition AttributeDefinition 
        {
            get
            {
                return this.GetAttributeDefinition();
            }

            set
            {
                this.SetAttributeDefinition(value);
            }
        }

        /// <summary>
        /// Gets or sets the value of this <see cref="AttributeValue"/>
        /// </summary>
        /// <remarks>
        /// This is a convenience property to get/set TheValue or Values in concrete implementation
        /// </remarks>
        public abstract object ObjectValue { get; set; }

        /// <summary>
        /// Gets the <see cref="AttributeDefinition"/> from the sub class
        /// </summary>
        /// <returns>
        /// an instance of <see cref="AttributeDefinition"/>
        /// </returns>
        protected abstract AttributeDefinition GetAttributeDefinition();

        /// <summary>
        /// Sets the <see cref="AttributeDefinition"/> to the sub class
        /// </summary>
        /// <param name="attributeDefinition">
        /// The <see cref="AttributeDefinition"/> to set.
        /// </param>
        protected abstract void SetAttributeDefinition(AttributeDefinition attributeDefinition);

        /// <summary>
        /// Generates a <see cref="AttributeValue"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public abstract void ReadXml(XmlReader reader);

        /// <summary>
        /// Converts a <see cref="AttributeValue"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public abstract void WriteXml(XmlWriter writer);

        /// <summary>
        /// Asynchronously converts a <see cref="AttributeValue"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public abstract Task WriteXmlAsync(XmlWriter writer);

        /// <summary>
        /// This method is reserved and should not be used.
        /// </summary>
        /// <returns>returns null</returns>
        /// <remarks>
        /// When implementing the IXmlSerializable interface, you should return null
        /// </remarks>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
    }
}
