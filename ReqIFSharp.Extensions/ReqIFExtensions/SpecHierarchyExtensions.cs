//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecHierarchyExtensions.cs" company="RHEA System S.A.">
// 
//    Copyright 2017-2024 RHEA System S.A.
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

    using ReqIFSharp;

    /// <summary>
    /// The <see cref="SpecHierarchyExtensions"/> class provides a number of extension methods for the <see cref="SpecHierarchy"/> class
    /// </summary>
    public static class SpecHierarchyExtensions
    {
        /// <summary>
        /// Queries all the contained (child) <see cref="SpecHierarchy"/> objects in a recursive manner from the <see cref="SpecHierarchy"/>
        /// </summary>
        /// <param name="specHierarchy">
        /// The subject <see cref="SpecHierarchy"/> from which all the child <see cref="SpecHierarchy"/> are queried.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecHierarchy}"/>
        /// </returns>
        public static IEnumerable<SpecHierarchy> QueryAllContainedSpecHierarchies(this SpecHierarchy specHierarchy)
        {
            var result = new List<SpecHierarchy>();

            foreach (var child in specHierarchy.Children)
            {
                result.Add(child);

                var subChildren = child.QueryAllContainedSpecHierarchies();

                result.AddRange(subChildren);
            }

            return result;
        }
    }
}
