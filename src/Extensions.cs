using System.Linq.Expressions;
using LinqToDB;

namespace LanguageExt.Effects.Database;

public static class Extensions
{
    public static T? ToNullable<T>(this Option<T> maybe)
        where T: class
        =>
        maybe.Case is T some ? some : null;

    public static Ret IfElse<T, Ret>(
        this IQueryable<T> query,
        Func<bool> condition,
        Func<IQueryable<T>, Ret> onTrue,
        Func<IQueryable<T>, Ret> onFalse)
        =>
        condition()
        ? onTrue(query)
        : onFalse(query);

    public static IQueryable<T> AndWhereO<T, A>(this ITable<T> table, Option<A> option, Func<A, Expression<Func<T, bool>>> filter)
        where T : notnull
        =>
        table.AsQueryable().AndWhereO(option, filter);

    public static IQueryable<T> AndWhereO<T, A>(this IQueryable<T> query, Option<A> option, Func<A, Expression<Func<T, bool>>> filter)
        =>
        option.Match(
            Some:  a => query.Where(filter(a)),
            None: () => query
            );
}