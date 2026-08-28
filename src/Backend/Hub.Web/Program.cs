using Hub.Application;
using Hub.Infrastructure;
using Hub.Web;
using Hub.Web.Hangfire;
using Microsoft.AspNetCore.HttpOverrides;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddWeb();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.Services.MigrateDatabaseAsync();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    await app.Seed();
    app.MapWebOpenApi();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapHangfireUi();
app.MapEndpoints();

app.Run();
