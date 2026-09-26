# Project Conventions

## Commits
- Short single subject line, conventional commit format. No verbose body.
- PRs use rebase merge (no merge commits or squash).
- PR descriptions link the issues they resolve with `Closes #N`, so issues don't outlive their fix.

## GitHub CLI
- `gh issue view N` fails with a Projects (classic) deprecation error. Use `gh issue view N --json title,body,comments`.
- Before working an issue, check recent commits for an existing fix (`git log --oneline -20`); issues can lag behind the code.

## Tests
- Test framework: TUnit.
- Platform-specific tests use `[LinuxOnly]`/`[WindowsOnly]` attributes (defined in `tests/Olve.Paths.Tests/PathTests.cs`).
- Run tests with `dotnet test --project <path>` — do NOT use `--logger` flag (causes TUnit to report zero tests).
- Test observable behaviour, not internal fields. No `InternalsVisibleTo` for tests, and no public members added only so tests can inspect state.

## README code snippets
- Code examples use [embedme](https://github.com/zakhenry/embedme) sourced from `ReadmeDemo.cs` test files. Run `npx embedme <README.md>` to populate.
- When embedme isn't feasible (e.g. ASP.NET middleware registration), hand-written inline snippets are acceptable.
- No test assertions in snippets. Use inline comments to show expected values instead (e.g. `dict.TryGet("alice", out var id); // 1`). The embedme line range should end before any assertions.
- Unexplained variables from test setup are fine. The reader can follow the embedme path to the test file for full context.
