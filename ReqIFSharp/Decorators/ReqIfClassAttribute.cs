// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIfClassAttribute.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;

    /// <summary>
    /// Attribute used to decorate the model classes with metadata sourced from the ReqIF metamodel,
    /// so that the relationships and characteristics of each class are self-documenting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ReqIfClassAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReqIfClassAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the ReqIF metaclass that the decorated class represents (e.g. <c>SPEC-OBJECT</c>).
        /// </param>
        /// <param name="isAbstract">
        /// A value indicating whether the decorated class is abstract. An abstract class does not provide
        /// a complete declaration and cannot be instantiated on its own.
        /// </param>
        public ReqIfClassAttribute(string name = "", bool isAbstract = false)
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
