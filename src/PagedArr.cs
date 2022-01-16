namespace LanguageExt.Effects.Database;

public record DataAndCount<T>(
    Arr<T> Data,
    int Count
    );

public record DataLimit(int Skip, int Take, string SortBy, SortDir SortDir);

public enum SortDir
{
    asc,
    desc,
}