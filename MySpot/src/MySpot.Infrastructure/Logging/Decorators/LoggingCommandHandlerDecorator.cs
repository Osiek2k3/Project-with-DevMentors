using System.Diagnostics;
using Humanizer;
using Microsoft.Extensions.Logging;
using MySpot.Application.Abstractions;

namespace MySpot.Infrastructure.Logging.Decorators
{
    internal sealed class LoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> _commandHandler;
        private readonly ILogger<ICommandHandler<TCommand>> _logger;

        public LoggingCommandHandlerDecorator(ICommandHandler<TCommand> commandHandler, ILogger<ICommandHandler<TCommand>> logger)
        {
            _commandHandler = commandHandler;
            _logger = logger;
        }

        public LoggingCommandHandlerDecorator()
        {

        }
        public async Task HandleAsync(TCommand command)
        {
            var commandName = typeof(TCommand).Name.Underscore();
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            _logger.LogInformation("Started handling a command: {CommandName}", commandName);
            await _commandHandler.HandleAsync(command);
            stopWatch.Stop();
            _logger.LogInformation("Completed handling a command: {CommandName} in {Elapsed} ", commandName, stopWatch.Elapsed);
        }
    }
}
