namespace Olve.TypeSpec;

/// <summary>
/// Marks a class as the handler of the operations whose generated handler interfaces it implements.
/// The generated <c>services.AddOperationHandlers()</c> registers exactly the marked classes, transient;
/// a class that only implements the interface (a decorator, a test double) is not registered.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class OperationHandlerAttribute : Attribute;
