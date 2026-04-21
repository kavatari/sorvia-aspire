# Sorvia Aspire

A collection of .NET Aspire hosting integrations by [Sorvia](https://github.com/jomaxso).

## Packages

| Package | Description | Version |
|---------|-------------|---------|
| [Sorvia.Aspire.Hosting.Dokploy](src/Sorvia.Aspire.Hosting.Dokploy/) | Deploy .NET Aspire apps to a self-hosted [Dokploy](https://dokploy.com) instance | 0.1.0 |

## Quick Start

Add the package to your AppHost project:

```shell
dotnet add package Sorvia.Aspire.Hosting.Dokploy
```

Then register the Dokploy deployment target:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Api>("api");

builder.AddDokployEnvironment("dokploy");

builder.Build().Run();
```

When running locally, the Dokploy resource is a no-op — everything runs as usual. When publishing (`dotnet run --publisher dokploy`), the integration generates Docker Compose artifacts and deploys them to your Dokploy instance via the REST API.

## Repository Structure

```
src/
  Sorvia.Aspire.Hosting.Dokploy/   # Dokploy hosting integration package
demo/
  Apphost/                          # Sample AppHost showing usage
```

## Prerequisites

- .NET 10 SDK
- A running [Dokploy](https://dokploy.com) instance
- A Dokploy API key (Settings → API in the Dokploy panel)

## Building

```shell
dotnet build
```

## License

[MIT](LICENSE)