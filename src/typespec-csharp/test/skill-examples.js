// Builds the olve-typespec skill's examples (olve-packages/skills/olve-typespec/references/examples/)
// from the widgets fixture: one short file per topic, each pairing a region of the contract with the
// C# the emitter generates for it and, where there is one, the conformance suite's hand-written
// handler. An agent reads the one topic it needs. emitter.test.js compares these files with the
// current output (UPDATE_SNAPSHOTS=1 rewrites them), so they can't go stale.

import { readFile } from "node:fs/promises";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const here = dirname(fileURLToPath(import.meta.url));
const repoRoot = join(here, "..", "..", "..");
const conformance = join(repoRoot, "tests", "Olve.TypeSpec.Conformance");

/** The lines between `// #region <name>` and the next `// #endregion` (or C# `#region`/`#endregion`). */
export function region(source, name) {
  const lines = source.split("\n");
  const start = lines.findIndex((l) => new RegExp(`^\\s*(//\\s*)?#region ${name}\\s*$`).test(l));
  if (start < 0) throw new Error(`no region '${name}'`);
  const end = lines.findIndex((l, i) => i > start && /^\s*(\/\/\s*)?#endregion\b/.test(l));
  if (end < 0) throw new Error(`region '${name}' is not closed`);
  return dedent(lines.slice(start + 1, end).join("\n"));
}

/** A top-level C# declaration by name (`public sealed record Widget`, `public enum Color`, …), braces matched. */
export function declaration(source, name) {
  const lines = source.split("\n");
  const re = new RegExp(`^(public|internal) [^=(]*\\b${name}\\b`);
  let start = lines.findIndex((l) => re.test(l));
  if (start < 0) throw new Error(`no declaration '${name}'`);
  // Attributes and doc comments directly above the declaration belong to it.
  while (start > 0 && /^\s*(\[|\/\/\/)/.test(lines[start - 1])) start--;
  let depth = 0;
  for (let i = start; i < lines.length; i++) {
    for (const ch of lines[i]) {
      if (ch === "{") depth++;
      else if (ch === "}") depth--;
    }
    if (lines[i].trimEnd().endsWith(";") && depth === 0) return lines.slice(start, i + 1).join("\n");
    if (depth === 0 && lines[i].includes("}")) return lines.slice(start, i + 1).join("\n");
  }
  throw new Error(`declaration '${name}' is not closed`);
}

/** The `Name = app.Map…(…)` entry of one operation in `Map<Prefix>Api`, up to its trailing comma. */
export function endpoint(source, operation) {
  const lines = source.split("\n");
  const start = lines.findIndex((l) => new RegExp(`^\\s+${operation} = app\\.Map`).test(l));
  if (start < 0) throw new Error(`no endpoint '${operation}'`);
  const end = lines.findIndex(
    (l, i) => i > start && /^\s+\.Produces[^,]*,\s*$/.test(l) && !/^\s+\.Produces/.test(lines[i + 1] ?? ""),
  );
  return dedent(lines.slice(start, end + 1).join("\n"));
}

function dedent(text) {
  const lines = text.replace(/^\n+|\n+$/g, "").split("\n");
  const indent = Math.min(...lines.filter((l) => l.trim()).map((l) => l.match(/^\s*/)[0].length));
  return lines.map((l) => l.slice(indent)).join("\n");
}

const fence = (lang, code) => `\`\`\`${lang}\n${code.replace(/\n+$/, "")}\n\`\`\``;

/**
 * @param files the emitter's output for the widgets fixture (name → content)
 * @returns the example files (name → markdown)
 */
export async function buildExamples(files) {
  const contract = await readFile(join(here, "fixtures", "widgets.tsp"), "utf8");
  const handlers = await readFile(join(conformance, "FixtureHandlers.cs"), "utf8");
  const app = await readFile(join(conformance, "FixtureApp.cs"), "utf8");
  const errors = await readFile(join(conformance, "FixtureErrors.cs"), "utf8");
  const models = files["FixtureModels.g.cs"];
  const api = files["FixtureApi.g.cs"];
  const tsp = (name) => fence("typespec", region(contract, name));
  const cs = (...parts) => fence("csharp", parts.join("\n\n"));

  const header = (title, intro) =>
    `# ${title}\n\nGenerated from \`src/typespec-csharp/test/fixtures/widgets.tsp\` by the emitter's tests; do not edit.\n\n${intro}\n`;

  return {
    "response-unions.md": [
      header(
        "Response unions",
        "An operation's declared responses become one closed union with a variant per status. The handler returns a variant, so a status the contract doesn't declare can't compile. A success body no other variant carries converts implicitly; errors are always named.",
      ),
      "## Contract\n",
      tsp("operations"),
      "\n## Generated: `Widgets_create` (201 | 202 | 400)\n",
      cs(
        declaration(api, "WidgetsCreateRequest"),
        declaration(api, "WidgetsCreateResponse"),
        declaration(api, "IWidgetsCreateHandler"),
      ),
      "\n## Generated: `Widgets_update` (200 | 400 | 404 | 409, implicit `Ok`)\n",
      cs(declaration(api, "WidgetsUpdateResponse")),
      "\n## Hand-written handlers\n\nThe conformance suite's handlers answer magic ids so its tests can drive every variant; a real handler would look up the id.\n",
      cs(region(handlers, "create")),
      "",
      cs(region(handlers, "update")),
      "",
      cs(region(handlers, "delete")),
    ].join("\n"),

    "visibility.md": [
      header(
        "Visibility: request shapes",
        "A property's `@visibility` decides which shapes carry it. The response keeps everything; a request shape is named for what it may send: `<Model>Create` for a POST, `<Model>Writable` when Create and Update send the same properties, `<Model>Update` when they differ.",
      ),
      "## Contract\n",
      tsp("visibility"),
      "\n## Generated\n",
      cs(declaration(models, "Widget"), declaration(models, "WidgetCreate"), declaration(models, "WidgetWritable")),
    ].join("\n"),

    "discriminated-union.md": [
      header(
        "Discriminated unions",
        'A `@discriminated(#{ envelope: "none", discriminatorPropertyName })` union becomes an abstract record with `[JsonPolymorphic]` and one `[JsonDerivedType]` per variant; the variants derive from it and lose the discriminator property (the polymorphism writes it). The discriminator may appear anywhere in a request object. A handler that receives the union switches on the variant records (`Circle circle => …, Square square => …`); one that answers it returns a variant, which converts implicitly.',
      ),
      "## Contract\n",
      tsp("discriminated-union"),
      "",
      tsp("shapes"),
      "\n## Generated\n",
      cs(declaration(models, "Shape"), declaration(models, "Circle"), declaration(models, "Square")),
      "",
      cs(
        declaration(api, "ShapesAreaRequest"),
        declaration(api, "ShapesAreaResponse"),
        declaration(api, "ShapesGetResponse"),
      ),
      "\n## Hand-written handlers\n\n`ShapeArea` receives the union and switches on its variants; `GetShape` answers one, a variant converting implicitly to `Ok`.\n",
      cs(region(handlers, "shapes")),
    ].join("\n"),

    "enums-and-parameters.md": [
      header(
        "Enums and parameters",
        "String enums serialize by wire value and bind from the query by wire value (`?color=red`), numeric enums by number. Parameters bind from path, query and header; a `@query(#{ explode: false })` string list binds from one comma-separated value.",
      ),
      "## Contract\n",
      tsp("enums"),
      "",
      tsp("parameters"),
      "\n## Generated\n",
      cs(declaration(models, "Color"), declaration(models, "Priority")),
      "",
      cs(endpoint(api, "WidgetsList"), endpoint(api, "TagsEcho")),
      "",
      cs(declaration(api, "FixtureParameters")),
      "\n## Hand-written handlers\n",
      cs(region(handlers, "list")),
      "",
      cs(region(handlers, "parameters")),
    ].join("\n"),

    "validation-and-errors.md": [
      header(
        "Validation and the error body",
        "Constraints on a request body (`@minLength`, `@maxValue`, …) become an `IValidator<T>` that runs before the handler. Its failures, and request-binding failures, are answered by the app's `IApiErrorWriter`: the runtime never knows the error body. The contract's error model maps onto the app's type with `external-types`.",
      ),
      "## Contract\n",
      tsp("errors"),
      "\n## Generated validators\n",
      cs(declaration(models, "WidgetCreateValidator"), declaration(models, "WidgetWritableValidator")),
      "\n## Hand-written error writer\n",
      cs(region(errors, "writer")),
    ].join("\n"),

    "sse.md": [
      header(
        "Server-sent events",
        "An operation returning `SSEStream<Events>` of an `@events` union gets a response union whose `Ok` carries `IAsyncEnumerable<ServerSentEvent<Events>>`. The handler yields events; the runtime writes each as a frame named by the variant's `type`, with the item's `Id` as the SSE id (`null` = no id, for heartbeats). There is no session state: `Last-Event-ID` is an ordinary header parameter the handler reads, reconnecting is the client's, and the stream ends when the client disconnects or the app stops.",
      ),
      "## Contract\n",
      tsp("sse"),
      "\n## Generated\n",
      cs(
        declaration(models, "WidgetEvent"),
        declaration(models, "WidgetChanged"),
        declaration(api, "WidgetEventsStreamResponse"),
      ),
      "",
      cs(endpoint(api, "WidgetEventsStream")),
      "\n## Hand-written handler\n",
      cs(region(handlers, "sse")),
    ].join("\n"),

    "content.md": [
      header(
        "Files, text, headers and raw bodies",
        "The content type decides how a body is written. `bytes` with a non-JSON content type is a `System.IO.Stream` the runtime copies to the response as it reads it (never buffered) and then disposes; a `text/*` `string` is written as is. A literal content type is a constant; for `bytes`, `@header contentType: string` leaves it to the handler (a `ContentType` member). Each `@header` of a response is a member of its variant, optional ones last and defaulting to null (unset = not sent); a string literal header is a constant. `{+path}` maps to a catch-all route (`a%2Fb`, as generated clients send it, binds as `a/b`; a `.` or `..` segment is a binding failure), and `@head` operations answer headers only. A `bytes` request body with a non-JSON content type (`*/*` for any) is the raw request stream, unread: the handler reads it, with its own size limit, and a signature over the exact bytes still verifies.",
      ),
      "## Contract\n",
      tsp("files"),
      "",
      tsp("text"),
      "",
      tsp("raw-body"),
      "\n## Generated\n",
      cs(
        declaration(api, "FilesGetResponse"),
        declaration(api, "FilesHeadResponse"),
        declaration(api, "NoticesGetResponse"),
        declaration(api, "DeliveriesReceiveRequest"),
      ),
      "",
      cs(endpoint(api, "FilesGet"), endpoint(api, "DeliveriesReceive")),
      "\n## Hand-written handlers\n",
      cs(region(handlers, "files")),
      "",
      cs(region(handlers, "text")),
      "",
      cs(region(handlers, "raw-body")),
    ].join("\n"),

    "wiring.md": [
      header(
        "Wiring",
        "`Add<Prefix>Api` registers the generated JSON context and makes binding failures reach the middleware; the app registers its handlers and one `IApiErrorWriter`; `Use<Prefix>Api` checks every handler is registered (fail fast, never a runtime 500), adds the middleware and maps every operation, returning the route builders for app-side conventions such as authorization.",
      ),
      "## Generated\n",
      cs(declaration(api, "FixtureApi")),
      "\n## Hand-written\n",
      cs(region(app, "wiring")),
    ].join("\n"),
  };
}
