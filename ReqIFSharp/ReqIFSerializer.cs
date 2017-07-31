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
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The <see cref="ReqIF"/> Serializer class
    /// </summary>
    public class ReqIFSerializer : IReqIFSerializer
    {
        /// <summary>
        /// The <see cref="ReqIF"/> namespace
        /// </summary>
        private const string ReqIFNamespace = @"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd";

        /// <summary>
        /// The <see cref="ReqIF"/> schema location
        /// </summary>
        private const string ReqIFSchemaUri = @"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd";

        /// <summary>
        /// The <see cref="XmlSerializer"/>
        /// </summary>
        private readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(ReqIF), ReqIFNamespace);

        /// <summary>
        /// The <see cref="XmlReaderSettings"/>
        /// </summary>
        private readonly XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();

#if NETFULL
        /// <summary>
        /// The <see cref="ReqIF"/> <see cref="XmlSchemaSet"/>
        /// </summary>
        private readonly XmlSchemaSet reqIFSchemaSet = new XmlSchemaSet();
 
        /// <summary>
        /// A value that indicates whether the <see cref="ReqIF"/> file should be validated against the schema
        /// </summary>
        private readonly bool shouldBeValidated;
#endif

#if NETFULL
        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIFSerializer"/> class. 
        /// </summary>
        /// <param name="shouldBeValidated">
        /// a value indicating whether the <see cref="ReqIF"/> file should be validated against the schema
        /// </param>
        public ReqIFSerializer(bool shouldBeValidated)
        {
            this.shouldBeValidated = shouldBeValidated;

            if (!this.shouldBeValidated)
            {
                return;
            }

            this.reqIFSchemaSet.Add(ReqIFNamespace, ReqIFSchemaUri);

            this.xmlReaderSettings.ValidationType = ValidationType.Schema;
            this.xmlReaderSettings.Schemas.Add(this.reqIFSchemaSet);
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIFSerializer"/> class. 
        /// </summary>
        public ReqIFSerializer()
        {
        }
#endif

#if NETFULL
        /// <summary>
        /// Serialize a <see cref="ReqIF"/> object and write its content in an XML-file in the corresponding path
        /// </summary>
        /// <param name="reqIf">
        /// The <see cref="ReqIF"/> object to serialize
        /// </param>
        /// <param name="fileUri">
        /// The path of the output file
        /// </param>
        /// <param name="validationEventHandler">
        /// The <see cref="ValidationEventHandler"/> that processes the result of the reqif validation.
        /// May be null if validation is off.
        /// </param>
        /// <exception cref="XmlSchemaValidationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        public void Serialize(ReqIF reqIf, string fileUri, ValidationEventHandler validationEventHandler)
        {
            if (reqIf == null)
            {
                throw new ArgumentNullException("reqIf", "The reqIf object cannot be null.");
            }

            if (fileUri == null)
            {
                throw new ArgumentNullException("fileUri", "The path of the file cannot be null.");
            }

            if (fileUri == string.Empty)
            {
                throw new ArgumentOutOfRangeException("fileUri", "The path of the file cannot be empty.");
            }

            if (this.shouldBeValidated)
            {
                using (var writer = new StringWriter())
                {
                    this.xmlSerializer.Serialize(writer, reqIf);

                    var xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(writer.ToString());
                    xmlDocument.Schemas = this.reqIFSchemaSet;

                    // throws XmlSchemaValidationException upon failure
                    xmlDocument.Validate(validationEventHandler);

                    using (var xmlTextWriter = new XmlTextWriter(fileUri, Encoding.UTF8) { Formatting = Formatting.Indented })
                    {
                        xmlDocument.Save(xmlTextWriter);
                    }
                }
            }
            else
            {
                using (var writer = XmlWriter.Create(fileUri, new XmlWriterSettings { Indent = true }))
                {
                    this.xmlSerializer.Serialize(writer, reqIf);
                }
            }
        }
#else
        /// <summary>
        /// Serialize a <see cref="ReqIF"/> object and write its content in an XML-file in the corresponding path
        /// </summary>
        /// <param name="reqIf">
        /// The <see cref="ReqIF"/> object to serialize
        /// </param>
        /// <param name="fileUri">
        /// The path of the output file
        /// </param>        
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="SecurityException"></exception>
        public void Serialize(ReqIF reqIf, string fileUri)
        {
            if (reqIf == null)
            {
                throw new ArgumentNullException("reqIf", "The reqIf object cannot be null.");
            }

            if (fileUri == null)
            {
                throw new ArgumentNullException("fileUri", "The path of the file cannot be null.");
            }

            if (fileUri == string.Empty)
            {
                throw new ArgumentOutOfRangeException("fileUri", "The path of the file cannot be empty.");
            }


            using (var fs = new FileStream(fileUri, FileMode.Create))
            {
                using (var writer = XmlWriter.Create(fs, new XmlWriterSettings { Indent = true }))
                {
                    this.xmlSerializer.Serialize(writer, reqIf);
                }
            }
        }
#endif
    }
}