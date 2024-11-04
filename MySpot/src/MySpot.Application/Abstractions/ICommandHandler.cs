
namespace MySpot.Application.Abstractions
{
    public interface ICommandHandler<TCommand> where TCommand : class
    {
        Task HandleAsync(TCommand command);
    }
}
