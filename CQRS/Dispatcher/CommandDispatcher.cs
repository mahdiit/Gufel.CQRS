using Gufel.CQRS.Base.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace Gufel.CQRS.Dispatcher
{
    public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
    {
        public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation)
        {
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();
            return handler.Handle(command, cancellation);
        }
    }
}
