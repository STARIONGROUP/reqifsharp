// -------------------------------------------------------------------------------------------------
// <copyright file="XmlAttribute.cs" company="RHEA System S.A.">
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
    /// <summary>
    /// Encapsulates an xml attribute extracted from a ReqIF document
    /// </summary>
    internal class XmlAttribute
    {
        /// <summary>
        /// Gets or sets the local name of the attribute
        /// </summary>
        internal string LocalName { get; set; }

        /// <summary>
        /// Gets or sets the value of the attribute.
        /// </summary>
        internal string Value { get; set; }
    }
}