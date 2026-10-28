// Snapshot fixture: exercises every shape the emitter supports (see ../emitter.test.js).
import "@typespec/http";
import "@typespec/streams";
import "@typespec/events";
import "@typespec/sse";

using Http;
using SSE;

@service(#{ title: "Fixture" })
@route("/api")
namespace Fixture;

// #region enums
enum Color {
  Red: "red",
  /** The default. */
  Green: "green",
}

enum Priority {
  Low: 1,
  High: 2,
}
// #endregion

// #region discriminated-union
model Circle {
  kind: "circle";
  radius: float64;
}

model Square {
  kind: "square";
  side: float64;
}

/** A shape, discriminated on `kind`. */
@discriminated(#{ envelope: "none", discriminatorPropertyName: "kind" })
union Shape {
  circle: Circle,
  square: Square,
}
// #endregion

// #region visibility
/** A widget. */
model Widget {
  @visibility(Lifecycle.Read) @format("uuid") id: string;

  /** Set once, at creation. */
  @visibility(Lifecycle.Create, Lifecycle.Read) serial: string;

  /** Only sent when updating. */
  @visibility(Lifecycle.Update) note?: string;

  @minLength(1) @maxLength(50) name: string;
  color?: Color;
  priority: Priority;
  description: string | null;
  parent?: string | null;
  @maxItems(10) tags?: string[];
  labels?: Record<string>;
  @minValue(0) @maxValue(100) weight?: int32;
  createdAt: utcDateTime;
  shape: Shape;
}
// #endregion

// #region errors
/** The error body (maps onto the app's own error type via external-types). */
model ErrorEnvelope {
  error: ErrorBody;
}

model ErrorBody {
  code: string;
  message: string;
  details: ErrorDetails;
}

model ErrorDetails {
  problems?: ErrorProblem[];
}

model ErrorProblem {
  code: string;
  message: string;
}

@error
model BadRequest {
  @statusCode status: 400;
  @body error: ErrorEnvelope;
}

@error
model NotFound {
  @statusCode status: 404;
  @body error: ErrorEnvelope;
}

@error
model Conflict {
  @statusCode status: 409;
  @body error: ErrorEnvelope;
}
// #endregion

// #region operations
@route("/widgets")
interface Widgets {
  /** List widgets. */
  @get list(
    @query color?: Color,
    @query limit?: int32,
    @header("x-request-id") requestId?: string,
  ): Widget[];

  /** Creates a widget now (201), or accepts it for later (202). */
  @post create(@body body: Widget):
    | (CreatedResponse & { @body body: Widget })
    | (AcceptedResponse & { @body body: Widget })
    | BadRequest;

  @put update(@path id: string, @query dryRun?: boolean, @body body: Widget):
    | Widget
    | BadRequest
    | NotFound
    | Conflict;

  @delete delete(@path id: string): NoContentResponse | NotFound;
}
// #endregion

// #region shapes
/** The area of a shape. */
model Area {
  value: float64;
}

@route("/shapes")
interface Shapes {
  @get get(@path id: string): Shape | NotFound;

  /** Takes a shape, answers its area: the handler switches on the variant. */
  @route("area") @post area(@body shape: Shape): Area;
}
// #endregion

// #region parameters
@route("/tags")
interface Tags {
  /** Echoes a required `explode: false` list (`?names=a,b`). */
  @get echo(@query(#{ explode: false }) names: string[]): string[];
}
// #endregion

// --- server-sent events ----------------------------------------------------------------------

// #region sse
/** Keeps the connection alive. */
model Ping {
  type: "ping";
  at: utcDateTime;
}

// Carries the nested Shape union rather than a Widget: @typespec/openapi3 0.86 fails with
// duplicate-type-name when an event nests a model with properties invisible on read (Widget's
// update-only `note`).
model WidgetChanged {
  type: "widget.changed";
  at: utcDateTime;
  @format("uuid") widgetId: string;
  name: string;
  shape: Shape;
}

model WidgetRemoved {
  type: "widget.removed";
  at: utcDateTime;
  @format("uuid") widgetId: string;
}

/** The widget event stream. */
@TypeSpec.Events.events
union WidgetEvent {
  @TypeSpec.Events.contentType("application/json") ping: Ping,
  @TypeSpec.Events.contentType("application/json") `widget.changed`: WidgetChanged,
  @TypeSpec.Events.contentType("application/json") `widget.removed`: WidgetRemoved,
}

@route("/widget-events")
interface WidgetEvents {
  /** Streams widget events; `type` limits them to these event names. */
  @get stream(
    @query(#{ explode: false }) type?: string[],
    @header("Last-Event-ID") lastEventId?: string,
  ): SSEStream<WidgetEvent> | BadRequest;
}
// #endregion

// --- files, text and raw bodies --------------------------------------------------------------

// #region files
/** A stored file, streamed as it is read. */
model FileContent {
  @header contentType: string;
  @header contentLength?: int64;
  @header contentDisposition?: string;
  @header("X-Revision") revision: int32;
  @body body: bytes;
}

/** A stored file's type and size, without its content. */
model FileHead {
  @header contentType: "application/octet-stream";
  @header contentLength: int64;
}

model NoFile {
  @statusCode status: 404;
}

@route("/files")
interface Files {
  // OpenAPI has no reserved expansion: clients send `a%2Fb`, which the generated binding accepts as `a/b`.
  #suppress "@typespec/openapi3/path-reserved-expansion" "Served as a catch-all"
  /** Streams a file; its path may span segments (`{+path}`). */
  @get @route("{+path}") get(@path path: string): FileContent | NotFound;

  #suppress "@typespec/openapi3/path-reserved-expansion" "Served as a catch-all"
  /** A file's headers, without its content. */
  @head @route("{+path}") head(@path path: string): FileHead | NoFile;
}
// #endregion

// #region text
model Notice {
  @header contentType: "text/plain";
  @body text: string;
}

model NoticeUnavailable {
  @statusCode status: 503;
  @header("Retry-After") retryAfter: int32;
  @header contentType: "text/plain";
  @body text: string;
}

@route("/notice")
interface Notices {
  /** Plain text, written as is. */
  @get get(@query busy?: boolean): Notice | NoticeUnavailable;
}
// #endregion

// #region raw-body
/** What the server received. */
model Digest {
  sha256: string;
  length: int64;
  signature?: string;
}

model TooLarge {
  @statusCode status: 413;
}

@route("/deliveries")
interface Deliveries {
  /** Takes the body exactly as sent, whatever its content type (signatures cover the raw bytes). */
  @post receive(
    @header contentType: "*/*",
    @header("X-Signature") signature?: string,
    @body body: bytes,
  ): Digest | TooLarge;
}
// #endregion
