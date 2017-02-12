// -------------------------------------------------------------------------------------------------
// <copyright file="ReqIFSerializerTestFixture.cs" company="RHEA System S.A.">
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

namespace ReqIFSharp.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using NUnit.Framework;
    using ReqIFSharp;

    [TestFixture]
    internal class ReqIFSerializerTestFixture
    {
        private ReqIF reqIF;

        private string resultFileUri;

        private const string ReqIFNamespace = @"http://www.omg.org/spec/ReqIF/20110401/reqif.xsd";

        private string specobject_1_id;

        private string specobject_2_id;

        private string specobject_3_id;

        private string enumdatatype_id;

        private string enum_value_low_id;

        private string enum_value_medium_id;

        private string xhtmlcontent;

        [SetUp]
        public void SetUp()
        {
            this.resultFileUri = Path.Combine(TestContext.CurrentContext.TestDirectory, "result.xml");

            this.enumdatatype_id = "enumeration";
            this.enum_value_low_id = "enumlow";
            this.enum_value_medium_id = "enummedium";

            this.specobject_1_id = "specobject_1";
            this.specobject_2_id = "specobject_2";
            this.specobject_3_id = "specobject_3";

            this.xhtmlcontent = "<xhtml:p>XhtmlPType<xhtml:a accesskey=\"a\" charset=\"UTF-8\" href=\"http://eclipse.org/rmf\" hreflang=\"en\" rel=\"LinkTypes\" rev=\"LinkTypes\" style=\"text-decoration:underline\" tabindex=\"1\" title=\"text\" type=\"text/html\"> text before br<xhtml:br/>text after br text before span<xhtml:span>XhtmlSpanType</xhtml:span>text after span text before em<xhtml:em>XhtmlEmType</xhtml:em>text after em text before strong<xhtml:strong>XhtmlStrongType</xhtml:strong>text after strong text before dfn<xhtml:dfn>XhtmlDfnType</xhtml:dfn>text after dfn text before code<xhtml:code>XhtmlCodeType</xhtml:code>text after code text before samp<xhtml:samp>XhtmlSampType</xhtml:samp>text after samp text before kbd<xhtml:kbd>XhtmlKbdType</xhtml:kbd>text after kbd text before var<xhtml:var>XhtmlVarType</xhtml:var>text after var text before cite<xhtml:cite>XhtmlCiteType</xhtml:cite>text after cite text before abbr<xhtml:abbr>XhtmlAbbrType</xhtml:abbr>text after abbr text before acronym<xhtml:acronym>XhtmlAcronymType</xhtml:acronym>text after acronym text before q<xhtml:q>XhtmlQType</xhtml:q>text after q text before tt<xhtml:tt>XhtmlInlPresType</xhtml:tt>text after tt text before i<xhtml:i>XhtmlInlPresType</xhtml:i>text after i text before b<xhtml:b>XhtmlInlPresType</xhtml:b>text after b text before big<xhtml:big>XhtmlInlPresType</xhtml:big>text after big text before small<xhtml:small>XhtmlInlPresType</xhtml:small>text after small text before sub<xhtml:sub>XhtmlInlPresType</xhtml:sub>text after sub text before sup<xhtml:sup>XhtmlInlPresType</xhtml:sup>text after sup text before ins<xhtml:ins>XhtmlEditType</xhtml:ins>text after ins text before del<xhtml:del>XhtmlEditType</xhtml:del>text after del</xhtml:a></xhtml:p>";

            this.reqIF = new ReqIF();
            this.reqIF.Lang = "en";

            var header = new ReqIFHeader();
            header.Comment = "this is a comment";
            header.CreationTime = DateTime.Parse("2015-12-01");
            header.Identifier = "reqifheader";
            header.RepositoryId = "a repos id";
            header.ReqIFToolId = "tool - CDP4";
            header.ReqIFVersion = "1.0";
            header.SourceToolId = "source tool - CDP4";
            header.Title = "this is a title";

            this.reqIF.TheHeader.Add(header);

            var reqIfContent = new ReqIFContent();
            this.reqIF.CoreContent.Add(reqIfContent);

            this.CreateDataTypes();
            this.CreateSpecObjectType();
            this.CreateSpecificationType();
            this.CreateSpecRelationType();
            this.CreateRelationGroupType();

            this.CreateSpecObjects();
            this.CreateSpecRelations();
            this.CreateSpecifications();
            this.CreateRelationGroup();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(this.resultFileUri); 
        }

        /// <summary>
        /// create all the different <see cref="DatatypeDefinition"/>s
        /// </summary>
        private void CreateDataTypes()
        {
            var reqIfContent = this.reqIF.CoreContent.SingleOrDefault();

            var datatypeDefinitionBoolean = new DatatypeDefinitionBoolean();
            datatypeDefinitionBoolean.Description = "boolean data type definition";
            datatypeDefinitionBoolean.Identifier = "boolean";
            datatypeDefinitionBoolean.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionBoolean.LongName = "a boolean";            
            reqIfContent.DataTypes.Add(datatypeDefinitionBoolean);
            
            var datatypeDefinitionDate = new DatatypeDefinitionDate();
            datatypeDefinitionDate.Description = "date data type definition";
            datatypeDefinitionDate.Identifier = "DateTime";
            datatypeDefinitionDate.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionDate.LongName = "a date";
            reqIfContent.DataTypes.Add(datatypeDefinitionDate);

            var datatypeDefinitionEnumeration = new DatatypeDefinitionEnumeration();
            datatypeDefinitionEnumeration.Description = "enum value type definition";
            datatypeDefinitionEnumeration.Identifier = this.enumdatatype_id;
            datatypeDefinitionEnumeration.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionEnumeration.LongName = "an enumeration";

            var enumValuelow = new EnumValue();            
            enumValuelow.Identifier = this.enum_value_low_id;
            enumValuelow.LastChange = DateTime.Parse("2015-12-01");
            enumValuelow.LongName = "low";

            var embeddedValueLow = new EmbeddedValue();
            embeddedValueLow.Key = 1;
            embeddedValueLow.OtherContent = "foo";
            enumValuelow.Properties = embeddedValueLow;

            var enumValuemedium = new EnumValue();
            enumValuemedium.Identifier = this.enum_value_medium_id;
            enumValuemedium.LastChange = DateTime.Parse("2015-12-01");
            enumValuemedium.LongName = "medium";

            var embeddedValueMedium = new EmbeddedValue();
            embeddedValueMedium.Key = 2;
            embeddedValueMedium.OtherContent = "bar";
            enumValuemedium.Properties = embeddedValueMedium;

            datatypeDefinitionEnumeration.SpecifiedValues.Add(enumValuelow);
            datatypeDefinitionEnumeration.SpecifiedValues.Add(enumValuemedium);

            reqIfContent.DataTypes.Add(datatypeDefinitionEnumeration);

            var datatypeDefinitionInteger = new DatatypeDefinitionInteger();
            datatypeDefinitionInteger.Description = "integer data type definition";
            datatypeDefinitionInteger.Identifier = "integer";
            datatypeDefinitionInteger.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionInteger.LongName = "an integer";
            datatypeDefinitionInteger.Min = 2;
            datatypeDefinitionInteger.Max = 6;
            reqIfContent.DataTypes.Add(datatypeDefinitionInteger);

            var datatypeDefinitionReal = new DatatypeDefinitionReal();
            datatypeDefinitionReal.Description = "real data type definition";
            datatypeDefinitionReal.Identifier = "real";
            datatypeDefinitionReal.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionReal.LongName = "a real";
            datatypeDefinitionReal.Accuracy = 5;
            datatypeDefinitionReal.Min = 1;
            datatypeDefinitionReal.Max = 5;
            reqIfContent.DataTypes.Add(datatypeDefinitionReal);

            var datatypeDefinitionString = new DatatypeDefinitionString();
            datatypeDefinitionString.Description = "string data type definition";
            datatypeDefinitionString.Identifier = "string";
            datatypeDefinitionString.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionString.MaxLength = 32000;
            datatypeDefinitionString.LongName = "a string";
            reqIfContent.DataTypes.Add(datatypeDefinitionString);

            var datatypeDefinitionXhtml = new DatatypeDefinitionXHTML();
            datatypeDefinitionXhtml.Description = "string data type definition";
            datatypeDefinitionXhtml.Identifier = "xhtml";
            datatypeDefinitionXhtml.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionXhtml.LongName = "a string";
            reqIfContent.DataTypes.Add(datatypeDefinitionXhtml);
        }

        /// <summary>
        /// create a <see cref="SpecObjectType"/> with attribute definitions
        /// </summary>
        private void CreateSpecObjectType()
        {
            var reqIfContent = this.reqIF.CoreContent.SingleOrDefault();

            var specObjectType = new SpecObjectType();
            specObjectType.LongName = "Requirement Type";
            specObjectType.Identifier = "requirement";
            specObjectType.LastChange = DateTime.Parse("2015-12-01");

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            attributeDefinitionBoolean.LongName = "boolean attribute";
            attributeDefinitionBoolean.Identifier = "requirement-boolean-attribute";
            attributeDefinitionBoolean.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionBoolean.Type = (DatatypeDefinitionBoolean)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionBoolean));
            specObjectType.SpecAttributes.Add(attributeDefinitionBoolean);

            var attributeDefinitionDate = new AttributeDefinitionDate();
            attributeDefinitionDate.LongName = "date attribute";
            attributeDefinitionDate.Identifier = "requirement-date-attribute";
            attributeDefinitionDate.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionDate.Type = (DatatypeDefinitionDate)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionDate));
            specObjectType.SpecAttributes.Add(attributeDefinitionDate);

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            attributeDefinitionEnumeration.LongName = "enumeration attribute";
            attributeDefinitionEnumeration.Identifier = "requirement-enumeration-attribute";
            attributeDefinitionEnumeration.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionEnumeration.Type = (DatatypeDefinitionEnumeration)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionEnumeration));
            specObjectType.SpecAttributes.Add(attributeDefinitionEnumeration);

            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            attributeDefinitionInteger.LongName = "integer attribute";
            attributeDefinitionInteger.Identifier = "requirement-integer-attribute";
            attributeDefinitionInteger.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionInteger.Type = (DatatypeDefinitionInteger)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionInteger));
            specObjectType.SpecAttributes.Add(attributeDefinitionInteger);

            var attributeDefinitionReal = new AttributeDefinitionReal();
            attributeDefinitionReal.LongName = "real attribute";
            attributeDefinitionReal.Identifier = "requirement-real-attribute";
            attributeDefinitionReal.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionReal.Type = (DatatypeDefinitionReal)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionReal));
            specObjectType.SpecAttributes.Add(attributeDefinitionReal);

            var attributeDefinitionString = new AttributeDefinitionString();
            attributeDefinitionString.LongName = "string attribute";
            attributeDefinitionString.Identifier = "requirement-string-attribute";
            attributeDefinitionString.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionString.Type = (DatatypeDefinitionString)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionString));
            specObjectType.SpecAttributes.Add(attributeDefinitionString);

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            attributeDefinitionXhtml.LongName = "xhtml attribute";
            attributeDefinitionXhtml.Identifier = "requirement-xhtml-attribute";
            attributeDefinitionXhtml.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionXhtml.Type = (DatatypeDefinitionXHTML)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionXHTML));
            specObjectType.SpecAttributes.Add(attributeDefinitionXhtml);

            reqIfContent.SpecTypes.Add(specObjectType);
        }

        /// <summary>
        /// create a <see cref="SpecificationType"/> with attribute definitions
        /// </summary>
        private void CreateSpecificationType()
        {
            var reqIfContent = this.reqIF.CoreContent.SingleOrDefault();

            var specificationType = new SpecificationType();
            specificationType.LongName = "Specification Type";
            specificationType.Identifier = "specificationtype";
            specificationType.LastChange = DateTime.Parse("2015-12-01");

            this.CreateAndAddAttributeDefinitionsToSpecType(specificationType, reqIfContent);

            reqIfContent.SpecTypes.Add(specificationType);
        }

        /// <summary>
        /// create a <see cref="SpecRelationType"/> with attribute definitions
        /// </summary>
        private void CreateSpecRelationType()
        {
            var reqIfContent = this.reqIF.CoreContent.SingleOrDefault();

            var specRelationType = new SpecRelationType();
            specRelationType.LongName = "Specification Relation Type";
            specRelationType.Identifier = "specificationrelation";
            specRelationType.LastChange = DateTime.Parse("2015-12-01");

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            attributeDefinitionBoolean.LongName = "boolean attribute";
            attributeDefinitionBoolean.Identifier = "specificationrelation-boolean-attribute";
            attributeDefinitionBoolean.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionBoolean.Type = (DatatypeDefinitionBoolean)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionBoolean));
            specRelationType.SpecAttributes.Add(attributeDefinitionBoolean);

            var attributeDefinitionDate = new AttributeDefinitionDate();
            attributeDefinitionDate.LongName = "date attribute";
            attributeDefinitionDate.Identifier = "specificationrelation-date-attribute";
            attributeDefinitionDate.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionDate.Type = (DatatypeDefinitionDate)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionDate));
            specRelationType.SpecAttributes.Add(attributeDefinitionDate);

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            attributeDefinitionEnumeration.LongName = "enumeration attribute";
            attributeDefinitionEnumeration.Identifier = "specificationrelation-enumeration-attribute";
            attributeDefinitionEnumeration.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionEnumeration.Type = (DatatypeDefinitionEnumeration)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionEnumeration));
            specRelationType.SpecAttributes.Add(attributeDefinitionEnumeration);

            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            attributeDefinitionInteger.LongName = "integer attribute";
            attributeDefinitionInteger.Identifier = "specificationrelation-integer-attribute";
            attributeDefinitionInteger.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionInteger.Type = (DatatypeDefinitionInteger)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionInteger));
            specRelationType.SpecAttributes.Add(attributeDefinitionInteger);

            var attributeDefinitionReal = new AttributeDefinitionReal();
            attributeDefinitionReal.LongName = "real attribute";
            attributeDefinitionReal.Identifier = "specificationrelation-real-attribute";
            attributeDefinitionReal.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionReal.Type = (DatatypeDefinitionReal)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionReal));
            specRelationType.SpecAttributes.Add(attributeDefinitionReal);

            var attributeDefinitionString = new AttributeDefinitionString();
            attributeDefinitionString.LongName = "string attribute";
            attributeDefinitionString.Identifier = "specificationrelation-string-attribute";
            attributeDefinitionString.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionString.Type = (DatatypeDefinitionString)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionString));
            specRelationType.SpecAttributes.Add(attributeDefinitionString);

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            attributeDefinitionXhtml.LongName = "xhtml attribute";
            attributeDefinitionXhtml.Identifier = "specificationrelation-xhtml-attribute";
            attributeDefinitionXhtml.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionXhtml.Type = (DatatypeDefinitionXHTML)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionXHTML));
            specRelationType.SpecAttributes.Add(attributeDefinitionXhtml);

            reqIfContent.SpecTypes.Add(specRelationType);
        }

        /// <summary>
        /// create a <see cref="RelationGroupType"/> with attribute definitions
        /// </summary>
        private void CreateRelationGroupType()
        {
            var reqIfContent = this.reqIF.CoreContent.SingleOrDefault();

            var relationGroupType = new RelationGroupType();
            relationGroupType.LongName = "Relation Group Type";
            relationGroupType.Identifier = "relationgrouptype";
            relationGroupType.LastChange = DateTime.Parse("2015-12-01");

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            attributeDefinitionBoolean.LongName = "boolean attribute";
            attributeDefinitionBoolean.Identifier = "relationgrouptype-boolean-attribute";
            attributeDefinitionBoolean.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionBoolean.Type = (DatatypeDefinitionBoolean)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionBoolean));
            relationGroupType.SpecAttributes.Add(attributeDefinitionBoolean);

            var attributeDefinitionDate = new AttributeDefinitionDate();
            attributeDefinitionDate.LongName = "date attribute";
            attributeDefinitionDate.Identifier = "relationgrouptype-date-attribute";
            attributeDefinitionDate.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionDate.Type = (DatatypeDefinitionDate)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionDate));
            relationGroupType.SpecAttributes.Add(attributeDefinitionDate);

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            attributeDefinitionEnumeration.LongName = "enumeration attribute";
            attributeDefinitionEnumeration.Identifier = "relationgrouptype-enumeration-attribute";
            attributeDefinitionEnumeration.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionEnumeration.Type = (DatatypeDefinitionEnumeration)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionEnumeration));
            relationGroupType.SpecAttributes.Add(attributeDefinitionEnumeration);

            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            attributeDefinitionInteger.LongName = "integer attribute";
            attributeDefinitionInteger.Identifier = "relationgrouptype-integer-attribute";
            attributeDefinitionInteger.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionInteger.Type = (DatatypeDefinitionInteger)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionInteger));
            relationGroupType.SpecAttributes.Add(attributeDefinitionInteger);

            var attributeDefinitionReal = new AttributeDefinitionReal();
            attributeDefinitionReal.LongName = "real attribute";
            attributeDefinitionReal.Identifier = "relationgrouptype-real-attribute";
            attributeDefinitionReal.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionReal.Type = (DatatypeDefinitionReal)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionReal));
            relationGroupType.SpecAttributes.Add(attributeDefinitionReal);

            var attributeDefinitionString = new AttributeDefinitionString();
            attributeDefinitionString.LongName = "string attribute";
            attributeDefinitionString.Identifier = "relationgrouptype-string-attribute";
            attributeDefinitionString.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionString.Type = (DatatypeDefinitionString)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionString));
            relationGroupType.SpecAttributes.Add(attributeDefinitionString);

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            attributeDefinitionXhtml.LongName = "xhtml attribute";
            attributeDefinitionXhtml.Identifier = "relationgrouptype-xhtml-attribute";
            attributeDefinitionXhtml.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionXhtml.Type = (DatatypeDefinitionXHTML)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionXHTML));
            relationGroupType.SpecAttributes.Add(attributeDefinitionXhtml);

            reqIfContent.SpecTypes.Add(relationGroupType);
        }

        /// <summary>
        /// Create and add <see cref="AttributeDefinition"/>s to a <see cref="SpecType"/>
        /// </summary>
        /// <param name="specType">
        /// The <see cref="SpecType"/> to add the <see cref="AttributeDefinition"/>s to
        /// </param>
        /// <param name="reqIfContent">
        /// An instance of <see cref="ReqIFContent"/>
        /// </param>
        private void CreateAndAddAttributeDefinitionsToSpecType(SpecType specType, ReqIFContent reqIfContent)
        {
            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            attributeDefinitionBoolean.LongName = "boolean attribute";
            attributeDefinitionBoolean.Identifier = "specification-boolean-attribute";
            attributeDefinitionBoolean.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionBoolean.Type = (DatatypeDefinitionBoolean)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionBoolean));
            specType.SpecAttributes.Add(attributeDefinitionBoolean);

            var attributeDefinitionDate = new AttributeDefinitionDate();
            attributeDefinitionDate.LongName = "date attribute";
            attributeDefinitionDate.Identifier = "specification-date-attribute";
            attributeDefinitionDate.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionDate.Type = (DatatypeDefinitionDate)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionDate));
            specType.SpecAttributes.Add(attributeDefinitionDate);

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            attributeDefinitionEnumeration.LongName = "enumeration attribute";
            attributeDefinitionEnumeration.Identifier = "specification-enumeration-attribute";
            attributeDefinitionEnumeration.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionEnumeration.Type = (DatatypeDefinitionEnumeration)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionEnumeration));
            specType.SpecAttributes.Add(attributeDefinitionEnumeration);

            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            attributeDefinitionInteger.LongName = "integer attribute";
            attributeDefinitionInteger.Identifier = "specification-integer-attribute";
            attributeDefinitionInteger.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionInteger.Type = (DatatypeDefinitionInteger)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionInteger));
            specType.SpecAttributes.Add(attributeDefinitionInteger);

            var attributeDefinitionReal = new AttributeDefinitionReal();
            attributeDefinitionReal.LongName = "real attribute";
            attributeDefinitionReal.Identifier = "specification-real-attribute";
            attributeDefinitionReal.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionReal.Type = (DatatypeDefinitionReal)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionReal));
            specType.SpecAttributes.Add(attributeDefinitionReal);

            var attributeDefinitionString = new AttributeDefinitionString();
            attributeDefinitionString.LongName = "string attribute";
            attributeDefinitionString.Identifier = "specification-string-attribute";
            attributeDefinitionString.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionString.Type = (DatatypeDefinitionString)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionString));
            specType.SpecAttributes.Add(attributeDefinitionString);

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            attributeDefinitionXhtml.LongName = "xhtml attribute";
            attributeDefinitionXhtml.Identifier = "specification-xhtml-attribute";
            attributeDefinitionXhtml.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionXhtml.Type = (DatatypeDefinitionXHTML)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionXHTML));
            specType.SpecAttributes.Add(attributeDefinitionXhtml);
        }

        /// <summary>
        /// create a <see cref="SpecObject"/>s with attribute values
        /// </summary>
        private void CreateSpecObjects()
        {
            var reqIfContent = this.reqIF.CoreContent.SingleOrDefault();

            var specObject = new SpecObject();
            specObject.LongName = "spec object 1";
            specObject.Identifier = this.specobject_1_id;
            specObject.LastChange = DateTime.Parse("2015-12-01");
            var specType = (SpecObjectType)reqIfContent.SpecTypes.SingleOrDefault(x => x.GetType() == typeof(SpecObjectType));
            specObject.Type = specType;
            this.CreateValuesForSpecElementWithAttributes(specObject, specType);            
            reqIfContent.SpecObjects.Add(specObject);

            var specobject_2 = new SpecObject();
            specobject_2.LongName = "spec object 2";
            specobject_2.Identifier = this.specobject_2_id;
            specobject_2.LastChange = DateTime.Parse("2015-12-01");
            specobject_2.Type = specType;
            this.CreateValuesForSpecElementWithAttributes(specobject_2, specType);
            reqIfContent.SpecObjects.Add(specobject_2);

            var specobject_3 = new SpecObject();
            specobject_3.LongName = "spec object 3";
            specobject_3.Identifier = this.specobject_3_id;
            specobject_3.LastChange = DateTime.Parse("2015-12-01");
            specobject_3.Type = specType;
            this.CreateValuesForSpecElementWithAttributes(specobject_3, specType);
            reqIfContent.SpecObjects.Add(specobject_3);
        }

        /// <summary>
        /// create a <see cref="SpecRelation"/>s
        /// </summary>
        private void CreateSpecRelations()
        {
            var reqIfContent = this.reqIF.CoreContent.SingleOrDefault();

            var specRelationType = (SpecRelationType)reqIfContent.SpecTypes.SingleOrDefault(x => x.GetType() == typeof(SpecRelationType));
            var source = reqIfContent.SpecObjects.SingleOrDefault(x => x.Identifier == this.specobject_1_id);
            var target = reqIfContent.SpecObjects.SingleOrDefault(x => x.Identifier == this.specobject_2_id);

            var specRelation = new SpecRelation();
            specRelation.Identifier = string.Format("{0}-{1}", source.Identifier, target.Identifier);
            specRelation.LastChange = DateTime.Parse("2015-12-01");
            specRelation.LongName = "A relationship between spec objects";
            specRelation.Type = specRelationType;
            specRelation.Source = source;
            specRelation.Target = target;
            this.CreateValuesForSpecElementWithAttributes(specRelation, specRelationType);
            
            reqIfContent.SpecRelations.Add(specRelation);
        }

        /// <summary>
        /// create a <see cref="Specification"/>s
        /// </summary>
        private void CreateSpecifications()
        {
            var reqIfContent = this.reqIF.CoreContent.SingleOrDefault();
            var specificationType = (SpecificationType)reqIfContent.SpecTypes.SingleOrDefault(x => x.GetType() == typeof(SpecificationType));

            var object1 = reqIfContent.SpecObjects.SingleOrDefault(x => x.Identifier == this.specobject_1_id);
            var object2 = reqIfContent.SpecObjects.SingleOrDefault(x => x.Identifier == this.specobject_2_id);
            var object3 = reqIfContent.SpecObjects.SingleOrDefault(x => x.Identifier == this.specobject_3_id);

            var specification1 = new Specification();
            specification1.Identifier = "specification-1";
            specification1.LastChange = DateTime.Parse("2015-12-01");
            specification1.LongName = "specification 1";
            specification1.Type = specificationType;
            this.CreateValuesForSpecElementWithAttributes(specification1, specificationType);
            
            var specHierarchy1 = new SpecHierarchy();
            specHierarchy1.Identifier = "spec-hierarchy-1";
            specHierarchy1.LastChange = DateTime.Parse("2015-12-01");
            specHierarchy1.LongName = "specHierarchy 1";
            specHierarchy1.Object = object1;
            
            var specHierarchy1_1 = new SpecHierarchy();
            specHierarchy1_1.Identifier = "spec-hierarchy-1-1";
            specHierarchy1_1.LastChange = DateTime.Parse("2015-12-01");
            specHierarchy1_1.LongName = "specHierarchy 1_1";
            specHierarchy1_1.Object = object2;

            var specHierarchy1_2 = new SpecHierarchy();
            specHierarchy1_2.Identifier = "spec-hierarchy-1-2";
            specHierarchy1_2.LastChange = DateTime.Parse("2015-12-01");
            specHierarchy1_2.LongName = "specHierarchy 1_2";
            specHierarchy1_2.Object = object3;

            specification1.Children.Add(specHierarchy1);

            specHierarchy1.Children.Add(specHierarchy1_1);
            specHierarchy1.Children.Add(specHierarchy1_2);

            reqIfContent.Specifications.Add(specification1);

            var specification2 = new Specification();
            specification2.Identifier = "specification-2";
            specification2.LastChange = DateTime.Parse("2015-12-01");
            specification2.LongName = "specification 2";
            specification2.Type = specificationType;
            this.CreateValuesForSpecElementWithAttributes(specification2, specificationType);
            reqIfContent.Specifications.Add(specification2);
        }

        /// <summary>
        /// Creates a <see cref="RelationGroup"/>
        /// </summary>
        private void CreateRelationGroup()
        {
            var reqIfContent = this.reqIF.CoreContent.SingleOrDefault();

            var relationGroupType = (RelationGroupType)reqIfContent.SpecTypes.SingleOrDefault(x => x.GetType() == typeof(RelationGroupType));

            var relationGroup = new RelationGroup();
            relationGroup.Identifier = "relationgroup-1";
            relationGroup.LastChange = DateTime.Parse("2015-12-01");
            relationGroup.LongName = "relationgroup 1";
            relationGroup.Type = relationGroupType;

            var sourceSpecification = reqIfContent.Specifications.SingleOrDefault(x => x.Identifier == "specification-1");
            var targetSpecification = reqIfContent.Specifications.SingleOrDefault(x => x.Identifier == "specification-2");

            relationGroup.SourceSpecification = sourceSpecification;
            relationGroup.TargetSpecification = targetSpecification;

            reqIfContent.SpecRelationGroups.Add(relationGroup);
        }

        /// <summary>
        /// Create <see cref="AttributeValue"/> For <see cref="SpecElementWithAttributes"/>
        /// </summary>
        /// <param name="specElementWithAttributes">
        /// The <see cref="SpecElementWithAttributes"/> to which <see cref="AttributeValue"/>s need to be added.
        /// </param>
        /// <param name="specType">
        /// The <see cref="SpecType"/> of the <see cref="specElementWithAttributes"/>
        /// </param>
        private void CreateValuesForSpecElementWithAttributes(SpecElementWithAttributes specElementWithAttributes, SpecType specType)
        {
            var attributeValueBoolean = new AttributeValueBoolean();
            attributeValueBoolean.Definition = (AttributeDefinitionBoolean)specType.SpecAttributes.SingleOrDefault(x => x.GetType() == typeof(AttributeDefinitionBoolean));
            attributeValueBoolean.TheValue = true;
            specElementWithAttributes.Values.Add(attributeValueBoolean);

            var attributeValueDate = new AttributeValueDate();
            attributeValueDate.Definition = (AttributeDefinitionDate)specType.SpecAttributes.SingleOrDefault(x => x.GetType() == typeof(AttributeDefinitionDate));
            attributeValueDate.TheValue = XmlConvert.ToDateTime("2015-12-01"); ;
            specElementWithAttributes.Values.Add(attributeValueDate);

            var attributeValueEnumeration = new AttributeValueEnumeration();
            attributeValueEnumeration.Definition = (AttributeDefinitionEnumeration)specType.SpecAttributes.SingleOrDefault(x => x.GetType() == typeof(AttributeDefinitionEnumeration));
            var enumValue = attributeValueEnumeration.Definition.Type.SpecifiedValues.FirstOrDefault();
            attributeValueEnumeration.Values.Add(enumValue);
            specElementWithAttributes.Values.Add(attributeValueEnumeration);

            var attributeValueInteger = new AttributeValueInteger();
            attributeValueInteger.Definition = (AttributeDefinitionInteger)specType.SpecAttributes.SingleOrDefault(x => x.GetType() == typeof(AttributeDefinitionInteger));
            attributeValueInteger.TheValue = 1;
            specElementWithAttributes.Values.Add(attributeValueInteger);

            var attributeValueReal = new AttributeValueReal();
            attributeValueReal.Definition = (AttributeDefinitionReal)specType.SpecAttributes.SingleOrDefault(x => x.GetType() == typeof(AttributeDefinitionReal));
            attributeValueReal.TheValue = 100;
            specElementWithAttributes.Values.Add(attributeValueReal);

            var attributeValueString = new AttributeValueString();
            attributeValueString.Definition = (AttributeDefinitionString)specType.SpecAttributes.SingleOrDefault(x => x.GetType() == typeof(AttributeDefinitionString));
            attributeValueString.TheValue = "a string value";
            specElementWithAttributes.Values.Add(attributeValueString);

            var attributeValueXhtml = new AttributeValueXHTML();
            attributeValueXhtml.Definition = (AttributeDefinitionXHTML)specType.SpecAttributes.SingleOrDefault(x => x.GetType() == typeof(AttributeDefinitionXHTML));
            attributeValueXhtml.TheValue = this.xhtmlcontent;
            specElementWithAttributes.Values.Add(attributeValueXhtml);
        }

        /// <summary>
        /// Create an <see cref="AlternativeId"/> for the <see cref="Identifiable"/>
        /// </summary>
        /// <param name="identifiable">
        /// The <see cref="Identifiable"/> for which an <see cref="AlternativeId"/> needs to be created
        /// </param>
        private void CreateAlternativeId(Identifiable identifiable)
        {
            var alternativeId = new AlternativeId();
            alternativeId.Identifier = identifiable.Identifier + "_alternative";

            alternativeId.Ident = identifiable;
            identifiable.AlternativeId = alternativeId;
        }

        [Test]
        public void VerifyThatClassesCanBeSerialized()
        {
            var xmlSerializer = new XmlSerializer(typeof(ReqIF), ReqIFNamespace);
            
            using (var writer = XmlWriter.Create(@"../../output.xml", new XmlWriterSettings { Indent = true }))
            {
                xmlSerializer.Serialize(writer, this.reqIF);
            }
        }

        [Test]
        public void VerifyThatArgumentExceptionIsRaisedOnSerialize()
        {
            var serializer = new ReqIFSerializer(false);
            
            Assert.That(
                () => serializer.Serialize(null, null, null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.EqualTo("The reqIf object cannot be null.\r\nParameter name: reqIf"));

            Assert.That(
                () => serializer.Serialize(this.reqIF, null, null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.EqualTo("The path of the file cannot be null.\r\nParameter name: fileUri"));

            Assert.That(
                () => serializer.Serialize(this.reqIF, string.Empty, null),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>()
                .With.Message.EqualTo("The path of the file cannot be empty.\r\nParameter name: fileUri"));
        }

        [Test]
        public void VerifyThatTheReqIfSerializerSerializesaReqIfDocumentWithoutValidation()
        {
            var serializer = new ReqIFSerializer(false);
            serializer.Serialize(this.reqIF, this.resultFileUri , null);

            Assert.IsTrue(File.Exists(this.resultFileUri));
        }        
    }
}