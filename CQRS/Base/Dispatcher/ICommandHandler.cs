namespace Gufel.CQRS.Base.Dispatcher
{
    public interface ICommandHandler<in TCommand, TCommandResult>
    {
        Task<TCommandResult> Handle(TCommand command, CancellationToken cancellation);
    }
}
