using Planetario.Geometry.Meshes;
using Planetario.Geometry.Primitive;

namespace Planetario.Platforms.Views
{
	public struct GridLocations : IPlatformLocations
	{
		private Grid _grid;

		public GridLocations(Grid platform)
		{
			_grid = platform;
		}

		public Location GetLocation(int locationId)
		{
			return _grid.GetLocation(locationId);
		}

		public int GetLocationCount()
		{
			return _grid.GetLocationCount();
		}

		public float GetObjectScale()
		{
			return _grid.cellSize;
		}
	}
}
