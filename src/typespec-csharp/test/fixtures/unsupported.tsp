// Shapes the emitter can't type must be reported, not silently mistyped. One operation per shape;
// test/emitter.test.js asserts the diagnostic each one produces. The list mirrors the "Supported /
// reported" section of the olve-typespec skill (olve-packages/skills/olve-typespec/SKILL.md).
import "@typespec/http";
import "@typespec/streams";
import "@typespec/events";
import "@typespec/sse";

using Http;
using SSE;

@service
namespace Unsupported;

// An untagged union: nothing says which branch a value is.
model A {
  x: string | int32;
}

@get op get(): A;

// An array parameter that isn't an `explode: false` string list in the query.
@route("/tags") @get op tags(@query(#{ explode: true }) tag?: string[]): A;

@route("/ids") @get op ids(@query(#{ explode: false }) id?: int32[]): A;

// An SSE event whose `type` disagrees with its event name.
model Tick {
  type: "tock";
}

@TypeSpec.Events.events
union Ticks {
  @TypeSpec.Events.contentType("application/json") tick: Tick,
}

@route("/ticks") @get op ticks(): SSEStream<Ticks>;

// A discriminated union with an envelope (the default), rather than `envelope: "none"`.
model Cat {
  meows: boolean;
}

model Dog {
  barks: boolean;
}

@discriminated
union Pet {
  cat: Cat,
  dog: Dog,
}

@route("/pets") @get op pet(): Pet;

// A cookie parameter.
@route("/session") @get op session(@cookie sid: string): A;

// A multipart body.
model Upload {
  @multipartBody body: {
    file: HttpPart<bytes>;
  };
}

@route("/upload") @post op upload(...Upload): A;

// A status code range.
@error
model ClientError {
  @minValue(400) @maxValue(499) @statusCode status: int32;
  message: string;
}

@route("/range") @get op range(): A | ClientError;

// A text body in the request (only JSON, or bytes read raw).
@route("/note") @post op note(@header contentType: "text/plain", @body text: string): A;

// A string response that is neither JSON nor text/*.
@route("/xml") @get op xml(): {
  @header contentType: "application/xml";
  @body body: string;
};

// A response header that is not a string, an integer or a string literal.
@route("/stamped") @get op stamped(): {
  @header("Last-Modified") lastModified: utcDateTime;
  @body body: A;
};
