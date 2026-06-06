// -------------------------------------------------------------------------------------------------
// <copyright file="ClassAttribute.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
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
    /// Attribute used to decorate the model classes with metadata sourced from the ReqIF metamodel,
    /// so that the relationships and characteristics of each class are self-documenting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ClassAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the ReqIF metaclass that the decorated class represents (e.g. <c>SPEC-OBJECT</c>).
        /// </param>
        /// <param name="isAbstract">
        /// A value indicating whether the decorated class is abstract. An abstract class does not provide
        /// a complete declaration and cannot be instantiated on its own.
        /// </param>
        public ClassAttribute(string name = "", bool isAbstract = false)
        {
            this.Name = name;
            this.IsAbstract = isAbstract;
        }

        /// <summary>
        /// Gets or sets the name of the ReqIF metaclass that the decorated class represents.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the decorated class is abstract.
        /// </summary>
        public bool IsAbstract { get; set; }
    }
}
