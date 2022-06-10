//  -------------------------------------------------------------------------------------------------
//  <copyright file="DatatypeDefinitionExtensions.cs" company="RHEA System S.A.">
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
    /// The <see cref="DatatypeDefinitionExtensions"/> class provides a number of extension methods for the <see cref="DatatypeDefinition"/> class
    /// </summary>
    public static class DatatypeDefinitionExtensions
    {
        /// <summary>
        /// Queries the human readable name of the <see cref="DatatypeDefinition"/>
        /// </summary>
        /// <param name="datatypeDefinition">
        /// The subject <see cref="DatatypeDefinition"/>
        /// </param>
        /// <returns>
        /// A human readable name (Boolean, Date, Enumeration, Integer, Real, String, XHTML).
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// thrown when the specified <see cref="DatatypeDefinition"/> is not supported
        /// </exception> 
        public static string QueryDatatypeName(this DatatypeDefinition datatypeDefinition)
        {
            switch (datatypeDefinition)
            {
                case DatatypeDefinitionBoolean:
                    return "Boolean";
                case DatatypeDefinitionDate:
                    return "Date";
                case DatatypeDefinitionEnumeration:
                    return "Enumeration";
                case DatatypeDefinitionInteger:
                    return "Integer";
                case DatatypeDefinitionReal:
                    return "Real";
                case DatatypeDefinitionString:
                    return "String";
                case DatatypeDefinitionXHTML:
                    return "XHTML";
                default:
                    throw new InvalidOperationException("");
            }
        }

        /// <summary>
        /// Queries the <see cref="AttributeDefinition"/>s that are referencing the <see cref="DatatypeDefinition"/>
        /// </summary>
        /// <param name="datatypeDefinition">
        /// The subject <see cref="DatatypeDefinition"/>
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{AttributeDefinition}"/> that are referencing the <see cref="DatatypeDefinition"/>
        /// </returns>
        public static IEnumerable<AttributeDefinition> QueryReferencingAttributeDefinitions(this DatatypeDefinition datatypeDefinition)
        {
            var result = new HashSet<AttributeDefinition>();

            var spectypes = datatypeDefinition.ReqIFContent.SpecTypes;
            foreach (var spectype in spectypes)
            {
                var attributeDefinitions = spectype.SpecAttributes.Where(x => x.DatatypeDefinition == datatypeDefinition);
                foreach (var attributeDefinition in attributeDefinitions)
                {
                    result.Add(attributeDefinition);
                }
            }

            return result;
        }
    }
}
