using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DfE.EducationProviderRegistry.IntegrationsApi.Api.Extensions;

internal static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddEducationProviderRegistryDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "eprapi_eprdat_dotnet_db_connection";

        string connectionString =
            configuration[ConnectionStringKey] ??
                throw new InvalidOperationException($"Configuration value '{ConnectionStringKey}' is missing.");

        // Validate connectionString
        _ = new NpgsqlConnectionStringBuilder(connectionString);

        services.AddDbContextFactory<EducationProviderRegistryDbContext>(options =>
        {
            options.UseNpgsql(connectionString)
                   .EnableDetailedErrors()
                   .LogTo(Console.WriteLine, LogLevel.Information);
        });

        return services;
    }
}
