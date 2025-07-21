# Olve.Results
[![NuGet](https://img.shields.io/nuget/v/Olve.Results?logo=nuget)](https://www.nuget.org/packages/Olve.Results)

Lightweight result type used for error handling throughout the repository. It represents success or a collection of problems instead of throwing exceptions.

## Installation

```bash
dotnet add package Olve.Results
```

## Usage

```csharp
Result<string> result = Result.Try(() => File.ReadAllText("/tmp/test.txt"));
if (result.TryPickProblems(out var problems, out var text))
{
    foreach (var p in problems)
        Console.WriteLine(p.Message);
    return;
}

Console.WriteLine(text);

// chain multiple steps
Result<(GL, IInputContext)> contexts = Result.Concat(
    () => Result.Try(() => window.CreateOpenGL(), "create GL"),
    () => Result.Try(() => window.CreateInput(), "create input"));

// aggregate results
foreach (var service in services)
    loadResults.Add(service.Load());
if (loadResults.TryPickProblems(out var loadProblems))
    return loadProblems.Prepend("scene load failed");
```

See the [API documentation](https://olivervea.github.io/Olve.Utilities/api/) for more details.
