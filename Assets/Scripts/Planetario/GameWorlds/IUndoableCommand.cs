using Planetario.GameWorlds.Systems;

namespace Planetario.GameWorlds
{
	/// <summary>
	/// An undoable command creates its Undo command before it is executed
	/// There are exception to this rule which is handled differently
	/// </summary>
	public interface IUndoableCommand : ICommand
	{
		ICommand GetUndoCommand(GameWorldSystem worldSystem);
	}
}
