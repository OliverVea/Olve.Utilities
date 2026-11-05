import "../src/index.js";

using TypeSpec.Reflection;

/** Decorators of @olivervea/typespec-csharp: hints the C# emitter reads and other emitters ignore. */
namespace Olve.CSharp;

/**
 * Emits ids of `of` as Olve.Utilities' typed `Id<T>` instead of `Guid`. The wire is unchanged (a
 * plain uuid string), so OpenAPI and other clients still see `format: uuid`.
 *
 * Tag a uuid-format string scalar once and every property and parameter of that scalar becomes
 * `Id<T>`; tag a single property (or parameter) for a one-off.
 *
 * @param of The model the id identifies; `T` is that model's generated C# type.
 * @param csharp A C# type to use as `T` instead, such as the app's own record type.
 *
 * @example
 * ```typespec
 * @format("uuid") scalar Uuid extends string;
 *
 * @id(Character) scalar CharacterId extends Uuid;                                // Id<Character>
 * @id(Place, "Olve.AiRpg.Persistence.PlaceRecord") scalar PlaceId extends Uuid;  // Id<Olve.AiRpg.Persistence.PlaceRecord>
 * ```
 */
extern dec id(target: Scalar | ModelProperty, of: Model, csharp?: valueof string);
