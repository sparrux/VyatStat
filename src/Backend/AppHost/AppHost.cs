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

var frontend = "../../Frontend";

var webClient = builder.AddJavaScriptApp("identity-app", frontend)
    .WithRunScript("start:identity")
    .WithReference(identityApi)
    .WithHttpEndpoint(port: 4200, env: "PORT")
    .WaitFor(identityApi);

var trackerApp = builder.AddJavaScriptApp("tracker-app", frontend)
    .WithRunScript("start:tracker")
    .WithReference(identityApi)
    .WithHttpEndpoint(port: 4201, env: "PORT")
    .WaitFor(identityApi);

var clientIsHttps = webClient.GetEndpoint("https").Exists;
var trackerIsHttps = trackerApp.GetEndpoint("https").Exists;
var apiIsHttps = identityApi.GetEndpoint("https").Exists;

var webClientEndpoint = webClient.GetEndpoint(clientIsHttps ? "https" : "http");
var trackerAppEndpoint = trackerApp.GetEndpoint(trackerIsHttps ? "https" : "http");
var identityApiEndpoint = identityApi.GetEndpoint(apiIsHttps ? "https" : "http");

identityApi.WithEnvironment("Clients:IdentityWebClient:Url", webClientEndpoint);
identityApi.WithEnvironment("Clients:TrackerWebClient:Url", trackerAppEndpoint);
identityApi.WithEnvironment("Idp:Authority", identityApiEndpoint);
identityApi.WithEnvironment("Idp:LoginPageUrl", $"{webClientEndpoint}/login");

builder.Build().Run();