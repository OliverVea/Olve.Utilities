// Snapshot tests: compile test/fixtures/<name>.tsp with the emitter and compare every emitted
// file with test/snapshots/<name>/. A missing or different snapshot fails; to accept new output,
// run `npm run test:update` (UPDATE_SNAPSHOTS=1) and review the diff.
//
// The widgets fixture and its output are also the olve-typespec skill's examples
// (olve-packages/skills/olve-typespec/references/, built by skill-examples.js): the same run keeps
// them current.

import assert from "node:assert/strict";
import { mkdtemp, mkdir, readdir, readFile, rm, writeFile } from "node:fs/promises";
import { tmpdir } from "node:os";
import { dirname, join } from "node:path";
import { test } from "node:test";
import { fileURLToPath } from "node:url";
import { compile, NodeHost } from "@typespec/compiler";
import { buildExamples } from "./skill-examples.js";

const here = dirname(fileURLToPath(import.meta.url));
const emitterDir = join(here, "..");
const skillReferences = join(emitterDir, "..", "..", "olve-packages", "skills", "olve-typespec", "references");
const update = process.env.UPDATE_SNAPSHOTS === "1";
const ERROR_ENVELOPE = { "external-types": { ErrorEnvelope: "FixtureErrorEnvelope" } };

async function emit(fixture, options = {}) {
  const outputDir = await mkdtemp(join(tmpdir(), "typespec-csharp-"));
  try {
    const program = await compile(NodeHost, join(here, "fixtures", `${fixture}.tsp`), {
      emit: [emitterDir],
      options: { "@olivervea/typespec-csharp": { "emitter-output-dir": outputDir, ...options } },
    });
    const files = {};
    for (const name of (await readdir(outputDir)).sort()) files[name] = await readFile(join(outputDir, name), "utf8");
    return { diagnostics: program.diagnostics, files };
  } finally {
    await rm(outputDir, { recursive: true, force: true });
  }
}

/** Compares `files` with the snapshot directory (or rewrites it under UPDATE_SNAPSHOTS=1). */
async function matchDirectory(dir, files, label) {
  if (update) {
    await rm(dir, { recursive: true, force: true });
    await mkdir(dir, { recursive: true });
    for (const [name, content] of Object.entries(files)) await writeFile(join(dir, name), content);
    return;
  }
  const expected = (await readdir(dir).catch(() => [])).sort();
  assert.deepEqual(
    Object.keys(files).sort(),
    expected,
    `emitted files differ from ${label} (UPDATE_SNAPSHOTS=1 to accept)`,
  );
  for (const name of expected) {
    assert.equal(files[name], await readFile(join(dir, name), "utf8"), `${label}/${name} differs from its snapshot`);
  }
}

const diagnosticCodes = (diagnostics) => diagnostics.map((d) => `${d.severity} ${d.code}: ${d.message}`);

test("widgets fixture matches its snapshots", async () => {
  const { diagnostics, files } = await emit("widgets", { namespace: "Fixture.Api", ...ERROR_ENVELOPE });
  assert.deepEqual(diagnosticCodes(diagnostics), []);
  await matchDirectory(join(here, "snapshots", "widgets"), files, "test/snapshots/widgets");
});

test("the skill's examples are built from the widgets fixture and its output", async () => {
  const { files } = await emit("widgets", { namespace: "Fixture.Api", ...ERROR_ENVELOPE });
  const fixture = await readFile(join(here, "fixtures", "widgets.tsp"), "utf8");
  await matchDirectory(
    join(skillReferences, "contract"),
    { "widgets.tsp": fixture },
    "olve-typespec/references/contract",
  );
  await matchDirectory(
    join(skillReferences, "examples"),
    await buildExamples(files),
    "olve-typespec/references/examples",
  );
});

test("output is deterministic", async () => {
  const [a, b] = [await emit("widgets", ERROR_ENVELOPE), await emit("widgets", ERROR_ENVELOPE)];
  assert.deepEqual(a.files, b.files);
});

