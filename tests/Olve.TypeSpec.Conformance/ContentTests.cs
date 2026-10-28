using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Olve.TypeSpec.Conformance;

/// <summary>
/// Bodies that aren't JSON: a streamed binary response with its declared headers, a catch-all
/// path, a headers-only HEAD, plain text, and a raw request body read exactly as sent.
/// </summary>
[ClassDataSource<FixtureApp>(Shared = SharedType.PerAssembly)]
public class ContentTests(FixtureApp fixture)
{
    [Test]
    [Arguments("/api/files/docs/a/readme.txt", "docs/a/readme.txt")]
    [Arguments("/api/files/docs%2Fa%2Freadme.txt", "docs/a/readme.txt")]
    [Arguments("/api/files/one%20file.txt", "one file.txt")]
    [Arguments("/api/files/r%C3%A9sum%C3%A9%20%22v2%22.txt", "résumé \"v2\".txt")]
    public async Task FileGet_StreamsTheBodyWithItsHeaders(string url, string path)
    {
        using var response = await fixture.CreateClient().GetAsync(url);
        var expected = FixtureHandlers.FileContent(path);

        await Assert.That(Convert.ToHexString(await response.Content.ReadAsByteArrayAsync())).IsEqualTo(Convert.ToHexString(expected));
        await Assert.That(response.Content.Headers.ContentType?.MediaType).IsEqualTo(FixtureHandlers.FileContentType);
        await Assert.That(response.Content.Headers.ContentLength).IsEqualTo(expected.Length);
        await Assert.That(response.Content.Headers.ContentDisposition?.FileNameStar).IsEqualTo(Path.GetFileName(path));
        await Assert.That(response.Headers.GetValues("X-Revision").Single()).IsEqualTo("3");
    }

    [Test]
    [Arguments("a%2Fb", "a/b")]
    [Arguments("a%2fb/c", "a/b/c")]
    [Arguments("..x/y.", "..x/y.")]
    public async Task CatchAll_ReadsEncodedSlashes(string value, string expected) =>
        await Assert.That(PathParameters.CatchAll(value)).IsEqualTo(expected);

    [Test]
    [Arguments("..%2Fx")]
    [Arguments("a%2F.%2Fb")]
    [Arguments("a/..")]
    public async Task CatchAll_RefusesDotSegments(string value) =>
        await Assert.That(() => PathParameters.CatchAll(value)).Throws<BadHttpRequestException>();

    [Test]
    public async Task FileHead_SendsHeadersWithoutABody()
    {
        using var response = await fixture.CreateClient().SendAsync(new HttpRequestMessage(HttpMethod.Head, "/api/files/docs/readme.txt"));

        await Assert.That((int)response.StatusCode).IsEqualTo(200);
        await Assert.That(response.Content.Headers.ContentType?.MediaType).IsEqualTo("application/octet-stream");
        await Assert.That(response.Content.Headers.ContentLength).IsEqualTo(FixtureHandlers.FileContent("docs/readme.txt").Length);
        await Assert.That(await response.Content.ReadAsByteArrayAsync()).IsEmpty();
    }

    [Test]
    public async Task Text_IsWrittenAsIs()
    {
        using var ok = await fixture.CreateClient().GetAsync("/api/notice");
        using var busy = await fixture.CreateClient().GetAsync("/api/notice?busy=true");

        await Assert.That(await ok.Content.ReadAsStringAsync()).IsEqualTo("hello \"world\"\n");
        await Assert.That(ok.Content.Headers.ContentType?.ToString()).IsEqualTo("text/plain; charset=utf-8");
        await Assert.That((int)busy.StatusCode).IsEqualTo(503);
        await Assert.That(busy.Headers.GetValues("Retry-After").Single()).IsEqualTo("30");
    }

    [Test]
    [Arguments("{ \"a\" :1 }\n", "application/json")]
    [Arguments("not json at all", "text/plain")]
    [Arguments("<x/>", null)]
    public async Task RawBody_IsReadExactlyAsSent(string body, string? contentType)
    {
        var bytes = Encoding.UTF8.GetBytes(body);
        var content = new ByteArrayContent(bytes);
        if (contentType is not null)
        {
            content.Headers.ContentType = new(contentType);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/deliveries") { Content = content };
        request.Headers.Add("X-Signature", "sha256=abc");
        using var response = await fixture.CreateClient().SendAsync(request);
        var digest = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;

        await Assert.That((int)response.StatusCode).IsEqualTo(200);
        await Assert.That(digest.GetProperty("sha256").GetString()).IsEqualTo(Convert.ToHexStringLower(SHA256.HashData(bytes)));
        await Assert.That(digest.GetProperty("length").GetInt64()).IsEqualTo(bytes.Length);
        await Assert.That(digest.GetProperty("signature").GetString()).IsEqualTo("sha256=abc");
    }
}
