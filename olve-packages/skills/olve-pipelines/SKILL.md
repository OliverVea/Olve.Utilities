---
name: olve-pipelines
description: How Olve.Pipelines works — a GitOps CD orchestration service for the homelab. Pipelines are bound to a Git repo and shaped by `.pipelines/config.yaml` (parallel production steps → ArtifactBundle → sequential processing steps, run as Kubernetes Jobs). Use when adding CD to a repo, writing/editing a `.pipelines/config.yaml`, or operating a pipeline (triggers, secrets, promotion gate).
user-invocable: false
---

# Olve.Pipelines

A lightweight **CD orchestration service** for the homelab — not a NuGet library. It runs your
build/deploy steps as Kubernetes Jobs. Source & docs: `OliverVea/Olve.Pipelines`.

## Model

```
Production [N steps, parallel] ──(ArtifactBundle)──> Processing 1 ──> … ──> Processing N [sequential]
```

- **Production steps** run in parallel; each writes to `bundle/<step-name>/`. Combined = the **ArtifactBundle**.
- **Processing steps** run sequentially (list order); each receives the full bundle. So an earlier step gates later ones.
- Every step is `(image, script, env)`, executed as a K8s Job.

## GitOps: shape is git-owned, operations are API-allowed

A pipeline is **always bound to a Git repo** — that binding is the only way to create one. Your repo's
`.pipelines/config.yaml` is the single source of truth: a reconcile loop (~5 min) materializes steps
and triggers to match it, then builds. **There are no config-mutation API endpoints** — you can't
create/reorder/delete steps over HTTP. What *is* API-mutable: triggering production, cancelling jobs,
setting secret *values*, and the **promotion gate** (block/unblock + re-promote a processing step).

Secrets are declared by **name only**; values live in the pipeline's k8s secret (`olve-pipeline-{id}`),
never in the repo. Reference as `$SECRET:NAME` (env values / poll headers) or as a plain env var in scripts.

## Adding CD to a repo

1. Add `.pipelines/config.yaml` (+ optional `.pipelines/scripts/*.sh`). Starter: `OliverVea/Olve.Template.Api`.
2. `POST /api/pipelines/with-repo` with `{ name, repo, branch?, path?, credentialsSecret }` — the only create path.
3. Set declared secret values: `PUT /api/pipelines/{id}/secrets/{name}`.
4. Push. Check `GET /api/pipelines/{id}/binding/status` for the reconcile result.

## Docs (don't reinvent — read these)

The repo's `docs/setup/` is the canonical, agent-friendly guide (also served live at `/docs`):

- [config-reference.md](https://github.com/OliverVea/Olve.Pipelines/blob/main/docs/setup/config-reference.md) — full `config.yaml` schema, `$ref`/`scriptFile`, validation rules
- [getting-started.md](https://github.com/OliverVea/Olve.Pipelines/blob/main/docs/setup/getting-started.md) · [binding-and-reconcile.md](https://github.com/OliverVea/Olve.Pipelines/blob/main/docs/setup/binding-and-reconcile.md) · [promotion-gate.md](https://github.com/OliverVea/Olve.Pipelines/blob/main/docs/setup/promotion-gate.md) · [troubleshooting.md](https://github.com/OliverVea/Olve.Pipelines/blob/main/docs/setup/troubleshooting.md)
- [README.md](https://github.com/OliverVea/Olve.Pipelines/blob/main/README.md) — in-code reference: full endpoint tables, configuration keys, client generation

When working **inside the Olve.Pipelines repo**, prefer its scoped skills (`deploy`, `setup-pipeline`, `manual-test`).
