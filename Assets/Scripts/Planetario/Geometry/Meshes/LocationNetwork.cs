using System;
using Planetario.Geometry.Primitive;
using Unity.Collections;

namespace Planetario.Geometry.Meshes
{
	public struct LocationNetwork : IDisposable, IPlatformLocations
	{
		public NativeList<Location> locations;
		public float objScale;

		public struct Edge
		{
			public int neighbor;
			public int length;
		}

		/// <summary>
		///     each location has a list of adjacent locations
		/// </summary>
		public NativeMultiHashMap<int, int> adjacency;

		public LocationNetwork(int capacity, Allocator allocator)
		{
			locations = new NativeList<Location>(capacity, allocator);
			adjacency = new NativeMultiHashMap<int, int>(capacity * 2, allocator);
			objScale = 1f;
		}

		public void Clear()
		{
			locations.Clear();
			adjacency.Clear();
		}

		public void Dispose()
		{
			locations.Dispose();
			adjacency.Dispose();
		}

		public Location GetLocation(int locationId)
		{
			return locations[locationId];
		}

		public int GetLocationCount()
		{
			return locations.Length;
		}

		public float GetObjectScale()
		{
			return objScale;
		}
	}
}
