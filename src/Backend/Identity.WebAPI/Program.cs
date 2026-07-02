using Identity.WebAPI;
using Identity.WebAPI.Middlewares;
using Identity.WebAPI.Services.Seed;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddWebServices();

var app = builder.Build();

app.UseExceptionHandler();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.UseSecurityStampValidation();

app.MapDefaultEndpoints();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapApiDocs();
}

await DatabaseSeeder.SeedDatabaseAsync(app);
await UsersSeeder.SeedAsync(app);
await OAuthApplicationSeeder.SeedClientsAsync(app);

app.Run();