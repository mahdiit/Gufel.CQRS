using Gufel.CQRS.Base.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.CQRS.Dispatcher
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider) =>
            _serviceProvider = serviceProvider;

        public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation)
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();
            return handler.Handle(command, cancellation);
        }
    }
}
