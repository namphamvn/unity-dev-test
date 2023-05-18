using Planetario.GameWorlds;
using Planetario.GameWorlds.Systems;
using Planetario.Interactions.Models;
using Planetario.Platforms.Models;

namespace Planetario.Platforms.Systems
{
	public class OccupancySystem : BaseSystem, ISystem<DropCommand>, ISystem<InstantiateEntityCommand>
	{
		public void OnCommandReceived(DropCommand command)
		{
			command.platform.GetComponentDataWithExactType<OccupancyState>()
				.SetOccupant(GetEntity(command.entity), command.locationId, GetEntity(command.platform));
		}

		public void OnCommandReceived(InstantiateEntityCommand command)
		{
			var platformEntity = GetEntity(command.platform);
			var newEntity = WorldSystem.World.Instantiate(GetEntity(command.source), platformEntity);

			var oldDroppable = newEntity.GetComponentDataWithExactType<Droppable>();
			oldDroppable.cloneOnDrop = false;
			newEntity.SetComponentData(oldDroppable);

			command.platform.GetComponentDataWithExactType<OccupancyState>()
				.SetOccupant(newEntity, command.locationId, platformEntity);
			WorldSystem.World.PushManualUndoCommand(new DeleteCommand
			{
				entity = newEntity
			});
		}
	}
}
