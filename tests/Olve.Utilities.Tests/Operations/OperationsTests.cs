using Microsoft.Extensions.DependencyInjection;
using Olve.Utilities.Operations;
using Olve.Utilities.Types.Results;

namespace Olve.Utilities.Tests.Operations;

public class OperationsTests
{
    private ServiceProvider _serviceProvider = null!;

    [Before(Test)]
    public void SetUp()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<Logger>();
        serviceCollection.AddTransient<EchoOperation>();
        serviceCollection.AddTransient<EchoOperation.Factory>();
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [After(Test)]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task Execute_WithMessage_LogsMessageAndReturnsSuccess()
    {
        // Arrange
        const string input = "Hello, World!";
        var logger = _serviceProvider.GetRequiredService<Logger>();
        var factory = _serviceProvider.GetRequiredService<EchoOperation.Factory>();
        var operation = factory.Build();

        // Act
        var result = operation.Execute(input);

        // Assert
        await Assert
            .That(result.Succeeded)
            .IsTrue();
        await Assert
            .That(logger.Messages.Count)
            .IsEqualTo(1);
        await Assert
            .That(logger.Messages)
            .Contains(input);
    }

    [Test]
    public async Task Execute_WithEmptyInput_LogsEmptyMessageAndReturnsSuccess()
    {
        // Arrange
        const string input = "";
        var logger = _serviceProvider.GetRequiredService<Logger>();
        var factory = _serviceProvider.GetRequiredService<EchoOperation.Factory>();
        var operation = factory.Build();

        // Act
        var result = operation.Execute(input);

        // Assert
        await Assert
            .That(result.Succeeded)
            .IsTrue();
        await Assert
            .That(logger.Messages.Count)
            .IsEqualTo(1);
        await Assert
            .That(logger.Messages)
            .Contains(input);
    }

    [Test]
    public async Task Factory_Build_CreatesNewOperationInstance()
    {
        // Arrange
        var factory = _serviceProvider.GetRequiredService<EchoOperation.Factory>();

        // Act
        var operation1 = factory.Build();
        var operation2 = factory.Build();

        // Assert
        await Assert
            .That(operation1)
            .IsNotNull();
        await Assert
            .That(operation2)
            .IsNotNull();
        await Assert
            .That(operation1)
            .IsNotEqualTo(operation2);
    }

    [Test]
    public async Task Logger_Log_AddsMessageToList()
    {
        // Arrange
        var logger = _serviceProvider.GetRequiredService<Logger>();
        const string message = "Test message";

        // Act
        logger.Log(message);

        // Assert
        await Assert
            .That(logger.Messages.Count)
            .IsEqualTo(1);
        await Assert
            .That(logger.Messages)
            .Contains(message);
    }

    public class Logger
    {
        private readonly List<string> _messages = [];
        public IReadOnlyList<string> Messages => _messages;

        public void Log(string message)
        {
            _messages.Add(message);
        }
    }

    private class EchoOperation(Logger logger) : IOperation<string>
    {
        public Result Execute(string input)
        {
            logger.Log(input);
            return Result.Success();
        }

        public class Factory(IServiceProvider serviceProvider) : OperationFactory<EchoOperation, string>(serviceProvider);
    }
}