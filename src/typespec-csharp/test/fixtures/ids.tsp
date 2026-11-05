// @id beyond the widgets fixture: a tag on a property or parameter, the @@id augment, and an
// untagged uuid staying Guid. test/emitter.test.js asserts the emitted types.
import "@typespec/http";
import "@olivervea/typespec-csharp";

using Http;
using Olve.CSharp;

@service
namespace Ids;

@format("uuid")
scalar Uuid extends string;

scalar CharacterId extends Uuid;

model Character {
  id: CharacterId;
}

model Sighting {
  @id(Character) @format("uuid") seen: string;
  @id(Character, "App.CharacterRecord") witness?: Uuid | null;
  plain: Uuid;
  by: CharacterId[];
}

@route("/sightings/{id}") @get op get(@path @id(Character) id: Uuid, @query by?: CharacterId): Sighting;
