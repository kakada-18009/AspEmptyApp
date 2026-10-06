using AspNetEmptyApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register services via extension methods
builder.Services.AddApplicationServices();
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddIdentityAndAuthServices(builder.Configuration);

var app = builder.Build();

// Seed initial database state (Roles, Users, Permissions)
await app.SeedDatabaseAsync();

// Middleware Pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
