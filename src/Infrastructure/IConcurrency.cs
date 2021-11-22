namespace LanguageExt.Effects.Database;

using System;

public interface IConcurrency<TKey>
    where TKey : IEquatable<TKey>
{
    TKey Id { get; set; }
    string ConcurrencyStamp { get; set; }
}
