// ReSharper disable once CheckNamespace

namespace LanguageExt.Effects.Database;

using System.Linq;
using System.Linq.Expressions;
using Database;
using Traits;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Linq;

public class DatabaseLive : DatabaseIO
{
    private readonly DataConnection _dbc;

    public DatabaseLive(DataConnection dbc) { _dbc = dbc; }

    // Add
    public Aff<TKey> Insert<T, TKey>(T entity, CancellationToken token = default)
        where T : class, IEntity<TKey>
        =>
            _dbc.InsertWithIdentityAsync<T, TKey>(entity, token: token).
                ToAff()
#pragma warning disable CS8622
                .
                Map(Optional<TKey>)
#pragma warning restore CS8622
                .
                Bind(
                    id => id.Match(
                        SuccessEff,
                        () => FailEff<TKey>(Error.New($"Unable to insert entity ${typeof(T).Name}"))
                    )
                );

    public Aff<TKey> Insert<T, TKey>(
        Func<IValueInsertable<T>, IValueInsertable<T>> provider,
        CancellationToken token = default
    )
        where T : class, IEntity<TKey>
        =>
            _dbc.InsertWithIdentityAsync<T, TKey>(provider(_dbc.Into(_dbc.GetTable<T>())), token).
                ToAff()
#pragma warning disable CS8622
                .
                Map(Optional<TKey>)
#pragma warning restore CS8622
                .
                Bind(
                    id => id.Match(
                        SuccessEff,
                        () => FailEff<TKey>(Error.New($"Unable to insert entity ${typeof(T).Name}"))
                    )
                );

    public Aff<Guid> InsertGuid<T>(T entity, CancellationToken token = default)
        where T : class, IEntity<Guid>
        =>
            _dbc.InsertWithGuidIdentityAsync(entity, token: token).
                ToAff()
#pragma warning disable CS8622
                .
                Map(Optional<Guid>)
#pragma warning restore CS8622
                .
                Bind(
                    id => id.Match(
                        SuccessEff,
                        () => FailEff<Guid>(Error.New($"Unable to insert entity ${typeof(T).Name}"))
                    )
                );

    public Aff<Guid> InsertGuid<T>(
        Func<IValueInsertable<T>, IValueInsertable<T>> provider,
        CancellationToken token = default
    )
        where T : class, IEntity<Guid>
        =>
            _dbc.InsertWithGuidIdentityAsync(provider(_dbc.Into(_dbc.GetTable<T>())), token).
                ToAff()
#pragma warning disable CS8622
                .
                Map(Optional<Guid>)
#pragma warning restore CS8622
                .
                Bind(
                    id => id.Match(
                        SuccessEff,
                        () => FailEff<Guid>(Error.New($"Unable to insert entity ${typeof(T).Name}"))
                    )
                );

    // Update
    public Aff<Unit> Update<T>(T entity, CancellationToken token = default)
        where T : class
        =>
            _dbc.UpdateAsync(entity, token: token).
                ToUnit().
                ToAff();

    public Aff<Unit> Update<T>(Func<ITable<T>, IUpdatable<T>> updater, CancellationToken token = default)
        where T : class
        =>
            updater(_dbc.GetTable<T>()).
                UpdateAsync(token).
                ToUnit().
                ToAff();

    // Remove
    public Aff<Unit> Delete<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
            _dbc.GetTable<T>().
                Where(filter).
                DeleteAsync(token).
                ToUnit().
                ToAff();

    // Find
    public Eff<ITable<T>> Table<T>()
        where T : class
        =>
            SuccessEff(_dbc.GetTable<T>());

    public Aff<Option<T>> FindOne<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
            _dbc.GetTable<T>().
                Where(filter).
                FirstOrDefaultAsync(token).
                ContinueWith(prev => prev.Map(v => v is null ? Option<T>.None : Option<T>.Some(v)), token).
                Flatten().
                ToAff();

    public Aff<Arr<T>> Find<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
            _dbc.GetTable<T>().
                Where(filter).
                ToListAsync(token).
                ToAff().
                Map(toArray);

    public Aff<int> Count<T>(Func<ITable<T>, IQueryable<T>> query, CancellationToken token = default)
        where T : class
        =>
            _dbc.GetTable<T>().
                Apply(query).
                CountAsync(token).
                ToAff();

    public Aff<DataAndCount<T>> FindAndCount<T>(
        IQueryable<T> query,
        DataLimit limit,
        CancellationToken token = default
    )
        where T : class {
        var sortedQuery = limit.SortDir == SortDir.asc
            ? query.OrderBy(p => Sql.Property<object>(p, limit.SortBy))
            : query.OrderByDescending(p => Sql.Property<object>(p, limit.SortBy));

        var (data, totalRecords) = QueryPaginator.Paginate(sortedQuery, limit.Skip, limit.Take, token);
        return SuccessAff(new DataAndCount<T>(data.ToArr(), totalRecords));
    }

    public Eff<IQueryable<A>> GetCte<T, A>(
        Func<IQueryable<T>, IQueryable<A>> builder,
        Option<string> name
    )
        where T : class
        =>
            name.Match(
                n => builder(_dbc.GetTable<T>()).
                    AsCte(n).
                    Apply(SuccessEff),
                () => builder(_dbc.GetTable<T>()).
                    AsCte().
                    Apply(SuccessEff)
            );

    public Eff<IQueryable<T>> GetRecursiveCte<T>(
        Func<IQueryable<T>, IQueryable<T>> body,
        Option<string> name
    )
        where T : class
        =>
            _dbc.GetCte(body, name.ToNullable()).
                Apply(SuccessEff);
}