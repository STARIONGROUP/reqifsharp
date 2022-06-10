//  -------------------------------------------------------------------------------------------------
//  <copyright file="AttributeValueExtensions.cs" company="RHEA System S.A.">
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
        /// the subjectr <see cref="attributeValue"/>
        /// </param>
        /// <returns>
        /// a formatted string
        /// </returns>
        public static string QueryFormattedValue(this AttributeValue attributeValue)
        {
            switch (attributeValue)
            {
                case AttributeValueBoolean attributeValueBoolean:
                    return attributeValueBoolean.TheValue.ToString();
                case AttributeValueDate attributeValueDate:
                    return attributeValueDate.TheValue.ToString("yyyy-MM-dd, HH:mm:ss", CultureInfo.InvariantCulture);
                case AttributeValueEnumeration attributeValueEnumeration:
                    return string.Join(";", attributeValueEnumeration.Values.Select(x => x.Properties.OtherContent));
                case AttributeValueInteger attributeValueInteger:
                    return attributeValueInteger.TheValue.ToString();
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
