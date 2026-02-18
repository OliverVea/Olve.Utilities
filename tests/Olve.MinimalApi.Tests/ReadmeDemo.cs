using System.Text.Json;
using Olve.Paths;
using Olve.Results;
using Olve.Results.TUnit;

namespace Olve.MinimalApi.Tests;

public class ReadmeDemo
{
    [Test]
    public async Task ResultMappingSuccess()
    {
        var success = Result.Success("hello");
        var httpResult = success.ToHttpResult();

        // assert
        await Assert.That(httpResult).IsNotNull();
    }

    [Test]
    public async Task ResultMappingFailure()
    {
        Result<string> failure = new ResultProblem("not found");
        var httpResult = failure.ToHttpResult();

        // assert
        await Assert.That(httpResult).IsNotNull();
    }

    [Test]
    public async Task ResultMappingEmpty()
    {
        var success = Result.Success();
        var httpResult = success.ToHttpResult();

        // assert
        await Assert.That(httpResult).IsNotNull();
    }

    [Test]
    public async Task PathJsonRoundTrip()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new PathJsonConverter());

        var original = Olve.Paths.Path.Create("folder/sub/file.txt");

        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<IPath>(json, options);

        // assert
        await Assert.That(deserialized).IsNotNull();
        await Assert.That(deserialized!.Path).IsEqualTo(original.Path);
    }
}