test("the prefix defaults to the service namespace's name", async () => {
  const { files } = await emit("widgets", ERROR_ENVELOPE);
  assert.deepEqual(Object.keys(files).sort(), ["FixtureApi.g.cs", "FixtureJsonContext.g.cs", "FixtureModels.g.cs"]);
  const api = files["FixtureApi.g.cs"];
  assert.match(api, /public static class FixtureOperations/);
  assert.match(
    api,
    /public static IServiceCollection AddFixtureApi\(this IServiceCollection services\) => services\.AddTypeSpecApi\(FixtureJsonContext\.Default\);/,
  );
  assert.match(
    api,
    /public static FixtureEndpoints UseFixtureApi\(this WebApplication app\) => app\.UseTypeSpecApi\(HandlerTypes\)\.MapFixtureApi\(\);/,
  );
  assert.match(api, /public static FixtureEndpoints MapFixtureApi\(this IEndpointRouteBuilder app\)/);

  const custom = await emit("widgets", { prefix: "Shop", ...ERROR_ENVELOPE });
  assert.deepEqual(Object.keys(custom.files).sort(), ["ShopApi.g.cs", "ShopJsonContext.g.cs", "ShopModels.g.cs"]);
  assert.match(custom.files["ShopApi.g.cs"], /UseShopApi/);
});

const record = (source, name) =>
  source.match(new RegExp(`public sealed record ${name}\\n\\{[\\s\\S]*?\\n\\}`))?.[0] ?? "";

test("read-only properties are split into per-visibility request shapes", async () => {
  const models = (await emit("widgets")).files["FixtureModels.g.cs"];
  // Response: id, no update-only note. POST (Create): serial, no id/note. PUT (Create|Update): serial and note.
  assert.match(record(models, "Widget"), /Guid Id/);
  assert.doesNotMatch(record(models, "Widget"), / Note /);
  assert.match(record(models, "WidgetCreate"), / Serial /);
  assert.doesNotMatch(record(models, "WidgetCreate"), / Note | Id /);
  assert.match(record(models, "WidgetWritable"), / Serial /);
  assert.match(record(models, "WidgetWritable"), / Note /);
  assert.doesNotMatch(record(models, "WidgetWritable"), / Id /);
});

// Every shape the olve-typespec skill lists as reported, one diagnostic each, nothing mistyped.
test("unsupported shapes are reported, not silently mistyped", async () => {
  const { diagnostics, files } = await emit("unsupported");
  assert.deepEqual(diagnosticCodes(diagnostics).sort(), [
    "warning @olivervea/typespec-csharp/unsupported-operation: Operation 'ids': array parameter 'id' must be a string list in the query with explode: false.",
    "warning @olivervea/typespec-csharp/unsupported-operation: Operation 'meta': a {+…} path parameter must be the route's only one, and its last segment.",
    "warning @olivervea/typespec-csharp/unsupported-operation: Operation 'note': a text/plain body must be JSON, or bytes (read raw).",
    "warning @olivervea/typespec-csharp/unsupported-operation: Operation 'range': status code ranges and default responses are not supported.",
    "warning @olivervea/typespec-csharp/unsupported-operation: Operation 'session': cookie parameter 'sid' is not supported.",
    "warning @olivervea/typespec-csharp/unsupported-operation: Operation 'stamped': response header 'Last-Modified' must be a string, an integer or a string literal.",
    "warning @olivervea/typespec-csharp/unsupported-operation: Operation 'tags': array parameter 'tag' must be a string list in the query with explode: false.",
    "warning @olivervea/typespec-csharp/unsupported-operation: Operation 'ticks': event 'tick' of 'Ticks' must be a model with `type: \"tick\"`.",
    "warning @olivervea/typespec-csharp/unsupported-operation: Operation 'upload': multipart bodies are not supported.",
    "warning @olivervea/typespec-csharp/unsupported-operation: Operation 'xml': a application/xml response must be JSON, text/* string or bytes.",
    "warning @olivervea/typespec-csharp/unsupported-type: Unsupported type shape (discriminated union with an envelope); emitted as System.Text.Json.JsonElement.",
    "warning @olivervea/typespec-csharp/unsupported-type: Unsupported type shape (union without a discriminator); emitted as System.Text.Json.JsonElement.",
  ]);
  // The mistyped shapes land as JsonElement, so a reader can't mistake them for a typed model.
  const models = files["UnsupportedModels.g.cs"];
  assert.match(record(models, "A"), /System\.Text\.Json\.JsonElement X/);
  assert.match(files["UnsupportedApi.g.cs"], /record PetRequest;[\s\S]*?Ok\(System\.Text\.Json\.JsonElement Body\)/);
});

