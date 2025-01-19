//  -------------------------------------------------------------------------------------------------
//  <copyright file="StringExtensions.cs" company="Starion Group S.A.">
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

namespace ReqIFSharp.Extensions
{
    using System;
    using System.Text;

    /// <summary>
    /// static extension methods for <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// BASE64 encodes a string
        /// </summary>
        /// <param name="text">
        /// the string that is to be encoded
        /// </param>
        /// <returns>
        /// a BASE64 encoded string
        /// </returns>
        public static string Base64Encode(this string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);

            var encodedString = Convert.ToBase64String(bytes);

            return encodedString;
        }

        /// <summary>
        /// BASE64 decodes a string
        /// </summary>
        /// <param name="text">
        /// the BASE64 encoded string that is to be decoded
        /// </param>
        /// <returns>
        /// a BASE64 decoded string
        /// </returns>
        public static string Base64Decode(this string text)
        {
            var bytes = Convert.FromBase64String(text);

            var decodedString = Encoding.UTF8.GetString(bytes);

            return decodedString;
        }
    }
}
