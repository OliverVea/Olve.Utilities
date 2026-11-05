// The C#-only half of ids.tsp: an augment keeps the main spec free of emitter hints.
import "./ids.tsp";
import "@olivervea/typespec-csharp";

using Olve.CSharp;

@@id(Ids.CharacterId, Ids.Character);
