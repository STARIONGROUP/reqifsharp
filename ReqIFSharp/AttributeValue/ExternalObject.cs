// -------------------------------------------------------------------------------------------------
// <copyright file="ExternalObject.cs" company="RHEA System S.A.">
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
    using System.IO.Compression;

    /// <summary>
    /// External objects are referenced binary objects that are referenced using the XHTML object element from the XHTML Object Module.
    /// The location of an external object MUST be specified via the data attribute which contains either
    ///   a) a URL relative to the location of the exchange XML document, or
    ///   b) an absolute URL.
    /// </summary>
    /// <remarks>
    /// The specification for the XTHML object element defines several XML attributes. For ReqIF, only a subset of these attributes is relevant and used.
    /// </remarks>
    public class ExternalObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalObject"/> class.
        /// </summary>
        /// <param name="attributeValueXhtml">
        /// The owning <see cref="AttributeValueXHTML"/>
        /// </param>
        public ExternalObject(AttributeValueXHTML attributeValueXhtml)
        {
            this.Owner = attributeValueXhtml;
        }

        /// <summary>
        /// Gets the owning <see cref="AttributeValueXHTML"/>
        /// </summary>
        public AttributeValueXHTML Owner { get; private set; }

        /// <summary>
        /// Gets or sets the Uri of the <see cref="ExternalObject"/>, this may be a relative uri or an absolute uri
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the MimeType of the external object represented as a string
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the height of the external object in case it is an image
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets the width of the external object in case it is an image
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Gets or sets the path of the reqif file that contains the data from which this <see cref="AttributeValueXHTML"/>
        /// object was created/
        /// </summary>
        internal string ReqIFFilePath { get; set; }

        /// <summary>
        /// Asserts whether the external object should be queried from the reqifz file or from
        /// another (absolute) location such as a resource available via HTTP or HTTPS
        /// </summary>
        /// <returns></returns>
        public bool IsDataLocal()
        {
            return !this.Uri.StartsWith("http");
        }

        /// <summary>
        /// Queries the local data from the reqifz file and writes the data to the provided target <see cref="Stream"/>
        /// </summary>
        /// <param name="target">
        /// The target <see cref="Stream"/> to which the data is written
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the <paramref name="target"/> is null
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// thrown when the <see cref="Uri"/> is an absolute uri and not a relative Uri or
        /// when the reqifz file is unknown
        /// </exception>
        public void QueryLocalData(Stream target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "The target stream may not be null");
            }

            if (!this.IsDataLocal())
            {
                throw new InvalidOperationException("The Uri of the External Object is not a relative Uri that can be found in the reqifz file");
            }

            if (string.IsNullOrEmpty(this.ReqIFFilePath))
            {
                throw new InvalidOperationException("The local data cannot be queried, the path to the reqifz file is unknown");
            }

            using (var reader = new FileStream(this.ReqIFFilePath, FileMode.Open))
            {
                using (var archive = new ZipArchive(reader, ZipArchiveMode.Read))
                {
                    var zipArchiveEntry = archive.GetEntry(this.Uri);
                    if (zipArchiveEntry != null)
                    {
                        var sourceStream = zipArchiveEntry.Open();
                        sourceStream.CopyTo(target);
                        sourceStream.Dispose();
                    }
                }
            }
        }
    }
}
