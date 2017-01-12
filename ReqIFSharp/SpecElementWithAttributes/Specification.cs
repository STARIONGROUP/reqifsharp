// -------------------------------------------------------------------------------------------------
// <copyright file="Specification.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    
    /// <summary>
    /// Represents a hierarchically structured requirements specification.
    /// It is the root node of the tree that hierarchically structures <see cref="SpecObject"/> instances.
    /// </summary>
    public class Specification : SpecElementWithAttributes
    {
        /// <summary>
        /// Backing field for the <see cref="Children"/> property.
        /// </summary>
        private readonly List<SpecHierarchy> children = new List<SpecHierarchy>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Specification"/> class.
        /// </summary>
        public Specification()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="reqIfContent"/>
        /// </param>
        internal Specification(ReqIFContent reqIfContent)
            : base(reqIfContent)
        {
            this.ReqIfContent.Specifications.Add(this);
        }

        /// <summary>
        /// Gets the links to next level of owned SpecHierarchy.
        /// </summary>
        public List<SpecHierarchy> Children 
        {
            get
            {
                return this.children;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SpecificationType"/> reference.
        /// </summary>
        public SpecificationType Type { get; set; }

        /// <summary>
        /// Converts a <see cref="Specification"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            if (this.Type == null)
            {
                throw new SerializationException(string.Format("The Type property of Specification {0}:{1} may not be null", this.Identifier, this.LongName));
            }

            base.WriteXml(writer);

            this.WriteType(writer);

            this.WriteChildren(writer);
        }

        /// <summary>
        /// Gets the <see cref="SpecType"/>class
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
            if (specType.GetType() != typeof(SpecificationType))
            {
                throw new ArgumentException("specType must of type SpecificationType");
            }

            this.Type = (SpecificationType)specType;
        }

        /// <summary>
        /// Reads the <see cref="SpecType"/> which is specific to the <see cref="Specification"/> class.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        protected override void ReadSpecType(XmlReader reader)
        {
            if (reader.ReadToDescendant("SPECIFICATION-TYPE-REF"))
            {
                var reference = reader.ReadElementContentAsString();
                var specType = this.ReqIfContent.SpecTypes.SingleOrDefault(x => x.Identifier == reference);
                this.Type = (SpecificationType)specType;
            }
        }

        /// <summary>
        /// The read hierarchy.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        protected override void ReadHierarchy(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SPEC-HIERARCHY")
                {
                    using (var subtree = reader.ReadSubtree())
                    {
                        subtree.MoveToContent();

                        var specHierarchy = new SpecHierarchy(this, this.ReqIfContent);
                        specHierarchy.ReadXml(subtree);
                    }
                }
            }
        }

        /// <summary>
        /// Writes the <see cref="Type"/>
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteType(XmlWriter writer)
        {
            writer.WriteStartElement("TYPE");
            writer.WriteElementString("SPECIFICATION-TYPE-REF", this.Type.Identifier);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the <see cref="SpecHierarchy"/> objects from the <see cref="Children"/> list.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        private void WriteChildren(XmlWriter writer)
        {
            if (this.children.Count == 0)
            {
                return;
            }

            writer.WriteStartElement("CHILDREN");

            foreach (var specHierarchy in this.children)
            {
                writer.WriteStartElement("SPEC-HIERARCHY");
                specHierarchy.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
