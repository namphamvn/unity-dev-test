namespace Planetario.GameWorlds
{
	public interface ISystem
	{
	}

	public interface ISystem<TCommand> : ISystem where TCommand : ICommand
	{
		void OnCommandReceived(TCommand command);
	}
}
