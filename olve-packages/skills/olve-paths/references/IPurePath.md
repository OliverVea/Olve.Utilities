# IPurePath

https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.IPurePath.html

Interface. Immutable, pure file system path independent of the actual filesystem. No disk access.

Properties:
- `string Path` -- string representation of the path
- `PathPlatform Platform` -- platform of the path (Unix or Windows)
- `PathType Type` -- path classification (Absolute, Relative, or Stub)
- `string? Name` -- name of the last path component (filename or directory name), or null if unavailable
- `IPurePath ParentPure` -- parent path; throws `InvalidOperationException` if no parent

Methods:
- `bool TryGetParentPure(out IPurePath? parent)` -- attempts to retrieve the parent path
- `bool TryGetName(out string? fileName)` -- attempts to retrieve the name of the path component
- `IPurePath Append(IPurePath right)` -- appends another pure path
- `IPurePath Append(string right)` -- appends a string segment (must be a stub, not absolute or relative)

Operators:
- `IPurePath operator /(IPurePath left, IPurePath right)` -- concatenates two pure paths
- `IPurePath operator /(IPurePath left, string right)` -- concatenates a path with a string segment

## Path Types

- **Absolute** -- rooted path (e.g., `/home/user` or `C:\users\user`)
- **Relative** -- starts with `.` or `..`, resolved during concatenation
- **Stub** -- bare name like `file.txt` or `subdir`, appended directly

## Usage

```csharp
var path = Path.CreatePure("/home/user/docs", PathPlatform.Unix);

path.Path;       // "/home/user/docs"
path.Platform;   // PathPlatform.Unix
path.Type;       // PathType.Absolute
path.Name;       // "docs"
path.ParentPure; // IPurePath for "/home/user"

var joined = path / "readme.txt";  // "/home/user/docs/readme.txt"
```
