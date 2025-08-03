# Olve.MinimalApi

[![NuGet](https://img.shields.io/nuget/v/Olve.MinimalApi?logo=nuget)](https://www.nuget.org/packages/Olve.MinimalApi)

Minimal API extensions for ASP.NET Core, including JSON conversion for `IPath`, result mapping, and validation helpers.

## Installation

```bash
dotnet add package Olve.MinimalApi
```

## Usage

```csharp
using Olve.MinimalApi;
var builder = WebApplication.CreateBuilder(args);

// Add Path JSON converter for IPath types
builder.Services.WithPathJsonConversion();

// Map Results to HTTP responses
builder.Services.AddResultMapping();

// Add validation endpoints
builder.Services.AddValidation();

var app = builder.Build();

app.MapGet("/files/{*path}", (IPath path) =>
    Results.Ok(new { path }));

app.Run();
```

Generated XML documentation and README are included in the NuGet package.

## Contributing

See [CONTRIBUTING.md](../../CONTRIBUTING.md) in the repository root.
