// -------------------------------------------------------------------------------------------------
// <copyright file="RelationGroupType.cs" company="RHEA System S.A.">
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
    /// The purpose of the <see cref="RelationGroupType"/> is to contain a set of attribute definitions for a <see cref="RelationGroup"/> element.    
    /// </summary>
    /// <remarks>
    /// Inherits a set of attribute definitions from <see cref="SpecType"/>. By using <see cref="RelationGroupType"/> elements, <see cref="RelationGroup"/> elements can
    /// be associated with attribute names, default values, data types, etc.
    /// </remarks>
    public class RelationGroupType : SpecType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationGroupType"/> class.
        /// </summary>
        public RelationGroupType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationGroupType"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="ReqIFContent"/>
        /// </param>
        internal RelationGroupType(ReqIFContent reqIfContent)
            : base(reqIfContent)
        {
        }
    }
}
