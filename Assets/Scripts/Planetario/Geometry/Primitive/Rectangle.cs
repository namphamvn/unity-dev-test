using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	public struct Rectangle : IComponentData, IRaycastable
	{
		public CartesianPlane plane;
		public float2 extends;

		public bool Raycast(Ray ray, out float enter)
		{
			if (plane.Raycast(ray, out enter))
			{
				var point = ray.GetPoint(enter);
				var coord = plane.GetCoord(point);
				var results = math.abs(coord) <= extends;
				return results.x & results.y;
			}

			return false;
		}
	}
}
