# Database Effect for LanguageExt

[![latest version](https://img.shields.io/nuget/v/LanguageExt.Effects.Database)](https://www.nuget.org/packages/LanguageExt.Effects.Database) [![preview version](https://img.shields.io/nuget/vpre/LanguageExt.Effects.Database)](https://www.nuget.org/packages/LanguageExt.Effects.Database/absoluteLatest) [![downloads](https://img.shields.io/nuget/dt/LanguageExt.Effects.Database)](https://www.nuget.org/packages/LanguageExt.Effects.Database)

## Dependencies
This library depends on `Linq2db` ([github repo](https://github.com/linq2db/linq2db)).

---

## Installation
DatabaseEff is available on NuGet.  

```bash
dotnet add package LanguageExt.Effects.Database
```
Use the `--prerelease` option to install latest preview version.

---

## Getting started

### 1️⃣ Runtime
Add DatabaseIO to your Runtime.

```csharp
using LanguageExt.Effects.Traits;

public class Runtime
    : HasDatabase<Runtime>
{
    private readonly RuntimeEnv _env;

    private Runtime(RuntimeEnv env) =>
        _env = env;

    private RuntimeEnv Env =>
        _env ??
        throw new InvalidOperationException(
            "Runtime Env not set. Perhaps because of using default(Runtime) or new Runtime() rather than Runtime.New()"
        );

    public static Runtime New(DatabaseIO database) =>
        new (new RuntimeEnv(new CancellationTokenSource(), database));

    public Runtime LocalCancel => new (
        new RuntimeEnv(
            new CancellationTokenSource(),
            Env.Database
        )
    );


    public CancellationToken CancellationToken => Env.Token;
    public CancellationTokenSource CancellationTokenSource => Env.Source;

    public Aff<Runtime, DatabaseIO> Database => Eff<Runtime, DatabaseIO>((rt) => rt._env.Database);
    
    private record RuntimeEnv(
        CancellationTokenSource Source,
        CancellationToken Token,
        DatabaseIO Database,
    )
    {
        public RuntimeEnv(
            CancellationTokenSource source,
            DatabaseIO database,
        ) :
            this(source, source.Token, database) {
        }

        public RuntimeEnv LocalCancel {
            get {
                var source = new CancellationTokenSource();
                return this with { Source = source, Token = source.Token };
            }
        }
    }
}
```

### 2️⃣ Prepare Connection
Creating database connection is pretty easy in Linq2db.
```csharp
var db = new LinqToDB.Data.DataConnection(
  LinqToDB.ProviderName.SqlServer2012,
  "Server=.\;Database=Northwind;Trusted_Connection=True;Enlist=False;");
```
Find more details in [the project repo](https://github.com/linq2db/linq2db/blob/master/README.md)
  
Note: Adding the DbContext to the DI is recommended.

```csharp
public class CustomDbContext : DataConnection
{
    public CustomDbContext(LinqToDbConnectionOptions<FileDbContext> options) : base(options) {
    }
}
```

### 3️⃣ Entities

```csharp
using LanguageExt.Effects.Database;
using LinqToDB.Mapping;

namespace Entities;

[Table("Book")]
public record BookEntity(
    [property: Column(SkipOnInsert = true, IsPrimaryKey = true)] Guid Id,
    [property: Column] string Name,
    [property: Column] DateTimeOffset CreatedAt,
);
```
__Core Domain__
```csharp
public record NewBook(string Name, DateTimeOffset CreatedAt);
public record Book(Guid Id, string Name, DateTimeOffset CreatedAt);
```

### 4️⃣ Creating Repository
```csharp
namespace Repositories;

public interface HasBookRepo<R> : HasDatabase<R>
    where R : struct,
    HasDatabase<R>,
    HasBookRepo<R>
{
}

public static class BookRepo<R>
    where R : struct,
    HasBookRepo<R>
{
    public static Aff<R, Book> Create(NewBook item)
        =>
            from id in Database<R>.Insert(new BookEntity(Guid.Empty, item.Name, item.CreatedAt))
            select new Book(id, item.Name, item.CreatedAt);
}

```

### 5️⃣ Using Repository
```csharp
public static class Functions
{
    public static Aff<R, Book> DoSomething<R>()
        where R : struct,
        HasBookRepo<R>,
        HasTime<R>
        =>
            from now in Time<R>.now
            let model = new NewBook("The Great Book", now)
            from result in BookRepo<R>.Create(model)
            select result;
}
```

### 6️⃣ Run the app

```csharp
var dbContext = serviceProvider.GetService<CustomDbContext>();
var database = new LanguageExt.Effects.Database.DatabaseLive(dbContext);

var runtime = Runtime.New(database);

var effect = Functions.DoSomething<Runtime>();
var effectResult = await effect.Run(runtime);
```
