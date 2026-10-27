// @olivervea/typespec-csharp: emits the C# minimal-API surface of a TypeSpec HTTP contract, on the
// Olve.TypeSpec runtime (NuGet). With the default prefix "Api":
//
//   ApiModels.g.cs       records (request shapes split by visibility), enums, discriminated
//                        unions, SSE event unions (IServerSentEvent, EventTypes), and IValidator<T>s
//                        for request bodies from @maxLength etc.
//   ApiApi.g.cs          per operation: a request record, a response union with one variant per
//                        declared status (an SSE variant carries IAsyncEnumerable<ServerSentEvent<TEvent>>)
//                        and I…Handler : IOperationHandler<Req, Response>; ApiOperations (declared
//                        statuses), ApiApi.HandlerTypes, AddApiApi()/UseApiApi()/MapApiApi(), and
//                        ApiParameters (string-enum parameters parsed by wire value)
//   ApiJsonContext.g.cs  System.Text.Json source-gen context for every DTO
//
// collect.js walks the TypeSpec program into a small C#-shaped model; render.js formats it.

import { emitFile, resolvePath } from "@typespec/compiler";
import { collect } from "./collect.js";
import { renderApi, renderJsonContext, renderModels } from "./render.js";

export { $lib } from "./lib.js";

export async function $onEmit(context) {
  const { program, options } = context;
  const namespace = options.namespace ?? "Generated.Api";
  const prefix = options.prefix ?? "Api";
  const ir = collect(program, options);
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
