using System.Diagnostics;

namespace Olve.Utilities.Assertions;

public static class Assert
{
    [Conditional("DEBUG")]
    public static void That(Func<bool> assertion, string message)
    {
#if DEBUG
        if (!assertion())
        {
            throw new AssertionError(message);
        }
#endif
    }
}