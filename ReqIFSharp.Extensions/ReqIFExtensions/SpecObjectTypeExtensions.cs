//  -------------------------------------------------------------------------------------------------
//  <copyright file="SpecObjectTypeExtensions.cs" company="Starion Group S.A.">
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
    /// The <see cref="SpecObjectTypeExtensions"/> class provides a number of extension methods for the <see cref="SpecObjectType"/> class
    /// </summary>
    public static class SpecObjectTypeExtensions
    {
        /// <summary>
        /// Queries the <see cref="SpecObject"/>s that are referencing the <see cref="SpecObjectType"/>
        /// </summary>
        /// <param name="specObjectType">
        /// The subject <see cref="SpecObjectType"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{SpecObject}"/> that are referencing the <see cref="SpecObjectType"/>
        /// </returns>
        public static IEnumerable<SpecObject> QueryReferencingSpecObjects(this SpecObjectType specObjectType)
        {
            if (specObjectType == null)
            {
                throw new ArgumentNullException(nameof(specObjectType));
            }

            if (specObjectType.ReqIFContent == null)
            {
                throw new InvalidOperationException("The owning ReqIFContent of the SpecObjectType is not set.");
            }

            var specObjects = specObjectType.ReqIFContent.SpecObjects;

            var result =  specObjects.Where(x => x.Type == specObjectType);
            
            return result;
        }
    }
}
