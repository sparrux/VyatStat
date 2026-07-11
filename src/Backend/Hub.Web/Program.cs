using Hub.Application;
using Hub.Infrastructure;
using Hub.Web;
using Hub.Web.Authentication;
using Hub.Web.Endpoints;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddWeb();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapWebOpenApi();
}

app.UseCors();
app.UseAuthentication();
app.UseUserProvisioning();
app.UseAuthorization();

app.MapEndpoints();

await app.Services.MigrateDatabaseAsync();

app.Run();