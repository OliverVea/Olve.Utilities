// @olivervea/typespec-csharp: emits the C# minimal-API surface of a TypeSpec HTTP contract, on the
// Olve.TypeSpec runtime (NuGet). Names carry a prefix: the `prefix` option, else the service
// namespace's name (`namespace Widgets;` → Widgets). For prefix Widgets:
//
//   WidgetsModels.g.cs       records (request shapes split by visibility), enums, discriminated
//                            unions, SSE event unions (IServerSentEvent, EventTypes), and IValidator<T>s
//                            for request bodies from @maxLength etc.
//   WidgetsApi.g.cs          per operation: a request record, a response union with one variant per
//                            declared status (an SSE variant carries IAsyncEnumerable<ServerSentEvent<TEvent>>)
//                            and I…Handler : IOperationHandler<Req, Response>; WidgetsOperations
//                            (declared statuses), WidgetsApi.HandlerTypes, AddWidgetsApi()/UseWidgetsApi()/
//                            MapWidgetsApi(), and WidgetsParameters (string-enum parameters parsed by wire value)
//   WidgetsJsonContext.g.cs  System.Text.Json source-gen context for every DTO
//
// collect.js walks the TypeSpec program into a small C#-shaped model; render.js formats it. What
// the emitter can't type is reported as an `unsupported-*` diagnostic (see lib.js), never mistyped.

import { emitFile, resolvePath } from "@typespec/compiler";
import { collect } from "./collect.js";
import { renderApi, renderJsonContext, renderModels } from "./render.js";

export { $lib } from "./lib.js";

export async function $onEmit(context) {
  const { program, options } = context;
  const namespace = options.namespace ?? "Generated.Api";
  const ir = collect(program, options);
  const prefix = options.prefix ?? ir.serviceName ?? "Api";
  if (program.compilerOptions.noEmit) return;

  const files = {
    [`${prefix}Models.g.cs`]: renderModels(ir, namespace, prefix),
    [`${prefix}Api.g.cs`]: renderApi(ir, namespace, prefix),
    [`${prefix}JsonContext.g.cs`]: renderJsonContext(ir, namespace, prefix),
  };
  for (const [name, content] of Object.entries(files)) {
    await emitFile(program, { path: resolvePath(context.emitterOutputDir, name), content, newLine: "lf" });
  }
}
