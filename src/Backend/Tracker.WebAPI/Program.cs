using ServiceDefaults;
using Tracker.Application;
using Tracker.Infrastructure;
using Tracker.WebAPI;
using Tracker.WebAPI.Authentication;
using Tracker.WebAPI.Services;
using Tracker.WebAPI.Services.Migrations;
using Tracker.WebAPI.Services.Seeders;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddWebServices();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseUserProvisioning();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapApiDocs();
}

await DatabaseMigrationService.MigrateAsync(app);
await DatabaseSeeder.SeedAsync(app);

app.Run();