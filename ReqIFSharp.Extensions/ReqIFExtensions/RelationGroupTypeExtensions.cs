//  -------------------------------------------------------------------------------------------------
//  <copyright file="RelationGroupTypeExtensions.cs" company="RHEA System S.A.">
// 
//    Copyright 2017-2022 RHEA System S.A.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  -------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.ReqIFExtensions
{
    using System.Collections.Generic;
    using System.Linq;

    using ReqIFSharp;

    /// <summary>
    /// The <see cref="RelationGroupTypeExtensions"/> class provides a number of extension methods for the <see cref="RelationGroupType"/> class
    /// </summary>
    public static class RelationGroupTypeExtensions
    {
        /// <summary>
        /// Queries the <see cref="RelationGroup"/>s that are referencing the <see cref="RelationGroupType"/>
        /// </summary>
        /// <param name="relationGroupType">
        /// The subject <see cref="SpecObjectType"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecObject}"/> that are referencing the <see cref="RelationGroupType"/>
        /// </returns>
        public static IEnumerable<RelationGroup> QueryReferencingRelationGroups(this RelationGroupType relationGroupType)
        {
            var relationGroups = relationGroupType.ReqIFContent.SpecRelationGroups;

            var result = relationGroups.Where(x => x.Type == relationGroupType);

            return result;
        }
    }
}
