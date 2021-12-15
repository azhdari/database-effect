namespace LanguageExt.Effects.Database;

using System.Linq;
using System.Linq.Expressions;
using LanguageExt.Effects.Database;
using LanguageExt.Effects.Traits;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Linq;

public class DatabaseLive : DatabaseIO
{
    private readonly DataConnection _dbc;

    public DatabaseLive(DataConnection dbc) {
        _dbc = dbc;
    }

    // Add
    public Aff<TKey> Insert<T, TKey>(T entity, CancellationToken token = default)
        where T : class, IEntity<TKey>
        =>
        _dbc.InsertWithIdentityAsync<T, TKey>(entity, token: token)
            .ToAff()
            #pragma warning disable CS8622
            .Map(Optional<TKey>)
            #pragma warning restore CS8622
            .Bind(id => id.Match(
                Some: v => SuccessEff(v),
                None: () => FailEff<TKey>(Error.New($"Unable to insert entity ${typeof(T).Name}"))
            ));
    
    public Aff<TKey> Insert<T, TKey>(Func<IValueInsertable<T>, IValueInsertable<T>> provider, CancellationToken token = default)
        where T : class, IEntity<TKey>
        =>
        _dbc.InsertWithIdentityAsync<T, TKey>(provider(_dbc.Into<T>(_dbc.GetTable<T>())), token)
            .ToAff()
            #pragma warning disable CS8622
            .Map(Optional<TKey>)
            #pragma warning restore CS8622
            .Bind(id => id.Match(
                Some: v => SuccessEff(v),
                None: () => FailEff<TKey>(Error.New($"Unable to insert entity ${typeof(T).Name}"))
            ));
    
    public Aff<Guid> InsertGuid<T>(T entity, CancellationToken token = default)
        where T : class, IEntity<Guid>
        =>
        _dbc.InsertWithGuidIdentityAsync<T>(entity, token: token)
            .ToAff()
            #pragma warning disable CS8622
            .Map(Optional<Guid>)
            #pragma warning restore CS8622
            .Bind(id => id.Match(
                Some: v => SuccessEff(v),
                None: () => FailEff<Guid>(Error.New($"Unable to insert entity ${typeof(T).Name}"))
            ));
    
    public Aff<Guid> InsertGuid<T>(Func<IValueInsertable<T>, IValueInsertable<T>> provider, CancellationToken token = default)
        where T : class, IEntity<Guid>
        =>
        _dbc.InsertWithGuidIdentityAsync<T>(provider(_dbc.Into<T>(_dbc.GetTable<T>())), token)
            .ToAff()
            #pragma warning disable CS8622
            .Map(Optional<Guid>)
            #pragma warning restore CS8622
            .Bind(id => id.Match(
                Some: v => SuccessEff(v),
                None: () => FailEff<Guid>(Error.New($"Unable to insert entity ${typeof(T).Name}"))
            ));

    // Update
    public Aff<Unit> Update<T>(T entity, CancellationToken token = default)
        where T : class
        =>
        _dbc.UpdateAsync(entity, token: token)
            .ToUnit()
            .ToAff();
    
    public Aff<Unit> Update<T>(Func<ITable<T>, IUpdatable<T>> updater, CancellationToken token = default)
        where T : class
        =>
        updater(_dbc.GetTable<T>())
            .UpdateAsync(token)
            .ToUnit()
            .ToAff();

    // Remove
    public Aff<Unit> Delete<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
        _dbc.GetTable<T>()
            .Where(filter)
            .DeleteAsync(token)
            .ToUnit()
            .ToAff();

    // Find
    public Eff<ITable<T>> Table<T>()
        where T : class
        =>
        SuccessEff(_dbc.GetTable<T>());

    public Aff<Option<T>> FindOne<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
        _dbc.GetTable<T>()
            .Where(filter)
            .FirstOrDefaultAsync(token)
            .ToAff()
            #pragma warning disable CS8622
            .Map(Optional<T>);
            #pragma warning restore CS8622
    
    public Aff<Arr<T>> Find<T>(Expression<Func<T, bool>> filter, CancellationToken token = default)
        where T : class
        =>
        _dbc.GetTable<T>()
            .Where(filter)
            .ToListAsync(token)
            .ToAff()
            .Map(toArray);

    public Eff<IQueryable<A>> GetCte<T, A>(Func<ITable<T>, IQueryable<A>> builder, Option<string> name)
        where T : class
        =>
        name.Match(
            Some:  n => builder(_dbc.GetTable<T>()).AsCte(n).Apply(SuccessEff),
            None: () => builder(_dbc.GetTable<T>()).AsCte().Apply(SuccessEff)
        );

    public Eff<IQueryable<T>> GetRecursiveCte<T>(Func<IQueryable<T>, IQueryable<T>> body, Option<string> name)
        where T : class
        =>
        _dbc.GetCte<T>(body, name.ToNullable())
            .Apply(SuccessEff);
}
