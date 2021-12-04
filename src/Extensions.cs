namespace LanguageExt.Effects.Database;

public static class Extensions
{
    public static A? ToNullable<A>(this Option<A> option)
        =>
        option.MatchUnsafe<A?>(
            Some: v => v,
            None: () => default
        );
}