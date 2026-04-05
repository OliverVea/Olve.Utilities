# IPath

https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.IPath.html

Interface. Extends `IPurePath` with environment-aware filesystem operations.

Inherited from IPurePath:
- `string Path`
- `PathPlatform Platform`
- `PathType Type`
- `string? Name`
- `IPurePath ParentPure`
- `bool TryGetParentPure(out IPurePath? parent)`
- `bool TryGetName(out string? fileName)`
- `IPurePath Append(IPurePath right)`
- `IPurePath Append(string right)`

Properties:
- `ElementType ElementType` -- type of the filesystem element (File, Directory, or None); throws `InvalidOperationException` if undetermined
- `IPath Absolute` -- absolute version of the path; throws `InvalidOperationException` if unavailable
- `IPath Parent` -- parent as IPath; throws `InvalidOperationException` if no parent
- `IEnumerable<IPath> Children` -- child elements; throws `InvalidOperationException` if no children

Methods:
- `bool TryGetElementType(out ElementType type)` -- attempts to determine the filesystem element type
- `bool TryGetAbsolute(out IPath? absolute)` -- attempts to resolve the absolute path
- `bool TryGetParent(out IPath? parent)` -- attempts to retrieve the parent as IPath
- `bool TryGetChildren(out IEnumerable<IPath>? children)` -- attempts to retrieve child elements (directories and files)
- `IPath AppendPath(IPurePath right)` -- appends a pure path, returns IPath
- `IPath AppendPath(string right)` -- appends a string segment, returns IPath
- `bool Exists()` -- checks whether the path exists on the filesystem
- `string GetLinkString(int? lineNumber = null, int? columnNumber = null)` -- returns a clickable link string (e.g., `file:///path:line:col`)

Operators:
- `IPath operator /(IPath left, IPurePath right)` -- concatenates via AppendPath
- `IPath operator /(IPath left, string right)` -- concatenates via AppendPath

## Usage

```csharp
var path = Path.Create("/home/user/documents");

path.Exists();       // true/false
path.ElementType;    // ElementType.Directory
path.Parent;         // IPath for "/home/user"

var child = path / "file.txt";
child.Exists();      // check filesystem

if (path.TryGetChildren(out var children))
{
    foreach (var c in children) { /* ... */ }
}

path.TryGetAbsolute(out var abs);  // resolve relative to absolute
```
