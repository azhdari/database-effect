namespace LanguageExt.Effects.Traits;

using System;
using System.Linq;
using System.Linq.Expressions;
using LanguageExt.Effects.Database;
using LinqToDB;
using LinqToDB.Extensions;
using LinqToDB.Linq;

public static class Database<R>
    where R : struct,
              HasDatabase<R>,
              HasCancel<R>
{
    public static Aff<R, TKey> Insert<T, TKey>(T entity, CancellationToken token = default)
        where T : class, IEntity<TKey>
        =>
        default(R).Database.Bind(rt => rt.Insert<T, TKey>(entity, token));

    public static Aff<R, TKey> Insert<T, TKey>(Func<IValueInsertable<T>, IValueInsertable<T>> provider, CancellationToken token = default)
        where T : class, IEntity<TKey>
        =>
        default(R).Database.Bind(rt => rt.Insert<T, TKey>(provider, token));

    public static Aff<R, Unit> Update<T>(T entity, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.Update<T>(entity, token));

    public static Aff<R, Unit> Update<T>(Func<ITable<T>, IUpdatable<T>> updater, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.Update<T>(updater, token));

    public static Aff<R, Unit> Delete<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.Delete<T>(filter, token));

    public static Aff<R, Option<T>> FindOne<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.FindOne<T>(filter, token));

    public static Aff<R, Arr<T>> Find<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.Find<T>(filter, token));

    public static Aff<R, Arr<A>> Find<T, A>(Func<ITable<T>, IQueryable<A>> filter, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.Table<T>())
                           .Map(table => filter(table))
                           .Bind(query => query.ToListAsync(token)
                                               .ToAff()
                                               .Map(toArray));

    public static Aff<R, ITable<T>> Table<T>()
        where T : class
        =>
        default(R).Database.Bind(rt => rt.Table<T>());

    public static Aff<R, IQueryable<A>> GetCte<T, A>(Func<ITable<T>, IQueryable<A>> body, Option<string> name = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.GetCte<T, A>(body, name));

    public static Aff<R, IQueryable<T>> GetRecursiveCte<T>(Func<IQueryable<T>, IQueryable<T>> body, Option<string> name = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.GetRecursiveCte(body, name));
}
