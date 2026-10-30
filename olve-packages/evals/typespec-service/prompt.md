---
description: A handler, an error writer and the wiring for a contract the skill's examples don't contain
tags: [olve-typespec]
allowed_tools: [Read, Glob, Grep, Skill]
max_turns: 25
---
This C# service uses Olve.TypeSpec: `spec/main.tsp` is the contract, compiled by `@olivervea/typespec-csharp` with `spec/tspconfig.yaml`. The generated code isn't checked in and you can't compile here, so write the files as you expect them to compile against what the emitter generates. Don't stop to ask.

Write these three files (all in the namespace `Gadgets.Api`, which also holds the generated types):

1. `src/RenameGadgetHandler.cs`: the handler of `Gadgets_rename`. Look the gadget up in the `GadgetStore` from `src/GadgetStore.cs`; an unknown id answers the contract's 404, a locked gadget its 409, both with the code `GADGET_NOT_FOUND` or `GADGET_LOCKED`. Otherwise store the new name and answer the updated gadget.
2. `src/GadgetErrorWriter.cs`: the app's error writer for binding and validation failures. It answers `AppError` with the code `INVALID_REQUEST` and the first problem as the message when the operation declares the status, and the bare status otherwise.
3. `src/Program.cs`: the minimal host that wires the generated API, the handlers and the error writer, and runs.
