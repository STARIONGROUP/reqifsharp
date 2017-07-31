// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFToolExtension.cs" company="RHEA System S.A.">
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
    using System.Xml;
    using System.Xml.Serialization;
    
    /// <summary>
    /// The <see cref="ReqIFToolExtension"/> class allows the optional inclusion of tool-specific information into a ReqIF Exchange Document.
    /// </summary>
    [Serializable]
    [XmlType(TypeName = "REQ-IF-TOOL-EXTENSION", Namespace = "http://www.omg.org/spec/ReqIF/20110401/reqif.xsd")]
    public class ReqIFToolExtension
    {
        [XmlAnyElement]
        public List<XmlElement> Any { get; set; }
    }
}