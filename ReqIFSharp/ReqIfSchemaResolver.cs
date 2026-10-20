// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIfSchemaResolver.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// The purpose of the <see cref="ReqIfSchemaResolver"/> is to resolve the imported and included
    /// XML namespaces in an <see cref="XmlSchema"/>
    /// </summary>
    public class ReqIfSchemaResolver : XmlUrlResolver
    {
        /// <summary>
        /// embedded resource location
        /// </summary>
        private readonly string resourcePath;

        /// <summary>
        /// The current executing assembly;
        /// </summary>
        private readonly Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIfSchemaResolver"/> class.
        /// </summary>
        public ReqIfSchemaResolver()
        {
            this.assembly = Assembly.GetExecutingAssembly();

            var type = this.GetType();
            var @namespace = type.Namespace;
            this.resourcePath = $"{@namespace}.Resources.";
        }

        /// <summary>
        /// Maps a URI to an object that contains the actual resource.
        /// </summary>
        /// <param name="absoluteUri">
        /// The URI returned from System.Xml.XmlResolver.ResolveUri(System.Uri,System.String). 
        /// </param>
        /// <param name="role">
        /// Currently not used.
        /// </param>
        /// <param name="ofObjectToReturn">
        /// The type of object to return. The current implementation only returns System.IO.Stream objects.
        /// </param>
        /// <returns>
        /// A stream object or null if a type other than stream is specified.
        /// </returns> 
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri == null)
            {
                throw new ArgumentNullException(nameof(absoluteUri));
            }

            var segments = absoluteUri.Segments;
            var upperBound = segments.GetUpperBound(0);
            var schemaName = segments[upperBound];

            var resource = this.resourcePath + schemaName;

            var stream = this.assembly.GetManifestResourceStream(resource);
            
            return stream;
        }
    }
}
