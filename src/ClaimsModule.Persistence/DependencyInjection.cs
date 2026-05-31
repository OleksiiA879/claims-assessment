using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClaimsModule.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost,1433;Database=ClaimsModule;User Id=sa;Password=Your_password123;TrustServerCertificate=True";

        services.AddScoped<Interceptors.DispatchDomainEventsInterceptor>();
        services.AddDbContext<ClaimsDbContext>((sp, options) =>
            options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(ClaimsDbContext).Assembly.FullName))
                .AddInterceptors(sp.GetRequiredService<Interceptors.DispatchDomainEventsInterceptor>()));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ClaimsDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IClaimNumberGenerator, ClaimNumberGenerator>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        return services;
    }
}
