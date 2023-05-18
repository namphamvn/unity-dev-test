using Planetario.GameWorlds.Systems;

namespace Planetario.GameWorlds
{
	public interface IUndoState
	{
		bool HasUndo();
		void PushUndoCommand(IUndoableCommand command, GameWorldSystem worldSystem);
		ICommand PopUndoCommand();
	}
}
