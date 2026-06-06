// -------------------------------------------------------------------------------------------------
// <copyright file="ModelMetadataTestFixture.cs" company="Starion Group S.A.">
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
// ------------------------------------------------------------------------------------------------

namespace ReqIFSharp.Tests.Decorators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using NUnit.Framework;

    using ReqIFSharp;

    /// <summary>
    /// Reflection-driven completeness guard that verifies every model class and metamodel property
    /// in the <see cref="ReqIFSharp"/> assembly is decorated with the <see cref="ReqIfClassAttribute"/>
    /// and <see cref="ReqIfPropertyAttribute"/> metadata decorators. When a new model class or property
    /// is added, this fixture fails until the decorators are applied (or the property is added to the
    /// documented exclusion list of infrastructure / back-reference members).
    /// </summary>
    [TestFixture]
    public class ModelMetadataTestFixture
    {
        /// <summary>
        /// Names of properties that are NOT part of the ReqIF metamodel and therefore intentionally
        /// carry no <see cref="ReqIfPropertyAttribute"/>: container back-references, owner links and
        /// programming-convenience accessors.
        /// </summary>
        private static readonly HashSet<string> ExcludedPropertyNames = new HashSet<string>
        {
            "ObjectValue",       // convenience accessor that aliases TheValue / Values
            "DatatypeDefinition", // AttributeDefinition base accessor that aliases the concrete Type
            "AttributeDefinition", // AttributeValue base accessor that aliases the concrete Definition
            "ExternalObjects",   // derived from the parsed XHTML content
            "OwningDefinition",  // back-reference to the owning AttributeDefinition
            "Definition",        // see note below - kept, handled explicitly (NOT excluded)
            "SpecElAt",          // back-reference to the owning SpecElementWithAttributes
            "SpecType",          // AttributeDefinition back-reference to its owning SpecType
            "ReqIFContent",      // back-reference to the owning ReqIFContent
            "ReqIfContent",      // back-reference (SpecHierarchy casing variant)
            "DocumentRoot",      // back-reference to the owning ReqIF
            "CoreContent",       // RelationGroup back-reference to the owning ReqIFContent
            "Root",              // SpecHierarchy back-reference to the owning Specification
            "Container",         // SpecHierarchy back-reference to the parent SpecHierarchy
            "DataTpeDefEnum",    // EnumValue back-reference to the owning DatatypeDefinitionEnumeration
            "EnumValue",         // EmbeddedValue back-reference to the owning EnumValue
            "Owner",             // ExternalObject back-reference to the owning AttributeValueXHTML
            "Ident"              // AlternativeId back-reference to the owning Identifiable
        };

        /// <summary>
        /// Returns every model class in the <see cref="ReqIFSharp"/> assembly. A model class is one
        /// that derives from <see cref="Identifiable"/> or <see cref="AttributeValue"/>, or is one of
        /// the remaining stand-alone model classes.
        /// </summary>
        private static IEnumerable<Type> ModelTypes()
        {
            var standalone = new[]
            {
                typeof(ReqIF), typeof(ReqIFContent), typeof(ReqIFHeader), typeof(ReqIFToolExtension),
                typeof(AlternativeId), typeof(EmbeddedValue), typeof(ExternalObject)
            };

            return typeof(Identifiable).Assembly.GetTypes()
                .Where(t => t.IsClass
                            && (typeof(Identifiable).IsAssignableFrom(t)
                                || typeof(AttributeValue).IsAssignableFrom(t)
                                || standalone.Contains(t)));
        }

        /// <summary>
        /// Returns the public, instance properties declared directly on <paramref name="type"/> that
        /// are expected to carry a <see cref="ReqIfPropertyAttribute"/> (i.e. metamodel features), filtering
        /// out the documented infrastructure / back-reference exclusions. The <c>Definition</c> property
        /// IS a metamodel reference and is therefore kept even though it appears in the lookup set above
        /// only for documentation - it is removed from the exclusion set here.
        /// </summary>
        private static IEnumerable<PropertyInfo> MetamodelProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => p.GetMethod != null && p.GetMethod.IsPublic)
                .Where(p => p.Name == "Definition" || !ExcludedPropertyNames.Contains(p.Name));
        }

        [Test]
        public void Verify_that_every_model_class_is_decorated_with_ReqIfClassAttribute()
        {
            var offenders = ModelTypes()
                .Where(t => t.GetCustomAttribute<ReqIfClassAttribute>() == null)
                .Select(t => t.Name)
                .ToList();

            Assert.That(offenders, Is.Empty,
                $"The following model classes are missing the [Class] decorator: {string.Join(", ", offenders)}");
        }

        [Test]
        public void Verify_that_ReqIfClassAttribute_IsAbstract_matches_the_type()
        {
            foreach (var type in ModelTypes())
            {
                var classAttribute = type.GetCustomAttribute<ReqIfClassAttribute>();

                Assert.That(classAttribute, Is.Not.Null, $"{type.Name} is missing the [Class] decorator.");
                Assert.That(classAttribute.IsAbstract, Is.EqualTo(type.IsAbstract),
                    $"[Class].IsAbstract on {type.Name} does not match the type's abstractness.");
                Assert.That(classAttribute.Name, Is.Not.Empty, $"[Class].Name on {type.Name} should not be empty.");
            }
        }

        [Test]
        public void Verify_that_every_metamodel_property_is_decorated_with_ReqIfPropertyAttribute()
        {
            var offenders = new List<string>();

            foreach (var type in ModelTypes())
            {
                foreach (var property in MetamodelProperties(type))
                {
                    if (property.GetCustomAttribute<ReqIfPropertyAttribute>() == null)
                    {
                        offenders.Add($"{type.Name}.{property.Name}");
                    }
                }
            }

            Assert.That(offenders, Is.Empty,
                $"The following metamodel properties are missing the [Property] decorator: {string.Join(", ", offenders)}");
        }

        [Test]
        public void Verify_that_every_ReqIfPropertyAttribute_has_consistent_multiplicity()
        {
            foreach (var type in ModelTypes())
            {
                foreach (var property in MetamodelProperties(type))
                {
                    var propertyAttribute = property.GetCustomAttribute<ReqIfPropertyAttribute>();

                    if (propertyAttribute == null)
                    {
                        continue;
                    }

                    Assert.That(propertyAttribute.LowerValue, Is.GreaterThanOrEqualTo(0),
                        $"{type.Name}.{property.Name} has a negative lower bound.");
                    Assert.That(propertyAttribute.UpperValue, Is.GreaterThanOrEqualTo(1),
                        $"{type.Name}.{property.Name} has an upper bound below 1.");
                    Assert.That(propertyAttribute.UpperValue, Is.GreaterThanOrEqualTo(propertyAttribute.LowerValue),
                        $"{type.Name}.{property.Name} has an upper bound below its lower bound.");
                }
            }
        }

        [Test]
        public void Verify_known_property_metadata_spot_checks()
        {
            AssertProperty(typeof(SpecObject), nameof(SpecObject.Type), AggregationKind.None, 1, 1);
            AssertProperty(typeof(ReqIFContent), nameof(ReqIFContent.SpecObjects), AggregationKind.Composite, 0, int.MaxValue);
            AssertProperty(typeof(Identifiable), nameof(Identifiable.Identifier), AggregationKind.None, 1, 1);
            AssertProperty(typeof(Identifiable), nameof(Identifiable.AlternativeId), AggregationKind.Composite, 0, 1);
            AssertProperty(typeof(AttributeDefinitionEnumeration), nameof(AttributeDefinitionEnumeration.IsMultiValued), AggregationKind.None, 1, 1);
            AssertProperty(typeof(DatatypeDefinitionEnumeration), nameof(DatatypeDefinitionEnumeration.SpecifiedValues), AggregationKind.Composite, 0, int.MaxValue);
        }

        private static void AssertProperty(Type type, string propertyName, AggregationKind aggregation, int lower, int upper)
        {
            var property = type.GetProperty(propertyName);
            Assert.That(property, Is.Not.Null, $"{type.Name}.{propertyName} does not exist.");

            var propertyAttribute = property.GetCustomAttribute<ReqIfPropertyAttribute>();
            Assert.That(propertyAttribute, Is.Not.Null, $"{type.Name}.{propertyName} is missing the [Property] decorator.");

            Assert.Multiple(() =>
            {
                Assert.That(propertyAttribute.Aggregation, Is.EqualTo(aggregation), $"{type.Name}.{propertyName} aggregation");
                Assert.That(propertyAttribute.LowerValue, Is.EqualTo(lower), $"{type.Name}.{propertyName} lower bound");
                Assert.That(propertyAttribute.UpperValue, Is.EqualTo(upper), $"{type.Name}.{propertyName} upper bound");
            });
        }
    }
}
