//  -------------------------------------------------------------------------------------------------
//  <copyright file="IdentifiableExtensionsTestFixture.cs" company="Starion Group S.A.">
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

namespace ReqIFSharp.Extensions.Tests.ReqIFExtensions
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using ReqIFSharp;
    using ReqIFSharp.Extensions.ReqIFExtensions;

    /// <summary>
    /// Suite of tests for the <see cref="IdentifiableExtensions"/> class
    /// </summary>
    [TestFixture]
    public class IdentifiableExtensionsTestFixture
    {
        private static IEnumerable<TestCaseData> QueryTypeDisplayNameTestCases()
        {
            yield return new TestCaseData(new AttributeDefinitionBoolean(), "Boolean Attribute Definition");
            yield return new TestCaseData(new AttributeDefinitionDate(), "Date Attribute Definition");
            yield return new TestCaseData(new AttributeDefinitionEnumeration(), "Enumeration Attribute Definition");
            yield return new TestCaseData(new AttributeDefinitionInteger(), "Integer Attribute Definition");
            yield return new TestCaseData(new AttributeDefinitionReal(), "Real Attribute Definition");
            yield return new TestCaseData(new AttributeDefinitionString(), "String Attribute Definition");
            yield return new TestCaseData(new AttributeDefinitionXHTML(), "XHTML Attribute Definition");
            yield return new TestCaseData(new DatatypeDefinitionBoolean(), "Boolean Datatype Definition");
            yield return new TestCaseData(new DatatypeDefinitionDate(), "Date Datatype Definition");
            yield return new TestCaseData(new DatatypeDefinitionEnumeration(), "Enumeration Datatype Definition");
            yield return new TestCaseData(new DatatypeDefinitionInteger(), "Integer Datatype Definition");
            yield return new TestCaseData(new DatatypeDefinitionReal(), "Real Datatype Definition");
            yield return new TestCaseData(new DatatypeDefinitionString(), "String Datatype Definition");
            yield return new TestCaseData(new DatatypeDefinitionXHTML(), "XHTML Datatype Definition");
            yield return new TestCaseData(new EnumValue(), "Enum Value");
            yield return new TestCaseData(new RelationGroup(), "Relation Group");
            yield return new TestCaseData(new SpecHierarchy(), "Spec Hierarchy");
            yield return new TestCaseData(new Specification(), "Specification");
            yield return new TestCaseData(new SpecObject(), "Spec Object");
            yield return new TestCaseData(new SpecRelation(), "Spec Relation");
            yield return new TestCaseData(new RelationGroupType(), "Relation Group Type");
            yield return new TestCaseData(new SpecificationType(), "Specification Type");
            yield return new TestCaseData(new SpecObjectType(), "Spec Object Type");
            yield return new TestCaseData(new SpecRelationType(), "Spec Relation Type");
        }

        [Test]
        [TestCaseSource(nameof(QueryTypeDisplayNameTestCases))]
        public void Verify_that_QueryTypeDisplayName_returns_expected_display_name(Identifiable identifiable, string expected)
        {
            Assert.That(identifiable.QueryTypeDisplayName(), Is.EqualTo(expected));
        }

        [Test]
        public void Verify_that_QueryTypeDisplayName_throws_when_identifiable_is_null()
        {
            Assert.That(() => IdentifiableExtensions.QueryTypeDisplayName(null),
                Throws.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("identifiable"));
        }

        [Test]
        public void Verify_that_QueryTypeDisplayName_throws_for_unknown_identifiable_type()
        {
            var identifiable = new UnknownIdentifiable();

            Assert.That(() => identifiable.QueryTypeDisplayName(),
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains(nameof(UnknownIdentifiable)));
        }

        private class UnknownIdentifiable : Identifiable
        {
        }
    }
}
