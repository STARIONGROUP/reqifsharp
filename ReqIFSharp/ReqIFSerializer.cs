// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFSerializer.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
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
    using System.IO;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// The <see cref="ReqIF"/> Serializer class
    /// </summary>
    public class ReqIFSerializer : IReqIFSerializer
    {
        /// <summary>
        /// Serialize a <see cref="ReqIF"/> object and write its content in an XML-file in the corresponding path
        /// </summary>
        /// <param name="reqIf">
        /// The <see cref="ReqIF"/> object to serialize
        /// </param>
        /// <param name="fileUri">
        /// The path of the output file
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Serialize(ReqIF reqIf, string fileUri)
        {
            if (reqIf == null)
            {
                throw new ArgumentNullException(nameof(reqIf), "The reqIf object cannot be null.");
            }

            if (fileUri == null)
            {
                throw new ArgumentNullException(nameof(fileUri), "The path of the file cannot be null.");
            }

            if (fileUri == string.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(fileUri), "The path of the file cannot be empty.");
            }
            
            using (var writer = XmlWriter.Create(fileUri, this.CreateXmlWriterSettings()))
            {
                this.WriteXml(writer, reqIf);
            }
        }

        /// <summary>
        /// Async Serialize a <see cref="ReqIF"/> object and write its content in an XML-file in the corresponding path
        /// </summary>
        /// <param name="reqIf">The <see cref="ReqIF"/> object to serialize</param>
        /// <param name="fileUri">The path of the output file</param>
        /// <exception cref="XmlSchemaValidationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        public async Task SerializeAsync(ReqIF reqIf, string fileUri)
        {
            if (reqIf == null)
            {
                throw new ArgumentNullException(nameof(reqIf), "The reqIf object cannot be null.");
            }

            if (fileUri == null)
            {
                throw new ArgumentNullException(nameof(fileUri), "The path of the file cannot be null.");
            }

            if (fileUri == string.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(fileUri), "The path of the file cannot be empty.");
            }

            using (var writer = XmlWriter.Create(fileUri, this.CreateXmlWriterSettings(true)))
            {
                await this.WriteXmlAsync(writer, reqIf);
            }
        }

        /// <summary>
        /// Serialize a <see cref="ReqIF"/> object and write its content to the provided <see cref="Stream"/>
        /// </summary>
        /// <param name="reqIf">
        /// The <see cref="ReqIF"/> object to serialize
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to serialize to
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Serialize(ReqIF reqIf, Stream stream)
        {
            if (reqIf == null)
            {
                throw new ArgumentNullException(nameof(reqIf), "The reqIf object cannot be null.");
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), "The stream cannot be null.");
            }
            
            using (var writer = XmlWriter.Create(stream, this.CreateXmlWriterSettings()))
            {
                this.WriteXml(writer, reqIf);
            }
        }

        /// <summary>
        /// Async Serialize a <see cref="ReqIF"/> object and write its content to the provided <see cref="Stream"/>
        /// </summary>
        /// <param name="reqIf">
        /// The <see cref="ReqIF"/> object to serialize
        /// </param>
        /// <param name="stream">
        /// The <see cref="Stream"/> to serialize to
        /// </param>
        /// <exception cref="XmlSchemaValidationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        public async Task SerializeAsync(ReqIF reqIf, Stream stream)
        {
            if (reqIf == null)
            {
                throw new ArgumentNullException(nameof(reqIf), "The reqIf object cannot be null.");
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), "The stream cannot be null.");
            }

            using (var writer = XmlWriter.Create(stream, this.CreateXmlWriterSettings(true)))
            {
                await this.WriteXmlAsync(writer, reqIf);
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
        private async Task WriteXmlAsync(XmlWriter xmlWriter, ReqIF reqIf)
        {
            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync(null,"REQ-IF", DefaultXmlAttributeFactory.ReqIFSchemaUri);
            await reqIf.WriteXmlAsync(xmlWriter);
            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
        }
    }
}
