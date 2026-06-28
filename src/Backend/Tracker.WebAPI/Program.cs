using ServiceDefaults;
using Tracker.WebAPI;
using Tracker.WebAPI.Services;
using Tracker.WebAPI.Services.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddWebServices();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await DatabaseMigrator.MigrateAsync(app);
await DatabaseSeeder.SeedAsync(app);

app.Run();