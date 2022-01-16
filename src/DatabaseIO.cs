namespace LanguageExt.Effects.Traits;

using System.Linq.Expressions;
using LanguageExt.Effects.Database;
using LinqToDB;
using LinqToDB.Linq;

public interface DatabaseIO
{
    Aff<TKey> Insert<T, TKey>(T entity, CancellationToken token = default) where T : class, IEntity<TKey>;
    Aff<TKey> Insert<T, TKey>(Func<IValueInsertable<T>, IValueInsertable<T>> provider, CancellationToken token = default) where T : class, IEntity<TKey>;

    Aff<Guid> InsertGuid<T>(T entity, CancellationToken token = default) where T : class, IEntity<Guid>;
    Aff<Guid> InsertGuid<T>(Func<IValueInsertable<T>, IValueInsertable<T>> provider, CancellationToken token = default) where T : class, IEntity<Guid>;

    Aff<Unit> Update<T>(T entity, CancellationToken token = default) where T : class;
    Aff<Unit> Update<T>(Func<ITable<T>, IUpdatable<T>> updater, CancellationToken token = default) where T : class;
    Aff<Unit> Delete<T>(Expression<Func<T, bool>> filter, CancellationToken token = default) where T : class;
    Aff<Option<T>> FindOne<T>(Expression<Func<T, bool>> filter, CancellationToken token = default) where T : class;
    Aff<Arr<T>> Find<T>(Expression<Func<T, bool>> filter, CancellationToken token = default) where T : class;

    Aff<int> Count<T>(Func<ITable<T>, IQueryable<T>> query, CancellationToken token = default) where T : class;
    Aff<DataAndCount<T>> FindAndCount<T>(DataLimit limit, Func<IQueryable<T>, IQueryable<T>> query, CancellationToken token = default) where T : class;

    Eff<ITable<T>> Table<T>() where T : class;
    Eff<IQueryable<A>> GetCte<T, A>(Func<ITable<T>, IQueryable<A>> body, Option<string> name) where T : class;
    Eff<IQueryable<T>> GetRecursiveCte<T>(Func<IQueryable<T>, IQueryable<T>> body, Option<string> name) where T : class;
}