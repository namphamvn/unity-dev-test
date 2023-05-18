using Planetario.GameWorlds.Models;

namespace Planetario.Platforms.Models
{
	public interface IOccupancyState
	{
		bool IsOccupied(int locationId);
		void SetOccupant(GameEntity occupant, int locationId, GameEntity platform);
	}
}
