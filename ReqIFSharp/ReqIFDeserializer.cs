// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFDeserializer.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The purpose of the <see cref="ReqIFDeserializer"/> is to deserialize a <see cref="ReqIF"/> XML document
    /// and to dereference it to a <see cref="ReqIF"/> complete object graph.
    /// </summary>
    public class ReqIFDeserializer : IReqIFDeSerializer
    {
        /// <summary>
        /// Deserializes a <see cref="ReqIF"/> XML document.
        /// </summary>
        /// <param name="xmlFilePath">
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
        public IEnumerable<ReqIF> Deserialize(string xmlFilePath, bool validate = false, ValidationEventHandler validationEventHandler = null)
        {
            if (string.IsNullOrEmpty(xmlFilePath))
            {
                throw new ArgumentException("The xml file path may not be null or empty");
            }

            using (var fileStream = File.OpenRead(xmlFilePath))
            {
                return this.Deserialize(fileStream, validate, validationEventHandler);
            }
        }

        /// <summary>
        /// Deserializes a <see cref="ReqIF"/> XML stream.
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
        /// <returns>
        /// Fully de-referenced <see cref="IEnumerable{ReqIF}"/> object graphs
        /// </returns>
        public IEnumerable<ReqIF> Deserialize(Stream stream, bool validate = false,  ValidationEventHandler validationEventHandler = null)
        {
            if (!validate && validationEventHandler != null)
            {
                throw new ArgumentException("validationEventHandler must be null when validate is false");
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream), $"The {nameof(stream)} may not be null");
            }

            if (stream.Length == 0)
            {
                throw new ArgumentException($"The {nameof(stream)} may not be empty", nameof(stream));
            }

            return this.DeserializeReqIF(stream, validate, validationEventHandler);
        }

        /// <summary>
        /// Deserialize the provided <see cref="Stream"/> to ReqIF
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
        /// <returns>
        /// Fully de-referenced <see cref="IEnumerable{ReqIF}"/> object graphs
        /// </returns>
        private IEnumerable<ReqIF> DeserializeReqIF(Stream stream, bool validate = false, ValidationEventHandler validationEventHandler = null)
        {
            XmlReader xmlReader;

            var settings = this.CreateXmlReaderSettings(validate, validationEventHandler);
            var xmlSerializer = new XmlSerializer(typeof(ReqIF));

            try
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    var reqIfEntries = archive.Entries.Where(x => x.Name.EndsWith(".reqif", StringComparison.CurrentCultureIgnoreCase)).ToArray();
                    if (reqIfEntries.Length == 0)
                    {
                        throw new FileNotFoundException($"No reqif file could be found in the archive.");
                    }

                    var reqifs = new List<ReqIF>();
                    foreach (var zipArchiveEntry in reqIfEntries)
                    {
                        using (xmlReader = XmlReader.Create(zipArchiveEntry.Open(), settings))
                        {
                            var reqif = (ReqIF)xmlSerializer.Deserialize(xmlReader);

                            //this.UpdateExternalObjectsReqIfFilePath(reqif, xmlFilePath);

                            reqifs.Add(reqif);
                        }
                    }

                    return reqifs;
                }
            }
            catch (Exception e)
            {
                if (e is InvalidDataException || e is NotSupportedException)
                {
                    using (xmlReader = XmlReader.Create(stream, settings))
                    {
                        var reqifs = new List<ReqIF>();
                        var reqif = (ReqIF)xmlSerializer.Deserialize(xmlReader);
                        reqifs.Add(reqif);
                        return reqifs;
                    }
                }

                throw;
            }
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
        /// <returns>
        /// An instance of <see cref="XmlReaderSettings"/>
        /// </returns>
        private XmlReaderSettings CreateXmlReaderSettings(bool validate = false, ValidationEventHandler validationEventHandler = null)
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

                // now combine and user the custom xmlresolver to serve all xsd references from resource manifest
                schemaSet.Compile();

                // register the resolved schema set to the reader settings
                settings.Schemas.Add(schemaSet);
            }
            else
            {
                settings = new XmlReaderSettings();
            }

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
        /// An fully resolved instance of <see cref="XmlSchema"/>
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
