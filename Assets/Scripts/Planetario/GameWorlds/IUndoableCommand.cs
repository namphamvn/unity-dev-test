using Planetario.GameWorlds.Systems;

namespace Planetario.GameWorlds
{
	public interface IUndoableCommand : ICommand
	{
		ICommand GetUndoCommand(GameWorldSystem worldSystem);
	}
}
