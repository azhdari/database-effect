namespace LanguageExt.Effects.Traits;

using System.Linq.Expressions;
using LanguageExt.Effects.Database;
using LinqToDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public interface DatabaseIO
{
    Aff<EntityEntry> Add(object entity);
    Aff<EntityEntry<TEntity>> Add<TEntity>(TEntity entity) where TEntity : class;
    Aff<Unit> AddRange(Lst<object> entities);
    Aff<Unit> AddRange(params object[] entities);

    Eff<EntityEntry> Update(object entity);
    Eff<EntityEntry<TEntity>> Update<TEntity>(TEntity entity) where TEntity : class;
    Eff<Unit> UpdateRange(params object[] entities);
    Eff<Unit> UpdateRange(Lst<object> entities);

    Eff<EntityEntry<TEntity>> Attach<TEntity>(TEntity entity) where TEntity : class;
    Eff<EntityEntry> Attach(object entity);
    Eff<Unit> AttachRange(params object[] entities);
    Eff<Unit> AttachRange(Lst<object> entities);

    Eff<EntityEntry> Entry(object entity);
    Eff<EntityEntry<TEntity>> Entry<TEntity>(TEntity entity) where TEntity : class;

    Aff<Option<TEntity>> Find<TEntity>(params object[] keyValues) where TEntity : class;
    Aff<Option<object>> Find(Type entityType, params object[] keyValues);

    Eff<IQueryable<TResult>> FromExpression<TResult>(Expression<Func<IQueryable<TResult>>> expression);

    Eff<EntityEntry> Remove(object entity);
    Eff<EntityEntry<TEntity>> Remove<TEntity>(TEntity entity) where TEntity : class;
    Eff<Unit> RemoveRange(params object[] entities);
    Eff<Unit> RemoveRange(Lst<object> entities);

    Aff<Unit> SaveChanges(bool acceptAllChangesOnSuccess);
    Aff<Unit> SaveChanges();

    Eff<DbSet<TEntity>> Set<TEntity>(string name) where TEntity : class;
    Eff<DbSet<TEntity>> Set<TEntity>() where TEntity : class;
    Eff<ITable<TEntity>> Table<TEntity>(string name) where TEntity : class;
    Eff<ITable<TEntity>> Table<TEntity>() where TEntity : class;

    Aff<A> StoredProc<A>(string storedProcName, Func<StoredProcQuery, Aff<A>> builder, bool prependDefaultSchema = true);
}