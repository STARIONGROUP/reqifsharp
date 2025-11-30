// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFDeserializer.cs" company="Starion Group S.A.">
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
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Schema;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="ReqIFDeserializer"/> is to deserialize a <see cref="ReqIF"/> XML document
    /// and to dereference it to a <see cref="ReqIF"/> complete object graph.
    /// </summary>
    public class ReqIFDeserializer : IReqIFDeSerializer
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<ReqIFDeserializer> logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIFDeserializer"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public ReqIFDeserializer(ILoggerFactory loggerFactory = null)
        {
            this.loggerFactory = loggerFactory;

            this.logger = this.loggerFactory == null ? NullLogger<ReqIFDeserializer>.Instance : this.loggerFactory.CreateLogger<ReqIFDeserializer>();
        }

        /// <summary>
        /// Deserializes a <see cref="IEnumerable{ReqIF}"/> from a file
        /// </summary>
        /// <param name="fileUri">
        /// The Path of the <see cref="ReqIF"/> file to deserialize
        /// </param>
        /// <param name="validate">
        /// a value indicating whether the XML document needs to be validated or not
        /// </param>
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that processes the result of the <see cref="ReqIF"/> validation.
        /// </param>
        /// <returns>
        /// Fully de-referenced <see cref="IEnumerable{ReqIF}"/> object graphs
        /// </returns>
        public IEnumerable<ReqIF> Deserialize(string fileUri, bool validate = false, ValidationEventHandler validationEventHandler = null)
        {
            FileBasedSerializerArgumentValidation(fileUri);

            var fileExtensionKind = fileUri.ConvertPathToSupportedFileExtensionKind();

            using (var fileStream = File.OpenRead(fileUri))
            {
                var sw = Stopwatch.StartNew();

                this.logger.LogTrace("start deserializing from {Path}", fileUri);

                var result = this.Deserialize(fileStream, fileExtensionKind, validate, validationEventHandler);

                this.logger.LogTrace("File {Path} deserialized in {Time} [ms]", fileUri, sw.ElapsedMilliseconds);

                return result;
            }
        }

        /// <summary>
        /// Deserializes a <see cref="IEnumerable{ReqIF}"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> that contains the reqifz file to deserialize
        /// </param>
        /// <param name="fileExtensionKind">
        /// The <see cref="SupportedFileExtensionKind"/> that specifies whether the input <see cref="Stream"/>
        /// contains the reqif file or a zip-archive of reqif files
        /// </param>
        /// <param name="validate">
        /// a value indicating whether the XML document needs to be validated or not
        /// </param>
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that processes the result of the <see cref="ReqIF"/> validation.
        /// </param>
        /// <returns>
        /// Fully de-referenced <see cref="IEnumerable{ReqIF}"/> object graphs
        /// </returns>
        public IEnumerable<ReqIF> Deserialize(Stream stream, SupportedFileExtensionKind fileExtensionKind, bool validate = false,  ValidationEventHandler validationEventHandler = null)
        {
            StreamBasedSerializerArgumentValidation(stream, validate, validationEventHandler);

            return this.DeserializeReqIF(stream, fileExtensionKind, validate, validationEventHandler);
        }

        /// <summary>
        /// Asynchronously deserializes a <see cref="IEnumerable{ReqIF}"/> from a file
        /// </summary>
        /// <param name="fileUri">
        /// The Path of the <see cref="ReqIF"/> file to deserialize
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <param name="validate">
        /// a value indicating whether the XML document needs to be validated or not
        /// </param>
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that processes the result of the <see cref="ReqIF"/> validation.
        /// </param>
        /// <returns>
        /// A fully de-referenced <see cref="ReqIF"/> object graph
        /// </returns>
        public async Task<IEnumerable<ReqIF>> DeserializeAsync(string fileUri, CancellationToken token, bool validate = false, ValidationEventHandler validationEventHandler = null)
        {
            FileBasedSerializerArgumentValidation(fileUri);

            var fileExtensionKind = fileUri.ConvertPathToSupportedFileExtensionKind();

            using (var fileStream = new FileStream(fileUri, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous))
            {
                var sw = Stopwatch.StartNew();
                this.logger.LogTrace("Start deserializing from {Path}", fileUri);

                try
                {
                    var result = await this.DeserializeAsync(fileStream, fileExtensionKind, token, validate, validationEventHandler);

                    this.logger.LogTrace("File {Path} deserialized successfully in {Time} ms", fileUri, sw.ElapsedMilliseconds);

                    return result;
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    this.logger.LogError(ex, "An error occurred while deserializing file {Path}", fileUri);
                    throw;
                }
            }
        }

        /// <summary>
        /// Asynchronously deserializes a <see cref="IEnumerable{ReqIF}"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> that contains the reqifz file to deserialize
        /// </param>
        /// <param name="fileExtensionKind">
        /// The <see cref="SupportedFileExtensionKind"/> that specifies whether the input <see cref="Stream"/>
        /// contains the reqif file or a zip-archive of reqif files
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <param name="validate">
        /// a value indicating whether the XML document needs to be validated or not
        /// </param>
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that processes the result of the <see cref="ReqIF"/> validation.
        /// </param>
        /// <returns>
        /// Fully de-referenced <see cref="IEnumerable{ReqIF}"/> object graphs
        /// </returns>
        public Task<IEnumerable<ReqIF>> DeserializeAsync(Stream stream, SupportedFileExtensionKind fileExtensionKind, CancellationToken token, bool validate = false, ValidationEventHandler validationEventHandler = null)
        {
            StreamBasedSerializerArgumentValidation(stream, validate, validationEventHandler);

            return this.DeserializeReqIFAsync(stream, fileExtensionKind, token, validate, validationEventHandler);
        }

        /// <summary>
        /// Argument validation for file based operations
        /// </summary>
        /// <param name="fileUri">
        /// The path of the input ReqIF file
        /// </param>
        private static void FileBasedSerializerArgumentValidation(string fileUri)
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
        /// <param name="stream">
        /// The <see cref="Stream"/> that contains the reqifz file to deserialize
        /// </param>
        /// <param name="validate">
        /// a value indicating whether the XML document needs to be validated or not
        /// </param>
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that processes the result of the <see cref="ReqIF"/> validation.
        /// </param>
        private static void StreamBasedSerializerArgumentValidation(Stream stream, bool validate = false, ValidationEventHandler validationEventHandler = null)
        {
            if (!validate && validationEventHandler != null)
            {
                throw new ArgumentException($"{nameof(validationEventHandler)} must be null when {nameof(validate)} is false");
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), $"The {nameof(stream)} may not be null");
            }

            if (stream.Length == 0)
            {
                throw new ArgumentException($"The {nameof(stream)} may not be empty", nameof(stream));
            }
        }

        /// <summary>
        /// Deserialize the provided <see cref="Stream"/> to ReqIF
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> that contains the reqifz file to deserialize
        /// </param>
        /// <param name="fileExtensionKind">
        /// The <see cref="SupportedFileExtensionKind"/> that specifies whether the input <see cref="Stream"/>
        /// contains the reqif file or a zip-archive of reqif files
        /// </param>
        /// <param name="validate">
        /// a value indicating whether the XML document needs to be validated or not
        /// </param>
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that processes the result of the <see cref="ReqIF"/> validation.
        /// </param>
        /// <returns>
        /// Fully de-referenced <see cref="IEnumerable{ReqIF}"/> object graphs
        /// </returns>
        private IEnumerable<ReqIF> DeserializeReqIF(Stream stream, SupportedFileExtensionKind fileExtensionKind, bool validate = false, ValidationEventHandler validationEventHandler = null)
        {
            XmlReader xmlReader;

            var settings = this.CreateXmlReaderSettings(validate, validationEventHandler);

            stream.Seek(0, SeekOrigin.Begin);

            switch (fileExtensionKind)
            {
                case SupportedFileExtensionKind.Reqif:

                    this.logger.LogTrace("reading from reqif");
                    
                    using (xmlReader = XmlReader.Create(stream, settings))
                    {
                        var sw = Stopwatch.StartNew();

                        var reqifs = new List<ReqIF>();

                        this.logger.LogTrace("starting to read xml");

                        while (xmlReader.Read())
                        {
                            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "REQ-IF"))
                            {
                                var reqif = new ReqIF(this.loggerFactory);
                                reqif.ReadXml(xmlReader);
                                reqifs.Add(reqif);
                            }
                        }

                        this.logger.LogTrace("xml read in {Time}", sw.ElapsedMilliseconds);
                        sw.Stop();

                        return reqifs;
                    }
                        
                case SupportedFileExtensionKind.Reqifz:

                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        this.logger.LogTrace("reading from reqifz");

                        var sw = Stopwatch.StartNew();

                        var reqIfEntries = archive.Entries.Where(x => x.Name.EndsWith(".reqif", StringComparison.OrdinalIgnoreCase)).ToArray();
                        if (reqIfEntries.Length == 0)
                        {
                            throw new FileNotFoundException($"No reqif file could be found in the archive.");
                        }

                        this.logger.LogTrace("found {Entries} in the reqif zip archive in {Time} [ms]", reqIfEntries.Length, sw.ElapsedMilliseconds);
                        sw.Stop();

                        var reqifs = new List<ReqIF>();
                        foreach (var zipArchiveEntry in reqIfEntries)
                        {
                            using (xmlReader = XmlReader.Create(zipArchiveEntry.Open(), settings))
                            {
                                sw.Start();

                                this.logger.LogTrace("starting to read xml");

                                while (xmlReader.Read())
                                {
                                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "REQ-IF"))
                                    {
                                        var reqif = new ReqIF(this.loggerFactory);
                                        reqif.ReadXml(xmlReader);
                                        reqifs.Add(reqif);
                                    }
                                }

                                this.logger.LogTrace("xml read in {Time}", sw.ElapsedMilliseconds);
                                sw.Stop();
                            }
                        }

                        return reqifs;
                    }
            }

            throw new SerializationException($"The {nameof(stream)} could not be deserialized");
        }

        /// <summary>
        /// Asynchronously deserialize the provided <see cref="Stream"/> to ReqIF
        /// </summary>
        /// <param name="stream">
        /// The <see cref="Stream"/> that contains the reqifz file to deserialize
        /// </param>
        /// <param name="fileExtensionKind">
        /// The <see cref="SupportedFileExtensionKind"/> that specifies whether the input <see cref="Stream"/>
        /// contains the reqif file or a zip-archive of reqif files
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <param name="validate">
        /// a value indicating whether the XML document needs to be validated or not
        /// </param>
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that processes the result of the <see cref="ReqIF"/> validation.
        /// </param>
        /// <returns>
        /// Fully de-referenced <see cref="IEnumerable{ReqIF}"/> object graphs
        /// </returns>
        private async Task<IEnumerable<ReqIF>> DeserializeReqIFAsync(Stream stream, SupportedFileExtensionKind fileExtensionKind, CancellationToken token, bool validate = false, ValidationEventHandler validationEventHandler = null)
        {
            XmlReader xmlReader;

            var settings = this.CreateXmlReaderSettings(validate, validationEventHandler, true);

            stream.Seek(0, SeekOrigin.Begin);

            switch (fileExtensionKind)
            {
                case SupportedFileExtensionKind.Reqif:

                    this.logger.LogTrace("reading from reqif");
                    
                    using (xmlReader = XmlReader.Create(stream, settings))
                    {
                        var sw = Stopwatch.StartNew();

                        this.logger.LogTrace("reading from reqif file");

                        var reqifs = new List<ReqIF>();

                        while (await xmlReader.ReadAsync())
                        {
                            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "REQ-IF"))
                            {
                                var reqif = new ReqIF(loggerFactory);
                                await reqif.ReadXmlAsync(xmlReader, token);
                                reqifs.Add(reqif);
                            }
                        }

                        this.logger.LogTrace("xml read in {Time}", sw.ElapsedMilliseconds);
                        sw.Stop();

                        return reqifs;
                    }
                    
                case SupportedFileExtensionKind.Reqifz:

                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        this.logger.LogTrace("reading from reqifz archive");

                        var sw = Stopwatch.StartNew();

                        var reqIfEntries = archive.Entries.Where(x => x.Name.EndsWith(".reqif", StringComparison.OrdinalIgnoreCase)).ToArray();
                        if (reqIfEntries.Length == 0)
                        {
                            throw new FileNotFoundException($"No reqif file could be found in the archive.");
                        }

                        this.logger.LogTrace("found {Entries} in the reqif zip archive in {Time} [ms]", reqIfEntries.Length, sw.ElapsedMilliseconds);
                        sw.Stop();

                        token.ThrowIfCancellationRequested();

                        var reqifs = new List<ReqIF>();
                        foreach (var zipArchiveEntry in reqIfEntries)
                        {
                            sw.Start();

                            this.logger.LogTrace("starting to read xml");

                            using (xmlReader = XmlReader.Create(zipArchiveEntry.Open(), settings))
                            {
                                while (await xmlReader.ReadAsync())
                                {
                                    if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "REQ-IF"))
                                    {
                                        var reqif = new ReqIF(loggerFactory);
                                        await reqif.ReadXmlAsync(xmlReader, token);
                                        reqifs.Add(reqif);
                                    }
                                }
                            }

                            this.logger.LogTrace("xml read in {Time}", sw.ElapsedMilliseconds);
                            sw.Stop();

                            token.ThrowIfCancellationRequested();
                        }

                        return reqifs;
                    }
            }

            throw new SerializationException($"The {nameof(stream)} could not be deserialized");
        }

        /// <summary>
        /// Creates an instance of <see cref="XmlReaderSettings"/> with or without validation settings
        /// </summary>
        /// <param name="validate">
        /// a value indicating whether the <see cref="XmlReaderSettings"/> should be created with validation settings or not
        /// </param>
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that processes the result of the <see cref="ReqIF"/> validation.
        /// </param>
        /// <param name="asynchronous">
        /// A value indicating whether asynchronous methods can be used on a particular <see cref="XmlReader"/> instance. 
        /// </param>
        /// <returns>
        /// An instance of <see cref="XmlReaderSettings"/>
        /// </returns>
        private XmlReaderSettings CreateXmlReaderSettings(bool validate = false, ValidationEventHandler validationEventHandler = null, bool asynchronous = false)
        {
            XmlReaderSettings settings;

            if (validate)
            {
                settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationEventHandler += validationEventHandler;

                var schemaSet = new XmlSchemaSet { XmlResolver = new ReqIfSchemaResolver() };

                // add validation schema
                schemaSet.Add(this.GetSchemaFromResource("reqif.xsd", validationEventHandler));
                schemaSet.ValidationEventHandler += validationEventHandler;

                // now combine and use the custom xml resolver to serve all xsd references from resource manifest
                schemaSet.Compile();

                // register the resolved schema set to the reader settings
                settings.Schemas.Add(schemaSet);
            }
            else
            {
                settings = new XmlReaderSettings();
            }

            settings.Async = asynchronous;

            return settings;
        }
        
        /// <summary>
        /// Gets the <see cref="ReqIF"/> schema for the embedded resources.
        /// </summary>
        /// <param name="resourceName">
        /// The resource Name.
        /// </param>
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that processes the result of the <see cref="ReqIF"/> validation.
        /// </param>
        /// <returns>
        /// A fully resolved instance of <see cref="XmlSchema"/>
        /// </returns>
        /// <exception cref="MissingManifestResourceException">
        /// the schema resource could not be found.
        /// </exception>
        private XmlSchema GetSchemaFromResource(string resourceName, ValidationEventHandler validationEventHandler)
        {
            var a = Assembly.GetExecutingAssembly();
            var type = this.GetType();
            var @namespace = type.Namespace;
            var reqifSchemaResourceName = $"{@namespace}.Resources.{resourceName}";

            var stream = a.GetManifestResourceStream(reqifSchemaResourceName);

            if (stream == null)
            {
                throw new MissingManifestResourceException($"The {reqifSchemaResourceName} resource could not be found");
            }
            
            return XmlSchema.Read(stream, validationEventHandler);
        }
    }
}
