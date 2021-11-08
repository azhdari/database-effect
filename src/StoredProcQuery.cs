namespace LanguageExt.Effects.Database;

using System.Data.Common;
using Microsoft.EntityFrameworkCore;

public record class StoredProcQuery(string Name, DbContext Context)
{
    public Arr<DbParameter> Parameters { get; init; } = Arr<DbParameter>.Empty;

    public override string ToString()
    {
        if (string.IsNullOrEmpty(Name))
        {
            throw new Exception("StoredProc name is not provided");
        }

        string param = string.Join(", ", Parameters.Where(x => x.Direction != System.Data.ParameterDirection.ReturnValue)
                                                   .Select(x => $"@{x.ParameterName} = @{x.ParameterName} {(x.Direction != System.Data.ParameterDirection.Input ? "OUTPUT" : string.Empty)}"));
        return $"{Name} {param}".Trim();
    }

    public IQueryable<TEntity> AsIQueryable<TEntity>()
        where TEntity : class
        =>
        Context.Set<TEntity>().FromSqlRaw(ToString(), Parameters.OfType<object>().ToArray());

    public Aff<Unit> Execute()
        =>
        Context.Database.ExecuteSqlRawAsync(ToString(), Parameters.OfType<object>().ToArray()).ToUnit().ToAff();
}