// -------------------------------------------------------------------------------------------------
// <copyright file="AccessControlledElement.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The <see cref="AccessControlledElement"/> is the base class for classes that may restrict user access to their information.
    /// </summary>
    [ReqIfClass(name: "ACCESS-CONTROLLED-ELEMENT", isAbstract: true)]
    public abstract class AccessControlledElement : Identifiable
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<AccessControlledElement> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessControlledElement"/> class
        /// </summary>
        protected AccessControlledElement()
        {
            this.logger = NullLogger<AccessControlledElement>.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessControlledElement"/> class
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected AccessControlledElement(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.logger = this.loggerFactory == null ? NullLogger<AccessControlledElement>.Instance : this.loggerFactory.CreateLogger<AccessControlledElement>();
        }

        /// <summary>
        /// Backing field for the <see cref="IsEditable"/> property
        /// </summary>
        private bool isEditable;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AccessControlledElement"/> is editable
        /// </summary>
        /// <remarks>
        /// True means that the element’s contents may be modified by the user of a tool containing the element.
        /// False or leaving isEditable out means that the element is read-only to the user of a tool containing the element.
        /// Setting this property also sets <see cref="IsEditableSpecified"/> to true, so that the value is written.
        /// </remarks>
        [ReqIfProperty(aggregation: AggregationKind.None, lowerValue: 0, upperValue: 1, isOrdered: false, isReadOnly: false, isDerived: false, isDerivedUnion: false, isUnique: true, defaultValue: null)]
        public bool IsEditable
        {
            get => this.isEditable;
            set
            {
                this.isEditable = value;
                this.IsEditableSpecified = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IS-EDITABLE attribute is written
        /// </summary>
        /// <remarks>
        /// This is true when the IS-EDITABLE attribute was present in the document that was read, or when
        /// <see cref="IsEditable"/> was set. When this and <see cref="IsEditable"/> are both false, the attribute is
        /// left out, which means the element is read-only; an <see cref="IsEditable"/> of true is always written.
        /// Set it to false to leave out an IS-EDITABLE attribute that is false.
        /// </remarks>
        public bool IsEditableSpecified { get; set; }

        /// <summary>
        /// Generates a <see cref="AccessControlledElement"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        internal override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);

            var xmlLineInfo = reader as IXmlLineInfo;

            this.logger.LogTrace("reading IS-EDITABLE at line:position {LineNumber}:{LinePosition}", xmlLineInfo?.LineNumber, xmlLineInfo?.LinePosition);

            var isEditableValue = reader.GetAttribute("IS-EDITABLE");
            if (!string.IsNullOrWhiteSpace(isEditableValue))
            {
                try
                {
                    this.IsEditable = XmlConvert.ToBoolean(isEditableValue);
                }
                catch (Exception e)
                {
                    throw new SerializationException($"The AccessControlledElement.IS-EDITABLE {isEditableValue} at line:position {xmlLineInfo?.LineNumber}:{xmlLineInfo?.LinePosition} could not be converted to a BOOLEAN", e);
                }
            }
        }

        /// <summary>
        /// Writes the IS-EDITABLE attribute when <see cref="IsEditableSpecified"/> or <see cref="IsEditable"/> is true
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        internal override void WriteXmlAttributes(XmlWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (this.IsEditableSpecified || this.IsEditable)
            {
                writer.WriteAttributeString("IS-EDITABLE", XmlConvert.ToString(this.IsEditable));
            }
        }

        /// <summary>
        /// Asynchronously writes the IS-EDITABLE attribute when <see cref="IsEditableSpecified"/> or <see cref="IsEditable"/> is true
        /// </summary>
        /// <param name="writer">
        /// an instance of <see cref="XmlWriter"/>
        /// </param>
        /// <param name="token">
        /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        internal override async Task WriteXmlAttributesAsync(XmlWriter writer, CancellationToken token)
        {
            await base.WriteXmlAttributesAsync(writer, token);

            if (this.IsEditableSpecified || this.IsEditable)
            {
                await writer.WriteAttributeStringAsync(null, "IS-EDITABLE", null, XmlConvert.ToString(this.IsEditable));
            }
        }
    }
}
