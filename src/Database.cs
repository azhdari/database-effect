namespace LanguageExt.Effects.Traits;

using System.Linq;
using System.Linq.Expressions;
using LanguageExt.Effects.Database;
using LinqToDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public static class Database<R>
    where R : struct,
              HasDatabase<R>,
              HasCancel<R>
{
    // Add
    public static Aff<R, EntityEntry> Add(object entity, CancellationToken token = default)
        =>
        default(R).Database.Bind(rt => rt.Add(entity, token));

    public static Aff<R, EntityEntry<TEntity>> Add<TEntity>(TEntity entity, CancellationToken token = default)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Add(entity, token));

    public static Aff<R, Unit> AddRange(Lst<object> entities, CancellationToken token = default)
        =>
        default(R).Database.Bind(rt => rt.AddRange(entities, token));

    public static Aff<R, Unit> AddRange(CancellationToken token = default, params object[] entities)
        =>
        default(R).Database.Bind(rt => rt.AddRange(entities, token));
    
    // Update
    public static Aff<R, EntityEntry> Update(object entity)
        =>
        default(R).Database.Bind(rt => rt.Update(entity));
    
    public static Aff<R, EntityEntry<TEntity>> Update<TEntity>(TEntity entity)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Update(entity));
    
    public static Aff<R, Unit> UpdateRange(params object[] entities)
        =>
        default(R).Database.Bind(rt => rt.UpdateRange());
    
    public static Aff<R, Unit> UpdateRange(Lst<object> entities)
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
    public static Aff<R, ITable<TEntity>> Table<TEntity>(string name)
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Table<TEntity>(name));

    public static Aff<R, ITable<TEntity>> Table<TEntity>()
        where TEntity : class
        =>
        default(R).Database.Bind(rt => rt.Table<TEntity>());

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
}
