﻿// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIfFactory.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
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
    using System;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The purpose of the <see cref="ReqIfFactory"/> is to create new instances of concrete classes
    /// that are defined in the <see cref="ReqIF"/> namespace.
    /// </summary>
    internal static class ReqIfFactory
    {
        /// <summary>
        /// returns the XML element name of the specified <see cref="AttributeDefinition"/>.
        /// </summary>
        /// <param name="attributeDefinition">
        /// an instance of <see cref="AttributeDefinition"/>.
        /// </param>
        /// <returns>
        /// a string that contains the XML element name.
        /// </returns>
        internal static string XmlName(AttributeDefinition attributeDefinition)
        {
            if (attributeDefinition is AttributeDefinitionBoolean)
            {
                return "ATTRIBUTE-DEFINITION-BOOLEAN";
            }

            if (attributeDefinition is AttributeDefinitionDate)
            {
                return "ATTRIBUTE-DEFINITION-DATE";
            }

            if (attributeDefinition is AttributeDefinitionEnumeration)
            {
                return "ATTRIBUTE-DEFINITION-ENUMERATION";
            }

            if (attributeDefinition is AttributeDefinitionInteger)
            {
                return "ATTRIBUTE-DEFINITION-INTEGER";
            }

            if (attributeDefinition is AttributeDefinitionReal)
            {
                return "ATTRIBUTE-DEFINITION-REAL";
            }

            if (attributeDefinition is AttributeDefinitionString)
            {
                return "ATTRIBUTE-DEFINITION-STRING";
            }

            if (attributeDefinition is AttributeDefinitionXHTML)
            {
                return "ATTRIBUTE-DEFINITION-XHTML";
            }

            throw new ArgumentException($"The {attributeDefinition.GetType()} type cannot be converted to an XML element name");
        }

        /// <summary>
        /// returns the XML element name of the specified <see cref="DatatypeDefinition"/>.
        /// </summary>
        /// <param name="datatypeDefinition">
        /// an instance of <see cref="DatatypeDefinition"/>.
        /// </param>
        /// <returns>
        /// a string that contains the XML element name.
        /// </returns>
        internal static string XmlName(DatatypeDefinition datatypeDefinition)
        {
            if (datatypeDefinition is DatatypeDefinitionBoolean)
            {
                return "DATATYPE-DEFINITION-BOOLEAN";
            }

            if (datatypeDefinition is DatatypeDefinitionDate)
            {
                return "DATATYPE-DEFINITION-DATE";
            }

            if (datatypeDefinition is DatatypeDefinitionEnumeration)
            {
                return "DATATYPE-DEFINITION-ENUMERATION";
            }

            if (datatypeDefinition is DatatypeDefinitionInteger)
            {
                return "DATATYPE-DEFINITION-INTEGER";
            }

            if (datatypeDefinition is DatatypeDefinitionReal)
            {
                return "DATATYPE-DEFINITION-REAL";
            }

            if (datatypeDefinition is DatatypeDefinitionString)
            {
                return "DATATYPE-DEFINITION-STRING";
            }

            if (datatypeDefinition is DatatypeDefinitionXHTML)
            {
                return "DATATYPE-DEFINITION-XHTML";
            }

            throw new ArgumentException($"The {datatypeDefinition.GetType()} type cannot be converted to an XML element name");
        }
        
        /// <summary>
        /// returns the XML element name of the specified <see cref="SpecType"/>.
        /// </summary>
        /// <param name="specType">
        /// an instance of <see cref="SpecType"/>.
        /// </param>
        /// <returns>
        /// a string that contains the XML element name.
        /// </returns>
        internal static string XmlName(SpecType specType)
        {
            if (specType is SpecObjectType)
            {
                return "SPEC-OBJECT-TYPE";
            }

            if (specType is SpecificationType)
            {
                return "SPECIFICATION-TYPE";
            }

            if (specType is SpecRelationType)
            {
                return "SPEC-RELATION-TYPE";
            }

            if (specType is RelationGroupType)
            {
                return "RELATION-GROUP-TYPE";
            }

            throw new ArgumentException($"The {specType.GetType()} type cannot be converted to an XML element name");
        }

        /// <summary>
        /// Constructs a new instance <see cref="AttributeDefinition"/> based on the XML Name
        /// </summary>
        /// <param name="xmlName">
        /// The XML name of the <see cref="AttributeDefinition"/> that is to be constructed
        /// </param>
        /// <param name="specType">
        /// The owning <see cref="SpecType"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        /// <returns>
        /// an instance of <see cref="AttributeDefinition"/>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when an invalid <paramref name="xmlName"/> is provided
        /// </exception>
        internal static AttributeDefinition AttributeDefinitionConstruct(string xmlName, SpecType specType, ILoggerFactory loggerFactory)
        {
            switch (xmlName)
            {
                case "ATTRIBUTE-DEFINITION-BOOLEAN":
                    return new AttributeDefinitionBoolean(specType, loggerFactory);
                case "ATTRIBUTE-DEFINITION-DATE":
                    return new AttributeDefinitionDate(specType, loggerFactory);
                case "ATTRIBUTE-DEFINITION-ENUMERATION":
                    return new AttributeDefinitionEnumeration(specType, loggerFactory);
                case "ATTRIBUTE-DEFINITION-INTEGER":
                    return new AttributeDefinitionInteger(specType, loggerFactory);
                case "ATTRIBUTE-DEFINITION-REAL":
                    return new AttributeDefinitionReal(specType, loggerFactory);
                case "ATTRIBUTE-DEFINITION-STRING":
                    return new AttributeDefinitionString(specType, loggerFactory);
                case "ATTRIBUTE-DEFINITION-XHTML":
                    return new AttributeDefinitionXHTML(specType, loggerFactory);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Constructs a new instance <see cref="DatatypeDefinition"/> based on the XML Name
        /// </summary>
        /// <param name="xmlname">
        /// The XML name of the <see cref="DatatypeDefinition"/> that is to be constructed
        /// </param>
        /// <param name="reqIfContent">
        /// The owning <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        /// <returns>
        /// an instance of <see cref="DatatypeDefinition"/>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when an invalid <paramref name="xmlname"/> is provided
        /// </exception>
        internal static DatatypeDefinition DatatypeDefinitionConstruct(string xmlname, ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
        {
            switch (xmlname)
            {
                case "DATATYPE-DEFINITION-BOOLEAN":
                    return new DatatypeDefinitionBoolean(reqIfContent, loggerFactory);
                case "DATATYPE-DEFINITION-DATE":
                    return new DatatypeDefinitionDate(reqIfContent, loggerFactory);
                case "DATATYPE-DEFINITION-ENUMERATION":
                    return new DatatypeDefinitionEnumeration(reqIfContent, loggerFactory);
                case "DATATYPE-DEFINITION-INTEGER":
                    return new DatatypeDefinitionInteger(reqIfContent, loggerFactory);
                case "DATATYPE-DEFINITION-REAL":
                    return new DatatypeDefinitionReal(reqIfContent, loggerFactory);
                case "DATATYPE-DEFINITION-STRING":
                    return new DatatypeDefinitionString(reqIfContent, loggerFactory);
                case "DATATYPE-DEFINITION-XHTML":
                    return new DatatypeDefinitionXHTML(reqIfContent, loggerFactory);
                default:
                    throw new ArgumentException($"{xmlname} is not a vaild DatatypeDefinition name");
            }
        }

        /// <summary>
        /// Constructs a new instance <see cref="SpecType"/> based on the XML Name
        /// </summary>
        /// <param name="xmlname">
        /// The XML name of the <see cref="SpecType"/> that is to be constructed
        /// </param>
        /// <param name="reqIfContent">
        /// The owning <see cref="ReqIFContent"/> object.
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        /// <returns>
        /// an instance of <see cref="SpecType"/>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when an invalid <paramref name="xmlname"/> is provided
        /// </exception>
        internal static SpecType SpecTypeConstruct(string xmlname, ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
        {
            switch (xmlname)
            {
                case "SPEC-OBJECT-TYPE":
                    return new SpecObjectType(reqIfContent, loggerFactory);
                case "SPECIFICATION-TYPE":
                    return new SpecificationType(reqIfContent, loggerFactory);
                case "SPEC-RELATION-TYPE":
                    return new SpecRelationType(reqIfContent, loggerFactory);
                case "RELATION-GROUP-TYPE":
                    return new RelationGroupType(reqIfContent, loggerFactory);
                default:
                    throw new ArgumentException($"{xmlname} is not a vaild SpecType name");
            }
        }
    }
}
