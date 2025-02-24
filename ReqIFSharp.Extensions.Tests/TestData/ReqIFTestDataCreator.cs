﻿//  -------------------------------------------------------------------------------------------------
//  <copyright file="ReqIFTestDataCreator.cs" company="Starion Group S.A.">
// 
//    Copyright 2017-2025 Starion Group S.A.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  -------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.Tests.TestData
{
    using System;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// The purpose of the <see cref="ReqIFTestDataCreator"/> is to create a <see cref="ReqIF"/> object
    /// that provides coverage
    /// </summary>
    public class ReqIFTestDataCreator
    {
        private ReqIF reqIF;

        private string specobject_1_id;

        private string specobject_2_id;

        private string specobject_3_id;

        private string enumdatatype_id;

        private string enum_value_low_id;

        private string enum_value_medium_id;

        private string xhtmlcontent;

        /// <summary>
        /// Creates a <see cref="ReqIF"/> object for testing purposes
        /// </summary>
        /// <returns>
        /// An instance of <see cref="ReqIF"/>
        /// </returns>
        public ReqIF Create()
        {
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

            this.reqIF.TheHeader = header;

            var reqIfContent = new ReqIFContent();
            this.reqIF.CoreContent = reqIfContent;

            this.CreateDataTypes();
            this.CreateSpecObjectType();
            this.CreateSpecificationType();
            this.CreateSpecRelationType();
            this.CreateRelationGroupType();

            this.CreateSpecObjects();
            this.CreateSpecRelations();
            this.CreateSpecifications();
            this.CreateRelationGroup();

            return this.reqIF;
        }

        /// <summary>
        /// create all the different <see cref="DatatypeDefinition"/>s
        /// </summary>
        private void CreateDataTypes()
        {
            var datatypeDefinitionBoolean = new DatatypeDefinitionBoolean();
            datatypeDefinitionBoolean.Description = "boolean data type definition";
            datatypeDefinitionBoolean.Identifier = "boolean";
            datatypeDefinitionBoolean.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionBoolean.LongName = "a boolean";
            CreateAlternativeId(datatypeDefinitionBoolean);
            this.reqIF.CoreContent.DataTypes.Add(datatypeDefinitionBoolean);
            datatypeDefinitionBoolean.ReqIFContent = this.reqIF.CoreContent;

            var datatypeDefinitionDate = new DatatypeDefinitionDate();
            datatypeDefinitionDate.Description = "date data type definition";
            datatypeDefinitionDate.Identifier = "DateTime";
            datatypeDefinitionDate.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionDate.LongName = "a date";
            CreateAlternativeId(datatypeDefinitionDate);
            this.reqIF.CoreContent.DataTypes.Add(datatypeDefinitionDate);
            datatypeDefinitionDate.ReqIFContent = this.reqIF.CoreContent;

            var datatypeDefinitionEnumeration = new DatatypeDefinitionEnumeration();
            datatypeDefinitionEnumeration.Description = "enum value type definition";
            datatypeDefinitionEnumeration.Identifier = this.enumdatatype_id;
            datatypeDefinitionEnumeration.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionEnumeration.LongName = "an enumeration";
            CreateAlternativeId(datatypeDefinitionEnumeration);

            var enumValuelow = new EnumValue();
            enumValuelow.Identifier = this.enum_value_low_id;
            enumValuelow.LastChange = DateTime.Parse("2015-12-01");
            enumValuelow.LongName = "low";
            CreateAlternativeId(enumValuelow);

            var embeddedValueLow = new EmbeddedValue();
            embeddedValueLow.Key = 1;
            embeddedValueLow.OtherContent = "foo";
            enumValuelow.Properties = embeddedValueLow;

            var enumValuemedium = new EnumValue();
            enumValuemedium.Identifier = this.enum_value_medium_id;
            enumValuemedium.LastChange = DateTime.Parse("2015-12-01");
            enumValuemedium.LongName = "medium";
            CreateAlternativeId(enumValuemedium);

            var embeddedValueMedium = new EmbeddedValue();
            embeddedValueMedium.Key = 2;
            embeddedValueMedium.OtherContent = "bar";
            enumValuemedium.Properties = embeddedValueMedium;

            datatypeDefinitionEnumeration.SpecifiedValues.Add(enumValuelow);
            datatypeDefinitionEnumeration.SpecifiedValues.Add(enumValuemedium);

            this.reqIF.CoreContent.DataTypes.Add(datatypeDefinitionEnumeration);
            datatypeDefinitionEnumeration.ReqIFContent = this.reqIF.CoreContent;

            var datatypeDefinitionInteger = new DatatypeDefinitionInteger();
            datatypeDefinitionInteger.Description = "integer data type definition";
            datatypeDefinitionInteger.Identifier = "integer";
            datatypeDefinitionInteger.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionInteger.LongName = "an integer";
            datatypeDefinitionInteger.Min = 2;
            datatypeDefinitionInteger.Max = 6;
            CreateAlternativeId(datatypeDefinitionInteger);
            this.reqIF.CoreContent.DataTypes.Add(datatypeDefinitionInteger);
            datatypeDefinitionInteger.ReqIFContent = this.reqIF.CoreContent;

            var datatypeDefinitionReal = new DatatypeDefinitionReal();
            datatypeDefinitionReal.Description = "real data type definition";
            datatypeDefinitionReal.Identifier = "real";
            datatypeDefinitionReal.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionReal.LongName = "a real";
            datatypeDefinitionReal.Accuracy = 5;
            datatypeDefinitionReal.Min = 1;
            datatypeDefinitionReal.Max = 5;
            CreateAlternativeId(datatypeDefinitionReal);
            this.reqIF.CoreContent.DataTypes.Add(datatypeDefinitionReal);
            datatypeDefinitionReal.ReqIFContent = this.reqIF.CoreContent;

            var datatypeDefinitionString = new DatatypeDefinitionString();
            datatypeDefinitionString.Description = "string data type definition";
            datatypeDefinitionString.Identifier = "string";
            datatypeDefinitionString.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionString.MaxLength = 32000;
            datatypeDefinitionString.LongName = "a string";
            CreateAlternativeId(datatypeDefinitionString);
            this.reqIF.CoreContent.DataTypes.Add(datatypeDefinitionString);
            datatypeDefinitionString.ReqIFContent = this.reqIF.CoreContent;

            var datatypeDefinitionXhtml = new DatatypeDefinitionXHTML();
            datatypeDefinitionXhtml.Description = "string data type definition";
            datatypeDefinitionXhtml.Identifier = "xhtml";
            datatypeDefinitionXhtml.LastChange = DateTime.Parse("2015-12-01");
            datatypeDefinitionXhtml.LongName = "a string";
            CreateAlternativeId(datatypeDefinitionXhtml);
            this.reqIF.CoreContent.DataTypes.Add(datatypeDefinitionXhtml);
            datatypeDefinitionXhtml.ReqIFContent = this.reqIF.CoreContent;
        }

        /// <summary>
        /// create a <see cref="SpecObjectType"/> with attribute definitions
        /// </summary>
        private void CreateSpecObjectType()
        {
            var specObjectType = new SpecObjectType();
            specObjectType.LongName = "Requirement Type";
            specObjectType.Identifier = "requirement";
            specObjectType.LastChange = DateTime.Parse("2015-12-01");
            CreateAlternativeId(specObjectType);

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            attributeDefinitionBoolean.LongName = "boolean attribute";
            attributeDefinitionBoolean.Identifier = "requirement-boolean-attribute";
            attributeDefinitionBoolean.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionBoolean.Type = (DatatypeDefinitionBoolean)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionBoolean));
            CreateAlternativeId(attributeDefinitionBoolean);
            specObjectType.SpecAttributes.Add(attributeDefinitionBoolean);

            var attributeDefinitionDate = new AttributeDefinitionDate();
            attributeDefinitionDate.LongName = "date attribute";
            attributeDefinitionDate.Identifier = "requirement-date-attribute";
            attributeDefinitionDate.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionDate.Type = (DatatypeDefinitionDate)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionDate));
            CreateAlternativeId(attributeDefinitionDate);
            specObjectType.SpecAttributes.Add(attributeDefinitionDate);

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            attributeDefinitionEnumeration.LongName = "enumeration attribute";
            attributeDefinitionEnumeration.Identifier = "requirement-enumeration-attribute";
            attributeDefinitionEnumeration.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionEnumeration.Type = (DatatypeDefinitionEnumeration)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionEnumeration));
            CreateAlternativeId(attributeDefinitionEnumeration);
            specObjectType.SpecAttributes.Add(attributeDefinitionEnumeration);

            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            attributeDefinitionInteger.LongName = "integer attribute";
            attributeDefinitionInteger.Identifier = "requirement-integer-attribute";
            attributeDefinitionInteger.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionInteger.Type = (DatatypeDefinitionInteger)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionInteger));
            CreateAlternativeId(attributeDefinitionInteger);
            specObjectType.SpecAttributes.Add(attributeDefinitionInteger);

            var attributeDefinitionReal = new AttributeDefinitionReal();
            attributeDefinitionReal.LongName = "real attribute";
            attributeDefinitionReal.Identifier = "requirement-real-attribute";
            attributeDefinitionReal.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionReal.Type = (DatatypeDefinitionReal)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionReal));
            CreateAlternativeId(attributeDefinitionReal);
            specObjectType.SpecAttributes.Add(attributeDefinitionReal);

            var attributeDefinitionString = new AttributeDefinitionString();
            attributeDefinitionString.LongName = "string attribute";
            attributeDefinitionString.Identifier = "requirement-string-attribute";
            attributeDefinitionString.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionString.Type = (DatatypeDefinitionString)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionString));
            CreateAlternativeId(attributeDefinitionString);
            specObjectType.SpecAttributes.Add(attributeDefinitionString);

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            attributeDefinitionXhtml.LongName = "xhtml attribute";
            attributeDefinitionXhtml.Identifier = "requirement-xhtml-attribute";
            attributeDefinitionXhtml.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionXhtml.Type = (DatatypeDefinitionXHTML)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionXHTML));
            CreateAlternativeId(attributeDefinitionXhtml);
            specObjectType.SpecAttributes.Add(attributeDefinitionXhtml);

            this.reqIF.CoreContent.SpecTypes.Add(specObjectType);
            specObjectType.ReqIFContent = this.reqIF.CoreContent;
        }

        /// <summary>
        /// create a <see cref="SpecificationType"/> with attribute definitions
        /// </summary>
        private void CreateSpecificationType()
        {
            var reqIfContent = this.reqIF.CoreContent;

            var specificationType = new SpecificationType();
            specificationType.LongName = "Specification Type";
            specificationType.Identifier = "specificationtype";
            specificationType.LastChange = DateTime.Parse("2015-12-01");
            CreateAlternativeId(specificationType);

            CreateAndAddAttributeDefinitionsToSpecType(specificationType, reqIfContent);

            reqIfContent.SpecTypes.Add(specificationType);
            specificationType.ReqIFContent = reqIfContent;
        }

        /// <summary>
        /// create a <see cref="SpecRelationType"/> with attribute definitions
        /// </summary>
        private void CreateSpecRelationType()
        {
            var reqIfContent = this.reqIF.CoreContent;

            var specRelationType = new SpecRelationType();
            specRelationType.LongName = "Specification Relation Type";
            specRelationType.Identifier = "specificationrelation";
            specRelationType.LastChange = DateTime.Parse("2015-12-01");
            CreateAlternativeId(specRelationType);

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            attributeDefinitionBoolean.LongName = "boolean attribute";
            attributeDefinitionBoolean.Identifier = "specificationrelation-boolean-attribute";
            attributeDefinitionBoolean.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionBoolean.Type = (DatatypeDefinitionBoolean)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionBoolean));
            CreateAlternativeId(attributeDefinitionBoolean);
            specRelationType.SpecAttributes.Add(attributeDefinitionBoolean);

            var attributeDefinitionDate = new AttributeDefinitionDate();
            attributeDefinitionDate.LongName = "date attribute";
            attributeDefinitionDate.Identifier = "specificationrelation-date-attribute";
            attributeDefinitionDate.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionDate.Type = (DatatypeDefinitionDate)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionDate));
            CreateAlternativeId(attributeDefinitionDate);
            specRelationType.SpecAttributes.Add(attributeDefinitionDate);

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            attributeDefinitionEnumeration.LongName = "enumeration attribute";
            attributeDefinitionEnumeration.Identifier = "specificationrelation-enumeration-attribute";
            attributeDefinitionEnumeration.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionEnumeration.Type = (DatatypeDefinitionEnumeration)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionEnumeration));
            CreateAlternativeId(attributeDefinitionEnumeration);
            specRelationType.SpecAttributes.Add(attributeDefinitionEnumeration);

            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            attributeDefinitionInteger.LongName = "integer attribute";
            attributeDefinitionInteger.Identifier = "specificationrelation-integer-attribute";
            attributeDefinitionInteger.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionInteger.Type = (DatatypeDefinitionInteger)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionInteger));
            CreateAlternativeId(attributeDefinitionInteger);
            specRelationType.SpecAttributes.Add(attributeDefinitionInteger);

            var attributeDefinitionReal = new AttributeDefinitionReal();
            attributeDefinitionReal.LongName = "real attribute";
            attributeDefinitionReal.Identifier = "specificationrelation-real-attribute";
            attributeDefinitionReal.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionReal.Type = (DatatypeDefinitionReal)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionReal));
            CreateAlternativeId(attributeDefinitionReal);
            specRelationType.SpecAttributes.Add(attributeDefinitionReal);

            var attributeDefinitionString = new AttributeDefinitionString();
            attributeDefinitionString.LongName = "string attribute";
            attributeDefinitionString.Identifier = "specificationrelation-string-attribute";
            attributeDefinitionString.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionString.Type = (DatatypeDefinitionString)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionString));
            CreateAlternativeId(attributeDefinitionString);
            specRelationType.SpecAttributes.Add(attributeDefinitionString);

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            attributeDefinitionXhtml.LongName = "xhtml attribute";
            attributeDefinitionXhtml.Identifier = "specificationrelation-xhtml-attribute";
            attributeDefinitionXhtml.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionXhtml.Type = (DatatypeDefinitionXHTML)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionXHTML));
            CreateAlternativeId(attributeDefinitionXhtml);
            specRelationType.SpecAttributes.Add(attributeDefinitionXhtml);

            reqIfContent.SpecTypes.Add(specRelationType);
            specRelationType.ReqIFContent = reqIfContent;
        }

        /// <summary>
        /// create a <see cref="RelationGroupType"/> with attribute definitions
        /// </summary>
        private void CreateRelationGroupType()
        {
            var relationGroupType = new RelationGroupType();
            relationGroupType.LongName = "Relation Group Type";
            relationGroupType.Identifier = "relationgrouptype";
            relationGroupType.LastChange = DateTime.Parse("2015-12-01");
            CreateAlternativeId(relationGroupType);

            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            attributeDefinitionBoolean.LongName = "boolean attribute";
            attributeDefinitionBoolean.Identifier = "relationgrouptype-boolean-attribute";
            attributeDefinitionBoolean.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionBoolean.Type = (DatatypeDefinitionBoolean)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionBoolean));
            CreateAlternativeId(attributeDefinitionBoolean);
            relationGroupType.SpecAttributes.Add(attributeDefinitionBoolean);

            var attributeDefinitionDate = new AttributeDefinitionDate();
            attributeDefinitionDate.LongName = "date attribute";
            attributeDefinitionDate.Identifier = "relationgrouptype-date-attribute";
            attributeDefinitionDate.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionDate.Type = (DatatypeDefinitionDate)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionDate));
            CreateAlternativeId(attributeDefinitionDate);
            relationGroupType.SpecAttributes.Add(attributeDefinitionDate);

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            attributeDefinitionEnumeration.LongName = "enumeration attribute";
            attributeDefinitionEnumeration.Identifier = "relationgrouptype-enumeration-attribute";
            attributeDefinitionEnumeration.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionEnumeration.Type = (DatatypeDefinitionEnumeration)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionEnumeration));
            CreateAlternativeId(attributeDefinitionEnumeration);
            relationGroupType.SpecAttributes.Add(attributeDefinitionEnumeration);

            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            attributeDefinitionInteger.LongName = "integer attribute";
            attributeDefinitionInteger.Identifier = "relationgrouptype-integer-attribute";
            attributeDefinitionInteger.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionInteger.Type = (DatatypeDefinitionInteger)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionInteger));
            CreateAlternativeId(attributeDefinitionInteger);
            relationGroupType.SpecAttributes.Add(attributeDefinitionInteger);

            var attributeDefinitionReal = new AttributeDefinitionReal();
            attributeDefinitionReal.LongName = "real attribute";
            attributeDefinitionReal.Identifier = "relationgrouptype-real-attribute";
            attributeDefinitionReal.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionReal.Type = (DatatypeDefinitionReal)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionReal));
            CreateAlternativeId(attributeDefinitionReal);
            relationGroupType.SpecAttributes.Add(attributeDefinitionReal);

            var attributeDefinitionString = new AttributeDefinitionString();
            attributeDefinitionString.LongName = "string attribute";
            attributeDefinitionString.Identifier = "relationgrouptype-string-attribute";
            attributeDefinitionString.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionString.Type = (DatatypeDefinitionString)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionString));
            CreateAlternativeId(attributeDefinitionString);
            relationGroupType.SpecAttributes.Add(attributeDefinitionString);

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            attributeDefinitionXhtml.LongName = "xhtml attribute";
            attributeDefinitionXhtml.Identifier = "relationgrouptype-xhtml-attribute";
            attributeDefinitionXhtml.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionXhtml.Type = (DatatypeDefinitionXHTML)this.reqIF.CoreContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionXHTML));
            CreateAlternativeId(attributeDefinitionXhtml);
            relationGroupType.SpecAttributes.Add(attributeDefinitionXhtml);

            this.reqIF.CoreContent.SpecTypes.Add(relationGroupType);
            relationGroupType.ReqIFContent = this.reqIF.CoreContent;
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
        private static void CreateAndAddAttributeDefinitionsToSpecType(SpecType specType, ReqIFContent reqIfContent)
        {
            var attributeDefinitionBoolean = new AttributeDefinitionBoolean();
            attributeDefinitionBoolean.LongName = "boolean attribute";
            attributeDefinitionBoolean.Identifier = "specification-boolean-attribute";
            attributeDefinitionBoolean.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionBoolean.DefaultValue = new AttributeValueBoolean { TheValue = true };
            attributeDefinitionBoolean.Type = (DatatypeDefinitionBoolean)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionBoolean));
            CreateAlternativeId(attributeDefinitionBoolean);
            specType.SpecAttributes.Add(attributeDefinitionBoolean);

            var attributeDefinitionDate = new AttributeDefinitionDate();
            attributeDefinitionDate.LongName = "date attribute";
            attributeDefinitionDate.Identifier = "specification-date-attribute";
            attributeDefinitionDate.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionDate.DefaultValue = new AttributeValueDate { TheValue = DateTime.MaxValue };
            attributeDefinitionDate.Type = (DatatypeDefinitionDate)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionDate));
            CreateAlternativeId(attributeDefinitionDate);
            specType.SpecAttributes.Add(attributeDefinitionDate);

            var attributeDefinitionEnumeration = new AttributeDefinitionEnumeration();
            attributeDefinitionEnumeration.LongName = "enumeration attribute";
            attributeDefinitionEnumeration.Identifier = "specification-enumeration-attribute";
            attributeDefinitionEnumeration.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionEnumeration.Type = (DatatypeDefinitionEnumeration)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionEnumeration));
            CreateAlternativeId(attributeDefinitionEnumeration);
            specType.SpecAttributes.Add(attributeDefinitionEnumeration);

            var attributeDefinitionInteger = new AttributeDefinitionInteger();
            attributeDefinitionInteger.LongName = "integer attribute";
            attributeDefinitionInteger.Identifier = "specification-integer-attribute";
            attributeDefinitionInteger.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionInteger.DefaultValue = new AttributeValueInteger { TheValue = 10 };
            attributeDefinitionInteger.Type = (DatatypeDefinitionInteger)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionInteger));
            CreateAlternativeId(attributeDefinitionInteger);
            specType.SpecAttributes.Add(attributeDefinitionInteger);

            var attributeDefinitionReal = new AttributeDefinitionReal();
            attributeDefinitionReal.LongName = "real attribute";
            attributeDefinitionReal.Identifier = "specification-real-attribute";
            attributeDefinitionReal.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionReal.DefaultValue = new AttributeValueReal { TheValue = 10d };
            attributeDefinitionReal.Type = (DatatypeDefinitionReal)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionReal));
            CreateAlternativeId(attributeDefinitionReal);
            specType.SpecAttributes.Add(attributeDefinitionReal);

            var attributeDefinitionString = new AttributeDefinitionString();
            attributeDefinitionString.LongName = "string attribute";
            attributeDefinitionString.Identifier = "specification-string-attribute";
            attributeDefinitionString.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionString.DefaultValue = new AttributeValueString { TheValue = "default value" };
            attributeDefinitionString.Type = (DatatypeDefinitionString)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionString));
            CreateAlternativeId(attributeDefinitionString);
            specType.SpecAttributes.Add(attributeDefinitionString);

            var attributeDefinitionXhtml = new AttributeDefinitionXHTML();
            attributeDefinitionXhtml.LongName = "xhtml attribute";
            attributeDefinitionXhtml.Identifier = "specification-xhtml-attribute";
            attributeDefinitionXhtml.LastChange = DateTime.Parse("2015-12-01");
            attributeDefinitionXhtml.DefaultValue = new AttributeValueXHTML { TheValue = "xhtml default value" };
            attributeDefinitionXhtml.Type = (DatatypeDefinitionXHTML)reqIfContent.DataTypes.SingleOrDefault(x => x.GetType() == typeof(DatatypeDefinitionXHTML));
            CreateAlternativeId(attributeDefinitionXhtml);
            specType.SpecAttributes.Add(attributeDefinitionXhtml);
        }

        /// <summary>
        /// create a <see cref="SpecObject"/>s with attribute values
        /// </summary>
        private void CreateSpecObjects()
        {
            var reqIfContent = this.reqIF.CoreContent;

            var specObject_1 = new SpecObject();
            specObject_1.LongName = "spec object 1";
            specObject_1.Identifier = this.specobject_1_id;
            specObject_1.LastChange = DateTime.Parse("2015-12-01");
            var specType = (SpecObjectType)reqIfContent.SpecTypes.SingleOrDefault(x => x.GetType() == typeof(SpecObjectType));
            specObject_1.Type = specType;
            this.CreateValuesForSpecElementWithAttributes(specObject_1, specType);
            CreateAlternativeId(specObject_1);
            reqIfContent.SpecObjects.Add(specObject_1);
            specObject_1.ReqIFContent = reqIfContent;

            var specobject_2 = new SpecObject();
            specobject_2.LongName = "spec object 2";
            specobject_2.Identifier = this.specobject_2_id;
            specobject_2.LastChange = DateTime.Parse("2015-12-01");
            specobject_2.Type = specType;
            this.CreateValuesForSpecElementWithAttributes(specobject_2, specType);
            CreateAlternativeId(specobject_2);
            reqIfContent.SpecObjects.Add(specobject_2);
            specobject_2.ReqIFContent = reqIfContent;

            var specobject_3 = new SpecObject();
            specobject_3.LongName = "spec object 3";
            specobject_3.Identifier = this.specobject_3_id;
            specobject_3.LastChange = DateTime.Parse("2015-12-01");
            specobject_3.Type = specType;
            this.CreateValuesForSpecElementWithAttributes(specobject_3, specType);
            CreateAlternativeId(specobject_3);
            reqIfContent.SpecObjects.Add(specobject_3);
            specobject_3.ReqIFContent = reqIfContent;
        }

        /// <summary>
        /// create a <see cref="SpecRelation"/>s
        /// </summary>
        private void CreateSpecRelations()
        {
            var reqIfContent = this.reqIF.CoreContent;

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
            CreateAlternativeId(specRelation);

            reqIfContent.SpecRelations.Add(specRelation);
            specRelation.ReqIFContent = reqIfContent;
        }

        /// <summary>
        /// create a <see cref="Specification"/>s
        /// </summary>
        private void CreateSpecifications()
        {
            var reqIfContent = this.reqIF.CoreContent;
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
            CreateAlternativeId(specification1);

            var specHierarchy1 = new SpecHierarchy();
            specHierarchy1.Identifier = "spec-hierarchy-1";
            specHierarchy1.LastChange = DateTime.Parse("2015-12-01");
            specHierarchy1.LongName = "specHierarchy 1";
            specHierarchy1.Object = object1;
            CreateAlternativeId(specHierarchy1);

            var specHierarchy1_1 = new SpecHierarchy();
            specHierarchy1_1.Identifier = "spec-hierarchy-1-1";
            specHierarchy1_1.LastChange = DateTime.Parse("2015-12-01");
            specHierarchy1_1.LongName = "specHierarchy 1_1";
            specHierarchy1_1.Object = object2;
            CreateAlternativeId(specHierarchy1_1);

            var specHierarchy1_2 = new SpecHierarchy();
            specHierarchy1_2.Identifier = "spec-hierarchy-1-2";
            specHierarchy1_2.LastChange = DateTime.Parse("2015-12-01");
            specHierarchy1_2.LongName = "specHierarchy 1_2";
            specHierarchy1_2.Object = object3;
            CreateAlternativeId(specHierarchy1_2);

            specification1.Children.Add(specHierarchy1);
            specHierarchy1.Children.Add(specHierarchy1_1);
            specHierarchy1.Children.Add(specHierarchy1_2);

            reqIfContent.Specifications.Add(specification1);
            specification1.ReqIFContent = reqIfContent;

            var specification2 = new Specification();
            specification2.Identifier = "specification-2";
            specification2.LastChange = DateTime.Parse("2015-12-01");
            specification2.LongName = "specification 2";
            specification2.Type = specificationType;
            this.CreateValuesForSpecElementWithAttributes(specification2, specificationType);
            CreateAlternativeId(specification2);
            reqIfContent.Specifications.Add(specification2);
            specification2.ReqIFContent = reqIfContent;
        }

        /// <summary>
        /// Creates a <see cref="RelationGroup"/>
        /// </summary>
        private void CreateRelationGroup()
        {
            var reqIfContent = this.reqIF.CoreContent;

            var relationGroupType = (RelationGroupType)reqIfContent.SpecTypes.SingleOrDefault(x => x.GetType() == typeof(RelationGroupType));

            var relationGroup = new RelationGroup();
            relationGroup.Identifier = "relationgroup-1";
            relationGroup.LastChange = DateTime.Parse("2015-12-01");
            relationGroup.LongName = "relationgroup 1";
            relationGroup.Type = relationGroupType;
            CreateAlternativeId(relationGroup);

            var sourceSpecification = reqIfContent.Specifications.SingleOrDefault(x => x.Identifier == "specification-1");
            var targetSpecification = reqIfContent.Specifications.SingleOrDefault(x => x.Identifier == "specification-2");

            relationGroup.SourceSpecification = sourceSpecification;
            relationGroup.TargetSpecification = targetSpecification;

            reqIfContent.SpecRelationGroups.Add(relationGroup);
            relationGroup.ReqIFContent = reqIfContent;
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
            attributeValueDate.TheValue = XmlConvert.ToDateTime("2015-12-01", XmlDateTimeSerializationMode.Utc);
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

            var simplifiedAttributeValueXhtml = new AttributeValueXHTML();
            simplifiedAttributeValueXhtml.Definition = (AttributeDefinitionXHTML)specType.SpecAttributes.SingleOrDefault(x => x.GetType() == typeof(AttributeDefinitionXHTML));
            simplifiedAttributeValueXhtml.IsSimplified = true;
            simplifiedAttributeValueXhtml.TheValue = "simplified content";
            simplifiedAttributeValueXhtml.TheOriginalValue = this.xhtmlcontent;
            specElementWithAttributes.Values.Add(simplifiedAttributeValueXhtml);
        }

        /// <summary>
        /// Create an <see cref="AlternativeId"/> for the <see cref="Identifiable"/>
        /// </summary>
        /// <param name="identifiable">
        /// The <see cref="Identifiable"/> for which an <see cref="AlternativeId"/> needs to be created
        /// </param>
        private static void CreateAlternativeId(Identifiable identifiable)
        {
            var alternativeId = new AlternativeId();
            alternativeId.Identifier = identifiable.Identifier + "_alternative";

            alternativeId.Ident = identifiable;
            identifiable.AlternativeId = alternativeId;
        }
    }
}
