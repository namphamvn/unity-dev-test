using System.Collections.Generic;
using Planetario.Geometry.Primitive;
using Unity.Mathematics;

namespace Planetario.Geometry
{
	public interface IPoints2D
	{
		IEnumerable<float2> GetPoints();
	}

	public static class Points2DExtensions
	{
		public static IEnumerable<float3> GetPoints(this IPoints2D points, CartesianPlane plane)
		{
			foreach (var point in points.GetPoints())
			{
				yield return plane.GetPoint(point);
			}
		}
	}
}
