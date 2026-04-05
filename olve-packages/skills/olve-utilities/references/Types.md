# Sentinel Types

API docs: [https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Types.html](https://olivervea.github.io/Olve.Utilities/api/Olve.Utilities.Types.html)

Source: `src/Olve.Utilities/Types/`

Zero-size marker types (`Size = 1` byte) for use with `OneOf<T>` discriminated unions. Each type has a non-generic and a generic variant.

## NotFound / NotFound\<T\>

Indicates a value was not found.

```csharp
public readonly struct NotFound
{
    public override string ToString(); // "NotFound"
}

public readonly struct NotFound<T>
{
    public override string ToString(); // "NotFound<TypeName>"
}
```

## Success / Success\<T\>

Indicates a successful operation.

```csharp
public readonly struct Success
{
    public override string ToString(); // "Success"
}

public readonly struct Success<T>
{
    public override string ToString(); // "Success<TypeName>"
}
```

## AlreadyExists / AlreadyExists\<T\>

Indicates something already exists.

```csharp
public readonly struct AlreadyExists
{
    public override string ToString(); // "AlreadyExists"
}

public readonly struct AlreadyExists<T>
{
    public override string ToString(); // "AlreadyExists<TypeName>"
}
```

## Skipped / Skipped\<T\>

Represents a skipped operation.

```csharp
public readonly struct Skipped
{
    public override string ToString(); // "Skipped"
}

public readonly struct Skipped<T>
{
    public override string ToString(); // "Skipped<TypeName>"
}
```

## Waiting / Waiting\<T\>

Represents a waiting state.

```csharp
public readonly struct Waiting
{
    public override string ToString(); // "Waiting"
}

public readonly struct Waiting<T>
{
    public override string ToString(); // "Waiting<TypeName>"
}
```

## Any / Any\<T\>

Represents anything / a wildcard.

```csharp
public readonly struct Any
{
    public override string ToString(); // "Any"
}

public readonly struct Any<T>
{
    public override string ToString(); // "Any<TypeName>"
}
```

## Yes / Yes\<T\>

Represents a type that is always true. The generic variant holds a value.

```csharp
public readonly struct Yes
{
    public override string ToString(); // "Yes"
}

public readonly record struct Yes<T>(T Value)
{
    public override string ToString(); // "Yes<TypeName>(Value)"
}
```

## Usage with OneOf

```csharp
// Return type expressing "found or not found"
OneOf<User, NotFound> GetUser(Id<User> id);

// Return type expressing "created, already existed, or error"
OneOf<Success, AlreadyExists, ResultProblem> CreateUser(User user);
```
