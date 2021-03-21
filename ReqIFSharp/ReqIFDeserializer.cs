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
#if NETFULL
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
#else
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
#endif

    /// <summary>
    /// The purpose of the <see cref="ReqIFDeserializer"/> is to deserialize a <see cref="ReqIF"/> XML document
    /// and to dereference it to a <see cref="ReqIF"/> complete object graph.
    /// </summary>
    public class ReqIFDeserializer : IReqIFDeSerializer
    {
#if NETFULL

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
        /// A fully dereferenced <see cref="ReqIF"/> object graph
        /// </returns>
        public IEnumerable<ReqIF> Deserialize(string xmlFilePath, bool validate = false, ValidationEventHandler validationEventHandler = null)
        {
            if (string.IsNullOrEmpty(xmlFilePath))
            {
                throw new ArgumentException("The xml file path may not be null or empty");
            }

            if (!validate && validationEventHandler != null)
            {
                throw new ArgumentException("validationEventHandler must be null when validate is false");
            }

            return validate ? this.ValidatingDeserialization(xmlFilePath, validationEventHandler) : this.NonValidatingDeserialization(xmlFilePath);
        }

#else

        /// <summary>
        /// Deserializes a <see cref="ReqIF"/> XML document.
        /// </summary>
        /// <param name="xmlFilePath">
        /// The Path of the <see cref="ReqIF"/> file to deserialize
        /// </param>
        /// <returns>
        /// A fully dereferenced <see cref="ReqIF"/> object graph
        /// </returns>
        public IEnumerable<ReqIF> Deserialize(string xmlFilePath)
        {
            if (string.IsNullOrEmpty(xmlFilePath))
            {
                throw new ArgumentException("The xml file path may not be null or empty");
            }
            
            return this.NonValidatingDeserialization(xmlFilePath);
        }

        /// <summary>
        /// Deserializes a <see cref="ReqIF"/> XML document without validation of the content of the document.
        /// </summary>
        /// <param name="xmlFilePath">
        ///     The Path of the <see cref="ReqIF"/> file to deserialize
        /// </param>
        /// <returns>
        /// A fully dereferenced <see cref="ReqIF"/> object graph
        /// </returns>
        private IEnumerable<ReqIF> NonValidatingDeserialization(string xmlFilePath)
        {
            XmlReader xmlReader;
            var settings = new XmlReaderSettings();
            var xmlSerializer = new XmlSerializer(typeof(ReqIF));

            try
            {
                using (var archive = ZipFile.OpenRead(xmlFilePath))
                {
                    var reqifEntries = archive.Entries.Where(x => x.Name.EndsWith(".reqif", StringComparison.CurrentCultureIgnoreCase)).ToArray();
                    if (reqifEntries.Length == 0)
                    {
                        throw new FileNotFoundException($"No reqif file could be found in the archive.");
                    }

                    var reqifs = new List<ReqIF>();
                    foreach (var zipArchiveEntry in reqifEntries)
                    {
                        using (xmlReader = XmlReader.Create(zipArchiveEntry.Open()))
                        {
                            reqifs.Add((ReqIF)xmlSerializer.Deserialize(xmlReader));
                        }
                    }

                    return reqifs;
                }
            }
            catch (Exception e)
            {
                if (e is InvalidDataException || e is NotSupportedException)
                {
                    using (xmlReader = XmlReader.Create(xmlFilePath, settings))
                    {
                        var reqifs = new List<ReqIF>();
                        reqifs.Add((ReqIF)xmlSerializer.Deserialize(xmlReader));

                        return reqifs;
                    }
                }

                throw;
            }
        }

#endif

#if NETFULL

        /// <summary>
        /// Deserializes a <see cref="ReqIF"/> XML document without validation of the content of the document.
        /// </summary>
        /// <param name="xmlFilePath">
        ///     The Path of the <see cref="ReqIF"/> file to deserialize
        /// </param>
        /// <returns>
        /// A fully dereferenced <see cref="ReqIF"/> object graph
        /// </returns>
        private IEnumerable<ReqIF> NonValidatingDeserialization(string xmlFilePath)
        {
            XmlReader xmlReader;
            var settings = new XmlReaderSettings();
            var xmlSerializer = new XmlSerializer(typeof(ReqIF));

            try
            {
                using (var reader = new FileStream(xmlFilePath, FileMode.Open))
                using (var archive = new ZipArchive(reader, ZipArchiveMode.Read))
                {
                    var reqIfEntries = archive.Entries.Where(x => x.Name.EndsWith(".reqif", StringComparison.CurrentCultureIgnoreCase)).ToArray();
                    if (reqIfEntries.Length == 0)
                    {
                        throw new FileNotFoundException($"No reqif file could be found in the archive.");
                    }

                    var reqifs = new List<ReqIF>();
                    foreach (var zipArchiveEntry in reqIfEntries)
                    {
                        using (xmlReader = XmlReader.Create(zipArchiveEntry.Open()))
                        {
                            var reqif = (ReqIF) xmlSerializer.Deserialize(xmlReader);
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
                    using (xmlReader = XmlReader.Create(xmlFilePath, settings))
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

        /// <summary>
        /// Deserializes a <see cref="ReqIF"/> XML document with validation of the content of the document.
        /// </summary>
        /// <param name="xmlFilePath">
        /// The Path of the <see cref="ReqIF"/> file to deserialize
        /// </param>
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that processes the result of the <see cref="ReqIF"/> validation.
        /// </param>
        /// <returns>
        /// A fully dereferenced <see cref="ReqIF"/> object graph
        /// </returns>
        private IEnumerable<ReqIF>  ValidatingDeserialization(string xmlFilePath, ValidationEventHandler validationEventHandler)
        {
            var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
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

            using (var streamReader = new StreamReader(xmlFilePath))
            {
                using (var reader = XmlReader.Create(streamReader, settings))
                {
                    var serializer = new XmlSerializer(typeof(ReqIF));
                    var reqIf = (ReqIF)serializer.Deserialize(reader);

                    var result = new List<ReqIF>();
                    result.Add(reqIf);
                    return result;
                }
            }
        }
#endif
    }
}
