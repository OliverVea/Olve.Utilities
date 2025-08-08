# Olve.Logging

A lightweight in-memory logging library used across the Olve.* libraries.

Features
- Log messages in-memory with levels and optional source path/line and tags.
- Convert between Olve.Logging.LogLevel and Microsoft.Extensions.Logging.LogLevel.
- Produce LogMessage instances from ResultProblem for consistent logging.
- An in-memory implementation (InMemoryLoggingManager) suitable for unit tests and short-lived applications.

Quick start

- Add a reference to `src/Olve.Logging/Olve.Logging.csproj` from your project or via NuGet when published.
- Use `ILoggingManager` to log messages:

```csharp
ILoggingManager logging = new InMemoryLoggingManager();
logging.Log(LogLevel.Info, "Application started");
```

Documentation

Public API is included in the repository and will be published via DocFX.
