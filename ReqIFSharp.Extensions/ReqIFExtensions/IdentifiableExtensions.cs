//  -------------------------------------------------------------------------------------------------
//  <copyright file="IdentifiableExtensions.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2026 Starion Group S.A.
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

    using ReqIFSharp;

    /// <summary>
    /// The <see cref="IdentifiableExtensions"/> class provides a number of extension methods for the <see cref="Identifiable"/> class
    /// </summary>
    public static class IdentifiableExtensions
    {
        /// <summary>
        /// Queries a human-friendly name for the type
        /// </summary>
        /// <param name="identifiable">
        /// The subject <see cref="Identifiable"/>
        /// </param>
        /// <returns>
        /// A string
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string QueryTypeDisplayName(this Identifiable identifiable)
        {
            if (identifiable == null)
            {
                throw new ArgumentNullException(nameof(identifiable));
            }

            switch (identifiable)
            {
                case AttributeDefinitionBoolean:
                    return "Boolean Attribute Definition";
                case AttributeDefinitionDate:
                    return "Date Attribute Definition";
                case AttributeDefinitionEnumeration:
                    return "Enumeration Attribute Definition";
                case AttributeDefinitionInteger:
                    return "Integer Attribute Definition";
                case AttributeDefinitionReal:
                    return "Real Attribute Definition";
                case AttributeDefinitionString:
                    return "String Attribute Definition";
                case AttributeDefinitionXHTML:
                    return "XHTML Attribute Definition";
                case DatatypeDefinitionBoolean:
                    return "Boolean Datatype Definition";
                case DatatypeDefinitionDate:
                    return "Date Datatype Definition";
                case DatatypeDefinitionEnumeration:
                    return "Enumeration Datatype Definition";
                case DatatypeDefinitionInteger:
                    return "Integer Datatype Definition";
                case DatatypeDefinitionReal:
                    return "Real Datatype Definition";
                case DatatypeDefinitionString:
                    return "String Datatype Definition";
                case DatatypeDefinitionXHTML:
                    return "XHTML Datatype Definition";
                case EnumValue:
                    return "Enum Value";
                case RelationGroup:
                    return "Relation Group";
                case SpecHierarchy:
                    return "Spec Hierarchy";
                case Specification:
                    return "Specification";
                case SpecObject:
                    return "Spec Object";
                case SpecRelation:
                    return "Spec Relation";
                case RelationGroupType:
                    return "Relation Group Type";
                case SpecificationType:
                    return "Specification Type";
                case SpecObjectType:
                    return "Spec Object Type";
                case SpecRelationType:
                    return "Spec Relation Type";
                default:
                    throw new InvalidOperationException($"{identifiable.GetType()} is not supported");
            }
        }
    }
}
