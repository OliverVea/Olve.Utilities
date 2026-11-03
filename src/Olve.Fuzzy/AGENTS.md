# Guidelines for Olve.Fuzzy

Fuzzy logic with a fluent rule syntax, ported from [FluentFuzzy](https://github.com/OliverVea/FluentFuzzy) (MIT, Oliver Vea).

## Model

- **`FuzzyInput`** reads its crisp value through a `Func<double>` on every evaluation and holds `IMemberFunction`s keyed by `int` antecedent. `Is`/`IsNot` create `ICondition`s.
- **`FuzzyOutput`** holds `IHasCentroid` member functions keyed by `int` consequent. Rules attach conditions to its consequents, so the output owns the rule set. `Evaluate()` takes, per consequent, the maximum truth of its conditions, clips the shape there and returns the weighted centroid of all clipped shapes (0 when nothing is active).
- **`FuzzyRule.If`** starts the builder: `IRuleBuilder` (`And` = min, `Or` = max, `Then`) and `IRuleApplier` (`And` adds a consequent, `Else` = 1 - truth, `ElseIf`).
- `ICondition` and `IConsequent` have internal members only, so they can't be implemented outside the library.

## Member functions (`Olve.Fuzzy.MemberFunctions`)

- `Triangle`, `Trapezoid`: input and output (`IHasCentroid`). Non-finite input gives `NaN`. A triangle equals a trapezoid with both centre points equal; a test holds them to that.
- `Line`, `Value`, `AtLeast`: input only.
- Ramp centroids use the ramp's midpoint, not its true centroid (inherited approximation, documented as "a version of the centroid method").

## Maintenance

- Inputs, outputs and rules are not thread-safe; a rule set is built per evaluation context.
- When changing behaviour, update this document and the README (snippets come from `tests/Olve.Fuzzy.Tests/ReadmeDemo.cs` via embedme).
