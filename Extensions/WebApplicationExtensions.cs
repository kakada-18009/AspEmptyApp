namespace AspNetEmptyApp.Extensions;

using AspNetEmptyApp.Data;
using Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        await DbSeeder.SeedAsync(app.Services);
    }
}
