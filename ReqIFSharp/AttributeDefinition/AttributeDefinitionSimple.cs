﻿// -------------------------------------------------------------------------------------------------
// <copyright file="AttributeDefinitionSimple.cs" company="Starion Group S.A.">
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
    /// The <see cref="AttributeDefinitionSimple"/> is the base class for simple type attributes.
    /// </summary>
    public abstract class AttributeDefinitionSimple : AttributeDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionSimple"/> class.
        /// </summary>
        protected AttributeDefinitionSimple()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionSimple"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        protected AttributeDefinitionSimple(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDefinitionSimple"/> class.
        /// </summary>
        /// <param name="specType">
        /// The owning <see cref="SpecType"/>.
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to setup logging
        /// </param>
        protected AttributeDefinitionSimple(SpecType specType, ILoggerFactory loggerFactory) 
            : base(specType, loggerFactory)
        {
        }
    }
}
