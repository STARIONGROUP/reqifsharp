// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionString.cs" company="RHEA System S.A.">
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
    using System.Xml;
    
    /// <summary>
    /// The purpose of the <see cref="DatatypeDefinitionBoolean"/> class is to define the primitive <see cref="string"/> data type
    /// </summary>
    /// <remarks>
    /// This element defines a data type for the representation of String data values in the Exchange Document.
    /// </remarks>
    public class DatatypeDefinitionString : DatatypeDefinitionSimple 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionString"/> class.
        /// </summary>
        public DatatypeDefinitionString()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionString"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        internal DatatypeDefinitionString(ReqIFContent reqIfContent) 
            : base(reqIfContent)
        { 
            this.ReqIFContent = reqIfContent;
        }

        /// <summary>
        /// Gets or sets the maximum permissible string length
        /// </summary>        
        public int MaxLength { get; set; }

        /// <summary>
        /// Generates a <see cref="DatatypeDefinitionReal"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        { 
            base.ReadXml(reader);
            
            var value = reader.GetAttribute("MAX-LENGTH"); 
            if ( !string.IsNullOrEmpty(value)) 
            { 
                this.MaxLength = XmlConvert.ToInt32(value);
            }
        }

        /// <summary>
        /// Converts a <see cref="DatatypeDefinitionReal"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        public override void WriteXml(XmlWriter writer)
        { 
            base.WriteXml(writer);
            
            writer.WriteAttributeString("MAX-LENGTH", XmlConvert.ToString(this.MaxLength));
        }
    }
}
