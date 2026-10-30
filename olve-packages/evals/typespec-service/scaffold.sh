#!/usr/bin/env bash
set -euo pipefail
mkdir -p spec src

cat > spec/main.tsp <<'TSP'
import "@typespec/http";

using Http;

@service(#{ title: "Gadgets" })
@route("/api")
namespace Gadgets;

model Gadget {
  @visibility(Lifecycle.Read) id: string;
  @minLength(1) @maxLength(40) name: string;
  locked: boolean;
}

model RenameBody {
  @minLength(1) @maxLength(40) name: string;
}

model AppError {
  code: string;
  message: string;
}

@error
model BadRequest {
  @statusCode status: 400;
  @body error: AppError;
}

@error
model NotFound {
  @statusCode status: 404;
  @body error: AppError;
}

@error
model Conflict {
  @statusCode status: 409;
  @body error: AppError;
}

@route("/gadgets")
interface Gadgets {
  @route("{id}/name")
  @put rename(@path id: string, @body body: RenameBody): Gadget | BadRequest | NotFound | Conflict;
}
TSP

cat > spec/tspconfig.yaml <<'YAML'
output-dir: "{project-root}/../artifacts"
emit:
  - "@olivervea/typespec-csharp"
options:
  "@olivervea/typespec-csharp":
    emitter-output-dir: "{output-dir}/generated"
    namespace: "Gadgets.Api"
    external-types:
      AppError: "Gadgets.Api.AppError"
YAML

cat > src/AppError.cs <<'CS'
using System.Text.Json.Serialization;

namespace Gadgets.Api;

public sealed record AppError(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message);

[JsonSerializable(typeof(AppError))]
internal sealed partial class AppErrorJsonContext : JsonSerializerContext;
CS

cat > src/GadgetStore.cs <<'CS'
using System.Collections.Concurrent;

namespace Gadgets.Api;

public sealed class GadgetStore
{
    private readonly ConcurrentDictionary<string, Gadget> _gadgets = new();

    public Gadget? Find(string id) => _gadgets.GetValueOrDefault(id);

    public void Save(Gadget gadget) => _gadgets[gadget.Id] = gadget;
}
CS
