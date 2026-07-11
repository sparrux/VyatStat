using Hub.Application;
using Hub.Infrastructure;
using Hub.Web;
using Microsoft.AspNetCore.HttpOverrides;
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

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

await app.Services.MigrateDatabaseAsync();

app.Run();
