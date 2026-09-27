using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Olve.Results;

namespace Olve.MinimalApi.Tests;

public class ResultMappingTests
{
    [Test]
    public async Task ToHttpResult_PlainProblem_Returns400()
    {
        Result result = new ResultProblem("Name is required");

        var http = result.ToHttpResult();

        await Assert.That(http).IsTypeOf<BadRequest<ResultProblem[]>>();
    }

    [Test]
    public async Task ToHttpResult_NotFoundProblem_Returns404()
    {
        Result result = new NotFoundProblem("User {0} not found", 42);

        var http = result.ToHttpResult();

        await Assert.That(http).IsTypeOf<NotFound<ResultProblem[]>>();
    }

    [Test]
    public async Task ToHttpResultOfT_NotFoundProblem_Returns404WithAllProblems()
    {
        Result<string> result = new NotFoundProblem("User {0} not found", 42);

        var http = result.ToHttpResult();

        await Assert.That(http).IsTypeOf<NotFound<ResultProblem[]>>();
        await Assert.That(((NotFound<ResultProblem[]>)http).Value!.Length).IsEqualTo(1);
    }

    [Test]
    public async Task ToHttpResult_NotFoundWithPrependedContext_Returns404()
    {
        Result notFound = new NotFoundProblem("User {0} not found", 42);
        Result result = notFound.Problems!.Prepend("Could not rename user");

        var http = result.ToHttpResult();

        await Assert.That(http).IsTypeOf<NotFound<ResultProblem[]>>();
        await Assert.That(((NotFound<ResultProblem[]>)http).Value!.Length).IsEqualTo(2);
    }

    [Test]
    public async Task WithResultMapping_Endpoints_AnswerByProblemType()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        await using var app = builder.Build();
        app.MapGet("/ok", () => Result.Success("hello")).WithResultMapping<string>();
        app.MapGet("/invalid", () => (Result<string>)new ResultProblem("Name is required")).WithResultMapping<string>();
        app.MapDelete("/missing", () => Task.FromResult((Result)new NotFoundProblem("User {0} not found", 42))).WithResultMapping();
        await app.StartAsync();

        var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First();
        using var client = new HttpClient { BaseAddress = new Uri(address) };

        using var ok = await client.GetAsync("/ok");
        using var invalid = await client.GetAsync("/invalid");
        using var missing = await client.DeleteAsync("/missing");

        await Assert.That(ok.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(invalid.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        await Assert.That(missing.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        var body = await missing.Content.ReadFromJsonAsync<ResultProblem[]>();
        await Assert.That(body![0].FormattedMessage).IsEqualTo("User 42 not found");

        await app.StopAsync();
    }
}
