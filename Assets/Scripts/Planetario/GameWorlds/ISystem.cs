namespace Planetario.GameWorlds
{
	/// <summary>
	/// A system listen to Command and make changes to Model and raises events
	/// </summary>
	public interface ISystem
	{
	}

	public interface ISystem<TCommand> : ISystem where TCommand : ICommand
	{
		void OnCommandReceived(TCommand command);
	}
}
