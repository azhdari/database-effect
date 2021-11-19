namespace Microsoft.Extensions.DependencyInjection;

using LinqToDB.EntityFrameworkCore;

public static class ServiceCollectionExtensions
{
    public static ServiceCollection AddLinq2DbForDatabaseEff(this ServiceCollection services) {
        LinqToDBForEFTools.Initialize();
        return services;
    }
}
