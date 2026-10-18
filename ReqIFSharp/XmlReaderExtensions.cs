// -------------------------------------------------------------------------------------------------
// <copyright file="XmlReaderExtensions.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
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
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Extension methods for <see cref="XmlReader"/>
    /// </summary>
    internal static class XmlReaderExtensions
    {
        /// <summary>
        /// Reads the text content of the current element and leaves the reader on its end tag
        /// (or on the element itself when it is empty).
        /// </summary>
        /// <remarks>
        /// <see cref="XmlReader.ReadElementContentAsString()"/> moves the reader past the end tag, onto the next node.
        /// In a loop that calls <see cref="XmlReader.Read"/> for every iteration, that skips the next sibling
        /// element when there is no whitespace between the elements. Use this method in such a loop instead.
        /// </remarks>
        /// <param name="reader">
        /// The <see cref="XmlReader"/>, positioned on an element
        /// </param>
        /// <returns>
        /// The text content of the element; an empty string when the element is empty
        /// </returns>
        /// <exception cref="XmlException">
        /// The reader is not on an element, or the element contains child elements
        /// </exception>
        internal static string ReadElementContentAsStringToEndElement(this XmlReader reader)
        {
            if (reader.MoveToContent() != XmlNodeType.Element)
            {
                throw CreateException(reader, $"The reader must be positioned on an element, not on {reader.NodeType}");
            }

            if (reader.IsEmptyElement)
            {
                return string.Empty;
            }

            var localName = reader.LocalName;

            reader.Read();

            var content = reader.NodeType == XmlNodeType.Element ? null : reader.ReadContentAsString();

            if (reader.NodeType != XmlNodeType.EndElement)
            {
                throw CreateException(reader, $"The {localName} element may only contain text");
            }

            return content;
        }

        /// <summary>
        /// Asynchronously reads the text content of the current element and leaves the reader on its end tag
        /// (or on the element itself when it is empty).
        /// </summary>
        /// <remarks>
        /// <see cref="XmlReader.ReadElementContentAsStringAsync()"/> moves the reader past the end tag, onto the next node.
        /// In a loop that calls <see cref="XmlReader.ReadAsync"/> for every iteration, that skips the next sibling
        /// element when there is no whitespace between the elements. Use this method in such a loop instead.
        /// </remarks>
        /// <param name="reader">
        /// The <see cref="XmlReader"/>, positioned on an element
        /// </param>
        /// <returns>
        /// The text content of the element; an empty string when the element is empty
        /// </returns>
        /// <exception cref="XmlException">
        /// The reader is not on an element, or the element contains child elements
        /// </exception>
        internal static async Task<string> ReadElementContentAsStringToEndElementAsync(this XmlReader reader)
        {
            if (await reader.MoveToContentAsync() != XmlNodeType.Element)
            {
                throw CreateException(reader, $"The reader must be positioned on an element, not on {reader.NodeType}");
            }

            if (reader.IsEmptyElement)
            {
                return string.Empty;
            }

            var localName = reader.LocalName;

            await reader.ReadAsync();

            var content = reader.NodeType == XmlNodeType.Element ? null : await reader.ReadContentAsStringAsync();

            if (reader.NodeType != XmlNodeType.EndElement)
            {
                throw CreateException(reader, $"The {localName} element may only contain text");
            }

            return content;
        }

        /// <summary>
        /// Creates an <see cref="XmlException"/> with the line and position of the reader, when it has them
        /// </summary>
        private static XmlException CreateException(XmlReader reader, string message)
        {
            var lineInfo = reader as IXmlLineInfo;

            return lineInfo != null && lineInfo.HasLineInfo()
                ? new XmlException(message, null, lineInfo.LineNumber, lineInfo.LinePosition)
                : new XmlException(message);
        }
    }
}
