namespace AspNetEmptyApp.Extensions;

using System.Text;
using AspNetEmptyApp.Data;
using AspNetEmptyApp.Feature.Auth.Authorization;
using AspNetEmptyApp.Feature.Auth.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public static class IdentityAndAuthExtensions
{
    public static IServiceCollection AddIdentityAndAuthServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. ASP.NET Core Identity with Guid PK
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // 2. JWT Authentication
        var jwtKey = configuration["Jwt:Key"] ?? "SuperSecretKeyForJwtTokenAuth1234567890!@#$%";
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"] ?? "AspNetEmptyApp",
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"] ?? "AspNetEmptyApp",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // 3. RBAC & Custom Permission Authorization
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddAuthorization();

        return services;
    }
}
