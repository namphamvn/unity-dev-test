using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	public struct RegularPolygon : IComponentData, IPoints
	{
		/// <summary>
		///     center of the polygon
		/// </summary>
		public float3 center;

		/// <summary>
		///     normal vector of the polygon
		///     have to be normalised
		/// </summary>
		public float3 up;

		/// <summary>
		///     This is the vector pointing to the first vertex
		///     have to be normalised
		/// </summary>
		public float3 forward;

		/// <summary>
		///     distance from each vertex to the center
		/// </summary>
		public float radius;

		/// <summary>
		///     number of edges, 3 or larger
		///     - 3: triangle
		///     - 4: rectangle
		///     - 5: pentagon
		///     - ...
		/// </summary>
		public int edgeCount;

		public RegularPolygon(float3 center, float3 up, float3 forward, float radius, int edgeCount)
		{
			this.center = center;
			this.up = up;
			this.forward = forward;
			this.radius = radius;
			this.edgeCount = edgeCount;
		}

		public IEnumerable<float3> GetPoints()
		{
			var angle = math.PI * 2 / edgeCount;
			var right = math.cross(up, forward);
			for (var i = 0; i < edgeCount; i++)
			{
				var curAngle = angle * i;
				var offset = (right * math.cos(curAngle) + forward * math.sin(curAngle)) * radius;
				yield return center + offset;
			}
		}
	}
}
