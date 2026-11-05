// The decorators of lib/main.tsp (namespace Olve.CSharp) and their validation.

import { getFormat, isNullType } from "@typespec/compiler";
import { $lib, reportDiagnostic } from "./lib.js";

/** `@id(of, csharp?)`: stores the tag; `$onValidate` checks it once every decorator has run. */
function $id(context, target, of, csharp) {
  context.program.stateMap($lib.stateKeys.id).set(target, { of, csharp });
}

export const $decorators = { "Olve.CSharp": { id: $id } };

/**
 * The `@id` tag that applies to a property or parameter typed `type`: the property's own first,
 * then the nearest one along the scalar chain. Undefined when untagged.
 */
export function getIdTag(program, type, prop) {
  const tags = program.stateMap($lib.stateKeys.id);
  if (prop && tags.has(prop)) return tags.get(prop);
  for (let s = type; s?.kind === "Scalar"; s = s.baseScalar) if (tags.has(s)) return tags.get(s);
  return undefined;
}

/** Whether `type` is a string scalar in uuid format (`prop`'s own `@format` counts too). */
export function isUuid(program, type, prop) {
  if (type?.kind !== "Scalar") return false;
  let format = prop && getFormat(program, prop);
  for (let s = type; s; s = s.baseScalar) {
    format ??= getFormat(program, s);
    if (s.name === "string" && s.namespace?.name === "TypeSpec") return format === "uuid";
  }
  return false;
}

/** `@id` only fits a uuid-format string: `Id<T>` is backed by a `Guid`. */
export function $onValidate(program) {
  for (const target of program.stateMap($lib.stateKeys.id).keys()) {
    const ok = target.kind === "Scalar" ? isUuid(program, target) : isUuid(program, nonNull(target.type), target);
    if (!ok) reportDiagnostic(program, { code: "id-not-uuid", target });
  }
}

/** `T | null` → T; anything else as is. */
function nonNull(type) {
  if (type.kind !== "Union") return type;
  const variants = [...type.variants.values()].map((v) => v.type).filter((t) => !isNullType(t));
  return variants.length === 1 ? variants[0] : type;
}
