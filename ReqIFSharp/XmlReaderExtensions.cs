// -------------------------------------------------------------------------------------------------
// <copyright file="XmlReaderExtensions.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;

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
        /// Skips the current element, with its content, when it is not a ReqIF element: when it is in a namespace
        /// other than the ReqIF namespace. Elements in no namespace are ReqIF elements of a document without namespace.
        /// </summary>
        /// <remarks>
        /// A warning is logged for the skipped element, and the reader is left on its end tag (or on the element
        /// itself when it is empty), so that the next <see cref="XmlReader.Read"/> moves to the node after it.
        /// The reader is first moved to content, see <see cref="XmlReader.MoveToContent"/>.
        /// </remarks>
        /// <param name="reader">
        /// The <see cref="XmlReader"/>
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger"/> used to log the warning; no warning is logged when it is null
        /// </param>
        /// <returns>
        /// true when the element has been skipped; false when the reader is not on an element or on a ReqIF element
        /// </returns>
        internal static bool SkipForeignElement(this XmlReader reader, ILogger logger)
        {
            if (reader.MoveToContent() != XmlNodeType.Element || IsReqIFNamespace(reader.NamespaceURI))
            {
                return false;
            }

            LogForeignElement(reader, logger);

            if (!reader.IsEmptyElement)
            {
                using (var subtree = reader.ReadSubtree())
                {
                    while (subtree.Read())
                    {
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Asynchronously skips the current element, with its content, when it is not a ReqIF element: when it is in a
        /// namespace other than the ReqIF namespace. Elements in no namespace are ReqIF elements of a document without namespace.
        /// </summary>
        /// <remarks>
        /// A warning is logged for the skipped element, and the reader is left on its end tag (or on the element
        /// itself when it is empty), so that the next <see cref="XmlReader.ReadAsync"/> moves to the node after it.
        /// The reader is first moved to content, see <see cref="XmlReader.MoveToContentAsync"/>.
        /// </remarks>
        /// <param name="reader">
        /// The <see cref="XmlReader"/>
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger"/> used to log the warning; no warning is logged when it is null
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// true when the element has been skipped; false when the reader is not on an element or on a ReqIF element
        /// </returns>
        internal static async Task<bool> SkipForeignElementAsync(this XmlReader reader, ILogger logger, CancellationToken token)
        {
            if (await reader.MoveToContentAsync() != XmlNodeType.Element || IsReqIFNamespace(reader.NamespaceURI))
            {
                return false;
            }

            LogForeignElement(reader, logger);

            if (!reader.IsEmptyElement)
            {
                using (var subtree = reader.ReadSubtree())
                {
                    while (await subtree.ReadAsync())
                    {
                        token.ThrowIfCancellationRequested();
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Advances the reader to the next descendant ReqIF element with the given local name, as
        /// <see cref="XmlReader.ReadToDescendant(string, string)"/> does. A ReqIF element is an element in the ReqIF
        /// namespace or in no namespace. Elements in another namespace are skipped with their content, and a warning
        /// is logged for each, also when the reader is on such an element.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/>, positioned on the element whose descendants are searched. The reader is first
        /// moved to content (see <see cref="XmlReader.MoveToContent"/>), so on whitespace the next element is searched.
        /// </param>
        /// <param name="localName">
        /// The local name of the element to find
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger"/> used to log skipped elements; no warning is logged when it is null
        /// </param>
        /// <returns>
        /// true when the element was found, with the reader on it; false when it was not found, with the reader on the
        /// end tag of the element it started on (or on that element when it is empty). When that element is not a ReqIF
        /// element it is skipped: the reader is on its end tag (or on it when it is empty) and false is returned.
        /// </returns>
        internal static bool ReadToReqIFDescendant(this XmlReader reader, string localName, ILogger logger)
        {
            if (reader.SkipForeignElement(logger) || reader.NodeType != XmlNodeType.Element || reader.IsEmptyElement)
            {
                return false;
            }

            var depth = reader.Depth;

            while (reader.Read() && reader.Depth > depth)
            {
                if (reader.NodeType != XmlNodeType.Element || reader.SkipForeignElement(logger))
                {
                    continue;
                }

                if (reader.LocalName == localName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Asynchronously advances the reader to the next descendant ReqIF element with the given local name, as
        /// <see cref="XmlReader.ReadToDescendant(string, string)"/> does. A ReqIF element is an element in the ReqIF
        /// namespace or in no namespace. Elements in another namespace are skipped with their content, and a warning
        /// is logged for each, also when the reader is on such an element.
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/>, positioned on the element whose descendants are searched. The reader is first
        /// moved to content (see <see cref="XmlReader.MoveToContent"/>), so on whitespace the next element is searched.
        /// </param>
        /// <param name="localName">
        /// The local name of the element to find
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger"/> used to log skipped elements; no warning is logged when it is null
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// true when the element was found, with the reader on it; false when it was not found, with the reader on the
        /// end tag of the element it started on (or on that element when it is empty). When that element is not a ReqIF
        /// element it is skipped: the reader is on its end tag (or on it when it is empty) and false is returned.
        /// </returns>
        internal static async Task<bool> ReadToReqIFDescendantAsync(this XmlReader reader, string localName, ILogger logger, CancellationToken token)
        {
            if (await reader.SkipForeignElementAsync(logger, token) || reader.NodeType != XmlNodeType.Element || reader.IsEmptyElement)
            {
                return false;
            }

            var depth = reader.Depth;

            while (await reader.ReadAsync() && reader.Depth > depth)
            {
                token.ThrowIfCancellationRequested();

                if (reader.NodeType != XmlNodeType.Element || await reader.SkipForeignElementAsync(logger, token))
                {
                    continue;
                }

                if (reader.LocalName == localName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reads the rest of a subtree reader, skipping every element in a namespace other than the ReqIF namespace
        /// (with a warning, see <see cref="SkipForeignElement"/>), so that such elements are reported also when they
        /// come after the content that was read.
        /// </summary>
        /// <param name="reader">
        /// A subtree reader, see <see cref="XmlReader.ReadSubtree"/>: it is read to its end
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger"/> used to log skipped elements; no warning is logged when it is null
        /// </param>
        internal static void SkipForeignElementsToEnd(this XmlReader reader, ILogger logger)
        {
            while (reader.Read())
            {
                reader.SkipForeignElement(logger);
            }
        }

        /// <summary>
        /// Asynchronously reads the rest of a subtree reader, skipping every element in a namespace other than the ReqIF
        /// namespace (with a warning, see <see cref="SkipForeignElementAsync"/>), so that such elements are reported also
        /// when they come after the content that was read.
        /// </summary>
        /// <param name="reader">
        /// A subtree reader, see <see cref="XmlReader.ReadSubtree"/>: it is read to its end
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger"/> used to log skipped elements; no warning is logged when it is null
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal static async Task SkipForeignElementsToEndAsync(this XmlReader reader, ILogger logger, CancellationToken token)
        {
            while (await reader.ReadAsync())
            {
                token.ThrowIfCancellationRequested();

                await reader.SkipForeignElementAsync(logger, token);
            }
        }

        /// <summary>
        /// Checks whether a namespace is the namespace of ReqIF elements: the ReqIF namespace, or no namespace
        /// </summary>
        private static bool IsReqIFNamespace(string namespaceUri)
        {
            return namespaceUri.Length == 0 || namespaceUri == DefaultXmlAttributeFactory.ReqIFSchemaUri;
        }

        /// <summary>
        /// Logs that the element the reader is on is not a ReqIF element and is skipped
        /// </summary>
        private static void LogForeignElement(XmlReader reader, ILogger logger)
        {
            var xmlLineInfo = reader as IXmlLineInfo;

            logger?.LogWarning("The {Name} element at line:position {LineNumber}:{LinePosition} is in the namespace {NamespaceUri} instead of {ReqIFNamespaceUri}; it is not a ReqIF element and has been skipped",
                reader.Name, xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition, reader.NamespaceURI, DefaultXmlAttributeFactory.ReqIFSchemaUri);
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
