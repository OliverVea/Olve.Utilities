## v0.19.2 (2025-04-12)

### Fix

- force redeploy

## v0.19.1 (2025-04-12)

### Fix

- bumped dependencies

## v0.19.0 (2025-04-12)

### Feat

- added directed graph and typed id classes

## v0.18.0 (2025-04-11)

### Feat

- using `Olve.Paths` in `Olve.Utilities`
- using `Olve.Paths` for `ProblemOriginInformation`

## v0.17.0 (2025-04-08)

### Feat

- globbing
- refactored purepath and path to be interfaces

## v0.16.0 (2025-04-06)

### Feat

- adding Olve.Paths

## v0.15.0 (2025-03-27)

### Feat

- **Olve.Results**: added `Result.IfProblem`
- adding static configurable default values to `ResultProblem.Tags`, `ResultProblem.Severity` and `ResultProblem.Source`

## v0.14.0 (2025-03-19)

### Feat

- adding `Result.Chain` to `Result<T>` and renaming the previous operation `Result.Concat` for clarity

### Refactor

- fixing file name

## v0.13.0 (2025-03-18)

### Feat

- **Olve.Result**: allow chaining `Result` using the static `Result.Chain` method

## v0.12.1 (2025-03-18)

### Fix

- removing `RequiresPreviewFeature` attributes

## v0.12.0 (2025-03-18)

### Feat

- adding `Result.Try<TException>` and `Result.Try<TValue, TException>`

## v0.11.0 (2025-03-18)

### Feat

- added more brief `ResultProblemCollection.Prepend` shorthand

### Fix

- fixed broken ci/cp pipeline

## v0.10.1 (2025-03-14)

### Fix

- fixing incorrect `NotNullWhen` boolean value for `TryPickProblems`

## v0.10.0 (2025-03-14)

### Feat

- adding missing `IEnumerable<Result>.TryPickProblems` for non-generic `Result`

### Perf

- added `[NotNullWhen]`, lazy initialization and allowed nullable results to `problems` and `values` in `TryPickProblems`

## v0.9.0 (2025-03-08)

### Feat

- adding `DeletionResult` for deletion operations
- adding `Success` class

### Fix

- specifying `problems` parameter in `Result.Success` static methods

## v0.8.0 (2025-03-05)

### Feat

- adding `ResultProblem.ToDebugString` with tests
- adding stacktrace information in `ResultProblem.OriginInformation`

### Fix

- setting .net version in CI to equal 9.0.100
- setting .net version in CI to equal 9.0.100

## v0.7.1 (2025-02-03)

### Fix

- improving support for linux in `ProjectFolderHelper.cs`

## v0.7.0 (2025-02-01)

### Feat

- adding `PickRandom`  and `PickRandomOrDefault` extension methods to `IEnumerable<T>`

### Refactor

- renamed unused lambda argument to `_`

## v0.6.0 (2025-01-31)

### Feat

- adding `GetOrAdd` and `TryUpdate` `Dictionary` extension methods

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
