namespace LanguageExt.Effects.Database;

using System;

public interface IEntity<TId>
{
    TId Id { get; }
}