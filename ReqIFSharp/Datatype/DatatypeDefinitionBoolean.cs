// -------------------------------------------------------------------------------------------------
// <copyright file="DatatypeDefinitionBoolean.cs" company="RHEA System S.A.">
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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// The purpose of the <see cref="DatatypeDefinitionBoolean"/> class is to define the primitive <see cref="bool"/> data type
    /// </summary>
    /// <remarks>
    /// This element defines a data type for the representation of Boolean data values in the Exchange Document.
    /// </remarks>
    public class DatatypeDefinitionBoolean : DatatypeDefinitionSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionBoolean"/> class.
        /// </summary>
        public DatatypeDefinitionBoolean()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatatypeDefinitionBoolean"/> class.
        /// </summary>
        /// <param name="reqIfContent">
        /// The owning <see cref="reqIfContent"/>
        /// </param>
        internal DatatypeDefinitionBoolean(ReqIFContent reqIfContent) 
            : base(reqIfContent)
        {
        }

        /// <summary>
        /// Generates a <see cref="AttributeDefinition"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            
            using (var subtree = reader.ReadSubtree())
            {
                while (subtree.Read())
                {
                    if (subtree.MoveToContent() == XmlNodeType.Element)
                    {
                        switch (subtree.LocalName)
                        {
                            case "ALTERNATIVE-ID":
                                var alternativeId = new AlternativeId(this);
                                alternativeId.ReadXml(subtree);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates a <see cref="AttributeDefinition"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">
        /// an instance of <see cref="XmlReader"/>
        /// </param>
        public override async Task ReadXmlAsync(XmlReader reader, CancellationToken token)
        {
            base.ReadXml(reader);

            using (var subtree = reader.ReadSubtree())
            {
                while (await subtree.ReadAsync())
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    if (await subtree.MoveToContentAsync() == XmlNodeType.Element)
                    {
                        switch (subtree.LocalName)
                        {
                            case "ALTERNATIVE-ID":
                                var alternativeId = new AlternativeId(this);
                                alternativeId.ReadXml(reader);
                                break;
                        }
                    }
                }
            }
        }
    }
}
