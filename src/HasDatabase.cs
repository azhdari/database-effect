namespace LanguageExt.Effects.Traits;

public interface HasDatabase<R>
            : HasCancel<R>
    where R : struct,
              HasCancel<R>,
              HasDatabase<R>
{
    Aff<R, DatabaseIO> Database { get; }
}
