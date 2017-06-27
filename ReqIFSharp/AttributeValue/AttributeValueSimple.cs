// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueSimple.cs" company="RHEA System S.A.">
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
    /// <summary>
    /// The <see cref="AttributeValueSimple"/> is the base class for simple type attribute values.
    /// </summary>
    public abstract class AttributeValueSimple : AttributeValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueSimple"/> class.
        /// </summary>
        protected AttributeValueSimple()
        {
        }

        /// <summary>
        /// Instantiated a new instance of the <see cref="AttributeValueSimple"/> class
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinitionSimple"/> for which this is the default value</param>
        /// <remarks>
        /// This constructor shall be used when setting the default value of an <see cref="AttributeDefinitionSimple"/>
        /// </remarks>
        internal AttributeValueSimple(AttributeDefinitionSimple attributeDefinition)
            : base(attributeDefinition)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeValueSimple"/> class.
        /// </summary>
        /// <param name="specElAt">
        /// The owning <see cref="SpecElementWithAttributes"/>
        /// </param>
        internal AttributeValueSimple(SpecElementWithAttributes specElAt)
            : base(specElAt)
        {
        }
    }
}