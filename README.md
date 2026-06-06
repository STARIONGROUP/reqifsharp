![reqifsharp](https://raw.githubusercontent.com/STARIONGROUP/reqifsharp/master/reqifsharp-logo.png)

**ReqIFSharp** and **ReqIFSharp.Extensions** are C# libraries for reading and writing ReqIF documents. ReqIFSharp is used in Starion products such as [CDP4-COMET](https://www.stariongroup.eu/services-solutions/system-engineering/concurrent-design/cdp4-comet/) and a web based ReqIF [Viewer](https://viewer.reqifsharp.org). Read more about it here: https://reqifsharp.org

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=coverage)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_reqifsharp&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_reqifsharp)

## Installation

The packages are available on Nuget:

- [ReqIFSharp](https://www.nuget.org/packages/ReqIFSharp): ![NuGet Version](https://img.shields.io/nuget/v/ReqIFSharp)
- [ReqIFSharp.Extensions](https://www.nuget.org/packages/ReqIFSharp.Extensions): ![NuGet Version](https://img.shields.io/nuget/v/ReqIFSharp.Extensions)

## Dependencies

`ReqIFSharp` targets `netstandard2.0` and has a single runtime dependency: `Microsoft.Extensions.Logging.Abstractions`. Its version is **deliberately floored at `6.0.0`** - the lowest version that exposes every logging API the library uses. Because a NuGet package reference is a *minimum*, a low floor keeps the library broadly consumable instead of forcing everyone onto the latest major.

**Using ReqIFSharp from .NET 8/10 (or any modern app)?** You don't have to do anything - NuGet automatically floats the dependency up to the version your application already uses (e.g. `Microsoft.Extensions.Logging.Abstractions 10.x` on .NET 10). If you want to be explicit, simply add a direct reference in your own project to the version that matches your runtime:

```xml
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.0" />
```

This pins the resolved version in your application without ReqIFSharp imposing it on consumers who are still on older lines.

## Build Status

GitHub actions are used to build and test the library

Branch | Build Status
------- | :------------
Master | ![Build Status](https://github.com/STARIONGROUP/reqifsharp/actions/workflows/CodeQuality.yml/badge.svg?branch=master)
Development | ![Build Status](https://github.com/STARIONGROUP/reqifsharp/actions/workflows/CodeQuality.yml/badge.svg?branch=development)

## Software Bill of Materials (SBOM)

As part of our commitment to security and transparency, this project includes a Software Bill of Materials (SBOM) in the associated NuGet packages. The SBOM provides a detailed inventory of the components and dependencies included in the package, allowing you to track and verify the software components, their licenses, and versions.

**Why SBOM?**

- **Improved Transparency**: Gain insight into the open-source and third-party components included in this package.
- **Security Assurance**: By providing an SBOM, we enable users to more easily track vulnerabilities associated with the included components.
- **Compliance**: SBOMs help ensure compliance with licensing requirements and make it easier to audit the project's dependencies.

You can find the SBOM in the NuGet package itself, which is automatically generated and embedded during the build process.

# License

**ReqIFSharp** and **ReqIFSharp.Extensions** are provided to the community under the Apache License 2.0 License.

# Contributions

Contributions to the code-base are welcome. However, before we can accept your contributions we ask any contributor to sign the Contributor License Agreement (CLA) and send this digitaly signed to s.gerene@stariongroup.eu. You can find the CLA's in the CLA folder.