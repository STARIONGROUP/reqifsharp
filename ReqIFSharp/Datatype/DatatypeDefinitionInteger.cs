// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionInteger.cs" company="RHEA System S.A.">
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
    using System.Globalization;
    using System.Xml;

    /// <summary>
    /// The purpose of the <see cref="DatatypeDefinitionBoolean"/> class is to define the primitive <see cref="int"/> data type
    /// </summary>
    /// <remarks>
    /// This element defines a data type for the representation of Integer data values in the Exchange Document.
    /// The representation of data values shall comply with the definitions in http://www.w3.org/TR/xmlschema-2/#integer
    /// </remarks>
    public class DatatypeDefinitionInteger : DatatypeDefinitionSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionInteger"/> class.
        /// </summary>
        public DatatypeDefinitionInteger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionInteger"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        internal DatatypeDefinitionInteger(ReqIFContent reqIfContent) 
            : base(reqIfContent)            
        {
            this.ReqIFContent = reqIfContent;
        }

        /// <summary>
        /// Gets or sets a value that denotes the largest negative data value representable by this data type.
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// Gets or sets a value that denotes the largest positive data value representable by this data type.
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// Converts a <see cref="AttributeDefinition"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteAttributeString("MIN", this.Min.ToString(NumberFormatInfo.InvariantInfo));
            writer.WriteAttributeString("MAX", this.Max.ToString(NumberFormatInfo.InvariantInfo));
        }
    }
}
