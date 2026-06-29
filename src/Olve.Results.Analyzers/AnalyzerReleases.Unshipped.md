; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
ORES001 | Usage    | Warning  | Return value of a type marked [MustBeUsedWhenReturned] must be observed
ORES002 | Usage    | Error    | A [GenerateResult] case factory may declare at most one parameter
ORES003 | Usage    | Warning  | A [GenerateResult] type declares no case factories
ORES004 | Usage    | Error    | A [GenerateResult] case factory must be declared 'static partial'
