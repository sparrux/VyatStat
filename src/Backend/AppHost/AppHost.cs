var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = builder.AddPostgres(
        "vyatka-db")
    .WithDataVolume("vyatka_postgres-db")
    .AddDatabase("IdentityDb");

var identityApi = builder.AddProject<Projects.Identity_WebAPI>(
        "identity-api")
    .WithExternalHttpEndpoints()
    .WithReference(postgresDb)
    .WithHttpHealthCheck("/health");

var webClient = builder.AddJavaScriptApp(
        "identity-app", "../../Frontend/identity-app")
    .WithReference(identityApi)
    .WithHttpEndpoint(port: 4200, env: "PORT")
    .WaitFor(identityApi);

var clientIsHttps = webClient.GetEndpoint("https").Exists;

identityApi.WithEnvironment("Clients:WebClient:Url", webClient.GetEndpoint(clientIsHttps ? "https" : "http"));

builder.Build().Run();