using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Infrastructure.Jobs;
using ClaimsModule.Infrastructure.Services;
using ClaimsModule.Infrastructure.Storage;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClaimsModule.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Storage:Provider"] ?? "LocalFileSystem";
        if (string.Equals(provider, "AzureBlob", StringComparison.OrdinalIgnoreCase))
            services.AddSingleton<IStorageService, AzureBlobStorageService>();
        else
            services.AddSingleton<IStorageService, LocalFileSystemStorageService>();

        services.AddScoped<IGlPostingJobScheduler, GlPostingJobScheduler>();
        services.AddScoped<PostGlReserveChangeJob>();
        services.AddScoped<SlaMonitoringJob>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost,1433;Database=ClaimsModule;User Id=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                SchemaName = "Hangfire"
            }));

        services.AddHangfireServer();

        return services;
    }

    public static void RegisterRecurringJobs()
    {
        RecurringJob.AddOrUpdate<SlaMonitoringJob>(
            "sla-monitoring",
            job => job.ExecuteAsync(),
            "*/15 * * * *");
    }
}
