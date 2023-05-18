using System.Collections.Generic;
using Planetario.GameWorlds;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Systems;
using Planetario.Interactions.Models;
using Planetario.Platforms.Models;

namespace Planetario.Platforms.Systems
{
	public class AdditionDeletionSystem : BaseSystem, ISystem<DeleteSelectedCommand>,
		ISystem<DeleteCommand>, ISystem<AddEntityCommand>
	{
		public void OnCommandReceived(AddEntityCommand command)
		{
			var entity = GetEntity(command.entity);
			var parentEntity = GetEntity(command.parent);
			WorldSystem.World.AddEntity(entity, parentEntity);
			if (parentEntity.TryGetComponentDataWithExactType<OccupancyState>(out var occupancyState))
			{
				var locationId = entity.GetComponentDataWithExactType<Occupant>().location;
				occupancyState.SetOccupant(entity, locationId, parentEntity);
			}
		}

		public void OnCommandReceived(DeleteCommand command)
		{
			var entity = GetEntity(command.entity);
			DeleteEntity(entity);
		}

		public void OnCommandReceived(DeleteSelectedCommand command)
		{
			var toBeRemoved = GetAllDeletableSelectedEntities();

			foreach (var entity in toBeRemoved)
			{
				DeleteEntity(entity);
			}
		}

		private void DeleteEntity(GameEntity entity)
		{
			if (entity.Parent != null && GetEntity(entity.Parent)
				    .TryGetComponentDataWithExactType<OccupancyState>(out var occupancyState))
			{
				occupancyState.RemoveOccupant(entity);
			}

			GetEntity(entity.Parent).RemoveEntity(entity);
			entity.Destroy();
		}

		public IEnumerable<GameEntity> GetAllDeletableSelectedEntities()
		{
			var toBeRemoved = new HashSet<GameEntity>();
			foreach (var entity in WorldSystem.World.Children)
			{
				ProcessEntity(entity, toBeRemoved);
			}

			return toBeRemoved;
		}

		private static void ProcessEntity(GameEntity entity, ISet<GameEntity> toBeRemoved)
		{
			if (entity.TryGetComponentDataWithExactType<Selectable>(out var selectable) &&
			    selectable.isSelected)
			{
				toBeRemoved.Add(entity);
			}
			else
			{
				foreach (var child in entity.Children)
				{
					ProcessEntity(child, toBeRemoved);
				}
			}
		}
	}
}
