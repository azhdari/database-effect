# Database Effect for LanguageExt

[![latest version](https://img.shields.io/nuget/v/LanguageExt.Effects.Database)](https://www.nuget.org/packages/LanguageExt.Effects.Database) [![preview version](https://img.shields.io/nuget/vpre/LanguageExt.Effects.Database)](https://www.nuget.org/packages/LanguageExt.Effects.Database/absoluteLatest) [![downloads](https://img.shields.io/nuget/dt/LanguageExt.Effects.Database)](https://www.nuget.org/packages/LanguageExt.Effects.Database)

!Note:
*Currently DatabaseEff only supports .net6* .   
!Note:
*This project is under heavy development at this stage, so everything is subject to change!*

## Dependencies
This library depends on `Linq2db` ([github repo](https://github.com/linq2db/linq2db)).

---

## Installation
DatabaseEff is available on NuGet.  

```
dotnet add package LanguageExt.Effects.Database
```
Use the `--prerelease` option to install latest preview version.

### Approach A. Global connection
Add this line to your project's startup file:
```csharp
services.AddDatabaseEff(Configuration, "ConnectionStringName");
```

### Approach B. Configure Multiple DataContext
You can use `OptionBuilder` to configure multiple `DataContext`:
```csharp
var fooOptionBuilder = services.GetDatabaseBuildOptions(Configuration, "FooConnectionString");
builder.Services.AddLinqToDbContext<FooDbContext>(options);

var barOptionBuilder = services.GetDatabaseBuildOptions(Configuration, "BarConnectionString");
builder.Services.AddLinqToDbContext<BarDbContext>(options);
```

---

## Documentation
[In progress]

## Samples
[In progress]
