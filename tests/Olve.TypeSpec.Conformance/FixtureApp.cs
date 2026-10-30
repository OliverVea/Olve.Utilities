using Microsoft.AspNetCore.TestHost;
using TUnit.Core.Interfaces;

namespace Olve.TypeSpec.Conformance;

/// <summary>
/// The widgets fixture's generated API, wired exactly as a service wires its own
/// (<c>AddFixtureApi</c> + <c>AddOperationHandlers</c> + <c>UseFixtureApi</c>) and hosted on a
/// <see cref="TestServer"/>. Shared by the tests; the handlers are stateless.
/// </summary>
public sealed class FixtureApp : IAsyncInitializer, IAsyncDisposable
{
    private WebApplication? _app;

    public WebApplication App => _app ?? throw new InvalidOperationException("Not initialized.");

    public IServiceProvider Services => App.Services;

    public HttpClient CreateClient() => App.GetTestClient();

    public async Task InitializeAsync()
    {
        _app = Build();
        await _app.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }

    #region wiring
    /// <summary>Builds (but doesn't start) the fixture app; <paramref name="adjust"/> changes its services first.</summary>
    public static WebApplication Build(Action<IServiceCollection>? adjust = null)
    {
        var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions { EnvironmentName = "Testing" });
        builder.WebHost.UseTestServer();
        builder.Services.AddFixtureApi().AddOperationHandlers();
        builder.Services.AddSingleton<IApiErrorWriter, FixtureErrorWriter>();
        adjust?.Invoke(builder.Services);

        var app = builder.Build();
        app.UseFixtureApi();
        return app;
    }
    #endregion
}
