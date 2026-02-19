# Project Conventions

## Commits
- Short single subject line, conventional commit format. No verbose body.

## Tests
- Test framework: TUnit.
- Platform-specific tests use `[LinuxOnly]`/`[WindowsOnly]` attributes (defined in `tests/Olve.Paths.Tests/PathTests.cs`).
- Run tests with `dotnet test --project <path>` — do NOT use `--logger` flag (causes TUnit to report zero tests).

## README code snippets
- Code examples use [embedme](https://github.com/zakhenry/embedme) sourced from `ReadmeDemo.cs` test files. Run `npx embedme <README.md>` to populate.
- When embedme isn't feasible (e.g. ASP.NET middleware registration), hand-written inline snippets are acceptable.
- No test assertions in snippets. Use inline comments to show expected values instead (e.g. `dict.TryGet("alice", out var id); // 1`). The embedme line range should end before any assertions.
- Unexplained variables from test setup are fine. The reader can follow the embedme path to the test file for full context.
