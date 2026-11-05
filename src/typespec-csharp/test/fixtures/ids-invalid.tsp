// @id targets that aren't uuid-format strings: each is an error (Id<T> is backed by a Guid).
import "@olivervea/typespec-csharp";

using Olve.CSharp;

namespace IdsInvalid;

model Character {}

@id(Character) scalar NotUuid extends string;

@format("uuid")
scalar Uuid extends string;

model Tags {
  @id(Character) count: int32;
  @id(Character) @format("email") email: string;
  @id(Character) many: Uuid[];
}
