# Path

https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.Path.html

Static class. Factory for creating `IPurePath` and `IPath` instances.

## Pure Path Creation

- `IPurePath CreatePure(string path)` -- creates a pure path using the current OS platform
- `IPurePath CreatePure(string path, PathPlatform platform)` -- creates a pure path for a specific platform

## Path Creation

- `IPath Create(string path, IPathEnvironment? pathEnvironment = null)` -- creates an environment-aware path using the current OS platform
- `IPath Create(string path, PathPlatform platform, IPathEnvironment? pathEnvironment = null)` -- creates an environment-aware path for a specific platform

## Assembly and Directory Paths

- `bool TryGetAssemblyExecutable(out IPath? path, IPathEnvironment? pathEnvironment = null)` -- attempts to get the current assembly's executable path
- `bool TryGetAssemblyExecutablePure(out IPurePath? purePath, IPathEnvironment? pathEnvironment = null)` -- attempts to get the assembly executable as a pure path
- `IPurePath GetCurrentDirectoryPure()` -- current working directory as IPurePath
- `IPath GetCurrentDirectory()` -- current working directory as IPath
- `IPurePath GetHomeDirectoryPure()` -- user's home directory as IPurePath (reads HOME env var)
- `IPath GetHomeDirectory()` -- user's home directory as IPath (reads HOME env var)

## Temporary Paths

- `IPath GetTempDirectory()` -- system temp directory
- `IPath CreateTempDirectory(string? prefix = null)` -- creates a unique temp subdirectory
- `IPath CreateTempFile()` -- creates a unique zero-byte temp file

## Behavior Notes

- On unsupported platforms, returns `UnsupportedPath`/`UnsupportedPurePath` which throw `PlatformNotSupportedException` on any access.
- When the path string points to an existing directory, a trailing separator is appended automatically.
- Windows paths normalize forward slashes to backslashes.
- `IPathEnvironment` can be injected for testing (provides current directory and assembly executable path).

## Usage

```csharp
// Auto-detect platform
var path = Path.Create("/home/user/docs");
var pure = Path.CreatePure("/home/user/docs");

// Explicit platform
var unix = Path.CreatePure("/home/user", PathPlatform.Unix);
var win = Path.CreatePure(@"C:\users\user", PathPlatform.Windows);

// Temp paths
var tmpDir = Path.CreateTempDirectory("my-app-");
var tmpFile = Path.CreateTempFile();

// Assembly path
if (Path.TryGetAssemblyExecutable(out var exe))
{
    var binDir = exe.Parent;
}
```
