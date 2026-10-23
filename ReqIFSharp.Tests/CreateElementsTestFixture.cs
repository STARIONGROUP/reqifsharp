// -------------------------------------------------------------------------------------------------
// <copyright file="CreateElementsTestFixture.cs" company="Starion Group S.A.">
//
//   SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Verifies the methods that create elements of a <see cref="ReqIF"/> document in code: each element gets a valid
    /// identifier, its <see cref="Identifiable.LastChange"/> and its back-references, and a document built with them is
    /// valid against the ReqIF schema.
    /// </summary>
    [TestFixture]
    public class CreateElementsTestFixture
    {
        private DateTime before;

        private ReqIF reqif;

        private ReqIFContent content;

        [SetUp]
        public void SetUp()
        {
            this.before = DateTime.UtcNow;
            this.reqif = ReqIF.Create("a title");
            this.content = this.reqif.CoreContent;
        }

        /// <summary>
        /// Asserts that the element was given a new identifier that is a valid xsd:ID, the long name and a last change
        /// between the start of the test and now
        /// </summary>
        private void AssertCreated(Identifiable identifiable, string longName)
        {
            Assert.Multiple(() =>
            {
                Assert.That(identifiable.Identifier, Does.StartWith("_"));
                Assert.That(() => XmlConvert.VerifyNCName(identifiable.Identifier), Throws.Nothing);
                Assert.That(identifiable.LongName, Is.EqualTo(longName));
                Assert.That(identifiable.LastChange, Is.InRange(this.before, DateTime.UtcNow));
                Assert.That(identifiable.LastChange.Kind, Is.EqualTo(DateTimeKind.Utc));
            });
        }

        [Test]
        public void Verify_that_CreateIdentifier_returns_a_unique_valid_xsd_ID()
        {
            var identifiers = Enumerable.Range(0, 100).Select(_ => Identifiable.CreateIdentifier()).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(identifiers, Is.Unique);
                Assert.That(identifiers, Has.All.StartWith("_"));
                Assert.That(() => identifiers.ForEach(x => XmlConvert.VerifyNCName(x)), Throws.Nothing);
            });
        }

        [Test]
        public void Verify_that_MarkAsChanged_sets_LastChange_to_now()
        {
            var specObject = new SpecObject { LastChange = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc) };

            var start = DateTime.UtcNow;
            specObject.MarkAsChanged();

            Assert.That(specObject.LastChange, Is.InRange(start, DateTime.UtcNow));
        }

        [Test]
        public void Verify_that_Create_returns_a_document_with_a_header_and_a_content()
        {
            var header = this.reqif.TheHeader;

            Assert.Multiple(() =>
            {
                Assert.That(header.Identifier, Does.StartWith("_"));
                Assert.That(header.Title, Is.EqualTo("a title"));
                Assert.That(header.CreationTime, Is.InRange(this.before, DateTime.UtcNow));
                Assert.That(header.CreationTime.Kind, Is.EqualTo(DateTimeKind.Utc));
                Assert.That(header.ReqIFVersion, Is.EqualTo("1.0"));
                Assert.That(header.ReqIFToolId, Is.EqualTo("ReqIFSharp"));
                Assert.That(header.SourceToolId, Is.EqualTo("ReqIFSharp"));
                Assert.That(header.DocumentRoot, Is.SameAs(this.reqif));
                Assert.That(this.content, Is.Not.Null);
                Assert.That(this.content.DocumentRoot, Is.SameAs(this.reqif));
            });
        }

        [Test]
        public void Verify_that_the_datatype_definitions_are_created_in_the_content()
        {
            var datatypes = new DatatypeDefinition[]
            {
                this.content.CreateDatatypeDefinitionBoolean("boolean"),
                this.content.CreateDatatypeDefinitionDate("date"),
                this.content.CreateDatatypeDefinitionEnumeration("enumeration"),
                this.content.CreateDatatypeDefinitionInteger("integer", -10, 10),
                this.content.CreateDatatypeDefinitionReal("real", 2, -1.5, 1.5),
                this.content.CreateDatatypeDefinitionString("string", 255),
                this.content.CreateDatatypeDefinitionXHTML("xhtml")
            };

            Assert.That(this.content.DataTypes, Is.EqualTo(datatypes));

            foreach (var datatype in datatypes)
            {
                Assert.That(datatype.ReqIFContent, Is.SameAs(this.content));
            }

            var names = new[] { "boolean", "date", "enumeration", "integer", "real", "string", "xhtml" };

            for (var i = 0; i < datatypes.Length; i++)
            {
                this.AssertCreated(datatypes[i], names[i]);
            }

            var integer = (DatatypeDefinitionInteger)datatypes[3];
            var real = (DatatypeDefinitionReal)datatypes[4];
            var text = (DatatypeDefinitionString)datatypes[5];

            Assert.Multiple(() =>
            {
                Assert.That(integer.Min, Is.EqualTo(-10));
                Assert.That(integer.Max, Is.EqualTo(10));
                Assert.That(real.Accuracy, Is.EqualTo(2));
                Assert.That(real.Min, Is.EqualTo(-1.5));
                Assert.That(real.Max, Is.EqualTo(1.5));
                Assert.That(text.MaxLength, Is.EqualTo(255));
            });
        }

        [Test]
        public void Verify_that_invalid_datatype_limits_throw()
        {
            Assert.Multiple(() =>
            {
                Assert.That(() => this.content.CreateDatatypeDefinitionInteger("integer", 1, 0), Throws.ArgumentException.With.Property("ParamName").EqualTo("min"));
                Assert.That(() => this.content.CreateDatatypeDefinitionReal("real", -1, 0, 1), Throws.ArgumentException.With.Property("ParamName").EqualTo("accuracy"));
                Assert.That(() => this.content.CreateDatatypeDefinitionReal("real", 2, 1, 0), Throws.ArgumentException.With.Property("ParamName").EqualTo("min"));
                Assert.That(() => this.content.CreateDatatypeDefinitionString("string", -1), Throws.ArgumentException.With.Property("ParamName").EqualTo("maxLength"));
                Assert.That(this.content.DataTypes, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_CreateEnumValue_adds_a_literal_with_its_properties()
        {
            var enumeration = this.content.CreateDatatypeDefinitionEnumeration("status");

            var draft = enumeration.CreateEnumValue("draft", 0);
            var approved = enumeration.CreateEnumValue("approved", 1, "green");

            Assert.That(enumeration.SpecifiedValues, Is.EqualTo(new[] { draft, approved }));
            this.AssertCreated(approved, "approved");

            Assert.Multiple(() =>
            {
                Assert.That(approved.DataTpeDefEnum, Is.SameAs(enumeration));
                Assert.That(approved.Properties.Key, Is.EqualTo(1));
                Assert.That(approved.Properties.OtherContent, Is.EqualTo("green"));
                Assert.That(approved.Properties.EnumValue, Is.SameAs(approved));
                Assert.That(draft.Properties.OtherContent, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_the_spec_types_are_created_in_the_content()
        {
            var specTypes = new SpecType[]
            {
                this.content.CreateSpecObjectType("requirement"),
                this.content.CreateSpecificationType("document"),
                this.content.CreateSpecRelationType("refines"),
                this.content.CreateRelationGroupType("trace")
            };

            Assert.That(this.content.SpecTypes, Is.EqualTo(specTypes));

            foreach (var specType in specTypes)
            {
                Assert.That(specType.ReqIFContent, Is.SameAs(this.content));
            }

            this.AssertCreated(specTypes[0], "requirement");
            this.AssertCreated(specTypes[1], "document");
            this.AssertCreated(specTypes[2], "refines");
            this.AssertCreated(specTypes[3], "trace");
        }

        [Test]
        public void Verify_that_the_attribute_definitions_are_created_in_the_spec_type()
        {
            var specType = this.content.CreateSpecObjectType("requirement");

            var enumeration = this.content.CreateDatatypeDefinitionEnumeration("status");

            var datatypes = new DatatypeDefinition[]
            {
                this.content.CreateDatatypeDefinitionBoolean("boolean"),
                this.content.CreateDatatypeDefinitionDate("date"),
                enumeration,
                this.content.CreateDatatypeDefinitionInteger("integer", 0, 10),
                this.content.CreateDatatypeDefinitionReal("real", 2, 0, 1),
                this.content.CreateDatatypeDefinitionString("string", 10),
                this.content.CreateDatatypeDefinitionXHTML("xhtml")
            };

            var definitions = new AttributeDefinition[]
            {
                specType.CreateAttributeDefinition((DatatypeDefinitionBoolean)datatypes[0], "boolean attribute"),
                specType.CreateAttributeDefinition((DatatypeDefinitionDate)datatypes[1], "date attribute"),
                specType.CreateAttributeDefinition(enumeration, "enumeration attribute", true),
                specType.CreateAttributeDefinition((DatatypeDefinitionInteger)datatypes[3], "integer attribute"),
                specType.CreateAttributeDefinition((DatatypeDefinitionReal)datatypes[4], "real attribute"),
                specType.CreateAttributeDefinition((DatatypeDefinitionString)datatypes[5], "string attribute"),
                specType.CreateAttributeDefinition((DatatypeDefinitionXHTML)datatypes[6], "xhtml attribute")
            };

            Assert.That(specType.SpecAttributes, Is.EqualTo(definitions));

            Assert.Multiple(() =>
            {
                for (var i = 0; i < definitions.Length; i++)
                {
                    Assert.That(definitions[i].SpecType, Is.SameAs(specType));
                    Assert.That(definitions[i].DatatypeDefinition, Is.SameAs(datatypes[i]));
                }
            });

            var names = new[] { "boolean attribute", "date attribute", "enumeration attribute", "integer attribute", "real attribute", "string attribute", "xhtml attribute" };

            for (var i = 0; i < definitions.Length; i++)
            {
                this.AssertCreated(definitions[i], names[i]);
            }

            Assert.Multiple(() =>
            {
                Assert.That(((AttributeDefinitionEnumeration)definitions[2]).IsMultiValued, Is.True);
                Assert.That(specType.CreateAttributeDefinition(enumeration, "single").IsMultiValued, Is.False);
                Assert.That(() => specType.CreateAttributeDefinition((DatatypeDefinitionString)null, "string"), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void Verify_that_the_spec_elements_are_created_in_the_content()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var document = this.content.CreateSpecificationType("document");
            var refines = this.content.CreateSpecRelationType("refines");
            var trace = this.content.CreateRelationGroupType("trace");

            var source = this.content.CreateSpecObject(requirement, "source");
            var target = this.content.CreateSpecObject(requirement);
            var specification = this.content.CreateSpecification(document, "specification");
            var relation = this.content.CreateSpecRelation(refines, source, target, "relation");
            var relationGroup = this.content.CreateRelationGroup(trace, specification, specification, "group");

            Assert.Multiple(() =>
            {
                Assert.That(this.content.SpecObjects, Is.EqualTo(new[] { source, target }));
                Assert.That(this.content.Specifications, Is.EqualTo(new[] { specification }));
                Assert.That(this.content.SpecRelations, Is.EqualTo(new[] { relation }));
                Assert.That(this.content.SpecRelationGroups, Is.EqualTo(new[] { relationGroup }));

                Assert.That(source.Type, Is.SameAs(requirement));
                Assert.That(source.ReqIFContent, Is.SameAs(this.content));
                Assert.That(target.LongName, Is.Null);
                Assert.That(specification.Type, Is.SameAs(document));
                Assert.That(specification.ReqIFContent, Is.SameAs(this.content));
                Assert.That(relation.Type, Is.SameAs(refines));
                Assert.That(relation.Source, Is.SameAs(source));
                Assert.That(relation.Target, Is.SameAs(target));
                Assert.That(relation.ReqIFContent, Is.SameAs(this.content));
                Assert.That(relationGroup.Type, Is.SameAs(trace));
                Assert.That(relationGroup.SourceSpecification, Is.SameAs(specification));
                Assert.That(relationGroup.TargetSpecification, Is.SameAs(specification));
                Assert.That(relationGroup.CoreContent, Is.SameAs(this.content));
            });

            this.AssertCreated(source, "source");
            this.AssertCreated(specification, "specification");
            this.AssertCreated(relation, "relation");
            this.AssertCreated(relationGroup, "group");
        }

        [Test]
        public void Verify_that_creating_a_spec_element_without_its_type_or_ends_throws()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var specObject = this.content.CreateSpecObject(requirement);
            var specification = this.content.CreateSpecification(this.content.CreateSpecificationType("document"));
            var refines = this.content.CreateSpecRelationType("refines");
            var trace = this.content.CreateRelationGroupType("trace");

            Assert.Multiple(() =>
            {
                Assert.That(() => this.content.CreateSpecObject(null), Throws.ArgumentNullException);
                Assert.That(() => this.content.CreateSpecification(null), Throws.ArgumentNullException);
                Assert.That(() => this.content.CreateSpecRelation(null, specObject, specObject), Throws.ArgumentNullException);
                Assert.That(() => this.content.CreateSpecRelation(refines, null, specObject), Throws.ArgumentNullException);
                Assert.That(() => this.content.CreateSpecRelation(refines, specObject, null), Throws.ArgumentNullException);
                Assert.That(() => this.content.CreateRelationGroup(null, specification, specification, "group"), Throws.ArgumentNullException);
                Assert.That(() => this.content.CreateRelationGroup(trace, null, specification, "group"), Throws.ArgumentNullException);
                Assert.That(() => this.content.CreateRelationGroup(trace, specification, null, "group"), Throws.ArgumentNullException);

                Assert.That(this.content.SpecObjects, Has.Count.EqualTo(1));
                Assert.That(this.content.Specifications, Has.Count.EqualTo(1));
                Assert.That(this.content.SpecRelations, Is.Empty);
                Assert.That(this.content.SpecRelationGroups, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_AddChild_builds_the_spec_hierarchy()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var specification = this.content.CreateSpecification(this.content.CreateSpecificationType("document"));

            var parentObject = this.content.CreateSpecObject(requirement);
            var childObject = this.content.CreateSpecObject(requirement);

            var parent = specification.AddChild(parentObject);
            var child = parent.AddChild(childObject);

            Assert.Multiple(() =>
            {
                Assert.That(specification.Children, Is.EqualTo(new[] { parent }));
                Assert.That(parent.Children, Is.EqualTo(new[] { child }));

                Assert.That(parent.Object, Is.SameAs(parentObject));
                Assert.That(parent.Root, Is.SameAs(specification));
                Assert.That(parent.Container, Is.Null);
                Assert.That(parent.ReqIfContent, Is.SameAs(this.content));

                Assert.That(child.Object, Is.SameAs(childObject));
                Assert.That(child.Root, Is.SameAs(specification));
                Assert.That(child.Container, Is.SameAs(parent));
                Assert.That(child.ReqIfContent, Is.SameAs(this.content));

                Assert.That(() => specification.AddChild(null), Throws.ArgumentNullException);
                Assert.That(() => parent.AddChild(null), Throws.ArgumentNullException);
            });

            this.AssertCreated(parent, null);
            this.AssertCreated(child, null);
        }

        [Test]
        public void Verify_that_the_attribute_values_are_created_in_the_spec_element()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");

            var enumeration = this.content.CreateDatatypeDefinitionEnumeration("status");
            var draft = enumeration.CreateEnumValue("draft", 0);
            var approved = enumeration.CreateEnumValue("approved", 1);

            var booleanDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionBoolean("boolean"), "boolean");
            var dateDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionDate("date"), "date");
            var enumerationDefinition = requirement.CreateAttributeDefinition(enumeration, "status", true);
            var integerDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionInteger("integer", 0, 10), "integer");
            var realDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionReal("real", 2, 0, 10), "real");
            var stringDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionString("string", 10), "string");
            var xhtmlDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionXHTML("xhtml"), "xhtml");

            var specObject = this.content.CreateSpecObject(requirement);
            var date = new DateTime(2026, 10, 22, 12, 0, 0, DateTimeKind.Utc);

            var values = new AttributeValue[]
            {
                specObject.CreateAttributeValue(booleanDefinition, true),
                specObject.CreateAttributeValue(dateDefinition, date),
                specObject.CreateAttributeValue(enumerationDefinition, draft, approved),
                specObject.CreateAttributeValue(integerDefinition, 7),
                specObject.CreateAttributeValue(realDefinition, 1.25),
                specObject.CreateAttributeValue(stringDefinition, "text"),
                specObject.CreateAttributeValue(xhtmlDefinition, "<xhtml:div>rich text</xhtml:div>")
            };

            Assert.That(specObject.Values, Is.EqualTo(values));

            Assert.Multiple(() =>
            {
                foreach (var value in values)
                {
                    Assert.That(value.SpecElAt, Is.SameAs(specObject));
                    Assert.That(value.AttributeDefinition, Is.SameAs(requirement.SpecAttributes[Array.IndexOf(values, value)]));
                }

                Assert.That(((AttributeValueBoolean)values[0]).TheValue, Is.True);
                Assert.That(((AttributeValueDate)values[1]).TheValue, Is.EqualTo(date));
                Assert.That(((AttributeValueEnumeration)values[2]).Values, Is.EqualTo(new[] { draft, approved }));
                Assert.That(((AttributeValueInteger)values[3]).TheValue, Is.EqualTo(7));
                Assert.That(((AttributeValueReal)values[4]).TheValue, Is.EqualTo(1.25));
                Assert.That(((AttributeValueString)values[5]).TheValue, Is.EqualTo("text"));
                Assert.That(((AttributeValueXHTML)values[6]).TheValue, Is.EqualTo("<xhtml:div xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">rich text</xhtml:div>"));
            });
        }

        [Test]
        public void Verify_that_an_attribute_value_that_does_not_fit_the_element_throws()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var other = this.content.CreateSpecObjectType("other");

            var enumeration = this.content.CreateDatatypeDefinitionEnumeration("status");
            var draft = enumeration.CreateEnumValue("draft", 0);
            var approved = enumeration.CreateEnumValue("approved", 1);
            var foreign = this.content.CreateDatatypeDefinitionEnumeration("other").CreateEnumValue("foreign", 0);

            var stringType = this.content.CreateDatatypeDefinitionString("string", 10);
            var stringDefinition = requirement.CreateAttributeDefinition(stringType, "string");
            var otherDefinition = other.CreateAttributeDefinition(stringType, "other string");
            var singleDefinition = requirement.CreateAttributeDefinition(enumeration, "status");
            var xhtmlDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionXHTML("xhtml"), "xhtml");

            var specObject = this.content.CreateSpecObject(requirement);
            specObject.CreateAttributeValue(stringDefinition, "first");

            Assert.Multiple(() =>
            {
                Assert.That(() => specObject.CreateAttributeValue((AttributeDefinitionString)null, "text"), Throws.ArgumentNullException);
                Assert.That(() => specObject.CreateAttributeValue(otherDefinition, "text"), Throws.ArgumentException.With.Message.Contains("is not an attribute of the spec type"));
                Assert.That(() => specObject.CreateAttributeValue(stringDefinition, "second"), Throws.ArgumentException.With.Message.Contains("already has a value"));
                Assert.That(() => specObject.CreateAttributeValue(singleDefinition, draft, approved), Throws.ArgumentException.With.Message.Contains("is not multi-valued"));
                Assert.That(() => specObject.CreateAttributeValue(singleDefinition, foreign), Throws.ArgumentException.With.Message.Contains("is not a literal"));
                Assert.That(() => specObject.CreateAttributeValue(singleDefinition, (EnumValue)null), Throws.ArgumentException.With.Property("ParamName").EqualTo("values"));
                Assert.That(() => specObject.CreateAttributeValue(singleDefinition, (EnumValue[])null), Throws.ArgumentNullException);
                Assert.That(() => specObject.CreateAttributeValue(xhtmlDefinition, null), Throws.ArgumentNullException);

                Assert.That(specObject.Values, Has.Count.EqualTo(1));
            });

            var noValue = specObject.CreateAttributeValue(singleDefinition);
            Assert.That(noValue.Values, Is.Empty);
        }

        [Test]
        public void Verify_that_an_attribute_value_for_an_element_without_a_spec_type_throws()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var definition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionString("string", 10), "string");

            SpecElementWithAttributes[] elements =
            [
                new SpecObject { Identifier = "spec-object" },
                new Specification { Identifier = "specification" },
                new SpecRelation { Identifier = "spec-relation" },
                new RelationGroup { Identifier = "relation-group" }
            ];

            Assert.Multiple(() =>
            {
                foreach (var element in elements)
                {
                    Assert.That(() => element.CreateAttributeValue(definition, "text"), Throws.InvalidOperationException.With.Message.Contains("has no spec type"), element.Identifier);
                    Assert.That(element.Values, Is.Empty, element.Identifier);
                }
            });
        }

        [Test]
        public void Verify_that_an_attribute_value_for_a_definition_without_a_datatype_throws_for_each_kind()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");

            var booleanDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionBoolean("boolean"), "boolean");
            var dateDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionDate("date"), "date");
            var enumeration = this.content.CreateDatatypeDefinitionEnumeration("status");
            var draft = enumeration.CreateEnumValue("draft", 0);
            var enumerationDefinition = requirement.CreateAttributeDefinition(enumeration, "status");
            var integerDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionInteger("integer", 0, 10), "integer");
            var realDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionReal("real", 2, 0, 10), "real");
            var stringDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionString("string", 10), "string");
            var xhtmlDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionXHTML("xhtml"), "xhtml");

            booleanDefinition.Type = null;
            dateDefinition.Type = null;
            enumerationDefinition.Type = null;
            integerDefinition.Type = null;
            realDefinition.Type = null;
            stringDefinition.Type = null;
            xhtmlDefinition.Type = null;

            var specObject = this.content.CreateSpecObject(requirement);

            var message = "has no datatype";

            Assert.Multiple(() =>
            {
                Assert.That(() => specObject.CreateAttributeValue(booleanDefinition, true), Throws.ArgumentException.With.Message.Contains(message));
                Assert.That(() => specObject.CreateAttributeValue(dateDefinition, DateTime.UtcNow), Throws.ArgumentException.With.Message.Contains(message));
                Assert.That(() => specObject.CreateAttributeValue(enumerationDefinition, draft), Throws.ArgumentException.With.Message.Contains(message));
                Assert.That(() => specObject.CreateAttributeValue(integerDefinition, 5), Throws.ArgumentException.With.Message.Contains(message));
                Assert.That(() => specObject.CreateAttributeValue(realDefinition, 5.0), Throws.ArgumentException.With.Message.Contains(message));
                Assert.That(() => specObject.CreateAttributeValue(stringDefinition, "text"), Throws.ArgumentException.With.Message.Contains(message));
                Assert.That(() => specObject.CreateAttributeValue(xhtmlDefinition, "<xhtml:div>text</xhtml:div>"), Throws.ArgumentException.With.Message.Contains(message));

                Assert.That(specObject.Values, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_equal_and_zero_datatype_limits_are_accepted()
        {
            var integer = this.content.CreateDatatypeDefinitionInteger("integer", 5, 5);
            var real = this.content.CreateDatatypeDefinitionReal("real", 0, 1.5, 1.5);
            var text = this.content.CreateDatatypeDefinitionString("string", 0);

            Assert.Multiple(() =>
            {
                Assert.That(integer.Min, Is.EqualTo(5));
                Assert.That(integer.Max, Is.EqualTo(5));
                Assert.That(real.Accuracy, Is.EqualTo(0));
                Assert.That(real.Min, Is.EqualTo(1.5));
                Assert.That(real.Max, Is.EqualTo(1.5));
                Assert.That(text.MaxLength, Is.EqualTo(0));
                Assert.That(this.content.DataTypes, Is.EqualTo(new DatatypeDefinition[] { integer, real, text }));
            });
        }

        [Test]
        public void Verify_that_CreateAttributeDefinition_throws_when_the_datatype_is_null()
        {
            var specType = this.content.CreateSpecObjectType("requirement");

            Assert.Multiple(() =>
            {
                Assert.That(() => specType.CreateAttributeDefinition((DatatypeDefinitionBoolean)null, "boolean"), Throws.ArgumentNullException);
                Assert.That(() => specType.CreateAttributeDefinition((DatatypeDefinitionDate)null, "date"), Throws.ArgumentNullException);
                Assert.That(() => specType.CreateAttributeDefinition((DatatypeDefinitionEnumeration)null, "enumeration"), Throws.ArgumentNullException);
                Assert.That(() => specType.CreateAttributeDefinition((DatatypeDefinitionInteger)null, "integer"), Throws.ArgumentNullException);
                Assert.That(() => specType.CreateAttributeDefinition((DatatypeDefinitionReal)null, "real"), Throws.ArgumentNullException);
                Assert.That(() => specType.CreateAttributeDefinition((DatatypeDefinitionString)null, "string"), Throws.ArgumentNullException);
                Assert.That(() => specType.CreateAttributeDefinition((DatatypeDefinitionXHTML)null, "xhtml"), Throws.ArgumentNullException);
                Assert.That(specType.SpecAttributes, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_a_second_value_for_the_same_definition_throws_for_each_kind()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var enumeration = this.content.CreateDatatypeDefinitionEnumeration("status");
            var draft = enumeration.CreateEnumValue("draft", 0);

            var booleanDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionBoolean("boolean"), "boolean");
            var dateDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionDate("date"), "date");
            var enumerationDefinition = requirement.CreateAttributeDefinition(enumeration, "status");
            var integerDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionInteger("integer", 0, 10), "integer");
            var realDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionReal("real", 2, 0, 10), "real");
            var xhtmlDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionXHTML("xhtml"), "xhtml");

            var specObject = this.content.CreateSpecObject(requirement);
            specObject.CreateAttributeValue(booleanDefinition, true);
            specObject.CreateAttributeValue(dateDefinition, DateTime.UtcNow);
            specObject.CreateAttributeValue(enumerationDefinition, draft);
            specObject.CreateAttributeValue(integerDefinition, 1);
            specObject.CreateAttributeValue(realDefinition, 1.0);
            specObject.CreateAttributeValue(xhtmlDefinition, "<xhtml:div>text</xhtml:div>");

            Assert.Multiple(() =>
            {
                Assert.That(() => specObject.CreateAttributeValue(booleanDefinition, false), Throws.ArgumentException.With.Message.Contains("already has a value"));
                Assert.That(() => specObject.CreateAttributeValue(dateDefinition, DateTime.UtcNow), Throws.ArgumentException.With.Message.Contains("already has a value"));
                Assert.That(() => specObject.CreateAttributeValue(enumerationDefinition, draft), Throws.ArgumentException.With.Message.Contains("already has a value"));
                Assert.That(() => specObject.CreateAttributeValue(integerDefinition, 2), Throws.ArgumentException.With.Message.Contains("already has a value"));
                Assert.That(() => specObject.CreateAttributeValue(realDefinition, 2.0), Throws.ArgumentException.With.Message.Contains("already has a value"));
                Assert.That(() => specObject.CreateAttributeValue(xhtmlDefinition, "<xhtml:div>other</xhtml:div>"), Throws.ArgumentException.With.Message.Contains("already has a value"));
                Assert.That(specObject.Values, Has.Count.EqualTo(6));
            });
        }

        [Test]
        public void Verify_that_a_value_for_an_attribute_of_another_type_throws_on_every_kind_of_spec_element()
        {
            var stringType = this.content.CreateDatatypeDefinitionString("string", 10);
            var foreignDefinition = this.content.CreateSpecObjectType("requirement").CreateAttributeDefinition(stringType, "foreign");

            var specificationType = this.content.CreateSpecificationType("document");
            var relationType = this.content.CreateSpecRelationType("refines");
            var groupType = this.content.CreateRelationGroupType("trace");
            specificationType.CreateAttributeDefinition(stringType, "title");
            relationType.CreateAttributeDefinition(stringType, "rationale");
            groupType.CreateAttributeDefinition(stringType, "comment");

            var specObject = this.content.CreateSpecObject(this.content.CreateSpecObjectType("other"));
            var specification = this.content.CreateSpecification(specificationType);
            var relation = this.content.CreateSpecRelation(relationType, specObject, specObject);
            var group = this.content.CreateRelationGroup(groupType, specification, specification, "group");

            Assert.Multiple(() =>
            {
                foreach (var element in new SpecElementWithAttributes[] { specification, relation, group })
                {
                    Assert.That(() => element.CreateAttributeValue(foreignDefinition, "text"), Throws.ArgumentException.With.Message.Contains("is not an attribute of the spec type"));
                    Assert.That(element.Values, Is.Empty);
                }
            });
        }

        [Test]
        public void Verify_that_the_logger_factory_of_the_document_is_passed_to_the_created_elements()
        {
            var loggerFactory = new RecordingLoggerFactory();
            var document = ReqIF.Create("a title", loggerFactory);
            var documentContent = document.CoreContent;

            var stringType = documentContent.CreateDatatypeDefinitionString("string", 10);
            var enumeration = documentContent.CreateDatatypeDefinitionEnumeration("status");
            var draft = enumeration.CreateEnumValue("draft", 0);
            var requirement = documentContent.CreateSpecObjectType("requirement");
            var idAttribute = requirement.CreateAttributeDefinition(stringType, "id");
            var statusAttribute = requirement.CreateAttributeDefinition(enumeration, "status");
            var specObject = documentContent.CreateSpecObject(requirement);
            specObject.CreateAttributeValue(idAttribute, "REQ-1");
            specObject.CreateAttributeValue(statusAttribute, draft);
            var specification = documentContent.CreateSpecification(documentContent.CreateSpecificationType("document"));
            specification.AddChild(specObject).AddChild(specObject);
            documentContent.CreateSpecRelation(documentContent.CreateSpecRelationType("refines"), specObject, specObject);
            documentContent.CreateRelationGroup(documentContent.CreateRelationGroupType("trace"), specification, specification, "group");

            var expected = new[]
            {
                typeof(ReqIF), typeof(ReqIFHeader), typeof(ReqIFContent), typeof(DatatypeDefinitionString),
                typeof(DatatypeDefinitionEnumeration), typeof(EnumValue), typeof(EmbeddedValue), typeof(SpecType),
                typeof(AttributeDefinitionString), typeof(AttributeDefinitionEnumeration), typeof(SpecObject),
                typeof(AttributeValueString), typeof(AttributeValueEnumeration), typeof(Specification),
                typeof(SpecHierarchy), typeof(SpecRelation), typeof(RelationGroup)
            };

            Assert.That(loggerFactory.Categories, Is.SupersetOf(expected.Select(x => x.FullName)));
        }

        /// <summary>
        /// An <see cref="Microsoft.Extensions.Logging.ILoggerFactory"/> that records the categories of the loggers it creates
        /// </summary>
        private sealed class RecordingLoggerFactory : Microsoft.Extensions.Logging.ILoggerFactory
        {
            public HashSet<string> Categories { get; } = new HashSet<string>();

            public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
            {
                this.Categories.Add(categoryName);
                return Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
            }

            public void AddProvider(Microsoft.Extensions.Logging.ILoggerProvider provider)
            {
            }

            public void Dispose()
            {
            }
        }

        [Test]
        public void Verify_that_a_missing_long_name_throws_where_the_specification_makes_it_mandatory()
        {
            var specType = this.content.CreateSpecObjectType("requirement");
            var enumeration = this.content.CreateDatatypeDefinitionEnumeration("status");
            var booleanType = this.content.CreateDatatypeDefinitionBoolean("boolean");
            var dateType = this.content.CreateDatatypeDefinitionDate("date");
            var integerType = this.content.CreateDatatypeDefinitionInteger("integer", 0, 10);
            var realType = this.content.CreateDatatypeDefinitionReal("real", 2, 0, 1);
            var stringType = this.content.CreateDatatypeDefinitionString("string", 10);
            var xhtmlType = this.content.CreateDatatypeDefinitionXHTML("xhtml");
            var specification = this.content.CreateSpecification(this.content.CreateSpecificationType("document"));
            var trace = this.content.CreateRelationGroupType("trace");

            var createDefinitions = new Func<string, AttributeDefinition>[]
            {
                x => specType.CreateAttributeDefinition(booleanType, x),
                x => specType.CreateAttributeDefinition(dateType, x),
                x => specType.CreateAttributeDefinition(enumeration, x),
                x => specType.CreateAttributeDefinition(integerType, x),
                x => specType.CreateAttributeDefinition(realType, x),
                x => specType.CreateAttributeDefinition(stringType, x),
                x => specType.CreateAttributeDefinition(xhtmlType, x)
            };

            Assert.Multiple(() =>
            {
                foreach (var createDefinition in createDefinitions)
                {
                    Assert.That(() => createDefinition(null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("longName"));
                    Assert.That(() => createDefinition(string.Empty), Throws.ArgumentException.With.Property("ParamName").EqualTo("longName"));
                }

                Assert.That(() => enumeration.CreateEnumValue(null, 0), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("longName"));
                Assert.That(() => enumeration.CreateEnumValue(string.Empty, 0), Throws.ArgumentException.With.Property("ParamName").EqualTo("longName"));
                Assert.That(() => this.content.CreateRelationGroup(trace, specification, specification, null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("longName"));
                Assert.That(() => this.content.CreateRelationGroup(trace, specification, specification, string.Empty), Throws.ArgumentException.With.Property("ParamName").EqualTo("longName"));

                Assert.That(specType.SpecAttributes, Is.Empty);
                Assert.That(enumeration.SpecifiedValues, Is.Empty);
                Assert.That(this.content.SpecRelationGroups, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_a_relation_group_can_have_values_as_the_metamodel_allows()
        {
            this.BuildDocument();

            var traceType = this.content.SpecTypes.OfType<RelationGroupType>().Single();
            var reviewedAttribute = traceType.CreateAttributeDefinition(this.content.DataTypes.OfType<DatatypeDefinitionBoolean>().Single(), "Reviewed");
            var group = this.content.SpecRelationGroups.Single();

            var value = group.CreateAttributeValue(reviewedAttribute, true);

            using var output = Write(this.reqif);
            var readGroup = Read(output).CoreContent.SpecRelationGroups.Single();

            Assert.Multiple(() =>
            {
                Assert.That(group.Values, Is.EqualTo(new[] { value }));
                Assert.That(value.SpecElAt, Is.SameAs(group));
                Assert.That(DescribeValues(readGroup), Is.EqualTo(DescribeValues(group)));

                // the ReqIF XML schema has no VALUES on RELATION-GROUP, unlike the metamodel
                Assert.That(ValidationMessages(output), Has.Exactly(1).Contains("'RELATION-GROUP'").And.Contains("'VALUES'"));
            });
        }

        [Test]
        public void Verify_that_an_xhtml_value_created_in_code_has_its_external_objects()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var xhtmlDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionXHTML("xhtml"), "xhtml");
            var specObject = this.content.CreateSpecObject(requirement);

            var value = specObject.CreateAttributeValue(xhtmlDefinition,
                "<xhtml:div><xhtml:p>a picture</xhtml:p><xhtml:object data=\"files/a%20b.png\" type=\"image/png\" height=\"20\" width=\"30\"/><xhtml:object data=\"files/c.pdf\" type=\"application/pdf\"/></xhtml:div>");

            Assert.That(value.ExternalObjects, Has.Count.EqualTo(2));

            var picture = value.ExternalObjects[0];
            var document = value.ExternalObjects[1];

            Assert.Multiple(() =>
            {
                Assert.That(picture.Owner, Is.SameAs(value));
                Assert.That(picture.Uri, Is.EqualTo("files/a b.png"));
                Assert.That(picture.MimeType, Is.EqualTo("image/png"));
                Assert.That(picture.Height, Is.EqualTo(20));
                Assert.That(picture.Width, Is.EqualTo(30));
                Assert.That(document.Uri, Is.EqualTo("files/c.pdf"));
                Assert.That(document.MimeType, Is.EqualTo("application/pdf"));
            });
        }

        [Test]
        public void Verify_that_xhtml_that_can_be_a_value_is_accepted()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var datatype = this.content.CreateDatatypeDefinitionXHTML("xhtml");

            var accepted = new[]
            {
                "<xhtml:div>prefix without a declaration</xhtml:div>",
                "<xhtml:div xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">prefix with a declaration</xhtml:div>",
                "<div xmlns=\"http://www.w3.org/1999/xhtml\">default namespace</div>",
                "<xhtml:p>a paragraph</xhtml:p>",
                "  <!-- a comment --> <xhtml:div>whitespace and a comment around the root</xhtml:div>  "
            };

            Assert.Multiple(() =>
            {
                for (var i = 0; i < accepted.Length; i++)
                {
                    var definition = requirement.CreateAttributeDefinition(datatype, $"xhtml {i}");
                    var specObject = this.content.CreateSpecObject(requirement);
                    var xhtml = accepted[i];

                    Assert.That(() => specObject.CreateAttributeValue(definition, xhtml), Throws.Nothing, xhtml);
                    Assert.That(specObject.Values, Has.Count.EqualTo(1), xhtml);
                }
            });
        }

        [Test]
        public void Verify_that_xhtml_that_cannot_be_a_value_throws()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var definition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionXHTML("xhtml"), "xhtml");
            var specObject = this.content.CreateSpecObject(requirement);

            var rejected = new[]
            {
                string.Empty,
                "plain text",
                "<xhtml:div>not closed",
                "<xhtml:div>one</xhtml:div><xhtml:div>two</xhtml:div>",
                "<xhtml:span>not a block</xhtml:span>",
                "<xhtml:DIV>wrong case</xhtml:DIV>",
                "<?xml version=\"1.0\"?><xhtml:div>an XML declaration</xhtml:div>",
                "<![CDATA[text]]><xhtml:div>CDATA before the root</xhtml:div>",
                "<!DOCTYPE div [<!ENTITY e \"x\">]><xhtml:div>&e;</xhtml:div>",
                "<div>no namespace</div>",
                "<other:div xmlns:other=\"urn:other\">another namespace</other:div>",
                "text before <xhtml:div>the root</xhtml:div>",
                "<xhtml:div>the root</xhtml:div> and text after",
                "<undeclared:div>an undeclared prefix</undeclared:div>"
            };

            Assert.Multiple(() =>
            {
                foreach (var xhtml in rejected)
                {
                    Assert.That(() => specObject.CreateAttributeValue(definition, xhtml), Throws.ArgumentException.With.Property("ParamName").EqualTo("xhtml"), xhtml);
                }

                Assert.That(specObject.Values, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_elements_of_another_document_are_rejected()
        {
            var other = ReqIF.Create("another document").CoreContent;
            var otherObjectType = other.CreateSpecObjectType("requirement");
            var otherSpecificationType = other.CreateSpecificationType("document");
            var otherRelationType = other.CreateSpecRelationType("refines");
            var otherGroupType = other.CreateRelationGroupType("trace");
            var otherString = other.CreateDatatypeDefinitionString("string", 10);
            var otherObject = other.CreateSpecObject(otherObjectType);
            var otherSpecification = other.CreateSpecification(otherSpecificationType);

            var objectType = this.content.CreateSpecObjectType("requirement");
            var specObject = this.content.CreateSpecObject(objectType);
            var specification = this.content.CreateSpecification(this.content.CreateSpecificationType("document"));
            var node = specification.AddChild(specObject);
            var relationType = this.content.CreateSpecRelationType("refines");
            var groupType = this.content.CreateRelationGroupType("trace");

            Assert.Multiple(() =>
            {
                Assert.That(() => this.content.CreateSpecObject(otherObjectType), Throws.ArgumentException.With.Property("ParamName").EqualTo("type"));
                Assert.That(() => this.content.CreateSpecification(otherSpecificationType), Throws.ArgumentException.With.Property("ParamName").EqualTo("type"));
                Assert.That(() => this.content.CreateSpecRelation(otherRelationType, specObject, specObject), Throws.ArgumentException.With.Property("ParamName").EqualTo("type"));
                Assert.That(() => this.content.CreateSpecRelation(relationType, otherObject, specObject), Throws.ArgumentException.With.Property("ParamName").EqualTo("source"));
                Assert.That(() => this.content.CreateSpecRelation(relationType, specObject, otherObject), Throws.ArgumentException.With.Property("ParamName").EqualTo("target"));
                Assert.That(() => this.content.CreateRelationGroup(otherGroupType, specification, specification, "group"), Throws.ArgumentException.With.Property("ParamName").EqualTo("type"));
                Assert.That(() => this.content.CreateRelationGroup(groupType, otherSpecification, specification, "group"), Throws.ArgumentException.With.Property("ParamName").EqualTo("sourceSpecification"));
                Assert.That(() => this.content.CreateRelationGroup(groupType, specification, otherSpecification, "group"), Throws.ArgumentException.With.Property("ParamName").EqualTo("targetSpecification"));
                Assert.That(() => objectType.CreateAttributeDefinition(otherString, "string"), Throws.ArgumentException.With.Property("ParamName").EqualTo("type"));
                Assert.That(() => specification.AddChild(otherObject), Throws.ArgumentException.With.Property("ParamName").EqualTo("specObject"));
                Assert.That(() => node.AddChild(otherObject), Throws.ArgumentException.With.Property("ParamName").EqualTo("specObject"));

                Assert.That(this.content.SpecObjects, Is.EqualTo(new[] { specObject }));
                Assert.That(this.content.Specifications, Is.EqualTo(new[] { specification }));
                Assert.That(this.content.SpecRelations, Is.Empty);
                Assert.That(this.content.SpecRelationGroups, Is.Empty);
                Assert.That(objectType.SpecAttributes, Is.Empty);
                Assert.That(specification.Children, Is.EqualTo(new[] { node }));
                Assert.That(node.Children, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_elements_that_belong_to_no_document_are_accepted()
        {
            var detachedType = new SpecObjectType();
            var detachedString = new DatatypeDefinitionString { Identifier = Identifiable.CreateIdentifier() };
            var specObjectType = this.content.CreateSpecObjectType("requirement");

            Assert.Multiple(() =>
            {
                Assert.That(() => this.content.CreateSpecObject(detachedType), Throws.Nothing);
                Assert.That(() => specObjectType.CreateAttributeDefinition(detachedString, "string"), Throws.Nothing);
            });
        }

        [Test]
        public void Verify_that_CreateAttributeValue_leaves_the_LastChange_of_the_element_unchanged()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var stringDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionString("string", 10), "string");
            var xhtmlDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionXHTML("xhtml"), "xhtml");
            var specObject = this.content.CreateSpecObject(requirement);

            var lastChange = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            specObject.LastChange = lastChange;

            specObject.CreateAttributeValue(stringDefinition, "text");
            specObject.CreateAttributeValue(xhtmlDefinition, "<xhtml:div>text</xhtml:div>");

            Assert.That(specObject.LastChange, Is.EqualTo(lastChange));
        }

        [Test]
        public void Verify_that_an_empty_created_document_is_valid_against_the_schema()
        {
            using var output = Write(this.reqif);

            Assert.That(ValidationMessages(output), Is.Empty);
        }

        [Test]
        public void Verify_that_values_within_the_limits_of_their_datatype_are_accepted()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var integerDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionInteger("integer", -5, 5), "integer");
            var realDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionReal("real", 2, -1.5, 1.5), "real");
            var stringDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionString("string", 3), "string");

            Assert.Multiple(() =>
            {
                foreach (var value in new long[] { -5, 0, 5 })
                {
                    Assert.That(() => this.content.CreateSpecObject(requirement).CreateAttributeValue(integerDefinition, value), Throws.Nothing, value.ToString());
                }

                foreach (var value in new[] { -1.5, 0, 1.5 })
                {
                    Assert.That(() => this.content.CreateSpecObject(requirement).CreateAttributeValue(realDefinition, value), Throws.Nothing, value.ToString());
                }

                foreach (var value in new[] { string.Empty, "abc" })
                {
                    Assert.That(() => this.content.CreateSpecObject(requirement).CreateAttributeValue(stringDefinition, value), Throws.Nothing, value);
                }
            });
        }

        [Test]
        public void Verify_that_values_outside_the_limits_of_their_datatype_throw()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var integerDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionInteger("integer", -5, 5), "integer");
            var realDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionReal("real", 2, -1.5, 1.5), "real");
            var stringDefinition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionString("string", 3), "string");
            var specObject = this.content.CreateSpecObject(requirement);

            Assert.Multiple(() =>
            {
                foreach (var value in new long[] { -6, 6, long.MinValue, long.MaxValue })
                {
                    Assert.That(() => specObject.CreateAttributeValue(integerDefinition, value), Throws.ArgumentException.With.Property("ParamName").EqualTo("value"), value.ToString());
                }

                foreach (var value in new[] { -1.51, 1.51, double.NaN, double.NegativeInfinity, double.PositiveInfinity })
                {
                    Assert.That(() => specObject.CreateAttributeValue(realDefinition, value), Throws.ArgumentException.With.Property("ParamName").EqualTo("value"), value.ToString());
                }

                Assert.That(() => specObject.CreateAttributeValue(stringDefinition, "abcd"), Throws.ArgumentException.With.Property("ParamName").EqualTo("value"));
                Assert.That(() => specObject.CreateAttributeValue(stringDefinition, null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("value"));

                Assert.That(specObject.Values, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_CreateAttributeDefinition_rejects_a_datatype_of_another_document_for_each_kind()
        {
            var other = ReqIF.Create("another document").CoreContent;
            var specType = this.content.CreateSpecObjectType("requirement");

            var createDefinitions = new Func<AttributeDefinition>[]
            {
                () => specType.CreateAttributeDefinition(other.CreateDatatypeDefinitionBoolean("boolean"), "boolean"),
                () => specType.CreateAttributeDefinition(other.CreateDatatypeDefinitionDate("date"), "date"),
                () => specType.CreateAttributeDefinition(other.CreateDatatypeDefinitionEnumeration("enumeration"), "enumeration"),
                () => specType.CreateAttributeDefinition(other.CreateDatatypeDefinitionInteger("integer", 0, 10), "integer"),
                () => specType.CreateAttributeDefinition(other.CreateDatatypeDefinitionReal("real", 2, 0, 1), "real"),
                () => specType.CreateAttributeDefinition(other.CreateDatatypeDefinitionString("string", 10), "string"),
                () => specType.CreateAttributeDefinition(other.CreateDatatypeDefinitionXHTML("xhtml"), "xhtml")
            };

            Assert.Multiple(() =>
            {
                foreach (var createDefinition in createDefinitions)
                {
                    Assert.That(() => createDefinition(), Throws.ArgumentException.With.Property("ParamName").EqualTo("type"));
                }

                Assert.That(specType.SpecAttributes, Is.Empty);
            });
        }

        [Test]
        public async Task Verify_that_an_xhtml_value_created_in_a_document_read_with_another_xhtml_prefix_is_written_well_formed()
        {
            this.BuildDocument();

            // a document from another tool that binds the XHTML namespace to another prefix only
            string xml;

            using (var original = Write(this.reqif))
            {
                xml = System.Text.Encoding.UTF8.GetString(original.ToArray())
                    .Replace("xmlns:xhtml=", "xmlns:reqif-xhtml=")
                    .Replace("<xhtml:", "<reqif-xhtml:")
                    .Replace("</xhtml:", "</reqif-xhtml:");
            }

            Assert.That(xml, Does.Not.Contain("xmlns:xhtml="));

            using var input = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
            var document = Read(input);
            var readContent = document.CoreContent;

            var requirementType = readContent.SpecTypes.OfType<SpecObjectType>().Single();
            var textAttribute = requirementType.SpecAttributes.OfType<AttributeDefinitionXHTML>().Single();
            var specObject = readContent.CreateSpecObject(requirementType, "REQ-4");
            specObject.CreateAttributeValue(textAttribute, "<xhtml:div>new</xhtml:div>");

            using var output = Write(document);

            using var asyncOutput = new MemoryStream();
            await new ReqIFSerializer().SerializeAsync(new[] { document }, asyncOutput, SupportedFileExtensionKind.Reqif, CancellationToken.None);

            Assert.Multiple(() =>
            {
                foreach (var stream in new[] { output, asyncOutput })
                {
                    var readBack = Read(stream).CoreContent.SpecObjects.Single(x => x.LongName == "REQ-4");
                    Assert.That(readBack.Values.OfType<AttributeValueXHTML>().Single().ExtractUnformattedTextFromValue(), Is.EqualTo("new"));
                    Assert.That(ValidationMessages(stream), Is.Empty);

                    // the root element keeps the declarations of the document that was read
                    var written = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                    var rootStart = written.IndexOf("<REQ-IF", StringComparison.Ordinal);
                    var rootElement = written.Substring(rootStart, written.IndexOf('>', rootStart) - rootStart);
                    Assert.That(rootElement, Does.Contain("xmlns:reqif-xhtml="));
                    Assert.That(rootElement, Does.Not.Contain("xmlns:xhtml="));
                }
            });
        }

        [Test]
        public async Task Verify_that_an_xhtml_value_is_read_with_the_whitespace_around_it()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var definition = requirement.CreateAttributeDefinition(this.content.CreateDatatypeDefinitionXHTML("xhtml"), "xhtml");
            var specObject = this.content.CreateSpecObject(requirement);
            var value = specObject.CreateAttributeValue(definition, "\n  <!-- a comment --><xhtml:div>text</xhtml:div>\n  ");
            value.TheOriginalValue = "\n  <xhtml:div xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">original</xhtml:div>\n";

            using var output = Write(this.reqif);

            output.Position = 0;
            var asyncDocument = (await new ReqIFDeserializer().DeserializeAsync(output, SupportedFileExtensionKind.Reqif, CancellationToken.None)).Single();

            Assert.Multiple(() =>
            {
                foreach (var document in new[] { Read(output), asyncDocument })
                {
                    var readValue = document.CoreContent.SpecObjects.Single().Values.OfType<AttributeValueXHTML>().Single();
                    Assert.That(readValue.TheValue, Is.EqualTo(value.TheValue));
                    Assert.That(readValue.TheOriginalValue, Is.EqualTo(value.TheOriginalValue));
                }
            });
        }

        [Test]
        public void Verify_that_a_created_xhtml_value_declares_the_xhtml_prefix_when_it_uses_it_undeclared()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var datatype = this.content.CreateDatatypeDefinitionXHTML("xhtml");
            const string declaration = "xmlns:xhtml=\"http://www.w3.org/1999/xhtml\"";

            var cases = new[]
            {
                ("<xhtml:div>text</xhtml:div>", $"<xhtml:div {declaration}>text</xhtml:div>"),
                ("<xhtml:div class=\"a\"><xhtml:p>text</xhtml:p></xhtml:div>", $"<xhtml:div {declaration} class=\"a\"><xhtml:p>text</xhtml:p></xhtml:div>"),
                ("<!-- a\r\ncomment -->\r\n  <xhtml:p>text</xhtml:p>", $"<!-- a\r\ncomment -->\r\n  <xhtml:p {declaration}>text</xhtml:p>"),
                ("<div xmlns=\"http://www.w3.org/1999/xhtml\"><xhtml:b>bold</xhtml:b></div>", $"<div {declaration} xmlns=\"http://www.w3.org/1999/xhtml\"><xhtml:b>bold</xhtml:b></div>"),
                ($"<xhtml:div {declaration}>declared</xhtml:div>", $"<xhtml:div {declaration}>declared</xhtml:div>"),
                ("<div xmlns=\"http://www.w3.org/1999/xhtml\">no prefix</div>", "<div xmlns=\"http://www.w3.org/1999/xhtml\">no prefix</div>")
            };

            Assert.Multiple(() =>
            {
                for (var i = 0; i < cases.Length; i++)
                {
                    var (xhtml, expected) = cases[i];
                    var definition = requirement.CreateAttributeDefinition(datatype, $"xhtml {i}");
                    var value = this.content.CreateSpecObject(requirement).CreateAttributeValue(definition, xhtml);

                    Assert.That(value.TheValue, Is.EqualTo(expected), xhtml);
                }
            });
        }

        /// <summary>
        /// Builds a document with every kind of element and attribute value, using only the create methods. The relation
        /// group has no values, so that the document is valid: the ReqIF XML schema has no VALUES on a RELATION-GROUP
        /// </summary>
        private void BuildDocument()
        {
            var stringType = this.content.CreateDatatypeDefinitionString("String", 255);
            var xhtmlType = this.content.CreateDatatypeDefinitionXHTML("Formatted text");
            var integerType = this.content.CreateDatatypeDefinitionInteger("Integer", 0, 100);
            var realType = this.content.CreateDatatypeDefinitionReal("Real", 2, 0, 100);
            var booleanType = this.content.CreateDatatypeDefinitionBoolean("Boolean");
            var dateType = this.content.CreateDatatypeDefinitionDate("Date");
            var statusType = this.content.CreateDatatypeDefinitionEnumeration("Status");
            var draft = statusType.CreateEnumValue("Draft", 0);
            var approved = statusType.CreateEnumValue("Approved", 1);

            var requirementType = this.content.CreateSpecObjectType("Requirement");
            var idAttribute = requirementType.CreateAttributeDefinition(stringType, "ID");
            var textAttribute = requirementType.CreateAttributeDefinition(xhtmlType, "Text");
            var priorityAttribute = requirementType.CreateAttributeDefinition(integerType, "Priority");
            var weightAttribute = requirementType.CreateAttributeDefinition(realType, "Weight");
            var safetyAttribute = requirementType.CreateAttributeDefinition(booleanType, "Safety");
            var dueAttribute = requirementType.CreateAttributeDefinition(dateType, "Due");
            var statusAttribute = requirementType.CreateAttributeDefinition(statusType, "Status");
            var tagsAttribute = requirementType.CreateAttributeDefinition(statusType, "Tags", isMultiValued: true);

            var specificationType = this.content.CreateSpecificationType("Document");
            var titleAttribute = specificationType.CreateAttributeDefinition(stringType, "Title");
            var refinesType = this.content.CreateSpecRelationType("refines");
            var rationaleAttribute = refinesType.CreateAttributeDefinition(stringType, "Rationale");
            var traceType = this.content.CreateRelationGroupType("trace");

            SpecObject CreateRequirement(string id, int priority, EnumValue status, params EnumValue[] tags)
            {
                var requirement = this.content.CreateSpecObject(requirementType, id);
                requirement.CreateAttributeValue(idAttribute, id);
                requirement.CreateAttributeValue(textAttribute, $"<xhtml:div>The text of <xhtml:b>{id}</xhtml:b></xhtml:div>");
                requirement.CreateAttributeValue(priorityAttribute, priority);
                requirement.CreateAttributeValue(weightAttribute, priority / 4.0);
                requirement.CreateAttributeValue(safetyAttribute, priority % 2 == 0);
                requirement.CreateAttributeValue(dueAttribute, new DateTime(2027, 1, priority, 0, 0, 0, DateTimeKind.Utc));
                requirement.CreateAttributeValue(statusAttribute, status);
                requirement.CreateAttributeValue(tagsAttribute, tags);
                return requirement;
            }

            var brew = CreateRequirement("REQ-1", 1, approved, draft, approved);
            var heat = CreateRequirement("REQ-2", 2, draft, approved);
            var tank = CreateRequirement("REQ-3", 3, draft);

            var specification = this.content.CreateSpecification(specificationType, "Coffee machine");
            specification.CreateAttributeValue(titleAttribute, "Coffee machine");
            specification.AddChild(brew).AddChild(heat);
            specification.AddChild(tank);

            var relation = this.content.CreateSpecRelation(refinesType, heat, brew, "REQ-2 refines REQ-1");
            relation.CreateAttributeValue(rationaleAttribute, "heating is part of brewing");

            var group = this.content.CreateRelationGroup(traceType, specification, specification, "Trace");
            group.SpecRelations.Add(relation);
        }

        private static List<string> ValidationMessages(MemoryStream output)
        {
            output.Position = 0;

            var messages = new List<string>();
            new ReqIFDeserializer().Deserialize(output, SupportedFileExtensionKind.Reqif, true, (_, e) => messages.Add($"{e.Severity}: {e.Message}"));

            return messages;
        }

        private static MemoryStream Write(ReqIF document)
        {
            var output = new MemoryStream();
            new ReqIFSerializer().Serialize(new[] { document }, output, SupportedFileExtensionKind.Reqif);
            return output;
        }

        private static ReqIF Read(MemoryStream stream)
        {
            stream.Position = 0;
            return new ReqIFDeserializer().Deserialize(stream, SupportedFileExtensionKind.Reqif).Single();
        }

        /// <summary>
        /// Describes the values of an element by attribute definition identifier, so they can be compared between the
        /// document that was built and the document that was read back
        /// </summary>
        private static List<string> DescribeValues(SpecElementWithAttributes element)
        {
            return element.Values.Select(x =>
            {
                var value = x switch
                {
                    AttributeValueEnumeration enumeration => string.Join(",", enumeration.Values.Select(v => v.Identifier)),
                    AttributeValueDate date => date.TheValue.ToUniversalTime().ToString("o"),
                    AttributeValueReal real => real.TheValue.ToString("R", System.Globalization.CultureInfo.InvariantCulture),
                    AttributeValueXHTML xhtml => xhtml.ExtractUnformattedTextFromValue(),
                    _ => Convert.ToString(x.ObjectValue, System.Globalization.CultureInfo.InvariantCulture)
                };

                return $"{x.AttributeDefinition.Identifier}={value}";
            }).ToList();
        }

        /// <summary>
        /// Describes a spec hierarchy as the identifiers of its spec objects, with the children in brackets
        /// </summary>
        private static string DescribeHierarchy(IEnumerable<SpecHierarchy> children)
        {
            return string.Join(" ", children.Select(x => x.Children.Count == 0 ? x.Object.Identifier : $"{x.Object.Identifier}[{DescribeHierarchy(x.Children)}]"));
        }

        [Test]
        public void Verify_that_a_document_built_with_the_create_methods_is_valid_against_the_schema()
        {
            this.BuildDocument();

            using var output = Write(this.reqif);

            Assert.That(ValidationMessages(output), Is.Empty);
        }

        [Test]
        public async Task Verify_that_a_document_built_with_the_create_methods_is_valid_against_the_schema_async()
        {
            this.BuildDocument();

            using var output = new MemoryStream();
            await new ReqIFSerializer().SerializeAsync(new[] { this.reqif }, output, SupportedFileExtensionKind.Reqif, CancellationToken.None);

            Assert.That(ValidationMessages(output), Is.Empty);
        }

        [Test]
        public async Task Verify_that_infinite_real_values_are_written_valid_against_the_schema_and_read_back()
        {
            var requirement = this.content.CreateSpecObjectType("requirement");
            var unbounded = this.content.CreateDatatypeDefinitionReal("unbounded", 2, double.NegativeInfinity, double.PositiveInfinity);
            var high = requirement.CreateAttributeDefinition(unbounded, "high");
            var low = requirement.CreateAttributeDefinition(unbounded, "low");
            var finite = requirement.CreateAttributeDefinition(unbounded, "finite");

            var specObject = this.content.CreateSpecObject(requirement);
            specObject.CreateAttributeValue(high, double.PositiveInfinity);
            specObject.CreateAttributeValue(low, double.NegativeInfinity);
            specObject.CreateAttributeValue(finite, 0.1);

            using var syncOutput = Write(this.reqif);

            using var asyncOutput = new MemoryStream();
            await new ReqIFSerializer().SerializeAsync(new[] { this.reqif }, asyncOutput, SupportedFileExtensionKind.Reqif, CancellationToken.None);

            foreach (var (output, name) in new[] { (syncOutput, "sync"), (asyncOutput, "async") })
            {
                var xml = System.Text.Encoding.UTF8.GetString(output.ToArray());
                var values = Read(output).CoreContent.SpecObjects.Single().Values.OfType<AttributeValueReal>().ToDictionary(x => x.Definition.LongName, x => x.TheValue);

                Assert.Multiple(() =>
                {
                    Assert.That(xml, Does.Contain("THE-VALUE=\"INF\"").And.Contain("THE-VALUE=\"-INF\"").And.Contain("THE-VALUE=\"0.1\""), name);
                    Assert.That(ValidationMessages(output), Is.Empty, name);
                    Assert.That(values["high"], Is.EqualTo(double.PositiveInfinity), name);
                    Assert.That(values["low"], Is.EqualTo(double.NegativeInfinity), name);
                    Assert.That(values["finite"], Is.EqualTo(0.1), name);
                });
            }
        }

        [Test]
        public void Verify_that_a_document_built_with_the_create_methods_is_read_back_with_the_same_content()
        {
            this.BuildDocument();

            using var output = Write(this.reqif);
            var readContent = Read(output).CoreContent;

            var specification = this.content.Specifications.Single();
            var readSpecification = readContent.Specifications.Single();
            var relation = this.content.SpecRelations.Single();
            var readRelation = readContent.SpecRelations.Single();
            var group = this.content.SpecRelationGroups.Single();
            var readGroup = readContent.SpecRelationGroups.Single();

            Assert.Multiple(() =>
            {
                Assert.That(readContent.DataTypes.Select(x => x.Identifier), Is.EqualTo(this.content.DataTypes.Select(x => x.Identifier)));
                Assert.That(readContent.SpecTypes.Select(x => x.Identifier), Is.EqualTo(this.content.SpecTypes.Select(x => x.Identifier)));
                Assert.That(readContent.SpecObjects.Select(x => x.Identifier), Is.EqualTo(this.content.SpecObjects.Select(x => x.Identifier)));

                foreach (var specObject in this.content.SpecObjects)
                {
                    var readObject = readContent.SpecObjects.Single(x => x.Identifier == specObject.Identifier);
                    Assert.That(readObject.LongName, Is.EqualTo(specObject.LongName));
                    Assert.That(readObject.Type.Identifier, Is.EqualTo(specObject.Type.Identifier));
                    Assert.That(DescribeValues(readObject), Is.EqualTo(DescribeValues(specObject)));
                }

                Assert.That(readSpecification.Identifier, Is.EqualTo(specification.Identifier));
                Assert.That(DescribeValues(readSpecification), Is.EqualTo(DescribeValues(specification)));
                Assert.That(DescribeHierarchy(readSpecification.Children), Is.EqualTo(DescribeHierarchy(specification.Children)));
                Assert.That(readSpecification.Children.Select(x => x.Identifier), Is.EqualTo(specification.Children.Select(x => x.Identifier)));

                Assert.That(readRelation.Identifier, Is.EqualTo(relation.Identifier));
                Assert.That(readRelation.Source.Identifier, Is.EqualTo(relation.Source.Identifier));
                Assert.That(readRelation.Target.Identifier, Is.EqualTo(relation.Target.Identifier));
                Assert.That(DescribeValues(readRelation), Is.EqualTo(DescribeValues(relation)));

                Assert.That(readGroup.Identifier, Is.EqualTo(group.Identifier));
                Assert.That(readGroup.SourceSpecification.Identifier, Is.EqualTo(specification.Identifier));
                Assert.That(readGroup.TargetSpecification.Identifier, Is.EqualTo(specification.Identifier));
                Assert.That(readGroup.SpecRelations.Select(x => x.Identifier), Is.EqualTo(new[] { relation.Identifier }));
            });
        }

        [Test]
        public void Verify_that_the_create_methods_extend_a_document_that_was_read()
        {
            this.BuildDocument();

            using var original = Write(this.reqif);
            var document = Read(original);
            var readContent = document.CoreContent;

            var requirementType = readContent.SpecTypes.OfType<SpecObjectType>().Single();
            var idAttribute = requirementType.SpecAttributes.OfType<AttributeDefinitionString>().Single();
            var statusAttribute = requirementType.SpecAttributes.OfType<AttributeDefinitionEnumeration>().First(x => !x.IsMultiValued);
            var approved = statusAttribute.Type.SpecifiedValues.Single(x => x.LongName == "Approved");
            var refinesType = readContent.SpecTypes.OfType<SpecRelationType>().Single();
            var specification = readContent.Specifications.Single();
            var brew = readContent.SpecObjects.Single(x => x.LongName == "REQ-1");

            var grind = readContent.CreateSpecObject(requirementType, "REQ-4");
            grind.CreateAttributeValue(idAttribute, "REQ-4");
            grind.CreateAttributeValue(statusAttribute, approved);
            var grindNode = specification.Children.First().AddChild(grind);
            var relation = readContent.CreateSpecRelation(refinesType, grind, brew);

            Assert.Multiple(() =>
            {
                Assert.That(grind.ReqIFContent, Is.SameAs(readContent));
                Assert.That(grindNode.Root, Is.SameAs(specification));
                Assert.That(grindNode.ReqIfContent, Is.SameAs(readContent));
                Assert.That(readContent.SpecRelations, Does.Contain(relation));
            });

            using var output = Write(document);

            Assert.That(ValidationMessages(output), Is.Empty);
        }
    }
}
