## v0.5.1 (2025-01-30)

### Fix

- tagging `TryAsSet` and `TryAsReadOnlySet` output with `NotNullWhen(true)`

## v0.5.0 (2025-01-30)

### BREAKING CHANGE

- name of arguments of types implementing `IOperation` or `IAsyncOperation` should be changed from `input` to `request`

### Fix

- `IOperation` and `IAsyncOperation` uses `TRequest request` instead of `TInput input` as argument

## v0.4.0 (2025-01-29)

### Feat

- adding `OneToManyLookup`

### Fix

- adding `Meziantou.Analyzer` and rectifying issues

## v0.3.0 (2025-01-29)

### Feat

- adding `TryAsSet<T>` and `TryAsReadOnlySet<T>` on `IEnumerable<T>`

## v0.2.0 (2025-01-17)

### Feat

- added `Assert.IsEmpty(IEnumerable<T>)` and `Assert.IsNotEmpty(IEnumerable<T>)`

## v0.1.2 (2025-01-17)

### Fix

- release moved to same action as bump to keep new changelog

## v0.1.1 (2025-01-17)

### Fix

- making release step not require git ref tag

## v0.1.0 (2025-01-17)

### Feat

- added extension method ToResultFunc to map this Action<T> to Func<T, Result>
