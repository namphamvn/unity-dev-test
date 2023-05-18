using System;
using System.Collections.Generic;
using Planetario.GameWorlds.Models;
using Unity.Entities;

namespace Planetario.Platforms.Models
{
	/// <summary>
	/// stores which locations have been occupied in a platform
	/// </summary>
	public class OccupancyState : IOccupancyState, IComponentData
	{
		private readonly HashSet<int> _occupants;

		public OccupancyState()
		{
			_occupants = new HashSet<int>();
		}

		public bool IsOccupied(int locationId)
		{
			return _occupants.Contains(locationId);
		}

		public void SetOccupant(GameEntity occupant, int locationId, GameEntity platform)
		{
			if (_occupants.Contains(locationId))
			{
				throw new Exception($"Location {locationId} is already occupied");
			}

			occupant.Parent.GetComponentDataWithExactType<OccupancyState>()._occupants
				.Remove(occupant.GetComponentDataWithExactType<Occupant>().location);

			_occupants.Add(locationId);
			occupant.SetParent(platform);
			occupant.SetComponentDataWithExactType(new Occupant
			{
				location = locationId
			});
		}

		public void RemoveOccupant(GameEntity entity)
		{
			var locationId = entity.GetComponentDataWithExactType<Occupant>().location;
			_occupants.Remove(locationId);
		}
	}
}
