//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecObjectExtensions.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2024 Starion Group S.A.
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
    /// The <see cref="SpecificationExtensions"/> class provides a number of extension methods for the <see cref="Specification"/> class
    /// </summary>
    public static class SpecObjectExtensions
    {
        /// <summary>
        /// Queries the <see cref="Specification"/> that the <see cref="SpecObject"/> is contained by
        /// </summary>
        /// <param name="specObject">
        /// The subject <see cref="SpecObject"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{Specification}"/> that the <see cref="SpecObject"/> is contained by
        /// </returns>
        public static IEnumerable<Specification> QueryContainerSpecifications(this SpecObject specObject)
        {
            if (specObject == null)
            {
                throw new ArgumentNullException(nameof(specObject));
            }

            if (specObject.ReqIFContent == null)
            {
                throw new InvalidOperationException("The owning ReqIFContent of the SpecObject is not set");
            }

            var result = new List<Specification>();

            foreach (var specification in specObject.ReqIFContent.Specifications)
            {
                var specobjects = specification.QueryAllContainedSpecObjects();

                if (specobjects.Any(x => x.Identifier == specObject.Identifier))
                {
                    result.Add(specification);
                }
            }

            return result;
        }

        /// <summary>
        /// Queries all the <see cref="SpecRelation"/>s that the <see cref="SpecObject"/> is either the source or target of
        /// </summary>
        /// <param name="specObject">
        /// The subject <see cref="SpecObject"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecRelation}"/> that the <see cref="SpecObject"/> is either the source or target of
        /// </returns>
        public static IEnumerable<SpecRelation> QuerySpecRelations(this SpecObject specObject)
        {
            if (specObject == null)
            {
                throw new ArgumentNullException(nameof(specObject));
            }

            var result = new List<SpecRelation>();

            foreach (var specRelation in specObject.ReqIFContent.SpecRelations)
            {
                if (specRelation.Source == specObject || specRelation.Target == specObject)
                {
                    result.Add(specRelation);
                }
            }

            return result;
        }
    }
}
