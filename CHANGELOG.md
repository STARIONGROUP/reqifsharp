# Changelog

All notable changes to ReqIFSharp are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and the project follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html). Numbers such as [#67](https://github.com/STARIONGROUP/ReqIFSharp/issues/67) refer to the issue or pull request of the change.

The repository ships two NuGet packages that version independently: **ReqIFSharp** and **ReqIFSharp.Extensions**. Each release is listed under its ReqIFSharp version; the ReqIFSharp.Extensions version released with it is named below the heading, and changes that only concern ReqIFSharp.Extensions are marked as such. A release of ReqIFSharp.Extensions alone is headed by its own name and version. Dates are the dates the packages were published on NuGet.

## [Unreleased]

### Added

- Methods to build a document in code. Each one adds the new element to its owner and sets its back-references; all except `CreateAttributeValue` also give it a valid identifier and a `LastChange` of now. So the written document is valid against the ReqIF schema (unless a relation group has values, see below) and the ReqIFSharp.Extensions queries work on it ([#85](https://github.com/STARIONGROUP/ReqIFSharp/issues/85)):
  - `ReqIF.Create(title)` creates a document with a header and a content.
  - `ReqIFContent.CreateDatatypeDefinitionBoolean`, `…Date`, `…Enumeration`, `…Integer`, `…Real`, `…String` and `…XHTML`, `CreateSpecObjectType`, `CreateSpecificationType`, `CreateSpecRelationType`, `CreateRelationGroupType`, `CreateSpecObject`, `CreateSpecification`, `CreateSpecRelation` and `CreateRelationGroup`.
  - `DatatypeDefinitionEnumeration.CreateEnumValue` and `SpecType.CreateAttributeDefinition`, with an overload for each kind of datatype.
  - `CreateAttributeDefinition`, `CreateEnumValue` and `CreateRelationGroup` require a long name, which the ReqIF specification makes mandatory for these elements; they throw when it is null or empty.
  - `Specification.AddChild` and `SpecHierarchy.AddChild` add a spec object to the hierarchy.
  - The methods throw an `ArgumentException` when a type, spec object, specification or datatype passed to them belongs to another document, so that a document only refers to its own elements.
  - `SpecElementWithAttributes.CreateAttributeValue`, with an overload for each kind of attribute definition. It throws an `InvalidOperationException` when the element has no type, and an `ArgumentException` when the definition has no datatype, is not an attribute of the element's type, the element already has a value for it, several enumeration literals are passed for a definition that is not multi-valued, an enumeration literal does not belong to the definition's enumeration, an integer or real value is outside the `Min` and `Max` of its datatype (or a real is not a number), a string is longer than the `MaxLength` of its datatype, or the XHTML is not well-formed with a single `xhtml:div` or `xhtml:p` root element (an XML declaration is not allowed either). A null string throws an `ArgumentNullException`, as the schema requires a value; use an empty string for no text. When the XHTML uses the `xhtml` prefix without declaring it, `xmlns:xhtml` is added to its root element, so the value is complete on its own, as a value read from a document is. An XHTML value gets an `ExternalObject` for each `xhtml:object` it contains, as when it is read. An attribute value has no identifier or `LastChange`, and the `LastChange` of the element is left as it is. A relation group can have values, as the ReqIF metamodel allows, but the ReqIF XML schema has no `VALUES` on a `RELATION-GROUP`, so a document with them is not valid against the schema.
- `Identifiable.CreateIdentifier()` returns a new identifier that is a valid `xsd:ID`: an underscore followed by a GUID. `Identifiable.MarkAsChanged()` sets `LastChange` to now; call it after changing an element ([#85](https://github.com/STARIONGROUP/ReqIFSharp/issues/85)).
- `AttributeValueXHTML.ExtractNormalizedTextFromValue(bool keepLineBreaks = false)` returns the text of an XHTML value as plain text: runs of spaces, tabs and line breaks are collapsed to one space (non-breaking spaces are kept), no space is added around inline elements, and block elements such as paragraphs and list items are separated by a space or, with `keepLineBreaks`, by a line feed. The text of a `pre` element is kept as written, and HTML named entities such as `&nbsp;` are turned into their characters. `ExtractUnformattedTextFromValue` is unchanged ([#92](https://github.com/STARIONGROUP/ReqIFSharp/issues/92)).
- `AccessControlledElement.IsEditableSpecified` tells whether the `IS-EDITABLE` attribute is written. It is true when the attribute was present in the document that was read or when `IsEditable` was set; set it to false to leave out an `IS-EDITABLE` that is false ([#86](https://github.com/STARIONGROUP/ReqIFSharp/issues/86)).

### Changed

- The `TheValue` and `TheOriginalValue` of an XHTML attribute value are read as they are in the document. Before, whitespace at the start and the end was removed, so a document that was read and written again could differ ([#85](https://github.com/STARIONGROUP/ReqIFSharp/issues/85)).
- Setting `IsEditable` to false on a spec hierarchy or attribute definition now writes `IS-EDITABLE="false"`; before, the attribute was left out. Both mean that the element is read-only. Set `IsEditableSpecified` to false to leave the attribute out ([#86](https://github.com/STARIONGROUP/ReqIFSharp/issues/86)).
- The source files carry the licence in short SPDX form (`SPDX-FileCopyrightText: 2017-2026 Starion Group S.A.`, `SPDX-License-Identifier: Apache-2.0`) instead of the full Apache-2.0 notice; the licence itself is unchanged, and `NOTICE` now gives the same years, 2017-2026 ([#103](https://github.com/STARIONGROUP/ReqIFSharp/issues/103)).
- A `REQ-IF` root element in a namespace other than the ReqIF namespace (`http://www.omg.org/spec/ReqIF/20110401/reqif.xsd`) is no longer read as a ReqIF document; it is skipped and a warning is logged. Before, such an element was read when it had no prefix. A root in no namespace is still read ([#87](https://github.com/STARIONGROUP/ReqIFSharp/issues/87)).
- An element other than `SPEC-RELATION-REF` inside the `SPEC-RELATIONS` of a relation group is now skipped with a warning, and the references after it are read. Before, it caused an `XmlException` ([#94](https://github.com/STARIONGROUP/ReqIFSharp/issues/94)).
- An element that has the name of a ReqIF element but is in another namespace (for example `<other:THE-VALUE xmlns:other="...">`) is no longer read as that ReqIF element. It is skipped, with its content, and a warning is logged. Elements in the ReqIF namespace or in no namespace are read as before, in any document ([#97](https://github.com/STARIONGROUP/ReqIFSharp/issues/97)).

### Fixed

- An infinite real attribute value was written as `Infinity` or `-Infinity`, which is not a valid `xsd:double`, so the document failed schema validation and could not be read back. It is now written as `INF` or `-INF`, as the `MIN` and `MAX` of a real datatype already were ([#85](https://github.com/STARIONGROUP/ReqIFSharp/issues/85)).
- A document that binds the ReqIF namespace to a prefix (`<reqif:REQ-IF xmlns:reqif="...">`) was read as zero documents, and type, source/target and relation-group references in it were not read. Elements are now matched on their local name and namespace ([#87](https://github.com/STARIONGROUP/ReqIFSharp/issues/87)).
- A document without whitespace between its elements (minified XML) lost data without an error: every other header field (such as `CREATION-TIME` and `REQ-IF-TOOL-ID`) and every other enumeration value of an attribute value was skipped, and the spec relations of a relation group were not read or caused an `XmlException` ([#94](https://github.com/STARIONGROUP/ReqIFSharp/issues/94)).
- `AttributeValueXHTML.TheOriginalValue` was not read from `THE-ORIGINAL-VALUE`, so it was always `null` after reading, and a document that was read and written again lost the original values of its simplified XHTML attribute values ([#95](https://github.com/STARIONGROUP/ReqIFSharp/issues/95)).
- Attribute values did not log anything, also when an `ILoggerFactory` was passed to the deserializer: the `AttributeValue` constructors did not keep the logger factory. An enumeration attribute value no longer reports its own element and its `DEFINITION` and `VALUES` elements as not supported ([#97](https://github.com/STARIONGROUP/ReqIFSharp/issues/97)).
- An explicit `IS-EDITABLE="false"` on a spec hierarchy or attribute definition was dropped when a document was read and written again, so the receiving tool fell back to its own default. It is now written whenever it was read or `IsEditable` was set, also when it is false; an element without the attribute is still written without it ([#86](https://github.com/STARIONGROUP/ReqIFSharp/issues/86)).
- Writing a spec hierarchy or attribute definition that has an `ALTERNATIVE-ID` threw an `InvalidOperationException` when it was editable or, for a spec hierarchy, `IS-TABLE-INTERNAL`: those attributes were written after the `ALTERNATIVE-ID` element ([#86](https://github.com/STARIONGROUP/ReqIFSharp/issues/86)).
- `new AttributeValueXHTML(loggerFactory)` left `ExternalObjects` `null`, so `QueryExternalObjects` and `QueryBase64PayloadsAsync` of ReqIFSharp.Extensions threw for a spec element that held such a value. `new AttributeValueXHTML()` left its logger `null`, so reading into it threw ([#98](https://github.com/STARIONGROUP/ReqIFSharp/issues/98)).

## [10.1.2] - 2026-10-18

ReqIFSharp.Extensions 5.2.2

### Changed

- Update Microsoft.Sbom.Targets to version 4.1.13 ([#81](https://github.com/STARIONGROUP/ReqIFSharp/issues/81)); also for ReqIFSharp.Extensions.
- Centralize the shared MSBuild and package settings in `Directory.Build.props` and `Directory.Build.targets` ([#83](https://github.com/STARIONGROUP/ReqIFSharp/issues/83)).
- Building from source requires the .NET 10 SDK (`global.json`) and C# 14 ([#83](https://github.com/STARIONGROUP/ReqIFSharp/issues/83)).
- ReqIFSharp.Extensions: update to ReqIFSharp 10.1.2.

## [10.1.1] - 2026-09-05

ReqIFSharp.Extensions 5.2.1

### Changed

- Update dependencies.
- ReqIFSharp.Extensions: update to ReqIFSharp 10.1.1.

## [10.1.0] - 2026-06-06

ReqIFSharp.Extensions 5.2.0

### Added

- UML-like metadata decorators on the model ([#27](https://github.com/STARIONGROUP/ReqIFSharp/issues/27)).

### Changed

- Lower the minimum required version of Microsoft.Extensions.Logging.Abstractions to 6.0.0 ([#77](https://github.com/STARIONGROUP/ReqIFSharp/issues/77)).
- ReqIFSharp.Extensions: update to ReqIFSharp 10.1.0.

### Fixed

- Retain the namespaces and attributes of the root element when a document is read and written again ([#44](https://github.com/STARIONGROUP/ReqIFSharp/issues/44)).

## [10.0.6] - 2026-05-18

ReqIFSharp.Extensions 5.1.0

### Added

- Deterministic builds and packing in CI, so packages are reproducible ([#71](https://github.com/STARIONGROUP/ReqIFSharp/issues/71)).

### Changed

- ReqIFSharp.Extensions: update to ReqIFSharp 10.0.6.

### Fixed

- Dispose the manifest resource stream in `GetSchemaFromResource` ([#69](https://github.com/STARIONGROUP/ReqIFSharp/issues/69)).
- ReqIFSharp.Extensions: remove the trailing space from `ExternalObject.CreateUrl` ([#68](https://github.com/STARIONGROUP/ReqIFSharp/issues/68)).
- ReqIFSharp.Extensions: honor the `CancellationToken` in `QueryBase64PayloadsAsync` ([#67](https://github.com/STARIONGROUP/ReqIFSharp/issues/67)).
- ReqIFSharp.Extensions: dispose the streams of `ReqIFLoaderService` deterministically ([#66](https://github.com/STARIONGROUP/ReqIFSharp/issues/66)).

## [10.0.5] - 2026-05-14

ReqIFSharp.Extensions 5.0.5

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 10.0.8.
- ReqIFSharp.Extensions: update to ReqIFSharp 10.0.5.

## [10.0.4] - 2026-04-24

ReqIFSharp.Extensions 5.0.4

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 10.0.7.
- ReqIFSharp.Extensions: update to ReqIFSharp 10.0.4.

## [10.0.3] - 2026-02-20

ReqIFSharp.Extensions 5.0.3

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 10.0.3.
- ReqIFSharp.Extensions: update to ReqIFSharp 10.0.3.

## [10.0.2] - 2025-12-20

ReqIFSharp.Extensions 5.0.2

### Changed

- Update Microsoft.Sbom.Targets to version 4.1.5.
- Update Microsoft.Extensions.Logging.Abstractions to version 10.0.1.
- ReqIFSharp.Extensions: update to ReqIFSharp 10.0.2.

## [10.0.1] - 2025-11-30

ReqIFSharp.Extensions 5.0.1

### Added

- Include the LICENSE and NOTICE files in the NuGet package; the NOTICE file is updated.

### Changed

- Update Microsoft.Sbom.Targets to version 4.1.4.
- Update Microsoft.Extensions.Logging.Abstractions to version 10.0.0.
- Call `token.ThrowIfCancellationRequested()` directly.
- ReqIFSharp.Extensions: update to ReqIFSharp 10.0.1.

## [10.0.0] - 2025-11-12

ReqIFSharp.Extensions 5.0.0

### Added

- Read `SpecHierarchy.IS-TABLE-INTERNAL`.

### Changed

- **Breaking:** all `int` properties are now `long`. The ReqIF specification defines integers as mathematically unbounded; `long` is the practical choice.
- When `XmlConvert` throws an overflow exception while reading an XML attribute, the property is set to its default value ([#60](https://github.com/STARIONGROUP/ReqIFSharp/issues/60)).
- Log the line number and line position when an element is not known.
- Update Microsoft.Sbom.Targets to version 4.1.3.
- ReqIFSharp.Extensions: update to ReqIFSharp 10.0.0.

## [9.2.4] - 2025-10-18

ReqIFSharp.Extensions 4.2.3

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 9.0.10.
- ReqIFSharp.Extensions: update to ReqIFSharp 9.2.4.

## [9.2.3] - 2025-10-04

ReqIFSharp.Extensions 4.2.2

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 9.0.9.
- ReqIFSharp.Extensions: update to ReqIFSharp 9.2.3.

## [9.2.2] - 2025-09-04

No ReqIFSharp.Extensions release.

### Changed

- Update Microsoft.Sbom.Targets to version 4.1.2.

## [9.2.1] - 2025-08-08

ReqIFSharp.Extensions 4.2.1

### Added

- Generate an SBOM and include it in the NuGet package; also for ReqIFSharp.Extensions.
- Include the XML documentation in the NuGet package; also for ReqIFSharp.Extensions.

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 9.0.8.
- Improve the documentation.
- ReqIFSharp.Extensions: update to ReqIFSharp 9.2.1.

## [9.2.0] - 2025-07-16

ReqIFSharp.Extensions 4.2.0

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 9.0.7.
- ReqIFSharp.Extensions: update to ReqIFSharp 9.2.0.

## [9.1.0] - 2025-02-19

ReqIFSharp.Extensions 4.1.0

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 9.0.2.
- ReqIFSharp.Extensions: update to ReqIFSharp 9.1.0.

## [9.0.0] - 2025-01-19

ReqIFSharp.Extensions 4.0.0

### Added

- ReqIFSharp.Extensions: `ReqIFLoaderService` implements `IDisposable`.
- ReqIFSharp.Extensions: `ReqIFLoaderService` constructor that takes an `ILoggerFactory`.

### Changed

- **Breaking:** async methods that return a `Task` are renamed to end in `...Async`; also in ReqIFSharp.Extensions.
- Improve the exceptions thrown by async methods, using `nameof` throughout.
- Improve logging.
- Update Microsoft.Extensions.Logging.Abstractions to version 9.0.1.
- ReqIFSharp.Extensions: improve argument validation.
- ReqIFSharp.Extensions: update to ReqIFSharp 9.0.0.

## [8.1.1] - 2024-10-19

ReqIFSharp.Extensions 3.1.1

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 8.0.2.
- ReqIFSharp.Extensions: update to ReqIFSharp 8.1.1.

## [8.1.0] - 2024-05-14

ReqIFSharp.Extensions 3.1.0

### Changed

- Apply SonarQube improvements.
- ReqIFSharp.Extensions: update to ReqIFSharp 8.1.0.

### Fixed

- (De)serialization of `AlternativeID` ([#54](https://github.com/STARIONGROUP/ReqIFSharp/issues/54)).
- Serialization of the `xml:lang` attribute.

## [8.0.0] - 2024-04-27

ReqIFSharp.Extensions 3.0.0

### Changed

- Copyright holder is now Starion Group S.A.; also for ReqIFSharp.Extensions.

## [7.1.1] - 2024-04-13

ReqIFSharp.Extensions 2.1.1

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 8.0.1.
- ReqIFSharp.Extensions: update to ReqIFSharp 7.1.1.

## [7.1.0] - 2023-12-07

ReqIFSharp.Extensions 2.1.0

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 8.0.0.
- ReqIFSharp.Extensions: update to ReqIFSharp 7.1.0.

## [7.0.1] - 2023-07-15

ReqIFSharp.Extensions 2.0.1

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 7.0.1.
- ReqIFSharp.Extensions: update to ReqIFSharp 7.0.1.

## [7.0.0] - 2022-11-09

ReqIFSharp.Extensions 2.0.0

### Changed

- Update Microsoft.Extensions.Logging.Abstractions to version 7.0.0.
- ReqIFSharp.Extensions: update to ReqIFSharp 7.0.0.

## [6.0.1] - 2022-08-04

ReqIFSharp.Extensions 1.0.1, the first stable release of ReqIFSharp.Extensions.

### Added

- `SupportedFileExtensionKind` on the deserializer ([#47](https://github.com/STARIONGROUP/ReqIFSharp/issues/47)).
- Serialize to the `reqif`, `reqifz` and `zip` formats ([#23](https://github.com/STARIONGROUP/ReqIFSharp/issues/23)).

### Changed

- **Breaking:** `IReqIFSerializer` and `ReqIFSerializer` are made symmetrical to the `IReqIFDeSerializer` interface.
- Update Microsoft.Extensions.Logging.Abstractions to version 6.0.1.
- ReqIFSharp.Extensions: update to ReqIFSharp 6.0.1.

### Fixed

- Set the stream to position 0 before creating the `XmlReader` ([#43](https://github.com/STARIONGROUP/ReqIFSharp/issues/43)).
- `ZipArchiveEntry` disposed error when `ReqIFSerializer.SerializeAsync` writes a large `.reqifz` file.

## [ReqIFSharp.Extensions 1.0.0-RC3] - 2022-07-23

Release of ReqIFSharp.Extensions only; it uses ReqIFSharp 6.0.0-RC1.

### Added

- ReqIFSharp.Extensions: `SpecRelationTypeExtensions.QueryReferencingSpecRelations`.
- ReqIFSharp.Extensions: `SpecificationTypeExtensions.QueryReferencingSpecifications`.
- ReqIFSharp.Extensions: `SpecObjectTypeExtensions.QueryReferencingSpecObjects`.
- ReqIFSharp.Extensions: a dedicated package icon.

### Changed

- ReqIFSharp.Extensions: improve exception messages.

## [6.0.0-RC1] - 2022-07-22

ReqIFSharp.Extensions 1.0.0-RC2

Pre-release of the changes listed under 6.0.1, except the logging update and the `ZipArchiveEntry` fix.

### Changed

- ReqIFSharp.Extensions: update to ReqIFSharp 6.0.0-RC1.

## [5.0.3] - 2022-06-10

ReqIFSharp.Extensions 1.0.0-RC1

### Added

- ReqIFSharp.Extensions: first pre-release of the package. It provides query extension methods on the ReqIFSharp model (attribute definitions and values, datatype definitions, external objects, spec hierarchies, spec objects, specifications) and the `ReqIFLoaderService`.

### Changed

- Improve the XML documentation.

## [5.0.2] - 2022-04-07

### Changed

- Improve the code quality of `ReqIF.cs` as reported by SonarQube.

## [5.0.1] - 2022-01-20

### Fixed

- `ExternalObject` deserialization when the `data` attribute is not the first attribute of the `xhtml:object` element ([#39](https://github.com/STARIONGROUP/ReqIFSharp/issues/39)).

## [5.0.0] - 2022-01-01

### Added

- Deserialize from a stream, also through the `IReqIFDeSerializer` interface ([#22](https://github.com/STARIONGROUP/ReqIFSharp/issues/22)).
- Async deserialization ([#24](https://github.com/STARIONGROUP/ReqIFSharp/issues/24)).
- `ExtractUnformattedTextFromValue` to extract the unformatted text from an XHTML attribute ([#25](https://github.com/STARIONGROUP/ReqIFSharp/issues/25)).
- Serialize to a stream ([#29](https://github.com/STARIONGROUP/ReqIFSharp/issues/29)).
- Async serialization ([#31](https://github.com/STARIONGROUP/ReqIFSharp/issues/31)).
- Logging through Microsoft.Extensions.Logging.Abstractions ([#36](https://github.com/STARIONGROUP/ReqIFSharp/issues/36)).
- `CreateExternalObjectsAsync`.

### Changed

- `ExternalObject.QueryLocalData` is refactored ([#22](https://github.com/STARIONGROUP/ReqIFSharp/issues/22)).
- `ReqIFSerializer` no longer uses `XmlSerializer`; all work is done with `XmlReader` and `XmlWriter` ([#30](https://github.com/STARIONGROUP/ReqIFSharp/issues/30)).
- `CreateExternalObjects` uses an `XmlReader`.
- **Breaking:** the read and write (async) XML methods are internal; they are not part of the public API.

### Removed

- **Breaking:** validation features of `IReqIFSerializer` and `ReqIFSerializer`.
- **Breaking:** the use of `IXmlSerializable`, including the `GetSchema()` method.

### Fixed

- Deserialization of `AlternativeId` for `DatatypeDefinition`, `SpecHierarchy` and `SpecType` ([#33](https://github.com/STARIONGROUP/ReqIFSharp/issues/33)).

## [4.0.0] - 2021-11-10

### Changed

- Serialization of the `DefaultValue` property of `Identifiable` ([#21](https://github.com/STARIONGROUP/ReqIFSharp/issues/21)).
- Update the icon and license properties of the project file.

### Removed

- **Breaking:** the `netstandard1.6` and `net45` targets ([#20](https://github.com/STARIONGROUP/ReqIFSharp/issues/20)).

## [3.1.0] - 2021-06-18

### Added

- `ExternalObject` class to access the external objects referenced in `AttributeValueXHTML` instances ([#18](https://github.com/STARIONGROUP/ReqIFSharp/issues/18)).

## [3.0.1] - 2021-06-16

### Fixed

- Serialization of the default values of attribute definitions ([#17](https://github.com/STARIONGROUP/ReqIFSharp/issues/17)).

## [3.0.0] - 2021-03-21

### Changed

- **Breaking:** `IReqIFDeSerializer` returns an `IEnumerable` of `ReqIF` to improve `.reqifz` support.

### Fixed

- Deserialization and serialization of `TOOL-EXTENSIONS` ([#16](https://github.com/STARIONGROUP/ReqIFSharp/issues/16)).

## [2.0.0] - 2019-10-24

### Changed

- **Breaking:** 32-bit integers (`int`) are replaced by 64-bit integers (`long`) ([#13](https://github.com/STARIONGROUP/ReqIFSharp/issues/13)).

## [1.4.1] - 2019-02-11

### Changed

- Serialization of the `IS-SIMPLIFIED` property of the `AttributeValueXHTML` class ([#11](https://github.com/STARIONGROUP/ReqIFSharp/issues/11)).

## [1.4.0] - 2018-11-13

### Added

- Create a mock object when a referenced object is not found in the sources ([#10](https://github.com/STARIONGROUP/ReqIFSharp/issues/10)).

### Changed

- `AttributeValueXHTML` uses `XmlConvert` instead of `bool.Parse` and `bool.ToString()`.

## [1.3.2] - 2018-06-04

### Added

- Read the `MAX-LENGTH` attribute of `DatatypeDefinitionString`.
- Read the `ACCURACY` attribute of `DatatypeDefinitionReal`.

### Changed

- `ReadXml` of `DatatypeDefinitionInteger`, `DatatypeDefinitionReal` and `DatatypeDefinitionString` uses the `XmlReader`.

### Fixed

- (De)serialization of the datatype definition classes uses `XmlConvert` instead of `int.Parse` and `double.Parse`.

## [1.3.1] - 2018-06-02

### Fixed

- `ReadXml` of `AttributeDefinition.IsMultiValued`.
- `ReadXml` of `DatatypeDefinitionInteger.Max` and `DatatypeDefinitionInteger.Min`.

## [1.3.0] - 2018-03-27

### Added

- Support for `.reqifz` files.

## [1.2.0] - 2018-01-07

### Added

- `AttributeValue.ValueObject` convenience property to get and set the value of any concrete `AttributeValue` from the abstract type.

## [1.1.0] - 2017-08-02

### Added

- Support for `netstandard1.6`.

## [1.0.6] - 2017-07-31

### Changed

- Upgrade the project to the Visual Studio 2017 project file format; update the NuGet authors and icon URL.

## [1.0.5] - 2017-06-28

### Fixed

- Processing of the default values of attributes.

## [1.0.4] - 2017-06-27

### Fixed

- Reading the default value of an `AttributeDefinition`.

## [1.0.3] - 2017-06-20

### Fixed

- Handling of XHTML attributes.

## [1.0.2] - 2017-01-20

### Fixed

- The source and target of a specification during deserialization.

## [1.0.1] - 2017-01-12

### Added

- First public release under the Apache 2.0 license, after extensive testing on CDP4™.

[Unreleased]: https://github.com/STARIONGROUP/ReqIFSharp/compare/10.1.2...development
[10.1.2]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/10.1.2
[10.1.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/10.1.1
[10.1.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/10.1.0
[10.0.6]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/10.0.6
[10.0.5]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/10.0.5
[10.0.4]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/10.0.4
[10.0.3]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/10.0.3
[10.0.2]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/10.0.2
[10.0.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/10.0.1
[10.0.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/10.0.0
[9.2.4]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/9.2.4
[9.2.3]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/9.2.3
[9.2.2]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/9.2.2
[9.2.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-9.2.1
[9.2.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-9.2.0
[9.1.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-9.1.0
[9.0.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-9.0.0
[8.1.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-8.1.1
[8.1.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-8.1.0
[8.0.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-8.0.0
[7.1.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-7.1.1
[7.1.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-7.1.0
[7.0.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-7.0.1
[7.0.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-7.0.0
[6.0.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp-6.0.1
[ReqIFSharp.Extensions 1.0.0-RC3]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/reqifsharp.extensions-1.0.0-RC3
[6.0.0-RC1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/6.0.0-RC1
[5.0.3]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/RS-5.0.3
[5.0.2]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/5.0.2
[5.0.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/5.0.1
[5.0.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/5.0.0
[4.0.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/4.0.0
[3.1.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/3.1.0
[3.0.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/3.0.1
[3.0.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/3.0.0
[2.0.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/2.0.0
[1.4.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/1.4.1
[1.4.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/1.4.0
[1.3.2]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/1.3.2
[1.3.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/1.3.1
[1.3.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/V1.3.0
[1.2.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/V1.2.0
[1.1.0]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/V1.1.0
[1.0.6]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/v1.0.6
[1.0.5]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/v1.0.5
[1.0.4]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/v1.0.4
[1.0.3]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/v1.0.3
[1.0.2]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/v1.0.2
[1.0.1]: https://github.com/STARIONGROUP/ReqIFSharp/releases/tag/v1.0.1
