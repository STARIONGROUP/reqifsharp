// -------------------------------------------------------------------------------------------------
// <copyright file="SpecObject.cs" company="RHEA System S.A.">
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
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    
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
        /// Initializes a new instance of the <see cref="SpecObject"/> class.
        /// </summary>
        public SpecObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecObject"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="reqIfContent"/>
        /// </param>
        internal SpecObject(ReqIFContent reqIfContent)
            : base(reqIfContent)
        {
            this.ReqIfContent.SpecObjects.Add(this);
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
                var specType = this.ReqIfContent.SpecTypes.SingleOrDefault(x => x.Identifier == reference);
                this.Type = (SpecObjectType)specType;                    
            }
        }

        /// <summary>
        /// The read hierarchy.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        protected override void ReadHierarchy(XmlReader reader)
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
        public override void WriteXml(XmlWriter writer)
        {
            if (this.Type == null)
            {
                throw new SerializationException(string.Format("The Type property of SpecObject {0}:{1} may not be null", this.Identifier, this.LongName));
            }

            base.WriteXml(writer);

            writer.WriteStartElement("TYPE");
            writer.WriteElementString("SPEC-OBJECT-TYPE-REF", this.Type.Identifier);
            writer.WriteEndElement();
        }
    }
}
