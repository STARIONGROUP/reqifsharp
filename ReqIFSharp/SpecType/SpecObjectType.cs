﻿// -------------------------------------------------------------------------------------------------
// <copyright file="SpecObjectType.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
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
// -------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Contains a set of attribute definitions for a SpecObject element.
    /// Inherits a set of attribute definitions from SpecType. By using SpecObjectType elements, multiple requirements can be
    /// associated with the same set of attribute definitions (attribute names, default values, data types, etc.).
    /// </summary>
    public class SpecObjectType : SpecType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecObjectType"/> class.
        /// </summary>
        public SpecObjectType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecObjectType"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        public SpecObjectType(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecObjectType"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The container <see cref="ReqIFContent"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        internal SpecObjectType(ReqIFContent reqIfContent, ILoggerFactory loggerFactory)
            : base(reqIfContent, loggerFactory)
        {
        }
    }
}
