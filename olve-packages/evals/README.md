# Skill evals

Each directory is a `claude plugin eval` case: a prompt, a scaffold that seeds the workspace, and graders. The default run also runs a baseline without the plugin, so the score delta shows what the skill adds.

Run from `olve-packages/`:

```bash
claude plugin eval . --scaffold --allow-tools Write --no-publish
```

`--scaffold` runs the case's `scaffold.sh` on your machine, and each agent run uses your own Claude credential. `--case <name>` picks one case, `--runs <n>` sets the repeats (default 3). Results land in `evals/results/`, which is gitignored.

| Case | Skill | Checks |
| --- | --- | --- |
| `typespec-service` | `olve-typespec` | A handler, an error writer and the wiring for a contract the skill's examples don't contain |

Graders read the source: they check generated names and calls, not that the code compiles. The prompt tells the agent to write the files without compiling, otherwise a baseline run stops and asks instead of showing what it knows.
