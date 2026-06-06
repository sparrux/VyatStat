var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = builder.AddPostgres("vyatka-db")
    .WithDataVolume("vyatka_postgres-db")
    .AddDatabase("vyatka-identity");

var identityApi = builder.AddProject<Projects.Identity_WebAPI>("identity-api")
    .WithExternalHttpEndpoints()
    .WithReference(postgresDb)
    .WithEnvironment("Clients:WebClient:Url", "")
    .WithHttpHealthCheck("/health");

var webClient = builder.AddJavaScriptApp("identity-client", "../Identity/Identity.Web")
    .WithReference(identityApi)
    .WithHttpEndpoint(port: 4200, env: "PORT")
    .WaitFor(identityApi);

var clientIsHttps = webClient.GetEndpoint("https").Exists;

identityApi.WithEnvironment("Clients:WebClient:Url", webClient.GetEndpoint(clientIsHttps ? "https" : "http"));

builder.Build().Run();