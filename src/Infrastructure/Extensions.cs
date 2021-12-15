namespace LinqToDB;

using LanguageExt.Effects.Database;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Linq;
using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

public static class Linq2Db
{
    public static int UpdateConcurrent<T, TKey>(this IDataContext dc, T obj)
        where T : class, IConcurrency<TKey>
        where TKey : IEquatable<TKey> {
        
        var stamp = Guid.NewGuid().ToString();
        
        var query = dc.GetTable<T>()
            .Where(_ => _.Id.Equals(obj.Id) && _.ConcurrencyStamp == obj.ConcurrencyStamp)
            .Set(_ => _.ConcurrencyStamp, stamp);
        
        var ed = dc.MappingSchema.GetEntityDescriptor(typeof(T));
        var p = Expression.Parameter(typeof(T));
        foreach (
            var column in
            ed.Columns.Where(_ => _.MemberName != nameof(IConcurrency<TKey>.ConcurrencyStamp) && !_.IsPrimaryKey && !_.SkipOnUpdate)
        ) {
            var expr = Expression.Lambda<Func<T, object>>(
                    Expression.Convert(Expression.PropertyOrField(p, column.MemberName), typeof(object)), p);

            var val = column.MemberAccessor.Getter?.Invoke(obj);

            if (expr is not null) {
                #pragma warning disable CS8620
                query = query.Set(expr, val);
                #pragma warning restore CS8620
            }
        }

        var res = query.Update();
        obj.ConcurrencyStamp = stamp;

        return res;
    }

    public static async Task<Guid?> InsertWithGuidIdentityAsync<T>(this ITable<T> target, Expression<Func<T>> setter, CancellationToken token = default)
        where T : notnull,
                  IEntity<Guid> {
        var idObj = await target.InsertWithIdentityAsync(setter, token);
        if (idObj is not null && Guid.TryParse(idObj.ToString(), out var id)) {
            return id;
        } else {
            return default;
        }
    }

    public static async Task<Guid?> InsertWithGuidIdentityAsync<T>(this IDataContext _, IValueInsertable<T> provider, CancellationToken token = default)
        where T : notnull,
                  IEntity<Guid> {
        var idObj = await provider.InsertWithIdentityAsync(token);
        if (idObj is not null && Guid.TryParse(idObj.ToString(), out var id)) {
            return id;
        } else {
            return default;
        }
    }

    public static async Task<Guid?> InsertWithGuidIdentityAsync<T>(this IDataContext dataContext, T obj, string? tableName = null, string? databaseName = null, string? schemaName = null, string? serverName = null, TableOptions tableOptions = TableOptions.NotSet, CancellationToken token = default)
        where T : notnull,
                  IEntity<Guid> {
        var idObj = await dataContext.InsertWithIdentityAsync(obj, tableName, databaseName, schemaName, serverName, tableOptions, token);
        if (idObj is not null && Guid.TryParse(idObj.ToString(), out var id)) {
            return id;
        } else {
            return default;
        }
    }

    public static async Task<TKey?> InsertWithIdentityAsync<T, TKey>(this ITable<T> target, Expression<Func<T>> setter, CancellationToken token = default)
        where T : notnull,
                  IEntity<TKey> {
        var idObj = await target.InsertWithIdentityAsync(setter, token);
        if (idObj is not null) {
            return target.DataContext.MappingSchema.ChangeTypeTo<TKey>(idObj);
        } else {
            return default;
        }
    }

    public static async Task<TKey?> InsertWithIdentityAsync<T, TKey>(this IDataContext dataContext, IValueInsertable<T> provider, CancellationToken token = default)
        where T : notnull,
                  IEntity<TKey> {
        var idObj = await provider.InsertWithIdentityAsync(token);
        if (idObj is not null) {
            return dataContext.MappingSchema.ChangeTypeTo<TKey>(idObj);
        } else {
            return default;
        }
    }

    public static async Task<TKey?> InsertWithIdentityAsync<T, TKey>(this IDataContext dataContext, T obj, string? tableName = null, string? databaseName = null, string? schemaName = null, string? serverName = null, TableOptions tableOptions = TableOptions.NotSet, CancellationToken token = default)
        where T : notnull,
                  IEntity<TKey> {
        var idObj = await dataContext.InsertWithIdentityAsync(obj, tableName, databaseName, schemaName, serverName, tableOptions, token);
        if (idObj is not null) {
            return dataContext.MappingSchema.ChangeTypeTo<TKey>(idObj);
        } else {
            return default;
        }
    }
}