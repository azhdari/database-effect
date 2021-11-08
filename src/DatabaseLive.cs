namespace LanguageExt.Effects.Database;

using System.Linq;
using System.Linq.Expressions;
using LanguageExt.Effects.Database;
using LanguageExt.Effects.Traits;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class DatabaseLive : DatabaseIO
{
    private readonly DbContext _dbContext;

    public DatabaseLive(DbContext dbContext) {
        _dbContext = dbContext;
    }

    // Add
    public Aff<EntityEntry> Add(object entity)
      =>
      _dbContext.AddAsync(entity).ToAff();

    public Aff<EntityEntry<TEntity>> Add<TEntity>(TEntity entity)
        where TEntity : class
        =>
        _dbContext.AddAsync<TEntity>(entity).ToAff();

    public Aff<Unit> AddRange(Lst<object> entities)
        => _dbContext.AddRangeAsync(entities).ToUnit().ToAff();

    public Aff<Unit> AddRange(params object[] entities)
        => _dbContext.AddRangeAsync(entities).ToUnit().ToAff();

    // Update
    public Eff<EntityEntry> Update(object entity)
        =>
        SuccessEff(_dbContext.Update(entity));

    public Eff<EntityEntry<TEntity>> Update<TEntity>(TEntity entity)
        where TEntity : class
        =>
        SuccessEff(_dbContext.Update(entity));

    public Eff<Unit> UpdateRange(params object[] entities) {
        _dbContext.UpdateRange(entities);
        return unitEff;
    }

    public Eff<Unit> UpdateRange(Lst<object> entities) {
        _dbContext.UpdateRange(entities);
        return unitEff;
    }

    // Attach
    public Eff<EntityEntry<TEntity>> Attach<TEntity>(TEntity entity)
        where TEntity : class
        => SuccessEff(_dbContext.Attach(entity));

    public Eff<EntityEntry> Attach(object entity)
        => SuccessEff(_dbContext.Attach(entity));

    public Eff<Unit> AttachRange(params object[] entities) {
        _dbContext.AttachRange(entities);
        return unitEff;
    }

    public Eff<Unit> AttachRange(Lst<object> entities) {
        _dbContext.AttachRange(entities);
        return unitEff;
    }

    // Entry
    public Eff<EntityEntry> Entry(object entity)
        => SuccessEff(_dbContext.Entry(entity));

    public Eff<EntityEntry<TEntity>> Entry<TEntity>(TEntity entity)
        where TEntity : class
        => SuccessEff(_dbContext.Entry(entity));

    // Find
    public Aff<Option<TEntity>> Find<TEntity>(object[] keyValues)
        where TEntity : class
        =>
        #pragma warning disable CS8622
        _dbContext.FindAsync<TEntity>(keyValues).ToAff().Map(Optional<TEntity>); 
        #pragma warning restore CS8622

    public Aff<Option<object>> Find(Type entityType, object[] keyValues)
        =>
        #pragma warning disable CS8622
        _dbContext.FindAsync(entityType, keyValues).ToAff().Map(Optional<object>);
        #pragma warning restore CS8622

    // Expression
    public Eff<IQueryable<TResult>> FromExpression<TResult>(Expression<Func<IQueryable<TResult>>> expression)
        => SuccessEff(_dbContext.FromExpression(expression));

    // Remove
    public Eff<EntityEntry> Remove(object entity)
        => SuccessEff(_dbContext.Remove(entity));

    public Eff<EntityEntry<TEntity>> Remove<TEntity>(TEntity entity)
        where TEntity : class
        =>
        SuccessEff(_dbContext.Remove(entity));

    public Eff<Unit> RemoveRange(params object[] entities) {
        _dbContext.RemoveRange(entities);
        return unitEff;
    }

    public Eff<Unit> RemoveRange(Lst<object> entities) {
        _dbContext.RemoveRange(entities);
        return unitEff;
    }

    // SaveChanges
    public Aff<Unit> SaveChanges(bool acceptAllChangesOnSuccess)
        =>
        _dbContext.SaveChangesAsync(acceptAllChangesOnSuccess)
                  .ToUnit()
                  .ToAff();

    public Aff<Unit> SaveChanges()
        =>
        _dbContext.SaveChangesAsync()
                  .ToUnit()
                  .ToAff();

    // Query
    public Eff<DbSet<TEntity>> Set<TEntity>(string name)
        where TEntity : class
        =>
        SuccessEff(_dbContext.Set<TEntity>(name));

    public Eff<DbSet<TEntity>> Set<TEntity>()
        where TEntity : class
        =>
        SuccessEff(_dbContext.Set<TEntity>());

    public Eff<ITable<TEntity>> Table<TEntity>(string name)
        where TEntity : class
        =>
        SuccessEff(_dbContext.Set<TEntity>(name).ToLinqToDBTable());

    public Eff<ITable<TEntity>> Table<TEntity>()
        where TEntity : class
        =>
        SuccessEff(_dbContext.Set<TEntity>().ToLinqToDBTable());

    // StoredProc
    public Aff<A> StoredProc<A>(string storedProcName, Func<StoredProcQuery, Aff<A>> builder, bool prependDefaultSchema = true)
        =>
        _dbContext.Model
                  .GetDefaultSchema()
                  #pragma warning disable CS8622
                  .Apply(Optional<string>)
                  #pragma warning restore CS8622
                  .Map(schemaName => prependDefaultSchema ? $"{schemaName}.{storedProcName}" : storedProcName)
                  .IfNone(storedProcName)
                  .Apply(spName => new StoredProcQuery(spName, _dbContext))
                  .Apply(builder);
}
