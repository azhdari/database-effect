# Database Effect for LanguageExt

[![latest version](https://img.shields.io/nuget/v/LanguageExt.Effects.Database)](https://www.nuget.org/packages/LanguageExt.Effects.Database) [![preview version](https://img.shields.io/nuget/vpre/LanguageExt.Effects.Database)](https://www.nuget.org/packages/LanguageExt.Effects.Database/absoluteLatest) [![downloads](https://img.shields.io/nuget/dt/LanguageExt.Effects.Database)](https://www.nuget.org/packages/LanguageExt.Effects.Database)

!Note:
*Currently DatabaseEff only supports .net6* .   
!Note:
*This is absolutely a new project, so use it carefully* .  

## Dependencies
This library depends on both `EfCore 6` and `Linq2Db` (Linq2Db for EfCore).  
Eventually I'll pick one of these, but for now, I experiment both.

---

## Installation
DatabaseEff is available on NuGet.  

```
dotnet add package LanguageExt.Effects.Database
```
Use the `--prerelease` option to install latest preview version.

Add this line to your project's startup file:
```csharp
services.AddLinq2DbForDatabaseEff();
```

---

## Documentation
[In progress]

## Samples
[In progress]
