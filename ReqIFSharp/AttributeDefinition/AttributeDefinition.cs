// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinition.cs" company="RHEA System S.A.">
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
    /// <summary>
    /// The <see cref="AttributeDefinition"/> is the base class for attribute definitions.
    /// </summary>
    /// <remarks>
    /// Each concrete attribute value that is created in a requirements authoring tool needs to be valid against its related data
    /// type. In <see cref="ReqIF"/>, each attribute value (<see cref="AttributeValue"/> element) is related to its data type (<see cref="DatatypeDefinition"/>  element) via
    /// an attribute definition (<see cref="AttributeDefinition"/> element).
    /// </remarks>
    public abstract class AttributeDefinition : AccessControlledElement 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinition"/> class.
        /// </summary>
        protected AttributeDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinition"/> class.
        /// </summary>
        /// <param name="specType">
        /// The owning <see cref="SpecType"/>.
        /// </param>
        internal AttributeDefinition(SpecType specType)
        {
            this.SpecType = specType;
            specType.SpecAttributes.Add(this);
        }

        /// <summary>
        /// Gets or sets the owning <see cref="SpecType"/>
        /// </summary>
        public SpecType SpecType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DatatypeDefinition"/>
        /// </summary>
        public DatatypeDefinition DatatypeDefinition 
        {
            get
            {
                return this.GetDatatypeDefinition();
            }

            set
            {
                this.SetDatatypeDefinition(value);
            }
        }

        /// <summary>
        /// Gets the <see cref="DatatypeDefinition"/>
        /// </summary>
        /// <returns>
        /// An instance of <see cref="DatatypeDefinition"/>
        /// </returns>
        protected abstract DatatypeDefinition GetDatatypeDefinition();

        /// <summary>
        /// Sets the <see cref="DatatypeDefinition"/>
        /// </summary>
        /// <param name="datatypeDefinition">
        /// The instance of <see cref="DatatypeDefinition"/> that is to be set.
        /// </param>
        protected abstract void SetDatatypeDefinition(DatatypeDefinition datatypeDefinition);
    }
}
