![ReqIFSharp](https://raw.githubusercontent.com/STARIONGROUP/reqifsharp/master/reqifsharp-logo.png)

**Read, write and query OMG ReqIF requirements documents from any .NET application.**

[![ReqIFSharp on NuGet](https://img.shields.io/nuget/v/ReqIFSharp?label=ReqIFSharp)](https://www.nuget.org/packages/ReqIFSharp)
[![ReqIFSharp.Extensions on NuGet](https://img.shields.io/nuget/v/ReqIFSharp.Extensions?label=ReqIFSharp.Extensions)](https://www.nuget.org/packages/ReqIFSharp.Extensions)
[![NuGet downloads](https://img.shields.io/nuget/dt/ReqIFSharp)](https://www.nuget.org/packages/ReqIFSharp)
[![Build status](https://github.com/STARIONGROUP/reqifsharp/actions/workflows/CodeQuality.yml/badge.svg?branch=development)](https://github.com/STARIONGROUP/reqifsharp/actions/workflows/CodeQuality.yml)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=coverage)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)
[![License: Apache 2.0](https://img.shields.io/badge/license-Apache%202.0-blue)](https://github.com/STARIONGROUP/reqifsharp/blob/development/LICENSE)

---

[ReqIF](https://www.omg.org/spec/ReqIF/) (Requirements Interchange Format) is the OMG standard that requirements tools such as IBM DOORS, Siemens Polarion, PTC Codebeamer and ReqIF Studio use to exchange specifications. **ReqIFSharp** gives you the complete ReqIF object model in C#, so you can import requirements into your own application, export them to other tools, or build pipelines that check, transform and report on them.

## Read a ReqIF file in ten lines

```csharp
using ReqIFSharp;
using ReqIFSharp.Extensions.ReqIFExtensions;

var reqif = new ReqIFDeserializer().Deserialize("traceability.reqif").First();

Console.WriteLine($"{reqif.TheHeader.Title}, exported by {reqif.TheHeader.SourceToolId}");

foreach (var specification in reqif.CoreContent.Specifications)
{
    Console.WriteLine(specification.LongName);

    foreach (var requirement in specification.QueryAllContainedSpecObjects())
    {
        var id   = requirement.Values.OfType<AttributeValueString>().FirstOrDefault()?.TheValue;
        var text = requirement.Values.OfType<AttributeValueXHTML>().FirstOrDefault()?.ExtractUnformattedTextFromValue();

        Console.WriteLine($"  {id,-7} {text}");
    }
}
```

```text
Traceability Template, exported by fmStudio (http://formalmind.com/studio)
Requirements Document
  REQ-1   Stakeholder Requirements
  REQ-2   This is a simple template that demonstrates the traceability capabilities of ReqIF Studio.
  REQ-6   Simply delete these instructions in the Stakeholder Requirements and System Requirements, and you are good to go.
  REQ-7   Traceability Basics
  REQ-3   You can quickly recognize whether outgoing traces exist by looking into the right-most column (called Link).
  ...
```

For text without the extra spaces around inline elements, and optionally with a line break between paragraphs, use `ExtractNormalizedTextFromValue()` (or `ExtractNormalizedTextFromValue(keepLineBreaks: true)`).

## Write one just as easily

```csharp
var content = new ReqIFContent();
var reqif = new ReqIF
{
    Lang = "en",
    TheHeader = new ReqIFHeader { Identifier = "header-1", Title = "Coffee machine", CreationTime = DateTime.UtcNow,
                                  ReqIFToolId = "my-tool", ReqIFVersion = "1.0", SourceToolId = "my-tool" },
    CoreContent = content
};

var text = new DatatypeDefinitionString { Identifier = "dt-text", LongName = "Text", MaxLength = 1000, LastChange = DateTime.UtcNow };
var textAttribute = new AttributeDefinitionString { Identifier = "ad-text", LongName = "Text", Type = text, LastChange = DateTime.UtcNow };
var requirementType = new SpecObjectType { Identifier = "sot-req", LongName = "Requirement", LastChange = DateTime.UtcNow };
requirementType.SpecAttributes.Add(textAttribute);

var requirement = new SpecObject { Identifier = "req-1", Type = requirementType, LastChange = DateTime.UtcNow };
requirement.Values.Add(new AttributeValueString { Definition = textAttribute, TheValue = "The machine shall brew a cup in under 30 seconds." });

content.DataTypes.Add(text);
content.SpecTypes.Add(requirementType);
content.SpecObjects.Add(requirement);

new ReqIFSerializer().Serialize(new[] { reqif }, "coffee.reqifz");   // or .reqif
```

The [Writing ReqIF](https://github.com/STARIONGROUP/reqifsharp/wiki/Writing-ReqIF) guide turns this into a full, schema-valid document with a specification tree and trace links.

## Why ReqIFSharp

- **The whole standard.** Datatypes, spec types, spec objects, specification hierarchies, relations and relation groups, enumerations and rich XHTML text: the complete ReqIF 1.2 object model, as plain C# classes.
- **Every file format.** `.reqif` XML, and `.reqifz` / `.zip` archives that hold several ReqIF documents plus embedded images and other attachments.
- **Optional schema validation.** Validate against the official ReqIF and XHTML schemas. They are embedded in the library, so validation works offline.
- **Faithful round-trips.** Namespace declarations and attributes of the original document are kept when you read a file and write it back.
- **Sync and async.** Every read and write has an `...Async` variant with cancellation support, from files or streams.
- **Query helpers.** `ReqIFSharp.Extensions` answers questions like "which specifications contain this requirement?", "which trace links start here?" and "show me this attribute value as text".
- **Runs everywhere .NET runs.** Targets `netstandard2.0`: .NET Framework 4.6.2+, .NET 6, 8 and 10, on Windows, Linux and macOS.
- **Built for regulated work.** Apache 2.0 licensed, a single runtime dependency, reproducible builds and an SBOM inside every NuGet package.

## Installation

```shell
dotnet add package ReqIFSharp
dotnet add package ReqIFSharp.Extensions   # optional: query helpers and a loader service
```

| Package | What it gives you |
| --- | --- |
| [ReqIFSharp](https://www.nuget.org/packages/ReqIFSharp) | The ReqIF object model, `ReqIFDeserializer` and `ReqIFSerializer`. |
| [ReqIFSharp.Extensions](https://www.nuget.org/packages/ReqIFSharp.Extensions) | Query extension methods over the model, and `ReqIFLoaderService` to load and cache a document and its attachments. |

## See it in action

Open any `.reqif` or `.reqifz` file in the browser with the [ReqIF Viewer](https://viewer.reqifsharp.org), a web application built on ReqIFSharp. The source is at [STARIONGROUP/reqifviewer](https://github.com/STARIONGROUP/reqifviewer).

## Documentation

The [wiki](https://github.com/STARIONGROUP/reqifsharp/wiki) has the guides:

- [Quick Start](https://github.com/STARIONGROUP/reqifsharp/wiki/Quick-Start): install, read and write a document in five minutes
- [ReqIF Concepts](https://github.com/STARIONGROUP/reqifsharp/wiki/ReqIF-Concepts): how a ReqIF document is structured, with a diagram
- [Reading ReqIF](https://github.com/STARIONGROUP/reqifsharp/wiki/Reading-ReqIF): files, streams, archives, validation, attribute values and logging
- [Writing ReqIF](https://github.com/STARIONGROUP/reqifsharp/wiki/Writing-ReqIF): build a document from scratch
- [ReqIFSharp.Extensions](https://github.com/STARIONGROUP/reqifsharp/wiki/ReqIFSharp-Extensions): query helpers, attachments and the loader service
- [FAQ](https://github.com/STARIONGROUP/reqifsharp/wiki/FAQ)

Release notes are in the [CHANGELOG](https://github.com/STARIONGROUP/reqifsharp/blob/development/CHANGELOG.md).

## Dependencies

`ReqIFSharp` has a single runtime dependency: `Microsoft.Extensions.Logging.Abstractions`, with a deliberately low minimum version of `6.0.0`. A NuGet package reference is a *minimum*, so the low floor keeps the library usable from older applications.

On .NET 8, 10 or any other modern runtime you don't have to do anything: NuGet picks the version your application already uses. To be explicit, reference the version that matches your runtime in your own project:

```xml
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.0" />
```

## Software Bill of Materials (SBOM)

Every NuGet package contains an SPDX 2.2 SBOM at `_manifest/spdx_2.2/manifest.spdx.json`, generated during the build. It lists the components and dependencies of the package with their versions, so you can track vulnerabilities and audit what you ship. See the [SBOM](https://github.com/STARIONGROUP/reqifsharp/wiki/SBOM) page for how to extract it.

## Contributing

Contributions are welcome: bug reports, documentation fixes and pull requests alike. Read the [contribution guidelines](https://github.com/STARIONGROUP/reqifsharp/blob/development/.github/CONTRIBUTING.md) first. Before we can accept a code contribution, we ask contributors to sign the Contributor License Agreement (CLA) and send it digitally signed to s.gerene@stariongroup.eu. The CLAs are in the [CLA](https://github.com/STARIONGROUP/reqifsharp/tree/development/CLA) folder.

## License

**ReqIFSharp** and **ReqIFSharp.Extensions** are provided to the community under the [Apache License 2.0](https://github.com/STARIONGROUP/reqifsharp/blob/development/LICENSE).
