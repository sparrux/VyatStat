using Scalar.AspNetCore;
using ServiceDefaults;
using Tracker.WebAPI;
using Tracker.WebAPI.Authentication;
using Tracker.WebAPI.Services;
using Tracker.WebAPI.Services.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddWebServices();

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseUserProvisioning();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

await DatabaseMigrator.MigrateAsync(app);
await DatabaseSeeder.SeedAsync(app);

app.Run();