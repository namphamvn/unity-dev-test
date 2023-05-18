using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Planetario.Geometry.Primitive
{
	[Serializable]
	public struct Sphere : IComponentData, IRaycastable
	{
		public float3 center;

		/// <summary>
		///     If the radius is less than 0, the sphere is inside-out
		/// </summary>
		public float radius;

		public Sphere(float3 center, float radius)
		{
			this.center = center;
			this.radius = radius;
		}

		public readonly bool Raycast(Ray ray, out float enter)
		{
			var b = math.dot(ray.direction, center - ray.origin);
			var c = math.lengthsq(ray.origin - center) - radius * radius;
			var d = b * b - c;

			if (d < 0.0f)
			{
				enter = default;
				return false;
			}

			enter = b + (radius < 0f ? 1 : -1) * math.sqrt(d);
			return true;
		}
	}
}
