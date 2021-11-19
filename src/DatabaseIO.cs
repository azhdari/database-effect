namespace LanguageExt.Effects.Traits;

using System.Linq.Expressions;
using LanguageExt.Effects.Database;
using LinqToDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public interface DatabaseIO
{
    DbContext DbContext { get; }

    Aff<EntityEntry<TEntity>> Add<TEntity>(TEntity entity, CancellationToken token = default) where TEntity : class;
    Aff<Unit> AddRange<TEntity>(Lst<TEntity> entities, CancellationToken token = default) where TEntity : class;

    Eff<EntityEntry<TEntity>> Update<TEntity>(TEntity entity) where TEntity : class;
    Eff<Unit> UpdateRange<TEntity>(Lst<TEntity> entities) where TEntity : class;

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

    Eff<IQueryable<A>> Cte<TEntity, A>(Func<ITable<TEntity>, IQueryable<A>> builder, Option<string> name) where TEntity : class;
}