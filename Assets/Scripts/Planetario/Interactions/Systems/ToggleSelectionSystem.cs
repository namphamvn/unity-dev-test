using Planetario.GameWorlds;
using Planetario.GameWorlds.Systems;
using Planetario.Interactions.Models;

namespace Planetario.Interactions.Systems
{
	public class ToggleSelectionSystem : BaseSystem, ISystem<ToggleSelectionCommand>
	{
		public void OnCommandReceived(ToggleSelectionCommand command)
		{
			var gameEntity = GetEntity(command.entity);
			var isSelected = gameEntity.GetComponentDataWithExactType<Selectable>().isSelected;
			gameEntity.SetComponentDataWithExactType(new Selectable
			{
				isSelected = !isSelected
			});
		}
	}
}
