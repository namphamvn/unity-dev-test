using Planetario.GameWorlds.Models;

namespace Planetario.GameWorlds.Systems
{
	/// <summary>
	/// handle basic commands
	/// </summary>
	public class BasicSystem : BaseSystem, ISystem<SetComponentDataCommand>, ISystem<CombinedCommand>
	{
		public void OnCommandReceived(CombinedCommand command)
		{
			foreach (var childCommand in command.commands)
			{
				WorldSystem.ExecuteCommand(childCommand);
			}
		}

		public void OnCommandReceived(SetComponentDataCommand command)
		{
			GetEntity(command.entity).SetComponentData(command.componentData);
		}
	}
}
