#pragma warning disable ASPIRECSHARPAPPS001

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDokployEnvironment("demo");

var server = builder.AddCSharpApp("server", "../demo.Server")
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
