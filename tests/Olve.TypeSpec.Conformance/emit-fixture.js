// Compiles the widgets fixture (src/typespec-csharp/test/fixtures/widgets.tsp) the way a service
// compiles its own spec: OpenAPI 3.2 JSON via @typespec/openapi3, and the C# surface via the
// emitter. Output goes to <repo>/artifacts/conformance/{spec,generated} (or the folder given as the
// first argument), never into src/. Invoked by the csproj before it builds; fails on any error diagnostic.

import { dirname, join, resolve } from "node:path";
import { fileURLToPath, pathToFileURL } from "node:url";
import { createRequire } from "node:module";

const here = dirname(fileURLToPath(import.meta.url));
const repoRoot = join(here, "..", "..");
const emitterDir = join(repoRoot, "src", "typespec-csharp");
const outDir = process.argv[2] ? resolve(process.argv[2]) : join(repoRoot, "artifacts", "conformance");

// The TypeSpec packages are the emitter's dev dependencies (src/typespec-csharp/node_modules).
const require = createRequire(join(emitterDir, "package.json"));
// A file URL, not a path: Windows absolute paths are not importable as-is.
const { compile, NodeHost } = await import(pathToFileURL(require.resolve("@typespec/compiler")).href);

const program = await compile(NodeHost, join(emitterDir, "test", "fixtures", "widgets.tsp"), {
  emit: ["@typespec/openapi3", emitterDir],
  options: {
    "@typespec/openapi3": {
      "openapi-versions": ["3.2.0"],
      "file-type": "json",
      "emitter-output-dir": join(outDir, "spec"),
    },
    "@olivervea/typespec-csharp": {
      "emitter-output-dir": join(outDir, "generated"),
      namespace: "Fixture.Api",
      prefix: "Fixture",
      "external-types": { ErrorEnvelope: "Olve.TypeSpec.Conformance.FixtureErrorEnvelope" },
    },
  },
});

for (const d of program.diagnostics) console.error(`${d.severity} ${d.code}: ${d.message}`);
if (program.diagnostics.some((d) => d.severity === "error")) process.exit(1);
console.log(`widgets fixture emitted to ${outDir}`);
