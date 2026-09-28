namespace FlashTix.Application.Abstractions.Messaging;

public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
        Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
}