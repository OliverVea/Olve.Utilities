// Polyfill so C# records (which emit `init`-only setters) compile against netstandard2.0, which does
// not ship System.Runtime.CompilerServices.IsExternalInit.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    }
}
