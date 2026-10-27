import { createTypeSpecLibrary, paramMessage } from "@typespec/compiler";

/**
 * Library definition: emitter options (validated by tsp) and the diagnostics this emitter reports.
 *
 * The `unsupported-*` diagnostics are the contract with authors: a shape the emitter can't type is
 * reported and emitted as JsonElement (or the operation is skipped), never silently mistyped. The
 * list of such shapes, and what is supported, is kept in the olve-typespec skill
 * (olve-packages/skills/olve-typespec/SKILL.md); test/fixtures/unsupported.tsp covers every entry.
 */
export const $lib = createTypeSpecLibrary({
  name: "@olivervea/typespec-csharp",
  diagnostics: {
    "unsupported-type": {
      severity: "warning",
      messages: {
        default: paramMessage`Unsupported type shape (${"shape"}); emitted as System.Text.Json.JsonElement.`,
      },
    },
    "unsupported-operation": {
      severity: "warning",
      messages: {
        default: paramMessage`Operation '${"operation"}': ${"reason"}.`,
      },
    },
  },
  emitter: {
    options: {
      type: "object",
      additionalProperties: false,
      properties: {
        namespace: {
          type: "string",
          nullable: true,
          description: "C# namespace of the generated code. Default: Generated.Api.",
        },
        prefix: {
          type: "string",
          nullable: true,
          description:
            "Prefix of the generated file and type names (<prefix>Models.g.cs, <prefix>Api, <prefix>Operations, <prefix>JsonContext, Add<prefix>Api/Use<prefix>Api). Default: the service namespace's name.",
        },
        "external-types": {
          type: "object",
          nullable: true,
          additionalProperties: { type: "string" },
          description:
            "Spec models that map onto existing C# types instead of being generated (spec model name → C# type).",
        },
      },
      required: [],
    },
  },
});

export const { reportDiagnostic } = $lib;
