// -------------------------------------------------------------------------------------------------
// <copyright file="CreatedDocumentQueriesTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Extensions.Tests.ReqIFExtensions
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.ReqIFExtensions;

    /// <summary>
    /// Verifies that the query extension methods work on a document built in code with the create methods of the
    /// ReqIFSharp model, without setting any reference by hand
    /// </summary>
    [TestFixture]
    public class CreatedDocumentQueriesTestFixture
    {
        private ReqIFContent content;

        private DatatypeDefinitionString stringType;

        private DatatypeDefinitionXHTML xhtmlType;

        private DatatypeDefinitionEnumeration statusType;

        private EnumValue approved;

        private SpecObjectType requirementType;

        private AttributeDefinitionString idAttribute;

        private AttributeDefinitionXHTML textAttribute;

        private AttributeDefinitionEnumeration statusAttribute;

        private SpecificationType specificationType;

        private SpecRelationType refinesType;

        private RelationGroupType traceType;

        private SpecObject brew;

        private SpecObject heat;

        private SpecObject tank;

        private Specification specification;

        private SpecHierarchy brewNode;

        private SpecRelation relation;

        private RelationGroup relationGroup;

        [SetUp]
        public void SetUp()
        {
            this.content = ReqIF.Create("Coffee machine requirements").CoreContent;

            this.stringType = this.content.CreateDatatypeDefinitionString("String", 255);
            this.xhtmlType = this.content.CreateDatatypeDefinitionXHTML("Formatted text");
            this.statusType = this.content.CreateDatatypeDefinitionEnumeration("Status");
            var draft = this.statusType.CreateEnumValue("Draft", 0, "draft");
            this.approved = this.statusType.CreateEnumValue("Approved", 1, "approved");

            this.requirementType = this.content.CreateSpecObjectType("Requirement");
            this.idAttribute = this.requirementType.CreateAttributeDefinition(this.stringType, "ID");
            this.textAttribute = this.requirementType.CreateAttributeDefinition(this.xhtmlType, "Text");
            this.statusAttribute = this.requirementType.CreateAttributeDefinition(this.statusType, "Status");

            this.specificationType = this.content.CreateSpecificationType("Document");
            this.refinesType = this.content.CreateSpecRelationType("refines");
            this.traceType = this.content.CreateRelationGroupType("trace");

            SpecObject CreateRequirement(string id, EnumValue status)
            {
                var requirement = this.content.CreateSpecObject(this.requirementType, id);
                requirement.CreateAttributeValue(this.idAttribute, id);
                requirement.CreateAttributeValue(this.textAttribute, $"<xhtml:div>The text of {id}</xhtml:div>");
                requirement.CreateAttributeValue(this.statusAttribute, status);
                return requirement;
            }

            this.brew = CreateRequirement("REQ-1", this.approved);
            this.heat = CreateRequirement("REQ-2", draft);
            this.tank = CreateRequirement("REQ-3", draft);

            this.specification = this.content.CreateSpecification(this.specificationType, "Coffee machine");
            this.brewNode = this.specification.AddChild(this.brew);
            this.brewNode.AddChild(this.heat);
            this.specification.AddChild(this.tank);

            this.relation = this.content.CreateSpecRelation(this.refinesType, this.heat, this.brew);

            this.relationGroup = this.content.CreateRelationGroup(this.traceType, this.specification, this.specification, "Trace");
            this.relationGroup.SpecRelations.Add(this.relation);
        }

        [Test]
        public void Verify_that_the_type_queries_find_the_elements_of_each_type()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.requirementType.QueryReferencingSpecObjects(), Is.EquivalentTo(new[] { this.brew, this.heat, this.tank }));
                Assert.That(this.specificationType.QueryReferencingSpecifications(), Is.EquivalentTo(new[] { this.specification }));
                Assert.That(this.refinesType.QueryReferencingSpecRelations(), Is.EquivalentTo(new[] { this.relation }));
                Assert.That(this.traceType.QueryReferencingRelationGroups(), Is.EquivalentTo(new[] { this.relationGroup }));
            });
        }

        [Test]
        public void Verify_that_the_datatype_queries_work()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.stringType.QueryReferencingAttributeDefinitions(), Is.EquivalentTo(new[] { this.idAttribute }));
                Assert.That(this.statusType.QueryReferencingAttributeDefinitions(), Is.EquivalentTo(new[] { this.statusAttribute }));
                Assert.That(this.stringType.QueryDatatypeName(), Is.EqualTo("String"));
                Assert.That(this.idAttribute.QueryDatatypeName(), Is.EqualTo("String"));
                Assert.That(this.statusAttribute.QueryDatatypeName(), Is.EqualTo("Enumeration"));
                Assert.That(this.idAttribute.QueryDefaultValueAsFormattedString(), Is.EqualTo("NOT SET"));
            });
        }

        [Test]
        public void Verify_that_the_spec_object_queries_work()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.heat.QuerySpecRelations(), Is.EquivalentTo(new[] { this.relation }));
                Assert.That(this.brew.QuerySpecRelations(), Is.EquivalentTo(new[] { this.relation }));
                Assert.That(this.tank.QuerySpecRelations(), Is.Empty);
                Assert.That(this.heat.QueryContainerSpecifications(), Is.EquivalentTo(new[] { this.specification }));
            });
        }

        [Test]
        public void Verify_that_the_specification_queries_work()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.specification.QueryAllContainedSpecObjects(), Is.EquivalentTo(new[] { this.brew, this.heat, this.tank }));
                Assert.That(this.specification.QueryAllContainedSpecHierarchies().Select(x => x.Object), Is.EquivalentTo(new[] { this.brew, this.heat, this.tank }));
                Assert.That(this.brewNode.QueryAllContainedSpecHierarchies().Select(x => x.Object), Is.EquivalentTo(new[] { this.heat }));
                Assert.That(this.specification.QueryAttributeDefinitions(), Is.EquivalentTo(new AttributeDefinition[] { this.idAttribute, this.textAttribute, this.statusAttribute }));
            });
        }

        [Test]
        public void Verify_that_the_value_queries_work()
        {
            var values = this.brew.Values;

            Assert.Multiple(() =>
            {
                Assert.That(values.Select(x => x.QueryFormattedValue()), Is.EqualTo(new[] { "REQ-1", "<xhtml:div xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">The text of REQ-1</xhtml:div>", "approved" }));
                Assert.That(this.brew.QueryExternalObjects(), Is.Empty);
                Assert.That(this.content.QueryExternalObjects(), Is.Empty);
                Assert.That(this.brew.QueryTypeDisplayName(), Is.EqualTo("Spec Object"));
            });
        }

        [Test]
        public void Verify_that_QueryExternalObjects_finds_an_object_in_a_created_xhtml_value()
        {
            var pump = this.content.CreateSpecObject(this.requirementType, "REQ-4");
            pump.CreateAttributeValue(this.textAttribute, "<xhtml:div>The pump <xhtml:object data=\"files/pump.png\" type=\"image/png\"/></xhtml:div>");

            Assert.Multiple(() =>
            {
                Assert.That(pump.QueryExternalObjects().Select(x => x.Uri), Is.EqualTo(new[] { "files/pump.png" }));
                Assert.That(this.content.QueryExternalObjects().Select(x => x.Uri), Is.EqualTo(new[] { "files/pump.png" }));
            });
        }
    }
}
