using Unity.Entities;

namespace Planetario.GameWorlds
{
	/// <summary>
	/// Commands are sent from View to System to interact with the game
	/// </summary>
	public interface ICommand : IComponentData
	{
	}
}