test("SSE operations stream their @events union with ids; explode:false lists bind from one string", async () => {
  const { files } = await emit("widgets", ERROR_ENVELOPE);
  const api = files["FixtureApi.g.cs"];
  const models = files["FixtureModels.g.cs"];
  assert.match(
    api,
    /IWidgetEventsStreamHandler : IOperationHandler<WidgetEventsStreamRequest, WidgetEventsStreamResponse>/,
  );
  assert.match(api, /record Ok\(IAsyncEnumerable<ServerSentEvent<WidgetEvent>> Events\) : WidgetEventsStreamResponse/);
  assert.match(api, /ToHttpResult\(\) => new ServerSentEventsResult<WidgetEvent>\(Events\)/);
  assert.match(
    api,
    /await handler\.HandleAsync\(new WidgetEventsStreamRequest\(QueryParameters\.List\(type\), lastEventId\), ct\)\)\.ToHttpResult\(\)/,
  );
  assert.match(api, /\.Produces<WidgetEvent>\(200, "text\/event-stream"\)/);
  assert.match(
    models,
    /\[JsonPolymorphic\(TypeDiscriminatorPropertyName = "type"\)\]\n\[JsonDerivedType\(typeof\(Ping\), "ping"\)\]/,
  );
  assert.match(models, /public abstract record WidgetEvent : IServerSentEvent/);
  assert.match(record(models, "WidgetChanged : WidgetEvent"), /override string EventType => "widget\.changed"/);
  // The discriminator is written by the polymorphism, not a property of its own.
  assert.doesNotMatch(record(models, "WidgetChanged : WidgetEvent"), /"type"/);
});

test("every declared response is a variant of the operation's response union", async () => {
  const { files } = await emit("widgets", ERROR_ENVELOPE);
  const api = files["FixtureApi.g.cs"];
  // 201 | 202 | 400: one variant each, named by status, each writing its own status.
  assert.match(api, /public abstract record WidgetsCreateResponse : IOperationResponse/);
  assert.match(api, /public sealed record Created\(Widget Body\) : WidgetsCreateResponse[\s\S]*?Status => 201;/);
  assert.match(api, /public sealed record Accepted\(Widget Body\) : WidgetsCreateResponse[\s\S]*?Status => 202;/);
  assert.match(api, /public sealed record BadRequest\(FixtureErrorEnvelope Body\) : WidgetsCreateResponse/);
  assert.match(api, /new\("Widgets_create", \[201, 202\], \[400\]\)/);
  // A success body only one variant carries converts implicitly; Widget is carried twice here.
  assert.doesNotMatch(api, /implicit operator WidgetsCreateResponse/);
  assert.match(api, /implicit operator WidgetsUpdateResponse\(Widget body\) => new Ok\(body\)/);
  // Errors never convert implicitly; interfaces can't.
  assert.doesNotMatch(api, /implicit operator \w+\(FixtureErrorEnvelope/);
  assert.doesNotMatch(api, /implicit operator \w+\(IReadOnlyList/);
  // A bodyless response.
  assert.match(
    api,
    /public sealed record NoContent\(\) : WidgetsDeleteResponse[\s\S]*?TypedResults\.StatusCode\(204\)/,
  );
});
