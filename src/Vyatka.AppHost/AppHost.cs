var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = builder.AddPostgres("vyatka-db")
    .WithDataVolume("vyatka_postgres-db")
    .AddDatabase("vyatka-identity");

var identityApi = builder.AddProject<Projects.Identity_WebAPI>("identity-api")
    .WithExternalHttpEndpoints()
    .WithReference(postgresDb)
    .WithHttpHealthCheck("/health");

builder.AddJavaScriptApp("identity-client", "../Identity/Identity.Web")
    .WithReference(identityApi)
    .WithHttpEndpoint(port: 4200, env: "PORT")
    .WaitFor(identityApi);

builder.Build().Run();