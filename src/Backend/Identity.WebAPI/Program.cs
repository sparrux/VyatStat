using Identity.WebAPI;
using Identity.WebAPI.Services.Seed;
using Scalar.AspNetCore;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddWebServices();

var app = builder.Build();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    await DatabaseSeeder.SeedDatabaseAsync(app);
}

await UsersSeeder.SeedAsync(app);
await OAuthApplicationSeeder.SeedClientsAsync(app);

app.Run();