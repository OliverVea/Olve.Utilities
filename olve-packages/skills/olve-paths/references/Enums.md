# Enums

## PathPlatform

https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.PathPlatform.html

Specifies the platform a path is intended for.

Values:
- `None` -- no specific platform
- `Windows` -- Windows-style path (backslash separator, drive letters)
- `Unix` -- Unix-style path (forward slash separator)

## PathType

https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.PathType.html

Describes the classification of a path string.

Values:
- `Stub` -- a bare name, not rooted (e.g., `file.txt`, `subdir`)
- `Relative` -- starts with `.` or `..` (e.g., `./file.txt`, `../parent`)
- `Absolute` -- rooted path (e.g., `/home/user`, `C:\users\user`)

Behavior during concatenation:
- **Stub** paths are appended directly to the left path's segments.
- **Relative** paths are resolved: `..` pops segments from the left path.
- **Absolute** paths cannot be on the right side of a concatenation (throws `NotSupportedException`).

## ElementType

https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.ElementType.html

Represents the type of a filesystem element.

Values:
- `None` -- no specific type (path does not exist or type is unknown)
- `Directory` -- the element is a directory
- `File` -- the element is a file
