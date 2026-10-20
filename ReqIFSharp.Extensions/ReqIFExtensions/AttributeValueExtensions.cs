// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeValueExtensions.cs" company="Starion Group S.A.">
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
    /// The <see cref="AttributeValueExtensions"/> class provides a number of extension methods for the <see cref="AttributeValue"/> class
    /// </summary>
    public static class AttributeValueExtensions
    {
        /// <summary>
        /// Queries the value of the <see cref="AttributeValue"/> as a formatted string
        /// </summary>
        /// <param name="attributeValue">
        /// the subject <see cref="AttributeValue"/>
        /// </param>
        /// <returns>
        /// a formatted string
        /// </returns>
        public static string QueryFormattedValue(this AttributeValue attributeValue)
        {
            if (attributeValue == null)
            {
                throw new ArgumentNullException(nameof(attributeValue));
            }

            switch (attributeValue)
            {
                case AttributeValueBoolean attributeValueBoolean:
                    return attributeValueBoolean.TheValue.ToString(CultureInfo.InvariantCulture);
                case AttributeValueDate attributeValueDate:
                    return attributeValueDate.TheValue.ToString("yyyy-MM-dd, HH:mm:ss", CultureInfo.InvariantCulture);
                case AttributeValueEnumeration attributeValueEnumeration:
                    return string.Join(";", attributeValueEnumeration.Values.Select(x => x.Properties.OtherContent));
                case AttributeValueInteger attributeValueInteger:
                    return attributeValueInteger.TheValue.ToString(CultureInfo.InvariantCulture);
                case AttributeValueReal attributeValueReal:
                    return attributeValueReal.TheValue.ToString(CultureInfo.InvariantCulture);
                case AttributeValueString attributeValueString:
                    return attributeValueString.TheValue;
                case AttributeValueXHTML attributeValueXHTML:
                    return attributeValueXHTML.TheValue;
                default:
                    throw new InvalidOperationException("");
            }
        }
    }
}
