# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ReqIFSharp is a .NET library to deserialize (read) and serialize (write) OMG ReqIF
requirements-interchange documents. The repository ships two NuGet packages:

- **ReqIFSharp** — the core object model plus (de)serializers. Targets `netstandard2.0`.
- **ReqIFSharp.Extensions** — query/convenience extension methods and the
  `ReqIFLoaderService`, built on top of the core. Targets `netstandard2.0`.

The two packages version independently (e.g. ReqIFSharp `10.x`, ReqIFSharp.Extensions `5.x`).
Test projects target `net10.0` and use NUnit. `LangVersion` is pinned to `12.0`.

## Common commands

```powershell
dotnet build ReqIFSharp.sln -c Release
dotnet test  ReqIFSharp.sln                                    # all tests
dotnet test  ReqIFSharp.Tests/ReqIFSharp.Tests.csproj          # one project
dotnet test --filter "FullyQualifiedName~ReqIFDeSerializerTestFixture"   # one fixture
dotnet test --filter "Name=YourTestMethod"                     # one test

# coverage (opencover format, see coverlet.runsettings)
dotnet test ReqIFSharp.sln --settings coverlet.runsettings
```

## Architecture

**Object graph.** `ReqIF` is the document root (`ReqIF.cs`). It holds a `ReqIFHeader`
and a `ReqIFContent` (the `CoreContent`), which in turn contains datatypes, spec types,
spec objects, specifications, and relations. Most domain classes derive from
`Identifiable` / `AccessControlledElement` and know how to read and write themselves
via internal `ReadXml`/`WriteXml`(`Async`) methods — serialization logic lives on the
model classes, not in a central mapper.

**Deserialize / serialize.** `ReqIFDeserializer` (`IReqIFDeSerializer`) and
`ReqIFSerializer` (`IReqIFSerializer`) are the public entry points. Both handle a plain
`.reqif` XML file/stream **or** a `.reqifz` zip archive that may contain multiple ReqIF
documents plus embedded external objects — hence the APIs operate on
`IEnumerable<ReqIF>` and take a `SupportedFileExtensionKind`. Every operation has a sync
and an `...Async` (CancellationToken) variant; keep both in sync when changing
serialization behavior.

**Polymorphic XML mapping.** `ReqIfFactory` (internal, static) is the single place that
maps concrete subtypes (attribute definitions, datatypes, attribute values, spec types)
to/from their ReqIF XML element names. When adding a new concrete model subtype, update
`ReqIfFactory` in addition to the class's own `ReadXml`/`WriteXml`.

**Schema validation.** The full ReqIF + XHTML XSD set is embedded as resources in
`ReqIFSharp/Resources` (see `EmbeddedResource` entries in `ReqIFSharp.csproj`).
`ReqIfSchemaResolver` resolves XSD imports against these embedded resources so
validation works without filesystem access. Deserialization can optionally validate and
report through a `ValidationEventHandler`.

**Original-XML round-tripping.** `ReqIF` retains the XML attributes/namespaces seen on
read so re-serialization reproduces the original document's namespace declarations.
Preserve this when touching read/write of the root element.

**Logging / DI.** Core types take an optional `ILoggerFactory` and fall back to
`NullLogger` when none is supplied — there is no other DI container. Test projects wire
Serilog into `Microsoft.Extensions.Logging` and assert on log output via
`TestLogEventSink`.

**ReqIFSharp.Extensions.** `ReqIFExtensions/*Extensions.cs` are stateless query helpers
over the core model (navigating spec hierarchies, resolving attribute values, etc.).
`Services/ReqIFLoaderService` (`IReqIFLoaderService`) loads and caches a ReqIF source so
the extensions can be used by consuming applications (e.g. the web viewer).

## Conventions

- Every source file starts with the Apache-2.0 Starion Group copyright header
  (`Copyright 2017-2026 Starion Group S.A.`). Copy it onto any new file.
- Tests use NUnit 4 (`[TestFixture]` / `[Test]`); fixtures are named `*TestFixture`.
  Sample ReqIF inputs live in `ReqIFSharp.Tests/TestData` and are copied to output.
- Contributions require a signed CLA (see `CLA/`); `development` is the main branch.
