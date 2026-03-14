using System.Runtime.InteropServices;

namespace Olve.TinyEXR.Tests;

public class LinuxOnlyAttribute() : SkipAttribute("This test is only run on Linux")
{
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
    {
        return Task.FromResult(!RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
    }

    protected override string GetSkipReason(TestRegisteredContext context)
    {
        return "This test is only run on Linux";
    }
}
