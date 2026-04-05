# Extension Classes

## PathExtensions

https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.PathExtensions.html

Extension methods for `IPath`.

- `bool EnsurePathExists(this IPath path)` -- ensures the directory at the given path exists, creating it if necessary. Returns `true` if the directory was created, `false` if it already existed.

```csharp
var dir = Path.Create("/home/user/new-dir");
var created = dir.EnsurePathExists(); // true if it was created
```

## PurePathMatchExtensions

https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.PurePathMatchExtensions.html

Functional pattern matching on `IPurePath` implementations. Returns a value based on the runtime type.

- `TOut Match<T, TOut>(this IPurePath path, Func<T, TOut> action, Func<TOut> fallback)` -- matches against one type
- `TOut Match<T1, T2, TOut>(this IPurePath path, Func<T1, TOut> action1, Func<T2, TOut> action2, Func<TOut> fallback)` -- matches against two types
- `TOut Match<T1, T2, T3, TOut>(this IPurePath path, Func<T1, TOut> action1, Func<T2, TOut> action2, Func<T3, TOut> action3, Func<TOut> fallback)` -- matches against three types

All type parameters are constrained to `IPurePath`. The first matching type wins; `fallback` is called if none match.

## PurePathSwitchExtensions

https://olivervea.github.io/Olve.Utilities/api/Olve.Paths.PurePathSwitchExtensions.html

Functional switch (void-returning) on `IPurePath` implementations. Executes an action based on the runtime type.

- `void Switch<T>(this IPurePath path, Action<T> action, Action? fallback = null)` -- switches on one type
- `void Switch<T1, T2>(this IPurePath path, Action<T1> action1, Action<T2> action2, Action? fallback = null)` -- switches on two types
- `void Switch<T1, T2, T3>(this IPurePath path, Action<T1> action1, Action<T2> action2, Action<T3> action3, Action? fallback = null)` -- switches on three types

All type parameters are constrained to `IPurePath`. The first matching type wins; `fallback` is called if none match.
