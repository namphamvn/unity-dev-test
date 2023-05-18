using System;
using Planetario.Geometry.Primitive;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Meshes
{
	[Serializable]
	public struct Grid : IComponentData, IPlatform
	{
		public int2 cellCount;
		public float cellSize;

		private float2 GetOrigin()
		{
			return new float2(-cellCount.x * 0.5f + 0.5f, -cellCount.y * 0.5f + 0.5f) * cellSize;
		}

		public Location GetLocation(int locationId)
		{
			var x = locationId / cellCount.y;
			var y = locationId % cellCount.y;
			var origin = GetOrigin();
			var pos = origin + cellSize * new float2(x, y);
			return new Location
			{
				position = new float3(pos.x, 0, pos.y),
				rotation = quaternion.identity
			};
		}

		public int GetLocationCount()
		{
			return cellCount.x * cellCount.y;
		}

		public float2 GetExtends()
		{
			return (float2)cellCount * cellSize * 0.5f;
		}
	}
}
