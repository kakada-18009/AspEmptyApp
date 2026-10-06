namespace AspNetEmptyApp.Extensions;

using Asp.Versioning;
using AspNetEmptyApp.Feature.Auth.Handler;
using AspNetEmptyApp.Feature.Auth.Services;
using AspNetEmptyApp.Feature.Inventory.Handler;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 1. Controllers
        services.AddControllers();

        // 2. API Versioning
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version")
            );
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // 3. FluentValidation
        services.AddValidatorsFromAssemblyContaining<Program>();

        // 4. Feature Handlers & Services (Direct DI, No MediatR)
        services.AddScoped<TokenService>();
        services.AddScoped<GetUserProfileHandler>();
        services.AddScoped<CreateProductionHandler>();
        services.AddScoped<GetProductionsHandler>();
        services.AddScoped<CreateCategoryHandler>();
        services.AddScoped<GetCategoriesHandler>();

        return services;
    }
}
