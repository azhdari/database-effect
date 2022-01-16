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
    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Add

    //------------------------
    // Add Generic
    public static Aff<R, TKey> Insert<T, TKey>(T entity, CancellationToken token = default)
        where T : class, IEntity<TKey>
        =>
        default(R).Database.Bind(rt => rt.Insert<T, TKey>(entity, token));

    public static Aff<R, TKey> Insert<T, TKey>(Func<IValueInsertable<T>, IValueInsertable<T>> provider, CancellationToken token = default)
        where T : class, IEntity<TKey>
        =>
        default(R).Database.Bind(rt => rt.Insert<T, TKey>(provider, token));
    
    //------------------------
    // Add Guid
    public static Aff<R, Guid> Insert<T>(T entity, CancellationToken token = default)
        where T : class, IEntity<Guid>
        =>
        default(R).Database.Bind(rt => rt.InsertGuid<T>(entity, token));

    public static Aff<R, Guid> Insert<T>(Func<IValueInsertable<T>, IValueInsertable<T>> provider, CancellationToken token = default)
        where T : class, IEntity<Guid>
        =>
        default(R).Database.Bind(rt => rt.InsertGuid<T>(provider, token));

    public static Aff<R, Guid> InsertGuid<T>(T entity, CancellationToken token = default)
        where T : class, IEntity<Guid>
        =>
        Insert(entity, token);

    public static Aff<R, Guid> InsertGuid<T>(Func<IValueInsertable<T>, IValueInsertable<T>> provider, CancellationToken token = default)
        where T : class, IEntity<Guid>
        =>
        Insert(provider, token);
    
    //------------------------
    // Add Int32
    public static Aff<R, Int32> InsertInt<T>(T entity, CancellationToken token = default)
        where T : class, IEntity<Int32>
        =>
        default(R).Database.Bind(rt => rt.Insert<T, Int32>(entity, token));

    public static Aff<R, Int32> InsertInt<T>(Func<IValueInsertable<T>, IValueInsertable<T>> provider, CancellationToken token = default)
        where T : class, IEntity<Int32>
        =>
        default(R).Database.Bind(rt => rt.Insert<T, Int32>(provider, token));
    
    //------------------------
    // Add Int64
    public static Aff<R, Int64> InsertInt64<T>(T entity, CancellationToken token = default)
        where T : class, IEntity<Int64>
        =>
        default(R).Database.Bind(rt => rt.Insert<T, Int64>(entity, token));

    public static Aff<R, Int64> InsertInt64<T>(Func<IValueInsertable<T>, IValueInsertable<T>> provider, CancellationToken token = default)
        where T : class, IEntity<Int64>
        =>
        default(R).Database.Bind(rt => rt.Insert<T, Int64>(provider, token));
    
    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Update
    public static Aff<R, Unit> Update<T>(T entity, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.Update<T>(entity, token));

    public static Aff<R, Unit> Update<T>(Func<ITable<T>, IUpdatable<T>> updater, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.Update<T>(updater, token));
    
    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Delete
    public static Aff<R, Unit> Delete<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.Delete<T>(filter, token));
    
    // /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Select
    public static Aff<R, Option<T>> FindOne<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.FindOne<T>(filter, token));

    public static Aff<R, Arr<T>> Find<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.Find<T>(filter, token));

    public static Aff<R, int> Count<T>(Func<ITable<T>, IQueryable<T>> query, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.Count<T>(query, token));

    public static Aff<R, DataAndCount<T>> FindAndCount<T>(DataLimit limit, Func<IQueryable<T>, IQueryable<T>> query, CancellationToken token = default)
        where T : class
        =>
        default(R).Database.Bind(rt => rt.FindAndCount<T>(limit, query, token));

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
