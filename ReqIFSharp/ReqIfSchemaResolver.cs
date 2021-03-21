// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIfSchemaResolver.cs" company="RHEA System S.A.">
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
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;

#if NETFRAMEWORK || NETSTANDARD2_0

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
            var segments = absoluteUri.Segments;
            var upperBound = segments.GetUpperBound(0);
            var schemaName = segments[upperBound];

            var resource = this.resourcePath + schemaName;

            var stream = this.assembly.GetManifestResourceStream(resource);
            
            return stream;
        }
    }

#endif
}
