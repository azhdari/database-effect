namespace LanguageExt.Effects.Database;

public static class Extensions
{
    public static T? ToNullable<T>(this Option<T> maybe)
        where T: class
        =>
        maybe.Case is T some ? some : null;
}