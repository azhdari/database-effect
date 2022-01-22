namespace LanguageExt.Effects.Database;

using System.Linq;
using LinqToDB;

public static class QueryPaginator
{
    private class Envelope<T>
    {
        public int TotalCount { get; init; }
        public T Data { get; init; }
    }

    public static (List<T> Data, int TotalRecords) Paginate<T>(IQueryable<T> query, int skip, int take, CancellationToken cancelToken = default)
    {
        var withCount = 
            from q in query
            select new Envelope<T>
            {
                TotalCount = Sql.Ext.Count().Over().ToValue(),
                Data = q
            };

        var items = withCount.Skip(skip).Take(take).AsEnumerable();

        using var enumerator = items.GetEnumerator();
        int totalRecords;
        
        List<T> result;
        if (!enumerator.MoveNext())
        {
            totalRecords = 0;
            result = new List<T>();
        }
        else
        {
            totalRecords = enumerator.Current?.TotalCount ?? 0;
            result = new List<T>(take);
            do
            {
                if (cancelToken.IsCancellationRequested) {
                    break;
                }
                
                result.Add(enumerator.Current.Data);
            } while (enumerator.MoveNext());
        }

        return (result, totalRecords);
    }
}
