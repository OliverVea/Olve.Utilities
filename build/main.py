import asyncio
import os
import sys
import dagger

async def main() -> None:
    version = os.environ.get("DOTNET_VERSION", "9.0.300")

    async with dagger.Connection(dagger.Config(log_output=sys.stderr)) as client:
        src = (
            client.container()
            .from_(f"mcr.microsoft.com/dotnet/sdk:{version}")
            .with_mounted_directory("/src", client.host().directory("."))
            .with_workdir("/src")
        )

        restored = src.with_exec(["dotnet", "restore"])
        restored = restored.with_exec(["dotnet", "tool", "restore"])
        built = restored.with_exec(["dotnet", "build", "--no-restore", "-c", "Release"])
        tested = built.with_exec(["dotnet", "test", "--no-build", "--no-restore", "-c", "Release"])
        packed = tested.with_exec(["dotnet", "pack", "--no-build", "-c", "Release"])
        docs = packed.with_exec(["dotnet", "docfx", "docs/docfx.json"])

        await docs.exit_code()

if __name__ == "__main__":
    asyncio.run(main())
