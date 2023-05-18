using Planetario.GameWorlds.Systems;

namespace Planetario.GameWorlds
{
	/// <summary>
	/// Query and interact with states for Undo feature
	/// </summary>
	public interface IUndoState
	{
		bool HasUndo();
		void PushUndoCommand(IUndoableCommand command, GameWorldSystem worldSystem);
		ICommand PopUndoCommand();
	}
}
