using Microsoft.AspNetCore.Http;

namespace Olve.TypeSpec;

/// <summary>Route-value helpers the generated binding uses.</summary>
public static class PathParameters
{
    /// <summary>
    /// A <c>{+path}</c> (catch-all) value: the server decodes every escape in a path except <c>%2F</c>,
    /// and OpenAPI can't express reserved expansion, so generated clients send <c>a%2Fb</c> where a
    /// browser link sends <c>a/b</c>. Both mean the same path. The server removes <c>.</c> and
    /// <c>..</c> segments before routing but not ones hidden behind <c>%2F</c>, so a value that
    /// decodes to one is a binding failure rather than a path that climbs out of its prefix.
    /// </summary>
    public static string CatchAll(string value)
    {
        var path = value.Replace("%2F", "/", StringComparison.OrdinalIgnoreCase);
        foreach (var segment in path.Split('/'))
        {
            if (segment is "." or "..")
            {
                throw new BadHttpRequestException($"The path \"{path}\" has a '{segment}' segment.");
            }
        }

        return path;
    }
}
