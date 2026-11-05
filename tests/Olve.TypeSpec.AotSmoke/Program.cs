using System.Text;
using Olve.TypeSpec;
using Olve.TypeSpec.Conformance;

var builder = WebApplication.CreateSlimBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Logging.ClearProviders();
builder.Services.AddFixtureApi().AddOperationHandlers();
builder.Services.AddSingleton<IApiErrorWriter, FixtureErrorWriter>();

await using var app = builder.Build();
app.UseFixtureApi();
await app.StartAsync();

// After start, the bound address (port 0 picks a free one).
var address = app.Urls.First();
using var client = new HttpClient { BaseAddress = new Uri(address) };

const string widget = """{"serial":"SN-1","name":"w","priority":2,"description":"d","createdAt":"2026-01-01T00:00:00Z","shape":{"kind":"square","side":3}}""";
const string IdA = "0b5c1d36-0f5e-4a8e-9b1a-6f3a2c1d0e01";
const string IdB = "0b5c1d36-0f5e-4a8e-9b1a-6f3a2c1d0e02";
const string link = $$"""{"from":"{{IdA}}","to":["{{IdA}}"],"stored":"{{IdB}}","label":"l"}""";
var tooLong = widget.Replace("\"name\":\"w\"", $"\"name\":\"{new string('x', 51)}\"");

// Request and response bodies, a union in both, the validator and error writer, an event stream, and typed ids.
var checks = new (string Name, Func<Task<HttpResponseMessage>> Send, int Status, string Contains)[]
{
    ("create a widget", () => client.PostAsync("/api/widgets", Json(widget)), 201, "\"kind\":\"square\""),
    ("validator failure", () => client.PostAsync("/api/widgets", Json(tooLong)), 400, FixtureErrors.InvalidRequest),
    ("get a shape", () => client.GetAsync("/api/shapes/c-1"), 200, "\"radius\":1.5"),
    ("stream events", () => client.GetAsync("/api/widget-events"), 200, "event: widget.changed"),
    ("typed ids", () => client.PostAsync($"/api/widget-links/{IdA}?after={IdB}", Json(link)), 200, $"\"to\":[\"{IdB}\",\"{IdA}\"]"),
};

var failures = 0;
foreach (var (name, send, status, contains) in checks)
{
    using var response = await send();
    var body = await response.Content.ReadAsStringAsync();
    var ok = (int)response.StatusCode == status && body.Contains(contains, StringComparison.Ordinal);
    Console.WriteLine($"{(ok ? "ok  " : "FAIL")} {name}: {(int)response.StatusCode} {body}");
    failures += ok ? 0 : 1;
}

await app.StopAsync();
return failures == 0 ? 0 : 1;

static StringContent Json(string body) => new(body, Encoding.UTF8, "application/json");
