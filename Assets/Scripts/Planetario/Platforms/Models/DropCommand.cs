using System;
using Planetario.GameWorlds;
using Planetario.GameWorlds.Models;
using Planetario.GameWorlds.Systems;

namespace Planetario.Platforms.Models
{
	[Serializable]
	public struct DropCommand : IUndoableCommand
	{
		public int locationId;
		public IGameEntityInfo entity;
		public IGameEntityInfo platform;

		public ICommand GetUndoCommand(GameWorldSystem worldSystem)
		{
			var currentPlatform = entity.Parent;
			var currentLocationId = entity.GetComponentDataWithExactType<Occupant>().location;
			return new DropCommand
			{
				entity = entity,
				platform = currentPlatform,
				locationId = currentLocationId
			};
		}
	}
}
