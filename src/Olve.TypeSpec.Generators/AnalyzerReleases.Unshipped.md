; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
OTSP001 | Usage    | Error    | More than one handler implements an operation
OTSP002 | Usage    | Error    | A handler is not accessible to AddOperationHandlers
OTSP003 | Usage    | Error    | A handler implements no operation
OTSP004 | Usage    | Error    | A handler can't be instantiated
OTSP005 | Usage    | Error    | An operation has no handler
