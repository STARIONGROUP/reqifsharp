// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionDate.cs" company="RHEA System S.A.">
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
// -------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;
    
    /// <summary>
    /// The purpose of the <see cref="DatatypeDefinitionBoolean"/> class is to define the <see cref="DateTime"/> data type
    /// </summary>
    /// <remarks>
    /// This element defines a data type for the representation of Date and Time data values in the Exchange Document.
    /// </remarks>
    public class DatatypeDefinitionDate : DatatypeDefinitionSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionDate"/> class.
        /// </summary>
        public DatatypeDefinitionDate()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionDate"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        internal DatatypeDefinitionDate(ReqIFContent reqIfContent) 
            : base(reqIfContent)            
        {
            this.ReqIFContent = reqIfContent;
        }
    }
}
