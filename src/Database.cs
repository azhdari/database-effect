// ReSharper disable once CheckNamespace
namespace LanguageExt.Effects.Traits;

using System;
using System.Linq;
using System.Linq.Expressions;
using Database;
using LinqToDB;
using LinqToDB.Linq;

public static class Database<R>
    where R : struct,
    HasDatabase<R>,
    HasCancel<R>
{
    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Add

    //------------------------
    // Add Generic
    public static Aff<R, TKey> Insert<T, TKey>(T entity)
        where T : class, IEntity<TKey>
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.Insert<T, TKey>(entity, cancelToken)
                    select result
            );

    public static Aff<R, TKey> Insert<T, TKey>(
        Func<IValueInsertable<T>, IValueInsertable<T>> provider
    )
        where T : class, IEntity<TKey>
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.Insert<T, TKey>(provider, cancelToken)
                    select result
            );

    //------------------------
    // Add Guid
    public static Aff<R, Guid> Insert<T>(T entity)
        where T : class, IEntity<Guid>
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.InsertGuid(entity, cancelToken)
                    select result
            );

    public static Aff<R, Guid> Insert<T>(
        Func<IValueInsertable<T>, IValueInsertable<T>> provider
    )
        where T : class, IEntity<Guid>
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.InsertGuid(provider, cancelToken)
                    select result
            );

    public static Aff<R, Guid> InsertGuid<T>(T entity)
        where T : class, IEntity<Guid>
        =>
            Insert(entity);

    public static Aff<R, Guid> InsertGuid<T>(
        Func<IValueInsertable<T>, IValueInsertable<T>> provider
    )
        where T : class, IEntity<Guid>
        =>
            Insert(provider);

    //------------------------
    // Add Int32
    public static Aff<R, Int32> InsertInt<T>(T entity)
        where T : class, IEntity<Int32>
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.Insert<T, Int32>(entity, cancelToken)
                    select result
            );

    public static Aff<R, Int32> InsertInt<T>(
        Func<IValueInsertable<T>, IValueInsertable<T>> provider
    )
        where T : class, IEntity<Int32>
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.Insert<T, Int32>(provider, cancelToken)
                    select result
            );

    //------------------------
    // Add Int64
    public static Aff<R, Int64> InsertInt64<T>(T entity)
        where T : class, IEntity<Int64>
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.Insert<T, Int64>(entity, cancelToken)
                    select result
            );

    public static Aff<R, Int64> InsertInt64<T>(
        Func<IValueInsertable<T>, IValueInsertable<T>> provider
    )
        where T : class, IEntity<Int64>
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.Insert<T, Int64>(provider, cancelToken)
                    select result
            );

    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Update
    public static Aff<R, Unit> Update<T>(T entity)
        where T : class
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.Update(entity, cancelToken)
                    select result
            );

    public static Aff<R, Unit> Update<T>(Func<ITable<T>, IUpdatable<T>> updater)
        where T : class
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.Update(updater, cancelToken)
                    select result
            );

    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Delete
    public static Aff<R, Unit> Delete<T>(Expression<Func<T, bool>> filter)
        where T : class
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.Delete(filter, cancelToken)
                    select result
            );

    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Select
    public static Aff<R, Option<T>> FindOne<T>(Expression<Func<T, bool>> filter)
        where T : class
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.FindOne(filter, cancelToken)
                    select result
            );

    public static Aff<R, Arr<T>> Find<T>(Expression<Func<T, bool>> filter)
        where T : class
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.Find(filter, cancelToken)
                    select result
            );

    public static Aff<R, int> Count<T>(Func<ITable<T>, IQueryable<T>> query)
        where T : class
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.Count(query, cancelToken)
                    select result
            );

    public static Aff<R, DataAndCount<T>> FindAndCount<T>(
        DataLimit limit,
        Func<IQueryable<T>, IQueryable<T>> query
    )
        where T : class
        =>
            default(R).Database.Bind(
                rt =>
                    from cancelToken in cancelToken<R>()
                    from result in rt.FindAndCount(limit, query, cancelToken)
                    select result
            );

    public static Aff<R, Arr<A>> Find<T, A>(Func<ITable<T>, IQueryable<A>> filter)
        where T : class
        =>
            default(R).Database.Bind(rt => rt.Table<T>()).
                Map(filter).
                Bind(
                    query =>
                        from cancelToken in cancelToken<R>()
                        from result in query.ToListAsync(cancelToken).
                            ToAff().
                            Map(toArray)
                        select result
                );

    public static Aff<R, ITable<T>> Table<T>()
        where T : class
        =>
            default(R).Database.Bind(rt => rt.Table<T>());

    public static Aff<R, IQueryable<A>> GetCte<T, A>(Func<ITable<T>, IQueryable<A>> body, Option<string> name = default)
        where T : class
        =>
            default(R).Database.Bind(rt => rt.GetCte(body, name));

    public static Aff<R, IQueryable<T>> GetRecursiveCte<T>(
        Func<IQueryable<T>, IQueryable<T>> body,
        Option<string> name = default
    )
        where T : class
        =>
            default(R).Database.Bind(rt => rt.GetRecursiveCte(body, name));
}