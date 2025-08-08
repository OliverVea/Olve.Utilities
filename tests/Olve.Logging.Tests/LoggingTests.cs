using System;
using System.Linq;
using System.Collections.Generic;
using Olve.Logging;
using Olve.Results;

public class LoggingTests
{
    [Test]
    public async Task LogLevelConversion_Works()
    {
        var ms = LogLevel.Info.ToMicrosoftLogLevel();
        await Assert.That(ms).IsEqualTo(Microsoft.Extensions.Logging.LogLevel.Information);

        var olve = ms.ToOlveLogLevel();
        await Assert.That(olve).IsEqualTo(LogLevel.Info);
    }

    [Test]
    public async Task InMemoryLoggingManager_AddsAndQueriesLogs()
    {
        var manager = new InMemoryLoggingManager();
        int called = 0;
        manager.LogsUpdatedEvent.Subscribe(() => called++);

        manager.Log(LogLevel.Info, "HelloTest", new[] { "tag1" });

        await Assert.That(called).IsEqualTo(1);

        var result = manager.GetLogs(new GetLogsRequest(Query: "HelloTest", Count: 10, LogLevel: LogLevel.Trace, Since: DateTime.MinValue));
        await Assert.That(result.Succeeded).IsTrue();

        var messages = result.Value!.Messages.ToList();
        await Assert.That(messages.Count).IsEqualTo(1);
        await Assert.That(messages[0].Message).IsEqualTo("HelloTest");
        await Assert.That(messages[0].Tags).IsNotNull();
    }

    [Test]
    public async Task ResultProblem_ToLogMessage_MapsFields()
    {
        var problem = new ResultProblem("Something bad");
        var log = problem.ToLogMessage();

        await Assert.That(log.Level).IsEqualTo(LogLevel.Error);
        await Assert.That(log.Message).IsEqualTo(problem.ToBriefString());
        await Assert.That(log.SourcePath).IsNotNull();
        await Assert.That(log.Tags).IsNotNull();
    }
}
