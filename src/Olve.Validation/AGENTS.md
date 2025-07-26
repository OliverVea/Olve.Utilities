# Guidelines for Olve.Results.Validation

This library provides small validator helpers built on top of **Olve.Results**. Validators return `Result` and can be implicitly converted.

Current validators:
- `StringValidator`
- `NumericValidator<T>` with `IntValidator`, `FloatValidator`, `DoubleValidator` and `DecimalValidator`
- `Validate` static class creates validators from values.
- `ValidatorFor<T>` and `ValidationDescriptor<T>` support strongly typed object validation.

Keep this document updated on API changes. `workflow-triggers.txt` lists files that trigger NuGet publishing when modified.
