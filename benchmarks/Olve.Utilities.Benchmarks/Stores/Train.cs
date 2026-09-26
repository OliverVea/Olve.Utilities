using Olve.Utilities.Ids;
using Olve.Utilities.Lookup;

namespace Olve.Utilities.Benchmarks.Stores;

/// <summary>An entity carrying its per-frame state, for the "store it on the entity" variant.</summary>
public sealed record Train(Id<Train> Id, float Position, float Speed) : IHasId<Id<Train>>;
