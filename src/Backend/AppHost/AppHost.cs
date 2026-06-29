var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("vyatka-db")
    .WithDataVolume("vyatka_postgres-db")
    .WithPgAdmin(pgAdmin => 
        pgAdmin.WithHostPort(5050));

var identityDb = postgres.AddDatabase("IdentityDb");
var trackerDb = postgres.AddDatabase("TrackerDb");

var identityApi = builder.AddProject<Projects.Identity_WebAPI>(
        "identity-api")
    .WithExternalHttpEndpoints()
    .WithReference(identityDb)
    .WithHttpHealthCheck("/health");

var trackerApi = builder.AddProject<Projects.Tracker_WebAPI>(
        "tracker-api")
    .WithExternalHttpEndpoints()
    .WithReference(identityApi)
    .WithReference(trackerDb)
    .WithHttpHealthCheck("/health");

var frontend = "../../Frontend";

var identityApp = builder.AddJavaScriptApp("identity-app", frontend)
    .WithRunScript("start:identity")
    .WithReference(identityApi)
    .WithHttpEndpoint(port: 4200, env: "PORT")
    .WaitFor(identityApi);

var trackerApp = builder.AddJavaScriptApp("tracker-app", frontend)
    .WithRunScript("start:tracker")
    .WithReference(identityApi)
    .WithReference(trackerApi)
    .WithHttpEndpoint(port: 4201, env: "PORT")
    .WaitFor(identityApi)
    .WaitFor(trackerApi);

var identityIsHttps = identityApp.GetEndpoint("https").Exists;
var trackerIsHttps = trackerApp.GetEndpoint("https").Exists;
var identityApiIsHttps = identityApi.GetEndpoint("https").Exists;

var webClientEndpoint = identityApp.GetEndpoint(identityIsHttps ? "https" : "http");
var trackerAppEndpoint = trackerApp.GetEndpoint(trackerIsHttps ? "https" : "http");
var identityApiEndpoint = identityApi.GetEndpoint(identityApiIsHttps ? "https" : "http");

identityApi.WithEnvironment("Clients:identity-app:Url", webClientEndpoint);
identityApi.WithEnvironment("Clients:tracker-app:Url", trackerAppEndpoint);
identityApi.WithEnvironment("Idp:Authority", identityApiEndpoint);
identityApi.WithEnvironment("Idp:LoginPageUrl", $"{webClientEndpoint}/login");

trackerApi.WithEnvironment("OpenIddict:Authority", identityApiEndpoint);
trackerApi.WithEnvironment("OpenIddict:Audience", "vyatka-tracker-api");
trackerApi.WithEnvironment("Clients:tracker-app:Url", trackerAppEndpoint);

builder.Build().Run();