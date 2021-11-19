namespace LanguageExt.Effects.Traits;

using System;
using System.Linq;
using System.Linq.Expressions;
using LanguageExt.Effects.Database;
using LinqToDB;
using LinqToDB.Extensions;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using LinqToDB.Linq;

public static class Database<R>
    where R : struct,
              HasDatabase<R>,
              HasCancel<R>
{
    // -------------------------------------- Ef Core
    // Add
    public static Aff<R, EntityEntry<TEntity>> Add<TEntity>(TEntity entity, CancellationToken token = default)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Add(entity, token));

    public static Aff<R, Unit> AddRange<TEntity>(Lst<TEntity> entities, CancellationToken token = default)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.AddRange(entities, token));

    // Update
    public static Aff<R, EntityEntry<TEntity>> Update<TEntity>(TEntity entity)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Update(entity));
    
    public static Aff<R, Unit> UpdateRange<TEntity>(Lst<TEntity> entities)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.UpdateRange(entities));

    // Attach
    public static Aff<R, EntityEntry<TEntity>> Attach<TEntity>(TEntity entity)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Attach(entity));

    public static Aff<R, EntityEntry> Attach(object entity)
        =>
        default(R).Database.Bind(rt => rt.Attach(entity));

    public static Aff<R, Unit> AttachRange(params object[] entities)
        =>
        default(R).Database.Bind(rt => rt.AttachRange(entities));

    public static Aff<R, Unit> AttachRange(Lst<object> entities)
        =>
        default(R).Database.Bind(rt => rt.AttachRange(entities));

    // Entry
    public static Aff<R, EntityEntry> Entry(object entity)
        =>
        default(R).Database.Bind(rt => rt.Entry(entity));
    
    public static Aff<R, EntityEntry<TEntity>> Entry<TEntity>(TEntity entity)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Entry(entity));

    // Find
    public static Aff<R, Option<TEntity>> Find<TEntity>(object keyValue, CancellationToken token = default)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Find<TEntity>(new[] { keyValue }, token));

    public static Aff<R, Option<TEntity>> Find<TEntity>(CancellationToken token = default, params object[] keyValues)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Find<TEntity>(keyValues, token));
    
    public static Aff<R, Option<object>> Find(Type entityType, CancellationToken token = default, params object[] keyValues)
        =>
        default(R).Database.Bind(rt => rt.Find(entityType, keyValues, token));
    
    // Expression
    public static Aff<R, IQueryable<TResult>> FromExpression<TResult>(Expression<Func<IQueryable<TResult>>> expression)
        =>
        default(R).Database.Bind(rt => rt.FromExpression(expression));
    
    // Remove
    public static Aff<R, EntityEntry> Remove(object entity)
        =>
        default(R).Database.Bind(rt => rt.Remove(entity));
    
    public static Aff<R, EntityEntry<TEntity>> Remove<TEntity>(TEntity entity)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Remove(entity));
    
    public static Aff<R, Unit> RemoveRange(params object[] entities)
        =>
        default(R).Database.Bind(rt => rt.RemoveRange(entities));
    
    public static Aff<R, Unit> RemoveRange(Lst<object> entities)
        =>
        default(R).Database.Bind(rt => rt.RemoveRange(entities));
    
    // SaveChange
    public static Aff<R, Unit> SaveChanges(bool acceptAllChangesOnSuccess, CancellationToken token = default)
        =>
        default(R).Database.Bind(rt => rt.SaveChanges(acceptAllChangesOnSuccess, token));
    
    public static Aff<R, Unit> SaveChanges(CancellationToken token = default)
        =>
        default(R).Database.Bind(rt => rt.SaveChanges(token));
    
    // Query
    public static Aff<R, DbSet<TEntity>> Set<TEntity>(string name)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Set<TEntity>(name));

    public static Aff<R, DbSet<TEntity>> Set<TEntity>()
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Set<TEntity>());
    
    // StoredProc
    public static Aff<R, A> StoredProc<A>(string command, Func<StoredProcQuery, Aff<A>> builder, bool prependDefaultSchema = true)
        =>
        default(R).Database.Bind(rt => rt.StoredProc(command, builder, prependDefaultSchema));

    // -------------------------------------- Linq2Db

    // Query
    public static Aff<R, ITable<TEntity>> Table<TEntity>(Option<string> name = default)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => name.Match(
            Some: n => rt.Table<TEntity>(n),
            None: () => rt.Table<TEntity>()
            ));

    // CTE
    public static Aff<R, IQueryable<A>> Cte<TEntity, A>(Func<ITable<TEntity>, IQueryable<A>> builder, Option<string> name)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Cte(builder, name));

    public static Aff<R, IQueryable<A>> Cte<TEntity, A>(Func<ITable<TEntity>, IQueryable<A>> builder)
        where TEntity : class
        =>
        Cte(builder, Option<string>.None);

    // Add
    public static Aff<R, TId> Insert<TEntity, TId>(TEntity entity, CancellationToken token = default)
        where TEntity : class, IEntity<TId>
        =>
        default(R).Database.Bind(rt => rt.Table<TEntity>())
                           .Map(table => table.DataContext)
                           .Bind(dtx => dtx.InsertWithIdentityAsync(entity, token: token)
                                           .ToAff()
                                           .Map(idO => dtx.MappingSchema.ChangeTypeTo<TId>(idO)));

    // Update
    public static Aff<R, Unit> Update<TEntity>(Func<ITable<TEntity>, IUpdatable<TEntity>> updater, CancellationToken token = default)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Table<TEntity>())
                           .Bind(table => updater(table).UpdateAsync(token)
                                                        .ToUnit()
                                                        .ToAff());

    // Find
    public static Aff<R, Option<TEntity>> Find<TEntity>(Func<ITable<TEntity>, IQueryable<TEntity>> queryBuilder, CancellationToken token = default)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Table<TEntity>())
                           .Bind(table => queryBuilder(table).FirstOrDefaultAsyncLinqToDB()
                                                             .Map(Optional<TEntity>)
                                                             .ToAff());
}
