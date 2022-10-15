//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecificationTypeExtensions.cs" company="RHEA System S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ReqIFSharp;

    /// <summary>
    /// The <see cref="SpecificationTypeExtensions"/> class provides a number of extension methods for the <see cref="SpecificationType"/> class
    /// </summary>
    public static class SpecificationTypeExtensions
    {
        /// <summary>
        /// Queries the <see cref="Specification"/>s that are referencing the <see cref="SpecificationType"/>
        /// </summary>
        /// <param name="specificationType">
        /// The subject <see cref="SpecificationType"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{Specification}"/> that are referencing the <see cref="SpecificationType"/>
        /// </returns>
        public static IEnumerable<Specification> QueryReferencingSpecifications(this SpecificationType specificationType)
        {
            if (specificationType.ReqIFContent == null)
            {
                throw new InvalidOperationException("The owning ReqIFContent of the SpecificationType is not set.");
            }

            var specifications = specificationType.ReqIFContent.Specifications;

            var result = specifications.Where(x => x.Type == specificationType);

            return result;
        }
    }
}
