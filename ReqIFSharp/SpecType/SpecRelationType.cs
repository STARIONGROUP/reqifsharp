// -------------------------------------------------------------------------------------------------
// <copyright file="SpecRelationType.cs" company="RHEA System S.A.">
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
    /// Contains a set of attribute definitions for a SpecRelation element.
    /// Inherits a set of attribute definitions from SpecType. By using SpecObjectType elements, multiple requirements can be
    /// associated with the same set of attribute definitions (attribute names, default values, data types, etc.).
    /// </summary>
    public class SpecRelationType : SpecType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecRelationType"/> class.
        /// </summary>
        public SpecRelationType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecRelationType"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="ReqIFContent"/>
        /// </param>
        internal SpecRelationType(ReqIFContent reqIfContent)
            : base(reqIfContent)
        {
        }
    }
}
