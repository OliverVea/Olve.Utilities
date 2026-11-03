# Olve.Fuzzy

[![NuGet](https://img.shields.io/nuget/v/Olve.Fuzzy?logo=nuget)](https://www.nuget.org/packages/Olve.Fuzzy)
[![Docs](https://img.shields.io/badge/docs-API%20Reference-blue)](https://olivervea.github.io/Olve.Utilities/api/Olve.Fuzzy.html)

Fuzzy logic with a fluent rule syntax: inputs with member functions, rules that read like sentences, and centroid defuzzification of the outputs.

Olve.Fuzzy is the core of [FluentFuzzy](https://github.com/OliverVea/FluentFuzzy) (MIT, Oliver Vea), moved into Olve.Utilities.

---

## Installation

```bash
dotnet add package Olve.Fuzzy
```

---

## Overview

| Type | Description |
| --- | --- |
| `FuzzyInput` | An input variable: a crisp value read through a `Func<double>`, and member functions keyed by `int`. |
| `FuzzyOutput` | An output variable: member functions keyed by `int`, and the rules attached to them. `Evaluate()` defuzzifies. |
| `FuzzyRule` | Entry point of the fluent rule syntax: `If(...).And(...).Or(...).Then(...).And(...).Else(...).ElseIf(...)`. |
| `Triangle`, `Trapezoid` | Member functions usable for inputs and outputs. |
| `Line`, `Value`, `AtLeast` | Member functions for inputs only (they have no useful centroid). |
| `IMemberFunction` | Implement for a custom input member function. |
| `IHasCentroid` | Implement for a custom output member function. |

---

## Usage

### Inputs and outputs

A `FuzzyInput` takes a `Func<double>` that returns its current crisp value. The function is called on every evaluation.

```cs
// ../../tests/Olve.Fuzzy.Tests/ReadmeDemo.cs#L10-L12

var healthValue = 35.0;
var health = new FuzzyInput(() => healthValue);
var flee = new FuzzyOutput();
```

### Member functions

Each input gets a member function per antecedent, keyed by an `int`:

```cs
// ../../tests/Olve.Fuzzy.Tests/ReadmeDemo.cs#L14-L20

const int low = 0;
const int medium = 1;
const int high = 2;

health.Set(low, new Trapezoid(0, 0, 25, 50));
health.Set(medium, new Triangle(25, 50, 75));
health.Set(high, new Trapezoid(50, 75, 100, 100));
```

Outputs take member functions that implement `IHasCentroid`, as defuzzification uses a version of the centroid method. `Line`, `Value` and `AtLeast` have no useful centroid, so they are input-only.

```cs
// ../../tests/Olve.Fuzzy.Tests/ReadmeDemo.cs#L22-L24

flee.Set(low, new Triangle(-0.5, 0, 0.5));
flee.Set(medium, new Triangle(0, 0.5, 1));
flee.Set(high, new Triangle(0.5, 1, 1.5));
```

### Rules

Rules connect input conditions to output consequents:

```cs
// ../../tests/Olve.Fuzzy.Tests/ReadmeDemo.cs#L26-L28

FuzzyRule.If(health.Is(high)).Then(flee.Is(low));
FuzzyRule.If(health.Is(medium)).Then(flee.Is(medium));
FuzzyRule.If(health.Is(low)).Then(flee.Is(high));
```

`And` takes the minimum, `Or` the maximum, and `IsNot`/`Else` the negation (1 - truth).

### Evaluating

```cs
// ../../tests/Olve.Fuzzy.Tests/ReadmeDemo.cs#L30-L30

var result = flee.Evaluate(); // ≈ 0.78
```

---

## Rule sets and threads

Inputs read their values through a `Func<double>`, and rules attach to the outputs. An output therefore holds its rule set, and every rule you build adds to it. Build the inputs, outputs and rules per evaluation context (e.g. per roll, per actor), and don't share them across threads.

---

## Documentation

Full API reference:
[https://olivervea.github.io/Olve.Utilities/api/Olve.Fuzzy.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Fuzzy.html)

---

## License

MIT License © [OliverVea](https://github.com/OliverVea)
