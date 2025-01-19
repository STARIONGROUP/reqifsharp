//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecRelationTypeExtensions.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2025 Starion Group S.A.
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
    /// The <see cref="SpecRelationTypeExtensions"/> class provides a number of extension methods for the <see cref="SpecRelationType"/> class
    /// </summary>
    public static class SpecRelationTypeExtensions
    {
        /// <summary>
        /// Queries the <see cref="SpecRelation"/>s that are referencing the <see cref="SpecRelationType"/>
        /// </summary>
        /// <param name="specRelationType">
        /// The subject <see cref="SpecRelationType"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecRelation}"/> that are referencing the <see cref="SpecRelationType"/>
        /// </returns>
        public static IEnumerable<SpecRelation> QueryReferencingSpecRelations(this SpecRelationType specRelationType)
        {
            if (specRelationType == null)
            {
                throw new ArgumentNullException(nameof(specRelationType));
            }

            if (specRelationType.ReqIFContent == null)
            {
                throw new InvalidOperationException("The owning ReqIFContent of the SpecRelationType is not set.");
            }

            var specRelations = specRelationType.ReqIFContent.SpecRelations;

            var result = specRelations.Where(x => x.Type == specRelationType);

            return result;
        }
    }
}
