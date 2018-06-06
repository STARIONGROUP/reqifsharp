// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionReal.cs" company="RHEA System S.A.">
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
    /// This element defines a data type for the representation of Real data values in the Exchange Document.
    /// </summary>
    public class DatatypeDefinitionReal : DatatypeDefinitionSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionReal"/> class.
        /// </summary>
        public DatatypeDefinitionReal()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionReal"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        internal DatatypeDefinitionReal(ReqIFContent reqIfContent)
            : base(reqIfContent)
        {
            this.ReqIFContent = reqIfContent;
        }

        /// <summary>
        /// Gets or sets a value that Denotes the supported maximum precision of real numbers represented by this data type.
        /// </summary>
        public int? Accuracy { get; set; }

        /// <summary>
        /// Gets or sets a value that denotes the largest negative data value representable by this data type.
        /// </summary>
        public double? Min { get; set; }

        /// <summary>
        /// Gets or sets a value that denotes the largest positive data value representable by this data type.
        /// </summary>
        public double? Max { get; set; }

        /// <summary>
        /// Generates a <see cref="AttributeDefinition"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            var value = reader.GetAttribute("ACCURACY");
            if (int.TryParse(value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out int accuracy))
            {
                this.Accuracy = accuracy;
            }

            value = reader.GetAttribute("MAX");
            if (double.TryParse(value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double max))
            {
                this.Max = max;
            }

            value = reader.GetAttribute("MIN");
            if (double.TryParse(value, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out double min))
            {
                this.Min = min;
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
            if (this.Accuracy.HasValue)
            {
                writer.WriteAttributeString("ACCURACY", this.Accuracy.Value.ToString(NumberFormatInfo.InvariantInfo));
            }

            if (this.Min.HasValue)
            {
                writer.WriteAttributeString("MIN", this.Min.Value.ToString(NumberFormatInfo.InvariantInfo));
            }

            if (this.Max.HasValue)
            {
                writer.WriteAttributeString("MAX", this.Max.Value.ToString(NumberFormatInfo.InvariantInfo));
            }
        }
    }
}