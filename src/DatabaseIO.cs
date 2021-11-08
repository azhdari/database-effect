namespace LanguageExt.Effects.Traits;

using System.Linq.Expressions;
using LanguageExt.Effects.Database;
using LinqToDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public interface DatabaseIO
{
    Aff<EntityEntry> Add(object entity, CancellationToken token = default);
    Aff<EntityEntry<TEntity>> Add<TEntity>(TEntity entity, CancellationToken token = default) where TEntity : class;
    Aff<Unit> AddRange(Lst<object> entities, CancellationToken token = default);
    Aff<Unit> AddRange(object[] entities, CancellationToken token = default);

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

    Aff<Option<TEntity>> Find<TEntity>(object[] keyValues, CancellationToken token = default) where TEntity : class;
    Aff<Option<object>> Find(Type entityType, object[] keyValues, CancellationToken token = default);

    Eff<IQueryable<TResult>> FromExpression<TResult>(Expression<Func<IQueryable<TResult>>> expression);

    Eff<EntityEntry> Remove(object entity);
    Eff<EntityEntry<TEntity>> Remove<TEntity>(TEntity entity) where TEntity : class;
    Eff<Unit> RemoveRange(params object[] entities);
    Eff<Unit> RemoveRange(Lst<object> entities);

    Aff<Unit> SaveChanges(bool acceptAllChangesOnSuccess, CancellationToken token = default);
    Aff<Unit> SaveChanges(CancellationToken token = default);

    Eff<DbSet<TEntity>> Set<TEntity>(string name) where TEntity : class;
    Eff<DbSet<TEntity>> Set<TEntity>() where TEntity : class;
    Eff<ITable<TEntity>> Table<TEntity>(string name) where TEntity : class;
    Eff<ITable<TEntity>> Table<TEntity>() where TEntity : class;

    Aff<A> StoredProc<A>(string storedProcName, Func<StoredProcQuery, Aff<A>> builder, bool prependDefaultSchema = true);
}