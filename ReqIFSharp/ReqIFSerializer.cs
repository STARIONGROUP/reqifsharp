// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFSerializer.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2022 RHEA System S.A.
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The <see cref="ReqIF"/> Serializer class
    /// </summary>
    public class ReqIFSerializer : IReqIFSerializer
    {
        /// <summary>
        /// Serialize a <see cref="IEnumerable{ReqIF}"/> object and write its content in an XML-file in the corresponding path
        /// </summary>
        /// <param name="reqIfs">
        /// The <see cref="IEnumerable{ReqIF}"/> object to serialize
        /// </param>
        /// <param name="fileUri">
        /// The path of the output file
        /// </param>
        public void Serialize(IEnumerable<ReqIF> reqIfs, string fileUri)
        {
            this.FileBasedSerializerArgumentValidation(fileUri);

            var fileExtensionKind = fileUri.ConvertPathToSupportedFileExtensionKind();

            using (var fileStream = new FileStream(fileUri, FileMode.OpenOrCreate))
            {
                this.Serialize(reqIfs, fileStream, fileExtensionKind);
            }
        }

        /// <summary>
        /// Serialize a <see cref="IEnumerable{ReqIF}"/> object and write its content to the provided <see cref="Stream"/>
        /// </summary>
        /// <param name="reqIfs">
        /// The <see cref="IEnumerable{ReqIF}"/> object to serialize
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to serialize to
        /// </param>
        /// <param name="fileExtensionKind">
        /// The <see cref="SupportedFileExtensionKind"/> that specifies whether the resulting <see cref="Stream"/>
        /// contains the reqif file or a zip-archive of reqif files
        /// </param>
        public void Serialize(IEnumerable<ReqIF> reqIfs, Stream stream, SupportedFileExtensionKind fileExtensionKind)
        {
            this.StreamBasedSerializerArgumentValidation(reqIfs, stream, fileExtensionKind);

            switch (fileExtensionKind)
            {
                case SupportedFileExtensionKind.Reqif:

                    this.WriteXmlToStream(reqIfs.First(), stream);

                    break;
                case SupportedFileExtensionKind.Reqifz:
                    
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
                    {
                        var i = 0;

                        foreach (var reqIf in reqIfs)
                        {
                            var reqifEntryName =  $"{i++}_{reqIf.TheHeader.Identifier}.reqif";

                            var reqifEntry = archive.CreateEntry(reqifEntryName, CompressionLevel.Optimal);

                            using (var reqifEntryStream = reqifEntry.Open())
                            {
                                this.WriteXmlToStream(reqIf, reqifEntryStream);
                            }
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Async Serialize a <see cref="IEnumerable{ReqIF}"/> object and write its content in an XML-file in the corresponding path
        /// </summary>
        /// <param name="reqIfs">
        /// The <see cref="IEnumerable{ReqIF}"/> object to serialize
        /// </param>
        /// <param name="fileUri">
        /// The path of the output file
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        public async Task SerializeAsync(IEnumerable<ReqIF> reqIfs, string fileUri, CancellationToken token)
        {
            this.FileBasedSerializerArgumentValidation(fileUri);

            var fileExtensionKind = fileUri.ConvertPathToSupportedFileExtensionKind();

            using (var fileStream = new FileStream(fileUri, FileMode.OpenOrCreate))
            {
                await this.SerializeAsync(reqIfs, fileStream, fileExtensionKind, token);
            }
        }

        /// <summary>
        /// Async Serialize a <see cref="IEnumerable{ReqIF}"/> object and write its content to the provided <see cref="Stream"/>
        /// </summary>
        /// <param name="reqIfs">
        /// The <see cref="IEnumerable{ReqIF}"/> object to serialize
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to serialize to
        /// </param>
        /// <param name="fileExtensionKind">
        /// The <see cref="SupportedFileExtensionKind"/> that specifies whether the resulting <see cref="Stream"/>
        /// contains the reqif file or a zip-archive of reqif files
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        public async Task SerializeAsync(IEnumerable<ReqIF> reqIfs, Stream stream, SupportedFileExtensionKind fileExtensionKind, CancellationToken token)
        {
            this.StreamBasedSerializerArgumentValidation(reqIfs, stream, fileExtensionKind);
            
            switch (fileExtensionKind)
            {
                case SupportedFileExtensionKind.Reqif:

                    await this.WriteXmlToStreamAsync(reqIfs.First(), stream, token);
                    break;

                case SupportedFileExtensionKind.Reqifz:

                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
                    {
                        var i = 0;

                        foreach (var reqIf in reqIfs)
                        {
                            var reqifEntryName = $"{i++}_{reqIf.TheHeader.Identifier}.reqif";

                            var reqifEntry = archive.CreateEntry(reqifEntryName, CompressionLevel.Optimal);

                            using (var reqifEntryStream = reqifEntry.Open())
                            {
                                await this.WriteXmlToStreamAsync(reqIf, reqifEntryStream, token);
                            }
                        }

                        break;
                    }
                default:
                    throw new ArgumentException("only .reqif, .reqifz and .zip are supported file extensions.", nameof(fileExtensionKind));
            }
        }

        /// <summary>
        /// Argument validation for file based operations
        /// </summary>
        /// <param name="fileUri">
        /// The path of the output ReqIF file
        /// </param>
        private void FileBasedSerializerArgumentValidation(string fileUri)
        {
            if (fileUri == null)
            {
                throw new ArgumentNullException(nameof(fileUri), "The path of the ReqIF file cannot be null.");
            }

            if (fileUri == string.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(fileUri), "The path of the ReqIF file cannot be empty.");
            }
        }

        /// <summary>
        /// Argument validation for stream based operations
        /// </summary>
        /// <param name="reqIfs">
        /// The <see cref="IEnumerable{ReqIF}"/> object to serialize
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to serialize to
        /// </param>
        /// <param name="extensionKind">
        /// The <see cref="SupportedFileExtensionKind"/> that determines whether the data is serialied to
        /// an xml file or a (zip) archive
        /// </param>

        private void StreamBasedSerializerArgumentValidation(IEnumerable<ReqIF> reqIfs, Stream stream, SupportedFileExtensionKind extensionKind)
        {
            if (reqIfs == null)
            {
                throw new ArgumentNullException(nameof(reqIfs), "The reqIfs object cannot be null.");
            }
            
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), "The stream cannot be null.");
            }

            switch (extensionKind)
            {
                case SupportedFileExtensionKind.Reqif:

                    if (reqIfs.Count() != 1)
                    {
                        throw new ArgumentException("One and only one ReqIF object can be serialized to a reqif file. If multiple ReqIF objects need to be serialized, please make use of the reqifz format.", nameof(extensionKind));
                    }

                    break;
                case SupportedFileExtensionKind.Reqifz:

                    if (!reqIfs.Any())
                    {
                        throw new ArgumentException("At least one ReqIF object must be serialized.", nameof(extensionKind));
                    }

                    break;
                default:
                    throw new ArgumentException("only .reqif, .reqifz and .zip are supported file extensions.", nameof(extensionKind));
            }
        }

        /// <summary>
        /// write the contents of the <see cref="ReqIF"/> to a file
        /// </summary>
        /// <param name="reqIf">
        /// The <see cref="ReqIF"/> object to serialize
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to serialize to
        /// </param>
        /// <returns>
        /// an awaitable task
        /// </returns>
        private void WriteXmlToStream(ReqIF reqIf, Stream stream)
        {
            using (var writer = XmlWriter.Create(stream, this.CreateXmlWriterSettings(true)))
            {
                this.WriteXml(writer, reqIf);
                writer.Flush();
            }
        }

        /// <summary>
        /// write the contents of the <see cref="ReqIF"/> to a file
        /// </summary>
        /// <param name="reqIf">
        /// The <see cref="ReqIF"/> object to serialize
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to serialize to
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        /// an awaitable task
        /// </returns>
        private async Task WriteXmlToStreamAsync(ReqIF reqIf, Stream stream, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            using (var writer = XmlWriter.Create(stream, this.CreateXmlWriterSettings(true)))
            {
                await this.WriteXmlAsync(writer, reqIf, token);
            }
        }

        /// <summary>
        /// Create <see cref="XmlWriterSettings"/>
        /// </summary>
        /// <returns>
        /// an instance of <see cref="XmlWriterSettings"/>
        /// </returns>
        private XmlWriterSettings CreateXmlWriterSettings(bool asynchronous = false)
        {
            return new XmlWriterSettings
                {
                    Indent = true, 
                    Encoding = Encoding.UTF8,
                    ConformanceLevel = ConformanceLevel.Document,
                    Async = asynchronous
                };
        }

        /// <summary>
        /// Write root xml element and contained objects
        /// </summary>
        /// <param name="xmlWriter">
        /// The <see cref="XmlWriter"/> used to write the <see cref="ReqIF"/> object
        /// </param>
        /// <param name="reqIf">
        /// The <see cref="ReqIF"/> object that is to be serialized
        /// </param>
        private void WriteXml(XmlWriter xmlWriter, ReqIF reqIf)
        {
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("REQ-IF", DefaultXmlAttributeFactory.ReqIFSchemaUri);
            reqIf.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
        }

        /// <summary>
        /// Asynchronously write root xml element and contained objects
        /// </summary>
        /// <param name="xmlWriter">
        /// The <see cref="XmlWriter"/> used to write the <see cref="ReqIF"/> object
        /// </param>
        /// <param name="reqIf">
        /// The <see cref="ReqIF"/> object that is to be serialized
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        private async Task WriteXmlAsync(XmlWriter xmlWriter, ReqIF reqIf, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync(null,"REQ-IF", DefaultXmlAttributeFactory.ReqIFSchemaUri);
            await reqIf.WriteXmlAsync(xmlWriter, token);
            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
        }
    }
}
