// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionExtensions.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.ReqIFExtensions
{
    using System;
    using System.Globalization;
    using System.Linq;

    using ReqIFSharp;

    /// <summary>
    /// The <see cref="AttributeDefinitionExtensions"/> class provides a number of extension methods for the <see cref="AttributeDefinition"/> class
    /// </summary>
    public static class AttributeDefinitionExtensions
    {
        /// <summary>
        /// Queries the human-readable name of the <see cref="AttributeDefinition"/>
        /// </summary>
        /// <param name="attributeDefinition">
        /// The subject <see cref="AttributeDefinition"/>
        /// </param>
        /// <returns>
        /// A human-readable name (Boolean, Date, Enumeration, Integer, Real, String, XHTML).
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// thrown when the specified <see cref="AttributeDefinition"/> is not supported
        /// </exception> 
        public static string QueryDatatypeName(this AttributeDefinition attributeDefinition)
        {
            if (attributeDefinition == null)
            {
                throw new ArgumentNullException(nameof(attributeDefinition));
            }

            switch (attributeDefinition)
            {
                case AttributeDefinitionBoolean:
                    return "Boolean";
                case AttributeDefinitionDate:
                    return "Date";
                case AttributeDefinitionEnumeration:
                    return "Enumeration";
                case AttributeDefinitionInteger:
                    return "Integer";
                case AttributeDefinitionReal:
                    return "Real";
                case AttributeDefinitionString:
                    return "String";
                case AttributeDefinitionXHTML:
                    return "XHTML";
                default:
                    throw new InvalidOperationException("");
            }
        }

        /// <summary>
        /// Queries the default value of the <see cref="AttributeDefinition"/> as formatted text
        /// </summary>
        /// <param name="attributeDefinition">
        /// the subject <see cref="AttributeDefinition"/>
        /// </param>
        /// <returns>
        /// a formatted string
        /// </returns>
        public static string QueryDefaultValueAsFormattedString(this AttributeDefinition attributeDefinition)
        {
            if (attributeDefinition == null)
            {
                throw new ArgumentNullException(nameof(attributeDefinition));
            }

            const string notSet = "NOT SET";

            switch (attributeDefinition)
            {
                case AttributeDefinitionBoolean attributeDefinitionBoolean:
                    return attributeDefinitionBoolean.DefaultValue != null
                        ? attributeDefinitionBoolean.DefaultValue.TheValue.ToString(CultureInfo.InvariantCulture)
                        : notSet;
                case AttributeDefinitionDate attributeDefinitionDate:
                    return attributeDefinitionDate.DefaultValue != null
                        ? attributeDefinitionDate.DefaultValue.TheValue.ToString("MMMM dd, yyyy",
                            CultureInfo.InvariantCulture)
                        : notSet;
                case AttributeDefinitionEnumeration attributeDefinitionEnumeration:
                    return attributeDefinitionEnumeration.DefaultValue != null
                        ? attributeDefinitionEnumeration.DefaultValue.Values.FirstOrDefault()?.ToString()
                        : notSet;
                case AttributeDefinitionInteger attributeDefinitionInteger:
                    return attributeDefinitionInteger.DefaultValue != null
                        ? attributeDefinitionInteger.DefaultValue.TheValue.ToString(CultureInfo.InvariantCulture)
                        : notSet;
                case AttributeDefinitionReal attributeDefinitionReal:
                    return attributeDefinitionReal.DefaultValue != null
                        ? attributeDefinitionReal.DefaultValue.TheValue.ToString(CultureInfo.InvariantCulture)
                        : notSet;
                case AttributeDefinitionString attributeDefinitionString:
                    return attributeDefinitionString.DefaultValue != null
                        ? attributeDefinitionString.DefaultValue.TheValue
                        : notSet;
                case AttributeDefinitionXHTML attributeDefinitionXHTML:
                    return attributeDefinitionXHTML.DefaultValue != null
                        ? attributeDefinitionXHTML.DefaultValue.TheValue
                        : notSet;
                default:
                    throw new InvalidOperationException("");
            }
        }
    }
}
