namespace AspNetEmptyApp.Extensions;

using AspNetEmptyApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. EF Core SQLite Context
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // 2. Dapper Context
        services.AddSingleton<IDapperContext, DapperFactory>();

        return services;
    }
}
